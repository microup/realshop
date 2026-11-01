using MelonLoader;
using System;
using System.Text;
using UnityEngine;

namespace RealShop
{
    internal static class BuildJobSystemCondition
    {
        private const int InteriorProbeMax = 14;
        private const int ReferenceProbeMax = 6;

        private static int _probedJobId = -1;

        internal static void Probe(CarLoader loader, int jobId, string tag)
        {
            Config config = RealParts.ActiveConfig;
            if (loader == null || config == null || !RealParts.ModIsEnabled
                || !config.EnableBuildFromScratchJobs || !BuildJobState.IsOursJobId(jobId)
                || jobId == _probedJobId)
            {
                return;
            }

            _probedJobId = jobId;

            Transform root = loader.GetRootTransform();
            if (root == null)
            {
                MelonLogger.Warning("[BuildJob] system probe: job=" + jobId + " root=null via=" + tag);
                return;
            }

            PartScript[] parts = null;
            MountObject[] objects = null;
            try
            {
                parts = root.GetComponentsInChildren<PartScript>(true);
                objects = root.GetComponentsInChildren<MountObject>(true);
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] system probe: job=" + jobId + " enumerate failed " + e.Message
                    + " via=" + tag);
            }

            LogPartScripts(jobId, parts, tag);
            LogMountObjects(jobId, objects, tag);
            LogInteriorSlots(jobId, loader, tag);
            LogInteractiveObjects(jobId, root, tag);
            LogPartPropertyTypes(jobId, parts, tag);
            try
            {
                LogInteriorProbe(jobId, parts, tag);
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] interior probe: job=" + jobId + " failed "
                    + e.Message + " via=" + tag);
            }
        }

        internal static void ClearProbedMarker(int jobId)
        {
            if (_probedJobId == jobId)
            {
                _probedJobId = -1;
            }
        }

        private static void LogPartScripts(int jobId, PartScript[] parts, string tag)
        {
            if (parts == null)
            {
                MelonLogger.Msg("[BuildJob] system probe: job=" + jobId + " parts=? via=" + tag);
                return;
            }

            int unmounted = 0;
            int mountable = 0;
            int sum = 0;
            int counted = 0;
            int min = 100;
            int max = 0;
            for (int i = 0; i < parts.Length; i++)
            {
                PartScript part = parts[i];
                if (part == null)
                {
                    continue;
                }

                try
                {
                    if (part.IsUnmounted)
                    {
                        unmounted = unmounted + 1;
                    }

                    if (part.MountObjects != null && part.MountObjects.Count > 0)
                    {
                        mountable = mountable + 1;
                    }

                    int percent = Percent(part.Condition);
                    if (percent >= 0)
                    {
                        sum = sum + percent;
                        counted = counted + 1;
                        min = percent < min ? percent : min;
                        max = percent > max ? percent : max;
                    }
                }
                catch (System.Exception)
                {
                }
            }

            MelonLogger.Msg("[BuildJob] system probe: job=" + jobId + " parts=" + parts.Length
                + " unmounted=" + unmounted + " mountable=" + mountable
                + " condition=" + Summarize(counted, sum, min, max)
                + " via=" + tag);
        }

        private static void LogMountObjects(int jobId, MountObject[] objects, string tag)
        {
            if (objects == null)
            {
                MelonLogger.Msg("[BuildJob] system objects: job=" + jobId + " count=? via=" + tag);
                return;
            }

            int sum = 0;
            int counted = 0;
            int min = 100;
            int max = 0;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < objects.Length; i++)
            {
                MountObject item = objects[i];
                if (item == null)
                {
                    continue;
                }

                try
                {
                    string name = item.name;
                    if (!string.IsNullOrEmpty(name) && sb.Length < 400)
                    {
                        if (sb.Length > 0)
                        {
                            sb.Append('|');
                        }

                        sb.Append(name);
                    }

                    int percent = Percent(item.Condition);
                    if (percent >= 0)
                    {
                        sum = sum + percent;
                        counted = counted + 1;
                        min = percent < min ? percent : min;
                        max = percent > max ? percent : max;
                    }
                }
                catch (System.Exception)
                {
                }
            }

            MelonLogger.Msg("[BuildJob] system objects: job=" + jobId + " count=" + objects.Length
                + " condition=" + Summarize(counted, sum, min, max)
                + " via=" + tag + " names=" + sb);
        }

        private static void LogInteriorSlots(int jobId, CarLoader loader, string tag)
        {
            try
            {
                InteriorParams slots = loader.InteriorParams;
                MelonLogger.Msg("[BuildJob] interior slots: job=" + jobId + " seatLeft=" + Slot(slots.SeatLeft)
                    + " seatRight=" + Slot(slots.SeatRight) + " wheel=" + Slot(slots.Wheel)
                    + " bench=" + Slot(slots.Bench) + " benchFront=" + Slot(slots.Bench_front) + " via=" + tag);
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] interior slots: job=" + jobId + " failed " + e.Message
                    + " via=" + tag);
            }
        }

        private static void LogInteractiveObjects(int jobId, Transform root, string tag)
        {
            try
            {
                InteractiveObject[] objects = root.GetComponentsInChildren<InteractiveObject>(true);
                if (objects == null)
                {
                    MelonLogger.Warning("[BuildJob] interactive objects: job=" + jobId + " count=? via=" + tag);
                    return;
                }

                int mounted = 0;
                int active = 0;
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < objects.Length; i++)
                {
                    InteractiveObject io = objects[i];
                    if (io == null)
                    {
                        continue;
                    }

                    try
                    {
                        if (io.on)
                        {
                            mounted = mounted + 1;
                        }

                        if (io.gameObject != null && io.gameObject.activeSelf)
                        {
                            active = active + 1;
                        }

                        if (sb.Length < 320)
                        {
                            if (sb.Length > 0)
                            {
                                sb.Append('|');
                            }

                            sb.Append(string.IsNullOrEmpty(io.id) ? io.name : io.id).Append(io.on ? '+' : '-');
                        }
                    }
                    catch (System.Exception)
                    {
                    }
                }

                MelonLogger.Msg("[BuildJob] interactive objects: job=" + jobId + " count=" + objects.Length
                    + " on=" + mounted + " activeSelf=" + active + " via=" + tag + " ids=" + sb);
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] interactive objects: job=" + jobId + " failed " + e.Message
                    + " via=" + tag);
            }
        }

        private static void LogInteriorProbe(int jobId, PartScript[] parts, string tag)
        {
            if (parts == null)
            {
                return;
            }

            GameInventory inventory = null;
            try
            {
                inventory = Singleton<GameInventory>.Instance;
            }
            catch (System.Exception)
            {
            }

            int count = 0;
            int dropped = 0;
            StringBuilder detail = new StringBuilder();
            for (int i = 0; i < parts.Length; i++)
            {
                PartScript part = parts[i];
                string name = BuildJobMountIndex.SafeName(parts, i);
                if (part == null || !BuildJobMissingInteractiveObjects.IsInteriorName(name))
                {
                    continue;
                }

                count++;
                string state = "?";
                try
                {
                    state = part.gameObject.activeInHierarchy
                        ? (part.IsUnmounted ? "off" : "on")
                        : "hidden";
                }
                catch (System.Exception)
                {
                }

                BuildJobMountIndex.Append(detail, name + ":" + state + ":t"
                    + BuildJobPartPropertyProbe.TypeOf(inventory, BuildJobPartPropertyProbe.PartId(part))
                    + ":m" + CountMountObjects(part), ref dropped);
            }

            MelonLogger.Msg("[BuildJob] interior probe: job=" + jobId + " parts=" + count
                + " truncated=" + dropped + " typeMember=" + BuildJobPartPropertyProbe.TypeSource
                + " via=" + tag + " detail=" + detail);
        }

        private static int CountMountObjects(PartScript part)
        {
            try
            {
                return part.MountObjects == null ? -1 : part.MountObjects.Count;
            }
            catch (System.Exception)
            {
                return -1;
            }
        }

        private static void LogPartPropertyTypes(int jobId, PartScript[] parts, string tag)
        {
            if (parts == null)
            {
                return;
            }

            try
            {
                GameInventory inventory = Singleton<GameInventory>.Instance;
                if (inventory == null)
                {
                    MelonLogger.Warning("[BuildJob] prop types: job=" + jobId + " inventory=null via=" + tag);
                    return;
                }

                int interior = 0;
                int references = 0;
                StringBuilder detail = new StringBuilder();
                for (int i = 0; i < parts.Length; i++)
                {
                    PartScript part = parts[i];
                    if (part == null)
                    {
                        continue;
                    }

                    string name = part.name;
                    bool isInterior = BuildJobMissingInteractiveObjects.IsInteriorName(name);
                    if (isInterior)
                    {
                        if (interior >= InteriorProbeMax)
                        {
                            continue;
                        }

                        interior++;
                    }
                    else
                    {
                        if (references >= ReferenceProbeMax)
                        {
                            continue;
                        }

                        references++;
                    }

                    if (detail.Length > 0)
                    {
                        detail.Append('|');
                    }

                    string id = BuildJobPartPropertyProbe.PartId(part);
                    detail.Append(name).Append('=').Append(BuildJobPartPropertyProbe.TypeOf(inventory, id))
                        .Append(isInterior ? "i" : "r")
                        .Append(BuildJobPartPropertyProbe.IsInterior(id) ? "g" : "-")
                        .Append(BuildJobPartPropertyProbe.IsBody(inventory, id) ? "b" : "-")
                        .Append(part.IsUnmounted ? "u" : "-");
                }

                MelonLogger.Msg("[BuildJob] prop types: job=" + jobId + " interior=" + interior
                    + " references=" + references + " typeMember=" + BuildJobPartPropertyProbe.TypeSource
                    + " via=" + tag + " detail=" + detail);
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] prop types: job=" + jobId + " failed " + e.Message + " via=" + tag);
            }
        }

        private static string Slot(string value)
        {
            return string.IsNullOrEmpty(value) ? "<empty>" : value;
        }

        private static string Summarize(int counted, int sum, int min, int max)
        {
            if (counted == 0)
            {
                return "unreadable";
            }

            return "count=" + counted + " avg=" + (sum / counted) + "% min=" + min + "% max=" + max + "%";
        }

        private static int Percent(float value)
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
            {
                return -1;
            }

            int percent = (int)Math.Round(value * 100f, MidpointRounding.AwayFromZero);
            return percent < 0 ? 0 : (percent > 100 ? 100 : percent);
        }
    }
}
