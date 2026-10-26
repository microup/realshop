using Harmony;
using MelonLoader;
using System;
using UnhollowerBaseLib;

#pragma warning disable CS0618

namespace RealShop
{
    [HarmonyPatch(typeof(CarLoader), "SetCondition")]
    internal static class BuildJobConditionOneGuardPatch
    {
        [HarmonyPriority(Priority.Last)]
        private static void Prefix(CarLoader __instance, [HarmonyArgument(1)] ref float condition)
        {
            BuildJobCondition.ForceCondition(__instance, ref condition, "SetCondition");
        }
    }

    [HarmonyPatch(typeof(CarLoader), "SetConditionOnBody", new[] { typeof(float) })]
    internal static class BuildJobConditionBodyGuardPatch
    {
        [HarmonyPriority(Priority.Last)]
        private static void Prefix(CarLoader __instance, [HarmonyArgument(0)] ref float newCondition)
        {
            BuildJobCondition.ForceCondition(__instance, ref newCondition, "SetConditionOnBody");
        }
    }

    [HarmonyPatch(typeof(CarLoader), "SetConditionOnBody", new[] { typeof(float), typeof(Il2CppStringArray) })]
    internal static class BuildJobConditionBodyMaterialsGuardPatch
    {
        [HarmonyPriority(Priority.Last)]
        private static void Prefix(CarLoader __instance, [HarmonyArgument(0)] ref float newCondition)
        {
            BuildJobCondition.ForceCondition(__instance, ref newCondition, "SetConditionOnBody(materials)");
        }
    }

    [HarmonyPatch(typeof(CarLoader), "SetConditionOnDetails", new[] { typeof(float), typeof(Il2CppStringArray) })]
    internal static class BuildJobConditionDetailsMaterialsGuardPatch
    {
        [HarmonyPriority(Priority.Last)]
        private static void Prefix(CarLoader __instance, [HarmonyArgument(0)] ref float newCondition)
        {
            BuildJobCondition.ForceDetailsCondition(__instance, ref newCondition, "SetConditionOnDetails(materials)");
        }
    }

    [HarmonyPatch(typeof(CarLoader), "SetRandomPartsConditions")]
    internal static class BuildJobRandomPartsConditionGuardPatch
    {
        [HarmonyPriority(Priority.Last)]
        private static void Prefix(CarLoader __instance, [HarmonyArgument(0)] ref float min,
            [HarmonyArgument(1)] ref float max)
        {
            BuildJobCondition.ForceRange(__instance, ref min, ref max, "SetRandomPartsConditions");
        }
    }

    [HarmonyPatch(typeof(CarLoader), "SetMountObjectsRandomCondition")]
    internal static class BuildJobMountConditionGuardPatch
    {
        [HarmonyPriority(Priority.Last)]
        private static void Prefix(CarLoader __instance, [HarmonyArgument(0)] ref float carCondition)
        {
            BuildJobCondition.ForceCondition(__instance, ref carCondition, "SetMountObjectsRandomCondition");
        }
    }

    [HarmonyPatch(typeof(CarLoader), "TweenInteriorConditionAndDust")]
    internal static class BuildJobInteriorTweenGuardPatch
    {
        [HarmonyPriority(Priority.Last)]
        private static void Prefix(CarLoader __instance, [HarmonyArgument(0)] ref float targetCondition,
            [HarmonyArgument(1)] ref float targetDust)
        {
            if (BuildJobCondition.ForceInteriorCondition(__instance, ref targetCondition, "TweenInteriorConditionAndDust"))
            {
                BuildJobCondition.ForceDust(__instance, ref targetDust, "TweenInteriorConditionAndDust");
            }
        }
    }

    [HarmonyPatch(typeof(CarLoader), "CreateEngine")]
    internal static class BuildJobConditionEnginePatch
    {
        private static bool _activeLogged;

        private static void Postfix(CarLoader __instance)
        {
            try
            {
                if (!BuildJobCondition.Active())
                {
                    return;
                }

                int jobId = BuildJobState.ActiveJobId >= 0 ? BuildJobState.ActiveJobId : BuildJobState.ArmJobId;
                if (jobId < 0)
                {
                    return;
                }

                if (!_activeLogged)
                {
                    _activeLogged = true;
                    MelonLogger.Msg("[BuildJob] hook active: CreateEngine condition stage="
                        + BuildJobCondition.StageEngine);
                }

                BuildJobCondition.TryApply(__instance, jobId, "CreateEngine", BuildJobCondition.StageEngine);
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] CreateEngine condition hook failed: " + e.Message);
            }
        }
    }

    [HarmonyPatch(typeof(CarLoader), "CreateEngineAsync")]
    internal static class BuildJobConditionEngineAsyncPatch
    {
        private static void Postfix(CarLoader __instance)
        {
            try
            {
                if (!BuildJobCondition.Active())
                {
                    return;
                }

                int jobId = BuildJobState.ActiveJobId >= 0 ? BuildJobState.ActiveJobId : BuildJobState.ArmJobId;
                if (jobId < 0)
                {
                    return;
                }

                BuildJobCondition.TryApply(__instance, jobId, "CreateEngineAsync", BuildJobCondition.StageEngine);
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] CreateEngineAsync condition hook failed: " + e.Message);
            }
        }
    }

    [HarmonyPatch(typeof(OrderGenerator), "PrepareJob", new[] { typeof(CarLoader), typeof(Job) })]
    internal static class BuildJobConditionPreparePatch
    {
        private static bool _activeLogged;

        private static void Postfix([HarmonyArgument(0)] CarLoader carLoader, [HarmonyArgument(1)] Job job)
        {
            try
            {
                Config config = RealParts.ActiveConfig;
                if (config == null || !config.EnableBuildFromScratchJobs || !BuildJobCondition.Active())
                {
                    return;
                }

                if (job == null || !BuildJobService.IsBuildJob(job) || carLoader == null)
                {
                    return;
                }

                if (!_activeLogged)
                {
                    _activeLogged = true;
                    MelonLogger.Msg("[BuildJob] hook active: PrepareJob condition stage="
                        + BuildJobCondition.StagePrepare);
                }

                BuildJobCondition.TryApply(carLoader, job.id, "PrepareJob", BuildJobCondition.StagePrepare);
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] PrepareJob condition hook failed: " + e.Message);
            }
        }
    }

    [HarmonyPatch(typeof(CarLoader), "GetInteriorCondition")]
    internal static class BuildJobInteriorConditionGuardPatch
    {
        private static bool _activeLogged;

        [HarmonyPriority(Priority.Last)]
        private static bool Prefix(CarLoader __instance, ref float __result)
        {
            if (!_activeLogged)
            {
                _activeLogged = true;
                MelonLogger.Msg("[BuildJob] hook active: GetInteriorCondition");
            }

            return !BuildJobCondition.ForceInteriorCondition(__instance, ref __result, "GetInteriorCondition");
        }
    }

    [HarmonyPatch(typeof(CarLoader), "GetDetailsCondition")]
    internal static class BuildJobDetailsConditionGuardPatch
    {
        private static bool _activeLogged;

        [HarmonyPriority(Priority.Last)]
        private static bool Prefix(CarLoader __instance, ref float __result)
        {
            if (!_activeLogged)
            {
                _activeLogged = true;
                MelonLogger.Msg("[BuildJob] hook active: GetDetailsCondition");
            }

            return !BuildJobCondition.ForceDetailsCondition(__instance, ref __result, "GetDetailsCondition");
        }
    }

    [HarmonyPatch(typeof(CarLoader), "SetConditionAll")]
    internal static class BuildJobStructureKeepGuardPatch
    {
        private static bool _activeLogged;

        private static void Prefix(CarLoader __instance, ref float[] __state)
        {
            if (!_activeLogged)
            {
                _activeLogged = true;
                MelonLogger.Msg("[BuildJob] hook active: SetConditionAll structure keep");
            }

            __state = BuildJobCondition.SnapshotStructure(__instance);
        }

        private static void Postfix(CarLoader __instance, ref float[] __state)
        {
            BuildJobCondition.RestoreStructure(__instance, __state);
            __state = null;
        }
    }
}

#pragma warning restore CS0618
