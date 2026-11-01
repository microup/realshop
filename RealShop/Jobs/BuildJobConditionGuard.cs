using MelonLoader;
using System;
using UnityEngine;

namespace RealShop
{
    internal static partial class BuildJobCondition
    {
        private const int GuardMaxSeconds = 60;

        private static int _guardJobId = -1;
        private static float _guardOpenedAt = -1f;
        private static float _guardUntil = -1f;
        private static int _overrideLoggedJobId = -1;
        private static int _engineLoggedJobId = -1;
        private static string _overrideFailSignature;

        internal static int GuardJobId
        {
            get { return _guardJobId; }
        }

        internal static void OpenGuard(int jobId)
        {
            if (jobId < 0)
            {
                return;
            }

            if (Percent() == 0)
            {
                CloseGuard("OpenGuard: percent 0");
                return;
            }

            float now = Time.realtimeSinceStartup;
            _guardJobId = jobId;
            _guardOpenedAt = now;
            _guardUntil = now + GuardSeconds();
            _reapplyJobId = -1;
            _reapplyCount = 0;
            _reapplyCapLoggedJobId = -1;
            _overrideLoggedJobId = -1;
            MelonLogger.Msg("[BuildJob] condition guard open: job=" + jobId + " seconds=" + GuardSeconds());
        }

        internal static void CloseGuard(string tag)
        {
            if (_guardJobId < 0)
            {
                return;
            }

            MelonLogger.Msg("[BuildJob] condition guard closed: job=" + _guardJobId + " (" + tag + ")");
            _guardJobId = -1;
            _guardOpenedAt = -1f;
            _guardUntil = -1f;
        }

        internal static bool IsGuardOpen(int jobId)
        {
            if (_guardJobId < 0 || jobId < 0 || _guardJobId != jobId)
            {
                return false;
            }

            if (Time.realtimeSinceStartup > _guardUntil)
            {
                CloseGuard("guard timeout");
                return false;
            }

            return true;
        }

        internal static bool GuardApplies(CarLoader loader)
        {
            if (loader == null || _guardJobId < 0 || Percent() == 0)
            {
                return false;
            }

            if (Time.realtimeSinceStartup > _guardUntil)
            {
                CloseGuard("guard timeout");
                return false;
            }

            return loader.orderConnection == _guardJobId;
        }

        internal static bool ForceCondition(CarLoader loader, ref float condition, string tag)
        {
            try
            {
                if (!GuardApplies(loader))
                {
                    return false;
                }

                condition = ConditionValue();
                LogOverride(tag);
                return true;
            }
            catch (System.Exception e)
            {
                LogOverrideFailure(tag, e);
                return false;
            }
        }

        internal static bool ForceDust(CarLoader loader, ref float dust, string tag)
        {
            try
            {
                if (!GuardApplies(loader) || DustPercent() == 0)
                {
                    return false;
                }

                dust = DustValue();
                LogOverride(tag);
                return true;
            }
            catch (System.Exception e)
            {
                LogOverrideFailure(tag, e);
                return false;
            }
        }

        internal static bool ForceRange(CarLoader loader, ref float min, ref float max, string tag)
        {
            try
            {
                if (!GuardApplies(loader))
                {
                    return false;
                }

                float value = ConditionValue();
                min = value;
                max = value;
                LogOverride(tag);
                return true;
            }
            catch (System.Exception e)
            {
                LogOverrideFailure(tag, e);
                return false;
            }
        }

        internal static bool InteriorAbsent(CarLoader loader)
        {
            if (loader == null || !StripSuppresses("CreateInterior"))
            {
                return false;
            }

            int jobId = loader.orderConnection;
            if (jobId < 0)
            {
                return false;
            }

            if (_guardJobId == jobId)
            {
                return true;
            }

            Config config = RealParts.ActiveConfig;
            return config != null && config.BuildJobConditionJobId == jobId;
        }

        internal static bool ForceInteriorCondition(CarLoader loader, ref float condition, string tag)
        {
            try
            {
                bool absent = InteriorAbsent(loader);
                if (!absent && !GuardApplies(loader))
                {
                    return false;
                }

                condition = absent ? 0f : ConditionValue();
                LogOverride(tag);
                return true;
            }
            catch (System.Exception e)
            {
                LogOverrideFailure(tag, e);
                return false;
            }
        }

        internal static bool ForceDetailsCondition(CarLoader loader, ref float condition, string tag)
        {
            try
            {
                if (!GuardApplies(loader))
                {
                    return false;
                }

                condition = StripSuppresses("CreateParts") ? 0f : ConditionValue();
                LogOverride(tag);
                return true;
            }
            catch (System.Exception e)
            {
                LogOverrideFailure(tag, e);
                return false;
            }
        }

        internal static bool KeepFrame()
        {
            Config config = RealParts.ActiveConfig;
            return config == null || config.BuildJobStripKeepFrame;
        }

        internal static bool StripSuppresses(string methodName)
        {
            try
            {
                return BuildJobStripTarget.IsMethodEnabled(methodName);
            }
            catch
            {
                return false;
            }
        }

        internal static float[] SnapshotStructure(CarLoader loader)
        {
            try
            {
                if (!GuardApplies(loader) || !KeepFrame())
                {
                    return null;
                }

                Il2CppSystem.Collections.Generic.List<CarPart> parts = loader.carParts;
                if (parts == null || parts.Count == 0)
                {
                    return null;
                }

                float[] values = new float[parts.Count];
                for (int i = 0; i < parts.Count; i++)
                {
                    CarPart part = parts[i];
                    values[i] = part != null ? part.StructureCondition : -1f;
                }

                return values;
            }
            catch
            {
                return null;
            }
        }

        internal static void RestoreStructure(CarLoader loader, float[] values)
        {
            if (values == null || loader == null)
            {
                return;
            }

            try
            {
                Il2CppSystem.Collections.Generic.List<CarPart> parts = loader.carParts;
                if (parts == null || parts.Count != values.Length)
                {
                    return;
                }

                for (int i = 0; i < parts.Count; i++)
                {
                    CarPart part = parts[i];
                    if (part != null && values[i] >= 0f)
                    {
                        part.StructureCondition = values[i];
                    }
                }
            }
            catch
            {
            }
        }

        internal static bool HideEngineObject(CarLoader loader, string tag)
        {
            try
            {
                if (!GuardApplies(loader))
                {
                    return false;
                }

                Config config = RealParts.ActiveConfig;
                if (config != null && !config.BuildJobStripEngineObject)
                {
                    return false;
                }

                GameObject engine = loader.GetEngine();
                if (engine == null)
                {
                    return false;
                }

                if (engine.activeSelf)
                {
                    engine.SetActive(false);
                }

                return true;
            }
            catch (System.Exception e)
            {
                LogOverrideFailure(tag, e);
                return false;
            }
        }

        private static void LogEngineState(int jobId, bool hidden)
        {
            if (_engineLoggedJobId == jobId)
            {
                return;
            }

            _engineLoggedJobId = jobId;
            MelonLogger.Msg("[BuildJob] engine object: job=" + jobId + " " + (hidden ? "hidden" : "visible")
                + " (BuildJobStripEngineObject)");
        }

        private static void ExtendGuard(int jobId)
        {
            if (_guardJobId != jobId || _guardOpenedAt < 0f)
            {
                return;
            }

            float now = Time.realtimeSinceStartup;
            float until = now + GuardSeconds();
            float cap = _guardOpenedAt + (IsPersisted(jobId) ? GuardKeepSeconds() : GuardMaxSeconds);
            _guardUntil = until > cap ? cap : until;
        }

        private static int GuardKeepSeconds()
        {
            Config config = RealParts.ActiveConfig;
            int seconds = config != null
                ? config.BuildJobGuardKeepSeconds
                : Config.ValueBuildJobGuardKeepSeconds;
            return seconds > 0 ? seconds : Config.ValueBuildJobGuardKeepSeconds;
        }

        private static int GuardSeconds()
        {
            Config config = RealParts.ActiveConfig;
            int seconds = config != null
                ? config.BuildJobConditionGuardSeconds
                : Config.ValueBuildJobConditionGuardSeconds;
            if (seconds <= 0)
            {
                return Config.ValueBuildJobConditionGuardSeconds;
            }

            return seconds > GuardMaxSeconds ? GuardMaxSeconds : seconds;
        }

        private static void LogOverride(string tag)
        {
            if (_overrideLoggedJobId == _guardJobId)
            {
                return;
            }

            _overrideLoggedJobId = _guardJobId;
            MelonLogger.Msg("[BuildJob] condition guard: game writes forced to " + Percent()
                + "% (first via " + tag + ")");
        }

        private static void LogOverrideFailure(string tag, System.Exception e)
        {
            string signature = tag + "|" + e.Message;
            if (string.Equals(_overrideFailSignature, signature, StringComparison.Ordinal))
            {
                return;
            }

            _overrideFailSignature = signature;
            MelonLogger.Warning("[BuildJob] condition guard failed: via=" + tag + " " + e.Message);
        }
    }
}
