using MelonLoader;
using System;
using UnityEngine;

namespace RealShop
{
    internal static partial class BuildJobCondition
    {
        internal const int StageArrival = 1;
        internal const int StageEngine = 2;
        internal const int StagePrepare = 3;
        internal const int StageWatchdog = 4;

        private const int MaxAttemptsPerSource = 3;
        private const int PersistStage = StagePrepare;

        private static int _appliedJobId = -1;
        private static int _appliedStage;
        private static int _attemptJobId = -1;
        private static string _attemptTag = null;
        private static int _attempts;
        private static int _waitingJobId = -1;
        private static int _gaveUpJobId = -1;
        private static int _persistedHitLoggedJobId = -1;
        private static int _reapplyJobId = -1;
        private static int _reapplyCount;
        private static int _reapplyCapLoggedJobId = -1;

        internal static int Percent()
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || !RealParts.ModIsEnabled || !config.EnableBuildFromScratchJobs)
            {
                return 0;
            }

            if (string.Equals(config.BuildJobStripMode, "off", StringComparison.OrdinalIgnoreCase))
            {
                return 0;
            }

            int percent = config.BuildJobStripConditionPercent;
            if (percent <= 0)
            {
                return 0;
            }

            return percent > 100 ? 100 : percent;
        }

        internal static int DustPercent()
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || !RealParts.ModIsEnabled || !config.EnableBuildFromScratchJobs)
            {
                return 0;
            }

            if (string.Equals(config.BuildJobStripMode, "off", StringComparison.OrdinalIgnoreCase))
            {
                return 0;
            }

            int percent = config.BuildJobStripDustPercent;
            if (percent <= 0)
            {
                return 0;
            }

            return percent > 100 ? 100 : percent;
        }

        internal static bool Active()
        {
            return Percent() > 0;
        }

        internal static float ConditionValue()
        {
            return Percent() / 100f;
        }

        internal static float DustValue()
        {
            return DustPercent() / 100f;
        }

        internal static void ClearAppliedMarker(int jobId)
        {
            if (_appliedJobId == jobId || _attemptJobId == jobId)
            {
                _appliedJobId = -1;
                _appliedStage = 0;
                _attemptJobId = -1;
                _attemptTag = null;
                _attempts = 0;
            }

            if (_waitingJobId == jobId || _gaveUpJobId == jobId || _persistedHitLoggedJobId == jobId
                || _reapplyCapLoggedJobId == jobId)
            {
                _waitingJobId = -1;
                _gaveUpJobId = -1;
                _persistedHitLoggedJobId = -1;
                _reapplyCapLoggedJobId = -1;
            }

            Config config = RealParts.ActiveConfig;
            if (config == null || config.BuildJobConditionJobId != jobId)
            {
                return;
            }

            try
            {
                config.ClearConditionJob();
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] condition marker clear failed: job=" + jobId + " " + e.Message);
            }
        }

        internal static bool TryApply(CarLoader loader, int jobId, string tag, int stage)
        {
            int percent = Percent();
            if (percent == 0 || loader == null || jobId < 0)
            {
                return false;
            }

            if (!IsGuardOpen(jobId))
            {
                return false;
            }

            if (IsPersisted(jobId))
            {
                return true;
            }

            if (_appliedJobId == jobId && stage <= _appliedStage)
            {
                return true;
            }

            if (_appliedJobId == jobId && !ReserveReapply(jobId, stage))
            {
                return true;
            }

            Il2CppSystem.Collections.Generic.List<CarPart> carParts = null;
            try
            {
                carParts = loader.carParts;
            }
            catch (System.Exception e)
            {
                LogWaiting(jobId, tag, "carParts read failed: " + e.Message);
                return false;
            }

            if (carParts == null || carParts.Count == 0)
            {
                LogWaiting(jobId, tag, "the car has no parts yet");
                return false;
            }

            if (_attemptJobId != jobId || !string.Equals(_attemptTag, tag, StringComparison.Ordinal))
            {
                _attemptJobId = jobId;
                _attemptTag = tag;
                _attempts = 0;
            }

            if (_attempts >= MaxAttemptsPerSource)
            {
                if (_gaveUpJobId != jobId)
                {
                    _gaveUpJobId = jobId;
                    MelonLogger.Warning("[BuildJob] condition strip abandoned: job=" + jobId
                        + " attempts=" + MaxAttemptsPerSource + " via=" + tag);
                }

                return false;
            }

            _attempts++;

            float condition = percent / 100f;
            int dustPercent = DustPercent();
            float dust = dustPercent / 100f;
            int parts = 0;

            bool setConditionAllOk = true;
            try
            {
                loader.SetConditionAll(condition);
            }
            catch (System.Exception e)
            {
                setConditionAllOk = false;
                MelonLogger.Warning("[BuildJob] condition strip: SetConditionAll failed job=" + jobId
                    + " " + e.Message + " (attempt " + _attempts + ")");
            }

            try
            {
                for (int i = 0; i < carParts.Count; i++)
                {
                    CarPart part = carParts[i];
                    if (part == null)
                    {
                        continue;
                    }

                    part.Condition = condition;
                    part.ConditionPaint = condition;
                    if (!KeepFrame())
                    {
                        part.StructureCondition = condition;
                    }
                    if (dustPercent > 0)
                    {
                        part.Dust = dust;
                    }

                    parts++;
                }
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] condition strip failed: job=" + jobId + " " + e.Message
                    + " (attempt " + _attempts + ")");
                return false;
            }

            if (parts == 0)
            {
                LogWaiting(jobId, tag, "no readable part in the list");
                return false;
            }

            if (!setConditionAllOk)
            {
                MelonLogger.Warning("[BuildJob] condition strip not confirmed: job=" + jobId
                    + " SetConditionAll failed, parts=" + parts + " via=" + tag);
                return false;
            }

            bool first = _appliedJobId != jobId;
            ExtendGuard(jobId);
            bool engineHidden = HideEngineObject(loader, tag);
            int unmounted = UnmountByPrefix(loader, tag);
            int wheelsHidden = HideWheels(loader, tag);
            int suspension = StripSuspension(loader, tag);
            int suspObjects = HideSuspensionObjects(loader, tag);
            bool pending = first && unmounted == 0 && wheelsHidden == 0 && suspension <= 0 && suspObjects == 0
                && StripWritesRequested();
            LogEngineState(jobId, engineHidden);
            if (!pending)
            {
                _appliedJobId = jobId;
                _appliedStage = stage;
                _gaveUpJobId = -1;
                _attemptJobId = -1;
                _attemptTag = null;
                _attempts = 0;
                if (stage >= PersistStage)
                {
                    TryPersist(jobId);
                }
            }

            MelonLogger.Msg("[BuildJob] condition strip: job=" + jobId + " percent=" + percent
                + "% dust=" + dustPercent + "% parts=" + parts + " stage=" + stage
                + " frame=" + (KeepFrame() ? "kept" : percent + "%")
                + " engine=" + (engineHidden ? "hidden" : "visible")
                + " unmounted=" + unmounted + " wheels=" + wheelsHidden + " suspension=" + suspension
                + " suspObjects=" + suspObjects
                + " " + WheelState(loader)
                + " pending=" + (pending ? "writes" : "no")
                + (first ? "" : " reapply=" + _reapplyCount)
                + " attempts=" + _attempts + " via=" + tag);
            return true;
        }

        private static bool ReserveReapply(int jobId, int stage)
        {
            if (stage != StageWatchdog)
            {
                return true;
            }

            int cap = ReapplyMax();
            if (cap <= 0)
            {
                return false;
            }

            if (_reapplyJobId != jobId)
            {
                _reapplyJobId = jobId;
                _reapplyCount = 0;
            }

            if (_reapplyCount >= cap)
            {
                if (_reapplyCapLoggedJobId != jobId)
                {
                    _reapplyCapLoggedJobId = jobId;
                    MelonLogger.Msg("[BuildJob] condition strip reapply cap: job=" + jobId + " max=" + cap);
                }

                return false;
            }

            _reapplyCount++;
            return true;
        }

        private static int ReapplyMax()
        {
            Config config = RealParts.ActiveConfig;
            int max = config != null ? config.BuildJobConditionReapplyMax : Config.ValueBuildJobConditionReapplyMax;
            return max < 0 ? 0 : max;
        }

        private static bool IsPersisted(int jobId)
        {
            Config config = RealParts.ActiveConfig;
            return config != null && config.BuildJobConditionJobId == jobId;
        }

        private static void LogWaiting(int jobId, string tag, string reason)
        {
            if (_waitingJobId == jobId)
            {
                return;
            }

            _waitingJobId = jobId;
            MelonLogger.Msg("[BuildJob] condition strip waiting: job=" + jobId + " " + reason + " via=" + tag);
        }

        private static void TryPersist(int jobId)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null)
            {
                return;
            }

            try
            {
                config.RememberConditionJob(jobId);
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] condition marker persist failed: job=" + jobId + " " + e.Message);
            }
        }
    }
}
