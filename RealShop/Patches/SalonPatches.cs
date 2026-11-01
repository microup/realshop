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
    internal static class FilterSalonCarsByYearPatch
    {
        private static bool _activeLogged;

        private static void Postfix(CarBundleLoader __instance, SceneType sceneType, ref Il2CppSystem.Collections.Generic.List<CarsIdWithConfig> __result)
        {
            if ((sceneType != SceneType.Salon && sceneType != SceneType.Showroom) || __result == null)
            {
                return;
            }

            Config config = RealParts.ActiveConfig;
            if (config == null)
            {
                return;
            }

            if (!_activeLogged)
            {
                _activeLogged = true;
                MelonLogger.Msg("[Salon] car pool year filter active (GetCarsForScene " + sceneType + ")");
            }

            int minYearInclusive = (int)config.RemoveCarsOlderThanYear;
            int removed = 0;

            for (int i = __result.Count - 1; i >= 0; i--)
            {
                CarsIdWithConfig car = __result[i];
                int carYear;
                if (!CarData.TryGetYearByCarId(car.CarID, out carYear))
                {
                    carYear = CarBundleLoaderExtension.GetCarYear(__instance, car.CarID, car.ConfigVersion);
                }

                if (carYear <= 0 || carYear < minYearInclusive)
                {
                    __result.RemoveAt(i);
                    removed++;
                }
            }

            MelonLogger.Msg("[Salon] pool year filter: scene=" + sceneType + " minYear=" + minYearInclusive
                + " kept=" + __result.Count + " removed=" + removed);
        }
    }
}
