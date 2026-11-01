using MelonLoader;
using System;
using UnhollowerBaseLib;

namespace RealShop
{
    internal static class BuildJobPrepareContext
    {
        private static bool _unusableLogged;
        private static int _unusableLoggedOrder = -1;
        private static int _resolvedLoggedOrder = -1;

        internal static Job Resolve(CarLoader carLoader, string tag)
        {
            if (carLoader == null)
            {
                return null;
            }

            int order;
            try
            {
                order = carLoader.orderConnection;
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] PrepareJob order lookup failed: " + e.Message);
                return null;
            }

            if (order < 0 || !BuildJobState.IsOursJobId(order))
            {
                return null;
            }

            Job candidate = BuildJobAcceptPatch.CandidateJob;
            if (candidate != null && IsBuildJobSafe(candidate, order))
            {
                return candidate;
            }

            Job job = FindJobById(order);
            if (job != null)
            {
                if (_resolvedLoggedOrder != order)
                {
                    _resolvedLoggedOrder = order;
                    MelonLogger.Warning("[BuildJob] PrepareJob resolved by car order=" + order + " via=" + tag
                        + " (the Job argument of this method arrives marshalled as garbage, so the job is identified"
                        + " by CarLoader.orderConnection and the generator lists)");
                }

                return job;
            }

            Job shadow;
            if (BuildJobState.TryGetShadowJob(order, out shadow))
            {
                if (_resolvedLoggedOrder != order)
                {
                    _resolvedLoggedOrder = order;
                    MelonLogger.Msg("[BuildJob] PrepareJob resolved from shadow registry: car order=" + order + " via=" + tag
                        + " (the offer expired / the game dropped the job, the shadow keeps it strip-able)");
                }

                return shadow;
            }

            LogUnusable(order, tag);
            return null;
        }

        internal static int CarOrder(CarLoader carLoader)
        {
            if (carLoader == null)
            {
                return -1;
            }

            try
            {
                return carLoader.orderConnection;
            }
            catch (System.Exception)
            {
                return -2;
            }
        }

        internal static Job FindJobById(int jobId)
        {
            OrderGenerator generator = BuildJobState.Generator;
            if (generator == null)
            {
                return null;
            }

            Job job = FindIn(generator.Jobs, jobId);
            if (job != null)
            {
                return job;
            }

            return FindIn(generator.selectedJobs, jobId);
        }

        private static bool IsBuildJobSafe(Job job, int order)
        {
            try
            {
                return job.id == order && BuildJobService.IsBuildJob(job);
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        private static Job FindIn(Il2CppSystem.Collections.Generic.List<Job> jobs, int jobId)
        {
            if (jobs == null)
            {
                return null;
            }

            try
            {
                for (int i = 0; i < jobs.Count; i++)
                {
                    Job job = jobs[i];
                    if (job != null && IsBuildJobSafe(job, jobId))
                    {
                        return job;
                    }
                }
            }
            catch (System.Exception)
            {
                return null;
            }

            return null;
        }

        private static void LogUnusable(int order, string tag)
        {
            if (_unusableLogged && _unusableLoggedOrder == order)
            {
                return;
            }

            _unusableLogged = true;
            _unusableLoggedOrder = order;
            MelonLogger.Warning("[BuildJob] PrepareJob build job not resolved: car order=" + order + " via=" + tag
                + " (the order is neither the accept candidate nor present in generator.Jobs/selectedJobs)");
        }
    }
}
