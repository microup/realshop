using MelonLoader;
using System;
using System.Text;
using UnhollowerBaseLib;

namespace RealShop
{
    internal static class BuildJobMountMode
    {
        private static int _appliedJobId = -1;

        internal static void ApplyPreparedMountMode(CarLoader loader, Job job)
        {
            if (loader == null || job == null || _appliedJobId == job.id)
            {
                return;
            }

            Config config = RealParts.ActiveConfig;
            if (config == null || !RealParts.ModIsEnabled || !BuildJobState.IsOursJobId(job.id)
                || string.Equals(config.BuildJobStripMode, "off", StringComparison.OrdinalIgnoreCase)
                || !BuildJobStripTarget.MatchesExact(loader))
            {
                return;
            }

            _appliedJobId = job.id;
            try
            {
                loader.SetAllPartsMissing();
                MelonLogger.Msg("[BuildJob] mount mode missing: job=" + job.id
                    + " via=PrepareJob SetAllPartsMissing");
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] mount mode missing: job=" + job.id
                    + " failed: " + e.Message + " via=PrepareJob");
            }

            try
            {
                BuildJobSystemCondition.Probe(loader, job.id, "PrepareJob");
                BuildJobMissingInteractiveObjects.Apply(loader, job.id, "PrepareJob");
                BuildJobInteriorStrip.Apply(loader, job.id, "PrepareJob");
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] mount mode probes: job=" + job.id
                    + " failed: " + e.Message + " via=PrepareJob");
            }
        }

        internal static void LogPreparedTasks(Job job)
        {
            if (job == null)
            {
                return;
            }

            Il2CppReferenceArray<JobTask> tasks = job.jobTasks;
            if (tasks == null)
            {
                MelonLogger.Warning("[BuildJob] mount mode tasks missing: job=" + job.id);
                return;
            }

            int listed = 0;
            int declared = 0;
            StringBuilder detail = new StringBuilder();
            for (int i = 0; i < tasks.Length; i++)
            {
                JobTask task = tasks[i];
                if (task == null)
                {
                    continue;
                }

                int count = task.Parts != null ? task.Parts.Count : 0;
                listed += count;
                declared += task.partsCount;
                if (detail.Length > 0)
                {
                    detail.Append('|');
                }

                detail.Append(task.type).Append('/').Append(task.subtype)
                    .Append('=').Append(count).Append('/').Append(task.partsCount);
            }

            string message = "[BuildJob] mount mode tasks: job=" + job.id + " tasks=" + tasks.Length
                + " listedParts=" + listed + " declaredParts=" + declared + " detail=" + detail;
            if (listed == 0)
            {
                MelonLogger.Warning(message);
                return;
            }

            MelonLogger.Msg(message);
        }
    }
}
