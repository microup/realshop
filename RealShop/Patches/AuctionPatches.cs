using CMS.Containers;
using CMS.Extensions;
using CMS.Helpers;
using CMS.SceneLoaders;
using Harmony;
using MelonLoader;

#pragma warning disable CS0618

namespace RealShop
{
    [HarmonyPatch(typeof(AuctionHelper), "GetRatingForCar")]
    internal static class AuctionRatingCapPatch
    {
        private static bool _firstSeenLogged;

        private static void Postfix(AuctionType auctionType, ref int __result)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || !config.EnableAuctionRatingCap)
            {
                return;
            }

            int cap = config.AuctionMaxRating;
            if (cap < 1)
            {
                cap = 1;
            }

            if (!_firstSeenLogged)
            {
                _firstSeenLogged = true;
                MelonLogger.Msg("[Auction] hook active: GetRatingForCar cap=" + cap);
            }

            if (__result <= cap)
            {
                return;
            }

            MelonLogger.Msg("[Auction] cap type=" + auctionType + " rating " + __result + " -> " + cap);
            __result = cap;
        }
    }

    [HarmonyPatch(typeof(CarBundleLoader), "GetCarsForScene")]
    internal static class FilterAuctionCarsByYearPatch
    {
        private static void Postfix(CarBundleLoader __instance, SceneType sceneType, ref Il2CppSystem.Collections.Generic.List<CarsIdWithConfig> __result)
        {
            if (sceneType != SceneType.Auction || __result == null)
            {
                return;
            }

            Config config = RealParts.ActiveConfig;
            if (config == null)
            {
                return;
            }

            int maxYearExclusive = (int)config.RemoveCarsOlderThanYear;
            int removed = 0;

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
                    removed++;
                }
            }

            MelonLogger.Msg("[Auction] pool year filter: cutoff=" + maxYearExclusive + " kept=" + __result.Count + " removed=" + removed);
        }
    }

    [HarmonyPatch(typeof(AuctionManager), "GenerateCars")]
    internal static class AuctionCarsYearFilterPatch
    {
        private static void Postfix(AuctionType auctionType, ref Il2CppSystem.Collections.Generic.List<AuctionCarData> __result)
        {
            if (__result == null)
            {
                return;
            }

            Config config = RealParts.ActiveConfig;
            if (config == null)
            {
                return;
            }

            int maxYearExclusive = (int)config.RemoveCarsOlderThanYear;
            int total = __result.Count;
            int kept = 0;

            for (int i = 0; i < total; i++)
            {
                int carYear;
                if (!CarData.TryGetYearByCarId(__result[i].Car, out carYear) || carYear < maxYearExclusive)
                {
                    kept++;
                }
            }

            if (kept == 0)
            {
                MelonLogger.Warning("[Auction] list year filter: all " + total + " generated cars are newer than cutoff=" + maxYearExclusive + " (type=" + auctionType + "); list left untouched");
                return;
            }

            for (int i = total - 1; i >= 0; i--)
            {
                int carYear;
                if (CarData.TryGetYearByCarId(__result[i].Car, out carYear) && carYear >= maxYearExclusive)
                {
                    __result.RemoveAt(i);
                }
            }

            MelonLogger.Msg("[Auction] list year filter: type=" + auctionType + " cutoff=" + maxYearExclusive + " kept=" + kept + " removed=" + (total - kept));
        }
    }
}
