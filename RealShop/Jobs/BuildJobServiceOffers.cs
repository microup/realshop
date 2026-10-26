using MelonLoader;
using System;
using System.Text;
using UnhollowerBaseLib;
using UnityEngine;


namespace RealShop
{
    internal static partial class BuildJobService
    {
        private static int _tickCounter;
        private static int _advancePollJobId = -1;

        internal static void TickOffers()
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || !config.EnableBuildFromScratchJobs)
            {
                return;
            }

            try
            {
                _tickCounter++;
                if (BuildJobState.AdvancePendingFor >= 0 && _tickCounter % 30 == 0)
                {
                    TryPayPendingAdvance(config);
                }

                int conditionJobId = BuildJobState.ActiveJobId >= 0 ? BuildJobState.ActiveJobId : BuildJobState.ArmJobId;
                if (_tickCounter % 30 == 0 && BuildJobCondition.Active()
                    && conditionJobId >= 0 && BuildJobCondition.IsGuardOpen(conditionJobId))
                {
                    CarLoader jobCar = FindCarLoaderByOrder(conditionJobId);
                    if (jobCar != null)
                    {
                        BuildJobCondition.TryApply(jobCar, conditionJobId, "tick", BuildJobCondition.StageWatchdog);
                    }
                }

                ExpireStaleOffer(config);
                TryScheduledOffer(config);
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] offer tick failed: " + e.Message);
            }
        }

        private static float _nextOfferAt = -1f;
        private const float GeneratorRetrySeconds = 5f;
        private const float FirstOfferSeconds = 5f;

        internal static void ResetOfferSchedule()
        {
            _nextOfferAt = Time.realtimeSinceStartup + FirstOfferSeconds;
            MelonLogger.Msg("[BuildJob] offer schedule armed in " + FirstOfferSeconds + "s");
        }

        private static void TryScheduledOffer(Config config)
        {
            if (BuildJobState.HasActiveJob || BuildJobState.HasOfferedJob)
            {
                return;
            }

            float now = Time.realtimeSinceStartup;
            if (now < _nextOfferAt)
            {
                return;
            }

            if (BuildJobState.Generator == null)
            {
                BuildJobState.SetGenerator(ResolveGenerator());
                if (BuildJobState.Generator == null)
                {
                    _nextOfferAt = now + GeneratorRetrySeconds;
                    WarnGeneratorMissing();
                    return;
                }

                _generatorMissLogged = 0;
                MelonLogger.Msg("[BuildJob] generator resolved by scene lookup");
            }

            _nextOfferAt = now + (config.BuildJobOfferSeconds > 0 ? config.BuildJobOfferSeconds : 1);
            TryOfferJob(config, true);
        }

        private static int _generatorMissLogged;

        private static void WarnGeneratorMissing()
        {
            _generatorMissLogged++;
            if (_generatorMissLogged == 1 || _generatorMissLogged % 12 == 0)
            {
                MelonLogger.Warning("[BuildJob] scheduled offer skipped: OrderGenerator instance not found, retry in "
                    + GeneratorRetrySeconds + "s (attempt " + _generatorMissLogged + ")");
            }
        }

        private static OrderGenerator ResolveGenerator()
        {
            try
            {
                UnityEngine.Object found;
#pragma warning disable CS0618
                found = UnityEngine.Object.FindObjectOfType(OrderGenerator.Il2CppType);
#pragma warning restore CS0618
                return found as OrderGenerator;
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] generator resolve failed: " + e.Message);
                return null;
            }
        }

        private static void ExpireStaleOffer(Config config)
        {
            if (!BuildJobState.HasOfferedJob || BuildJobState.HasActiveJob)
            {
                return;
            }

            if (Time.realtimeSinceStartup < BuildJobState.OfferExpiresAt)
            {
                return;
            }

            int jobId = BuildJobState.OfferedJobId;
            if (BuildJobState.CandidateJobId == jobId)
            {
                MelonLogger.Msg("[BuildJob] offer expiry deferred: id=" + jobId + " accept candidate pending");
                BuildJobState.ExtendOffer(60f);
                return;
            }

            OrderGenerator generator = BuildJobState.Generator;
            if (generator != null)
            {
                RemoveJob(generator, jobId);
            }

            BuildJobState.ClearOfferedJob(jobId);
            MelonLogger.Msg("[BuildJob] offer expired without acceptance: id=" + jobId);
            TryOfferJob(config, true);
        }

    }
}
