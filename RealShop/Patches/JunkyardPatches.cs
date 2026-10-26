using CMS.UI.Logic;
using CMS.UI.Logic.Map;
using CMS.UI.Logic.RepairPart;
using CMS.UI.Windows;
using CMS.Containers;
using CMS.Helpers;
using CMS.Extensions;
using Harmony;
using MelonLoader;
using System;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS0618

namespace RealShop
{
    [HarmonyPatch(typeof(CarBundleLoader), "GetCarsForScene")]
    internal static class FilterJunkyardCarsByYearPatch
    {
        internal static readonly System.Collections.Generic.HashSet<string> JunkyardCarIds = new System.Collections.Generic.HashSet<string>();

        private static void Postfix(CarBundleLoader __instance, SceneType sceneType, ref Il2CppSystem.Collections.Generic.List<CarsIdWithConfig> __result)
        {
            if (sceneType != SceneType.Junkyard || __result == null)
            {
                return;
            }

            Config config = RealParts.ActiveConfig;
            if (config == null)
            {
                return;
            }

            int maxYearExclusive = (int)config.RemoveCarsOlderThanYear;

            for (int i = __result.Count - 1; i >= 0; i--)
            {
                CarsIdWithConfig car = __result[i];
                int carYear;
                if (!CarData.TryGetYearByCarId(car.CarID, out carYear))
                {
                    carYear = CarBundleLoaderExtension.GetCarYear(__instance, car.CarID, car.ConfigVersion);
                }

                if (carYear <= 0 || carYear >= maxYearExclusive)
                {
                    __result.RemoveAt(i);
                }
            }

            JunkyardCarIds.Clear();
            for (int i = 0; i < __result.Count; i++)
            {
                JunkyardCarIds.Add(NormalizeCarId(__result[i].CarID));
            }

            MelonLogger.Msg("[DonorCond] START: junkyard car set populated with " + JunkyardCarIds.Count + " cars (older than cutoff)");
        }

        internal static bool TryGetJunkyardCarId(CarLoader loader, out string id)
        {
            id = null;
            if (loader == null || JunkyardCarIds.Count == 0)
            {
                return false;
            }

            string raw = GetCarLoaderId(loader);
            if (string.IsNullOrEmpty(raw))
            {
                return false;
            }

            string normalized = NormalizeCarId(raw);
            if (!JunkyardCarIds.Contains(normalized))
            {
                return false;
            }

            id = normalized;
            return true;
        }

        internal static string GetCarLoaderId(CarLoader loader)
        {
            try
            {
                var type = loader.GetType();
                var prop = type.GetProperty("CarID");
                if (prop != null)
                {
                    string value = prop.GetValue(loader, null) as string;
                    if (!string.IsNullOrEmpty(value))
                    {
                        return value;
                    }
                }

                var field = type.GetField("CarID");
                if (field != null)
                {
                    string value = field.GetValue(loader) as string;
                    if (!string.IsNullOrEmpty(value))
                    {
                        return value;
                    }
                }
            }
            catch
            {
            }

            return null;
        }

        internal static string NormalizeCarId(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return id;
            }

            string value = id.ToLowerInvariant();
            if (value.StartsWith("car_"))
            {
                value = value.Substring(4);
            }

            return value;
        }
    }

    [HarmonyPatch(typeof(CarLoader), "SetConditionOnDetails", new[] { typeof(float) })]
    internal static class JunkyardDonorConditionOnDetailsPatch
    {
        private static void Prefix(CarLoader __instance, [HarmonyArgument(0)] ref float condition)
        {
            JunkyardDonorConditionCore.Apply(__instance, ref condition);
            BuildJobCondition.ForceCondition(__instance, ref condition, "SetConditionOnDetails");
        }
    }

    [HarmonyPatch(typeof(CarLoader), "SetConditionAll")]
    internal static class JunkyardDonorConditionAllPatch
    {
        private static void Prefix(CarLoader __instance, [HarmonyArgument(0)] ref float condition)
        {
            JunkyardDonorConditionCore.Apply(__instance, ref condition);
            BuildJobCondition.ForceCondition(__instance, ref condition, "SetConditionAll");
        }
    }

    internal static class JunkyardDonorConditionCore
    {
        private static bool _firstSeenLogged = false;

        internal static void Apply(CarLoader loader, ref float condition)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || !config.EnableDonorCondition || condition <= 0f)
            {
                return;
            }

            if (!_firstSeenLogged)
            {
                _firstSeenLogged = true;
                string probe = FilterJunkyardCarsByYearPatch.GetCarLoaderId(loader);
                MelonLogger.Msg("[DonorCond] PROCESS: SetCondition hook active; junkyardSet=" + FilterJunkyardCarsByYearPatch.JunkyardCarIds.Count + " firstCarId=" + (probe ?? "<null>"));
            }

            string carId;
            if (!FilterJunkyardCarsByYearPatch.TryGetJunkyardCarId(loader, out carId))
            {
                return;
            }

            float before = condition;
            MelonLogger.Msg("[DonorCond] START: donor car=" + carId + " condition=" + before);
            condition = Mathf.Clamp01(condition * config.DonorConditionScale);
            MelonLogger.Msg("[DonorCond] DONE: scale=" + config.DonorConditionScale + " condition " + before + " -> " + condition);
        }
    }
}
