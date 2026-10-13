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
    [HarmonyPatch(typeof(MapWindow), "SubmitItem")]
    internal static class MarkFreeTravelFromMapSubmitPatch
    {
        private static void Prefix(MapDestinationButton item)
        {
            if (item != null && FreeTravelService.IsFreeTravelDestination(item.ID))
            {
                FreeTravelService.MarkFreeTravelPayment(item.ID);
            }
        }
    }

    [HarmonyPatch(typeof(MapWindow), "SubmitItemAction")]
    internal static class MarkFreeTravelFromMapActionPatch
    {
        private static void Prefix(MapWindow __instance)
        {
            if (FreeTravelService.IsFreeTravelDestination(__instance.currentSelectedDestination))
            {
                FreeTravelService.MarkFreeTravelPayment(__instance.currentSelectedDestination);
            }
        }
    }

    [HarmonyPatch(typeof(SceneHelper), "GetLevelForScene")]
    internal static class UnlockEarlyDestinationsGetLevelForScenePatch
    {
        private static void Postfix(SceneType sceneType, ref int __result)
        {
            if (IsUnlockedFromStart(sceneType))
            {
                __result = 1;
            }
        }

        internal static bool IsUnlockedFromStart(SceneType sceneType)
        {
            return sceneType == SceneType.Junkyard || sceneType == SceneType.Auction;
        }
    }

    [HarmonyPatch(typeof(SceneHelper), "CanGoToScene")]
    internal static class UnlockEarlyDestinationsCanGoToScenePatch
    {
        private static void Postfix(SceneType sceneType, ref MapDestinationLockReason reason, int playerLevel, ref bool __result)
        {
            if (UnlockEarlyDestinationsGetLevelForScenePatch.IsUnlockedFromStart(sceneType))
            {
                reason = MapDestinationLockReason.None;
                __result = true;
            }
        }
    }

    [HarmonyPatch(typeof(SceneHelper), "CanDriveToDestination")]
    internal static class UnlockEarlyDestinationsCanDriveToDestinationPatch
    {
        private static void Postfix(MapDestinationID destinationID, ref MapDriveButtonLockReason reason, ref int price, ref bool __result)
        {
            if (destinationID == MapDestinationID.Junkyard || destinationID == MapDestinationID.Auction)
            {
                reason = MapDriveButtonLockReason.None;
                if (FreeTravelService.IsFreeTravelDestination(destinationID))
                {
                    price = 0;
                }

                __result = true;
            }
        }
    }
}
