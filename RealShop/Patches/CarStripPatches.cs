using Harmony;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable CS0618

namespace RealShop
{
    internal static class BuildJobStripTarget
    {
        private static bool _firstSeenLogged;
        private static bool _chassisLogged;
        private static string _methodsRaw;
        private static HashSet<string> _methods;

        private static readonly HashSet<string> KnownMethods = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase)
        {
            "CreateEngine",
            "CreateEngineAsync",
            "CreateExterior",
            "CreateInterior",
            "CreateParts",
            "CreateWheels",
            "CreateWheelsIO",
            "CreateDriveshaft",
            "CreateCarLightParts",
            "CreateBonusParts"
        };

        private static readonly HashSet<string> LoadBlockingMethods = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase)
        {
            "CreateWheels",
            "CreateWheelsIO"
        };

        internal static void ResetLoadMarkers()
        {
            _chassisLogged = false;
            _firstSeenLogged = false;
        }

        internal static bool IsMethodEnabled(string tag)
        {
            Config config = RealParts.ActiveConfig;
            string raw = config != null ? config.BuildJobStripMethods : null;
            if (string.IsNullOrWhiteSpace(raw))
            {
                raw = Config.ValueBuildJobStripMethods;
            }

            if (!string.Equals(raw, _methodsRaw, System.StringComparison.Ordinal))
            {
                _methods = ParseMethods(raw);
                if (_methods.Count == 0)
                {
                    MelonLogger.Warning("[BuildJob] strip methods: no valid names in '" + raw + "', using defaults");
                    raw = Config.ValueBuildJobStripMethods;
                    _methods = ParseMethods(raw);
                }

                _methodsRaw = raw;

                foreach (string name in _methods)
                {
                    if (!KnownMethods.Contains(name))
                    {
                        MelonLogger.Warning("[BuildJob] strip methods: unknown name '" + name + "' matches no CarLoader patch");
                    }
                    else if (LoadBlockingMethods.Contains(name))
                    {
                        MelonLogger.Warning("[BuildJob] strip methods: '" + name + "' is known to hang the arrival load (loading screen never finishes)");
                    }
                }

                MelonLogger.Msg("[BuildJob] strip methods: " + string.Join(",", _methods.OrderBy(x => x)));
            }

            return _methods != null && _methods.Contains(tag);
        }

        private static HashSet<string> ParseMethods(string raw)
        {
            return new HashSet<string>(raw
                .Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries)
                .Select(part => part.Trim())
                .Where(part => part.Length > 0),
                System.StringComparer.OrdinalIgnoreCase);
        }

        internal static string MethodsSummary()
        {
            Config config = RealParts.ActiveConfig;
            string raw = config != null ? config.BuildJobStripMethods : null;
            return string.IsNullOrWhiteSpace(raw) ? Config.ValueBuildJobStripMethods : raw;
        }

        private static bool _matchFailLogged;

        internal static bool Matches(CarLoader loader)
        {
            try
            {
                return MatchesCore(loader);
            }
            catch (System.Exception e)
            {
                if (!_matchFailLogged)
                {
                    _matchFailLogged = true;
                    MelonLogger.Warning("[BuildJob] strip match failed: " + e.Message);
                }

                return false;
            }
        }

        private static bool MatchesCore(CarLoader loader)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || !config.EnableBuildFromScratchJobs)
            {
                return false;
            }

            if (string.Equals(config.BuildJobStripMode, "off", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (loader == null)
            {
                return false;
            }

            int activeId = BuildJobState.ActiveJobId;
            int order = loader.orderConnection;
            if (activeId >= 0 && order == activeId)
            {
                return true;
            }

            if (!BuildJobState.ArmActive)
            {
                return false;
            }

            if (BuildJobState.MatchesArmedJob(order))
            {
                return true;
            }

            int armedId = BuildJobState.ArmJobId;
            if (order > 0 && order != armedId)
            {
                return false;
            }

            if (!BuildJobState.ModelFallbackActive)
            {
                return false;
            }

            int place = BuildJobState.ActivePlace >= 0 ? BuildJobState.ActivePlace : BuildJobState.ArmPlace;
            if (place < 0 || loader.placeNo < 0 || loader.placeNo != place)
            {
                return false;
            }

            return BuildJobState.MatchesArmedCar(ModelOf(loader));
        }

        internal static bool IsBuildCar(CarLoader loader, string tag)
        {
            if (!IsMethodEnabled(tag))
            {
                return false;
            }

            if (!Matches(loader))
            {
                return false;
            }

            BuildJobState.MarkStripped(BuildJobState.ActiveJobId);
            BuildJobState.MarkStripped(BuildJobState.ArmJobId);

            if (!_firstSeenLogged)
            {
                _firstSeenLogged = true;
                MelonLogger.Msg("[BuildJob] strip hook active: first suppression at " + tag
                    + " car=" + (ModelOf(loader) ?? "<null>") + " order=" + loader.orderConnection
                    + " place=" + loader.placeNo + " activeJob=" + BuildJobState.ActiveJobId);
            }

            MelonLogger.Msg("[BuildJob] strip: skipped " + tag + " car=" + (ModelOf(loader) ?? "<null>")
                + " order=" + loader.orderConnection + " place=" + loader.placeNo);
            return true;
        }

        internal static void LogChassisKept(CarLoader loader)
        {
            if (_chassisLogged)
            {
                return;
            }

            _chassisLogged = true;
            MelonLogger.Msg("[BuildJob] strip: chassis kept, car=" + (ModelOf(loader) ?? "<null>"));
        }

        private static string ModelOf(CarLoader loader)
        {
            try
            {
                return loader.carToLoad;
            }
            catch
            {
                return null;
            }
        }
    }

    [HarmonyPatch(typeof(CarLoader), "CreateChassis")]
    internal static class BuildJobChassisKeptPatch
    {
        private static void Prefix(CarLoader __instance)
        {
            if (BuildJobStripTarget.Matches(__instance))
            {
                BuildJobStripTarget.LogChassisKept(__instance);
            }
        }
    }

    [HarmonyPatch(typeof(CarLoader), "CreateChassis")]
    internal static class BuildJobChassisPanelStripPatch
    {
        private static bool _activeLogged;

        private static void Postfix(CarLoader __instance)
        {
            try
            {
                if (!BuildJobPanels.Active() || !BuildJobStripTarget.Matches(__instance))
                {
                    return;
                }

                if (!_activeLogged)
                {
                    _activeLogged = true;
                    MelonLogger.Msg("[BuildJob] hook active: CreateChassis panels chance=" + BuildJobPanels.Chance());
                }

                int jobId = BuildJobState.ActiveJobId >= 0 ? BuildJobState.ActiveJobId : BuildJobState.ArmJobId;
                BuildJobPanels.TryStrip(__instance, jobId, "CreateChassis");
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] CreateChassis panel strip hook failed: " + e.Message);
            }
        }
    }

    [HarmonyPatch(typeof(OrderGenerator), "PrepareJob", new[] { typeof(CarLoader), typeof(Job) })]
    internal static class BuildJobPanelStripPatch
    {
        private static bool _activeLogged;

        private static void Prefix([HarmonyArgument(0)] CarLoader carLoader, [HarmonyArgument(1)] Job job)
        {
            try
            {
                Config config = RealParts.ActiveConfig;
                if (config == null || !config.EnableBuildFromScratchJobs)
                {
                    return;
                }

                if (!BuildJobPanels.Active() || !BuildJobService.IsBuildJob(job))
                {
                    return;
                }

                if (!_activeLogged)
                {
                    _activeLogged = true;
                    MelonLogger.Msg("[BuildJob] hook active: PrepareJob panels chance=" + BuildJobPanels.Chance()
                        + " prefixes=" + BuildJobPanels.PrefixesSummary());
                }

                BuildJobPanels.TryStrip(carLoader, job.id, "PrepareJob");
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] PrepareJob panel strip hook failed: " + e.Message);
            }
        }
    }

    internal static class BuildJobStripArm
    {
        internal static void ArmCandidate(Job job, string tag)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || !config.EnableBuildFromScratchJobs)
            {
                return;
            }

            if (string.Equals(config.BuildJobStripMode, "off", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (job == null)
            {
                return;
            }

            MelonLogger.Msg("[BuildJob] strip armed before accept: id=" + job.id + " via=" + tag);
            BuildJobState.ArmStrip(job.id, job.carFile, job.carLoaderID);
        }

        internal static void DisarmCandidateWindow(string tag)
        {
            if (!BuildJobState.ArmActive)
            {
                return;
            }

            if (BuildJobState.ActiveJobId >= 0 && BuildJobState.ArmJobId == BuildJobState.ActiveJobId)
            {
                return;
            }

            BuildJobState.DisarmStrip(tag);
        }
    }

    [HarmonyPatch(typeof(CarLoader), "CreateEngine")]
    internal static class BuildJobStripEnginePatch
    {
        private static bool Prefix(CarLoader __instance)
        {
            return !BuildJobStripTarget.IsBuildCar(__instance, "CreateEngine");
        }
    }

    [HarmonyPatch(typeof(CarLoader), "CreateEngineAsync")]
    internal static class BuildJobStripEngineAsyncPatch
    {
        private static bool Prefix(CarLoader __instance)
        {
            return !BuildJobStripTarget.IsBuildCar(__instance, "CreateEngineAsync");
        }
    }

    [HarmonyPatch(typeof(CarLoader), "CreateExterior")]
    internal static class BuildJobStripExteriorPatch
    {
        private static bool Prefix(CarLoader __instance)
        {
            return !BuildJobStripTarget.IsBuildCar(__instance, "CreateExterior");
        }
    }

    [HarmonyPatch(typeof(CarLoader), "CreateInterior")]
    internal static class BuildJobStripInteriorPatch
    {
        private static bool Prefix(CarLoader __instance)
        {
            return !BuildJobStripTarget.IsBuildCar(__instance, "CreateInterior");
        }
    }

    [HarmonyPatch(typeof(CarLoader), "CreateParts")]
    internal static class BuildJobStripPartsPatch
    {
        private static bool Prefix(CarLoader __instance)
        {
            return !BuildJobStripTarget.IsBuildCar(__instance, "CreateParts");
        }
    }

    [HarmonyPatch(typeof(CarLoader), "CreateWheels")]
    internal static class BuildJobStripWheelsPatch
    {
        private static bool Prefix(CarLoader __instance)
        {
            return !BuildJobStripTarget.IsBuildCar(__instance, "CreateWheels");
        }
    }

    [HarmonyPatch(typeof(CarLoader), "CreateWheelsIO")]
    internal static class BuildJobStripWheelsIOPatch
    {
        private static bool Prefix(CarLoader __instance)
        {
            return !BuildJobStripTarget.IsBuildCar(__instance, "CreateWheelsIO");
        }
    }

    [HarmonyPatch(typeof(CarLoader), "CreateDriveshaft")]
    internal static class BuildJobStripDriveshaftPatch
    {
        private static bool Prefix(CarLoader __instance)
        {
            return !BuildJobStripTarget.IsBuildCar(__instance, "CreateDriveshaft");
        }
    }

    [HarmonyPatch(typeof(CarLoader), "CreateCarLightParts")]
    internal static class BuildJobStripCarLightPatch
    {
        private static bool Prefix(CarLoader __instance)
        {
            return !BuildJobStripTarget.IsBuildCar(__instance, "CreateCarLightParts");
        }
    }

    [HarmonyPatch(typeof(CarLoader), "CreateBonusParts", new[] { typeof(bool), typeof(bool) })]
    internal static class BuildJobStripBonusPartsPatch
    {
        private static bool Prefix(CarLoader __instance)
        {
            return !BuildJobStripTarget.IsBuildCar(__instance, "CreateBonusParts");
        }
    }
}

#pragma warning restore CS0618
