using MelonLoader;
using System;
using System.Text;
using UnhollowerBaseLib;
using UnityEngine;


namespace RealShop
{
    internal static partial class BuildJobService
    {
        private const float AdvanceCarTimeoutSeconds = 180f;

        private static float _advanceDeadlineAt = -1f;

        private static void TryPayPendingAdvance(Config config)
        {
            int jobId = BuildJobState.AdvancePendingFor;
            if (jobId < 0)
            {
                _advancePollJobId = -1;
                _advanceDeadlineAt = -1f;
                return;
            }

            if (_advancePollJobId != jobId)
            {
                _advancePollJobId = jobId;
                _advanceDeadlineAt = -1f;
            }

            CarLoader loader = FindCarLoaderByOrder(jobId);
            if (loader == null)
            {
                return;
            }

            if (_advanceDeadlineAt < 0f)
            {
                _advanceDeadlineAt = Time.realtimeSinceStartup + AdvanceCarTimeoutSeconds;
            }
            else if (Time.realtimeSinceStartup > _advanceDeadlineAt)
            {
                MelonLogger.Warning("[BuildJob] advance abandoned: id=" + jobId
                    + " car price unknown after " + AdvanceCarTimeoutSeconds + "s of waiting for the job car");
                _advanceDeadlineAt = -1f;
                BuildJobState.ClearAdvancePending();
                return;
            }

            if (!TryFinishedPrice(loader, out int price, out string source))
            {
                return;
            }

            int reward = ClampReward(Mathf.RoundToInt(price * config.BuildJobRewardMultiplier));
            int advance = Mathf.RoundToInt(reward * config.BuildJobAdvancePercent / 100f);
            if (advance < 0)
            {
                advance = 0;
            }

            if (advance > reward)
            {
                advance = reward;
            }

            if (advance <= 0)
            {
                BuildJobState.ClearAdvancePending();
                return;
            }

            try
            {
                GlobalData.AddPlayerMoney(advance);
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] advance payout failed, will retry: id=" + jobId + " " + e.Message);
                return;
            }

            try
            {
                config.RememberAdvance(jobId, advance);
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] advance persist failed, payout already done: " + e.Message);
            }

            BuildJobState.ClearAdvancePending();
            _advanceDeadlineAt = -1f;
            MelonLogger.Msg("[BuildJob] advance paid: id=" + jobId + " finishedPrice=" + price
                + " reward=" + reward + " advance=" + advance + " (" + config.BuildJobAdvancePercent + "%) [" + source + "]");
            BuildJobPopup.Show(BuildJobText.Pick("advancePopup", config.BuildJobAdvancePopup) + ": " + advance + " CR ("
                + config.BuildJobAdvancePercent + "%) - "
                + BuildJobText.Pick("advanceSuffix", config.BuildJobLabelAdvanceSuffix));
        }

        internal static int AdvanceFor(int jobId)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || config.BuildJobAdvanceJobId != jobId)
            {
                return 0;
            }

            return config.BuildJobAdvanceAmount > 0 ? config.BuildJobAdvanceAmount : 0;
        }

        internal static CarLoader FindCarLoaderByOrder(int jobId)
        {
            if (jobId < 0)
            {
                return null;
            }

            try
            {
                CarLoaderPlaces places = CarLoaderPlaces.Get();
                if (places == null)
                {
                    return null;
                }

                int count = places.GetCarLoadersCount();
                for (int i = 0; i < count; i++)
                {
                    CarLoader loader = places.GetCarLoaderByIndex(i);
                    if (loader != null && loader.orderConnection == jobId)
                    {
                        return loader;
                    }
                }
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] car lookup by order failed: " + e.Message);
            }

            return null;
        }

        internal static bool HasCarForOrder(int jobId)
        {
            return FindCarLoaderByOrder(jobId) != null;
        }

        internal static void ForfeitAdvance(int jobId, string tag)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null)
            {
                return;
            }

            int advance = AdvanceFor(jobId);
            if (advance > 0)
            {
                try
                {
                    GlobalData.AddPlayerMoney(-advance);
                    config.ClearAdvance();
                    MelonLogger.Msg("[BuildJob] advance clawed back: id=" + jobId + " amount=" + advance + " (" + tag + ")");
                }
                catch (System.Exception e)
                {
                    MelonLogger.Warning("[BuildJob] advance clawback failed, marker kept for retry: id=" + jobId + " " + e.Message);
                }
            }
        }

        private static void RemoveJob(OrderGenerator generator, int jobId)
        {
            try
            {
                Il2CppSystem.Collections.Generic.List<Job> jobs = generator.Jobs;
                if (jobs == null)
                {
                    return;
                }

                for (int i = 0; i < jobs.Count; i++)
                {
                    Job job = jobs[i];
                    if (job != null && job.id == jobId)
                    {
                        jobs.RemoveAt(i);
                        return;
                    }
                }
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] remove expired offer failed: " + e.Message);
            }
        }

        private static int CountBuildJobs(OrderGenerator generator)
        {
            try
            {
                Il2CppSystem.Collections.Generic.List<Job> jobs = generator.Jobs;
                if (jobs == null)
                {
                    return 0;
                }

                int count = 0;
                for (int i = 0; i < jobs.Count; i++)
                {
                    if (IsBuildJob(jobs[i]))
                    {
                        count++;
                    }
                }

                return count;
            }
            catch
            {
                return 0;
            }
        }

    }
}
