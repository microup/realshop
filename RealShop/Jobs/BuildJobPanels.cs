using MelonLoader;
using System;
using System.Linq;

namespace RealShop
{
    internal static class BuildJobPanels
    {
        private const int MaxAttemptsPerJob = 3;

        private static string _prefixesRaw;
        private static string[] _prefixes;
        private static int _strippedJobId = -1;
        private static int _attemptJobId = -1;
        private static int _attempts;

        internal static int Chance()
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || !config.EnableBuildFromScratchJobs)
            {
                return 0;
            }

            if (string.Equals(config.BuildJobStripMode, "off", StringComparison.OrdinalIgnoreCase))
            {
                return 0;
            }

            int chance = config.BuildJobStripPanelChance;
            if (chance <= 0)
            {
                return 0;
            }

            return chance > 100 ? 100 : chance;
        }

        internal static bool Active()
        {
            return Chance() > 0;
        }

        internal static bool PanelsSuppressed()
        {
            if (Active())
            {
                return true;
            }

            Config config = RealParts.ActiveConfig;
            if (config == null || !config.EnableBuildFromScratchJobs)
            {
                return false;
            }

            if (string.Equals(config.BuildJobStripMode, "off", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return BuildJobStripTarget.IsMethodEnabled("CreateExterior");
        }

        internal static bool PanelsStrippedOn(CarLoader loader)
        {
            if (!PanelsSuppressed() || loader == null)
            {
                return false;
            }

            bool known;
            int mounted = CountMountedPanelsKnown(loader, out known);
            return known && mounted == 0;
        }

        internal static string PrefixesSummary()
        {
            return string.Join(",", Prefixes());
        }

        internal static bool IsPanelPart(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            string[] prefixes = Prefixes();
            for (int i = 0; i < prefixes.Length; i++)
            {
                if (name.StartsWith(prefixes[i], StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static string[] Prefixes()
        {
            Config config = RealParts.ActiveConfig;
            string raw = config != null ? config.BuildJobPanelNamePrefixes : null;
            if (string.IsNullOrWhiteSpace(raw))
            {
                raw = Config.ValueBuildJobPanelNamePrefixes;
            }

            if (!string.Equals(raw, _prefixesRaw, StringComparison.Ordinal))
            {
                string[] parsed = raw
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(part => part.Trim())
                    .Where(part => part.Length > 0)
                    .ToArray();

                if (parsed.Length == 0)
                {
                    MelonLogger.Warning("[BuildJob] panel prefixes: no valid names in '" + raw + "', using defaults");
                    parsed = Config.ValueBuildJobPanelNamePrefixes
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(part => part.Trim())
                        .Where(part => part.Length > 0)
                        .ToArray();
                    raw = _prefixesRaw + "\u0000fallback";
                }

                _prefixesRaw = raw;
                _prefixes = parsed;
            }

            return _prefixes ?? new string[0];
        }

        internal static int CountMountedPanels(CarLoader loader)
        {
            bool known;
            return CountParts(loader, true, false, out known);
        }

        internal static int CountMountedPanelsKnown(CarLoader loader, out bool known)
        {
            return CountParts(loader, true, false, out known);
        }

        internal static int CountUnmountedPanels(CarLoader loader)
        {
            bool known;
            return CountParts(loader, true, true, out known);
        }

        internal static int CountMountedOther(CarLoader loader)
        {
            bool known;
            return CountParts(loader, false, false, out known);
        }

        internal static int CountUnmountedOther(CarLoader loader)
        {
            bool known;
            return CountParts(loader, false, true, out known);
        }

        private static int CountParts(CarLoader loader, bool panels, bool unmounted, out bool known)
        {
            known = false;
            if (loader == null)
            {
                return 0;
            }

            try
            {
                Il2CppSystem.Collections.Generic.List<CarPart> parts = loader.carParts;
                if (parts == null)
                {
                    return 0;
                }

                int count = 0;
                for (int i = 0; i < parts.Count; i++)
                {
                    CarPart part = parts[i];
                    if (part == null)
                    {
                        continue;
                    }

                    if (IsPanelPart(part.name) != panels)
                    {
                        continue;
                    }

                    if (part.Unmounted == unmounted)
                    {
                        count++;
                    }
                }

                known = true;
                return count;
            }
            catch
            {
                return 0;
            }
        }

        internal static void TryStrip(CarLoader loader, int jobId, string tag)
        {
            int chance = Chance();
            if (chance <= 0 || loader == null)
            {
                return;
            }

            if (jobId >= 0 && jobId == _strippedJobId)
            {
                return;
            }

            if (jobId >= 0 && jobId == _attemptJobId && _attempts >= MaxAttemptsPerJob)
            {
                return;
            }

            bool known;
            int before = CountParts(loader, true, false, out known);

            try
            {
                loader.SetRandomMissingPanels(chance);
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] panel strip failed: job=" + jobId + " via=" + tag + " - " + e.Message);
                return;
            }

            if (jobId >= 0)
            {
                if (jobId == _attemptJobId)
                {
                    _attempts++;
                }
                else
                {
                    _attemptJobId = jobId;
                    _attempts = 1;
                }
            }

            int mounted = CountMountedPanels(loader);
            int unmounted = CountUnmountedPanels(loader);
            bool effective = known && mounted < before;

            if (effective && mounted == 0 && jobId >= 0)
            {
                _strippedJobId = jobId;
            }

            MelonLogger.Msg("[BuildJob] panels stripped: job=" + jobId + " chance=" + chance
                + " mountedPanels=" + mounted + " unmountedPanels=" + unmounted
                + " before=" + before + " effective=" + effective
                + " attempts=" + _attempts + " via=" + tag);
        }

        internal static void ClearStrippedMarker(int jobId)
        {
            if (_strippedJobId == jobId)
            {
                _strippedJobId = -1;
            }

            if (_attemptJobId == jobId)
            {
                _attemptJobId = -1;
                _attempts = 0;
            }
        }
    }
}
