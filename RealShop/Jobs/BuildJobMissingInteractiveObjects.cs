using MelonLoader;
using System;
using System.Text;
using UnityEngine;

namespace RealShop
{
    internal static class BuildJobMissingInteractiveObjects
    {
        private static readonly string[] StructuralNames = { "body", "details" };

        private static readonly string[] StructuralNamePrefixes =
        {
            "chassis", "frame", "unibody", "subframe", "monocoque", "shell"
        };

        private const int NamesCap = 420;
        private const int UnreadableType = -2;
        private const int UnremovableTypeA = 1;
        private const int UnremovableTypeB = 6;

        private static int _appliedJobId = -1;
        private static int _verifiedJobId = -1;

        internal static void Apply(CarLoader loader, int jobId, string tag)
        {
            if (loader == null || jobId < 0 || _appliedJobId == jobId)
            {
                return;
            }

            Config config = RealParts.ActiveConfig;
            if (config == null || !RealParts.ModIsEnabled || !config.EnableBuildFromScratchJobs
                || !config.BuildJobStripInteractiveObjects
                || string.Equals(config.BuildJobStripMode, "off", StringComparison.OrdinalIgnoreCase)
                || !BuildJobStripTarget.MatchesExact(loader))
            {
                return;
            }

            _appliedJobId = jobId;

            Transform root = null;
            try
            {
                root = loader.GetRootTransform();
                if (root == null)
                {
                    MelonLogger.Warning("[BuildJob] interactive missing: job=" + jobId + " root=null via=" + tag);
                    return;
                }

                InteractiveObject[] objects = root.GetComponentsInChildren<InteractiveObject>(true);
                if (objects == null)
                {
                    MelonLogger.Warning("[BuildJob] interactive missing: job=" + jobId + " count=? via=" + tag);
                    return;
                }

                IntPtr rootPointer = RootPointer(root);
                if (rootPointer == IntPtr.Zero)
                {
                    MelonLogger.Warning("[BuildJob] interactive missing: job=" + jobId
                        + " root io not found on the car root, stripping every io via=" + tag);
                }

                int processed = 0;
                int rootSkipped = 0;
                int skipped = 0;
                int failed = 0;
                StringBuilder ids = new StringBuilder();
                for (int i = 0; i < objects.Length; i++)
                {
                    InteractiveObject io = objects[i];
                    if (io == null)
                    {
                        skipped++;
                        continue;
                    }

                    string name = null;
                    try
                    {
                        name = io.name;
                        if (io.Pointer == rootPointer)
                        {
                            rootSkipped++;
                            continue;
                        }

                        io.GenerateAllMissing(null);
                        processed++;
                        Append(ids, name);
                    }
                    catch (System.Exception e)
                    {
                        failed++;
                        MelonLogger.Warning("[BuildJob] interactive missing: job=" + jobId
                            + " io=\"" + name + "\" failed: " + e.Message);
                    }
                }

                MelonLogger.Msg("[BuildJob] interactive missing: job=" + jobId + " count=" + objects.Length
                    + " processed=" + processed + " rootSkipped=" + rootSkipped + " skipped=" + skipped
                    + " failed=" + failed + " via=" + tag + " names=" + ids);

            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] interactive missing: job=" + jobId + " failed " + e.Message
                    + " via=" + tag);
            }

            StripUnremovable(root, jobId, tag);
            VerifyInterior(loader, jobId, tag);
        }

        internal static void ClearAppliedMarker(int jobId)
        {
            if (_appliedJobId == jobId)
            {
                _appliedJobId = -1;
            }

            if (_verifiedJobId == jobId)
            {
                _verifiedJobId = -1;
            }
        }

        internal static bool IsInteriorName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            return name.StartsWith("seat", StringComparison.OrdinalIgnoreCase)
                || name.StartsWith("bench", StringComparison.OrdinalIgnoreCase)
                || name.StartsWith("steering", StringComparison.OrdinalIgnoreCase)
                || name.StartsWith("dash", StringComparison.OrdinalIgnoreCase)
                || name.StartsWith("radio", StringComparison.OrdinalIgnoreCase)
                || name.StartsWith("glove", StringComparison.OrdinalIgnoreCase)
                || name.StartsWith("interior", StringComparison.OrdinalIgnoreCase);
        }

        private static void StripUnremovable(Transform root, int jobId, string tag)
        {
            Config config = RealParts.ActiveConfig;
            if (root == null || config == null || !config.BuildJobStripUnremovableParts)
            {
                return;
            }

            try
            {
                PartScript[] parts = root.GetComponentsInChildren<PartScript>(false);
                GameInventory inventory = Singleton<GameInventory>.Instance;
                if (parts == null || inventory == null)
                {
                    MelonLogger.Warning("[BuildJob] unremovable strip: job=" + jobId
                        + " parts=" + (parts == null ? "?" : parts.Length.ToString())
                        + " inventory=" + (inventory == null ? "null" : "ok") + " via=" + tag);
                    return;
                }

                int[] types = new int[parts.Length];
                PartProperty[] properties = new PartProperty[parts.Length];
                int mounted = 0;
                int strippedTypeHits = 0;
                int failed = 0;
                for (int i = 0; i < parts.Length; i++)
                {
                    PartScript part = parts[i];
                    if (part == null)
                    {
                        types[i] = UnreadableType;
                        continue;
                    }

                    try
                    {
                        properties[i] = inventory.GetItemProperty(BuildJobPartPropertyProbe.PartId(part));
                        int type;
                        types[i] = BuildJobPartPropertyProbe.TryType(properties[i], out type)
                            ? type : UnreadableType;
                        if (part.IsUnmounted)
                        {
                            if (types[i] == UnremovableTypeB
                                && !IsStructuralPart(properties[i], part.name)
                                && part.MountObjects != null && part.MountObjects.Count > 0)
                            {
                                strippedTypeHits++;
                            }

                            continue;
                        }

                        mounted++;
                    }
                    catch (System.Exception e)
                    {
                        types[i] = UnreadableType;
                        failed++;
                        MelonLogger.Warning("[BuildJob] unremovable strip: job=" + jobId
                            + " probe part=\"" + part.name + "\" failed: " + e.Message);
                    }
                }

                if (strippedTypeHits > 0)
                {
                    MelonLogger.Warning("[BuildJob] unremovable strip: job=" + jobId + " aborted: type="
                        + UnremovableTypeB + " on " + strippedTypeHits
                        + " unmounted non-structural parts (the game cannot strip type " + UnremovableTypeB
                        + "), typeMember=" + BuildJobPartPropertyProbe.TypeSource + " via=" + tag);
                    return;
                }

                int gated = 0;
                int stripped = 0;
                int typeOne = 0;
                StringBuilder names = new StringBuilder();
                for (int i = 0; i < parts.Length; i++)
                {
                    PartScript part = parts[i];
                    int type = types[i];
                    if (part == null || (type != UnremovableTypeA && type != UnremovableTypeB))
                    {
                        continue;
                    }

                    string name = null;
                    try
                    {
                        name = part.name;
                        if (part.IsUnmounted || IsStructuralPart(properties[i], name))
                        {
                            continue;
                        }

                        gated++;
                        if (type == UnremovableTypeA)
                        {
                            typeOne++;
                            Append(names, name + ":" + type);
                            continue;
                        }

                        if (part.MountObjects == null || part.MountObjects.Count == 0)
                        {
                            continue;
                        }

                        part.HideBySavegame(true, null);
                        stripped++;
                        Append(names, name + ":" + type);
                    }
                    catch (System.Exception e)
                    {
                        failed++;
                        MelonLogger.Warning("[BuildJob] unremovable strip: job=" + jobId
                            + " part=\"" + name + "\" failed: " + e.Message);
                    }
                }

                MelonLogger.Msg("[BuildJob] unremovable strip: job=" + jobId + " parts=" + parts.Length
                    + " mounted=" + mounted + " gated=" + gated + " stripped=" + stripped
                    + " failed=" + failed + " typeOne=" + typeOne
                    + " typeMember=" + BuildJobPartPropertyProbe.TypeSource
                    + " via=" + tag + " names=" + names);
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] unremovable strip: job=" + jobId + " failed " + e.Message
                    + " via=" + tag);
            }
        }

        private static bool IsStructuralPart(PartProperty property, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return true;
            }

            for (int i = 0; i < StructuralNames.Length; i++)
            {
                if (string.Equals(name, StructuralNames[i], StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            for (int i = 0; i < StructuralNamePrefixes.Length; i++)
            {
                if (name.StartsWith(StructuralNamePrefixes[i], StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return property == null;
        }

        private static IntPtr RootPointer(Transform root)
        {
            try
            {
                InteractiveObject io = root.GetComponent<InteractiveObject>();
                return io == null ? IntPtr.Zero : io.Pointer;
            }
            catch (System.Exception)
            {
                return IntPtr.Zero;
            }
        }

        private static void Append(StringBuilder names, string name)
        {
            if (string.IsNullOrEmpty(name) || names.Length >= NamesCap)
            {
                return;
            }

            if (names.Length > 0)
            {
                names.Append('|');
            }

            names.Append(name);
        }

        private static void VerifyInterior(CarLoader loader, int jobId, string tag)
        {
            if (_verifiedJobId == jobId)
            {
                return;
            }

            _verifiedJobId = jobId;

            try
            {
                Il2CppSystem.Collections.Generic.List<CarPart> parts = loader.carParts;
                if (parts == null)
                {
                    return;
                }

                int count = 0;
                int unmounted = 0;
                StringBuilder detail = new StringBuilder();
                for (int i = 0; i < parts.Count; i++)
                {
                    CarPart part = parts[i];
                    string name = part != null ? part.name : null;
                    if (!IsInteriorName(name))
                    {
                        continue;
                    }

                    count++;
                    try
                    {
                        if (part.Unmounted)
                        {
                            unmounted++;
                        }

                        if (detail.Length > 0)
                        {
                            detail.Append('|');
                        }

                        detail.Append(name).Append(part.Unmounted ? "-off" : "-on");
                    }
                    catch (System.Exception)
                    {
                    }
                }

                MelonLogger.Msg("[BuildJob] interior verify: job=" + jobId + " parts=" + count
                    + " unmounted=" + unmounted + " via=" + tag + " detail=" + detail);
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] interior verify: job=" + jobId + " failed " + e.Message);
            }
        }
    }
}
