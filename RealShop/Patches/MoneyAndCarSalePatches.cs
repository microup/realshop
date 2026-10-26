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
    [HarmonyPatch(typeof(GlobalData), "AddPlayerMoney")]
    internal static class FreeMapTravelPaymentPatch
    {
        private static void Prefix(ref int money)
        {
            if (SellItemCapPatch.PendingSellCap >= 0f)
            {
                int before = money;
                bool clamped = false;
                if (money > 0 && money > SellItemCapPatch.PendingSellCap)
                {
                    money = (int)SellItemCapPatch.PendingSellCap;
                    clamped = true;
                }

                MelonLogger.Msg("[SellCap] DONE: AddPlayerMoney before=" + before + " cap=" + SellItemCapPatch.PendingSellCap + " after=" + money + " clamped=" + clamped);
                SellItemCapPatch.PendingSellCap = -1f;
                return;
            }

            if (FreeTravelService.PendingFreeTravelCost <= 0 || money >= 0)
            {
                return;
            }

            if (money == -FreeTravelService.PendingFreeTravelCost)
            {
                money = 0;
                FreeTravelService.PendingFreeTravelCost = 0;
            }
        }
    }

    [HarmonyPatch(typeof(GlobalData), "SetPlayerMoney")]
    internal static class FreeMapTravelSetMoneyPatch
    {
        private static void Prefix(ref int money)
        {
            if (FreeTravelService.PendingFreeTravelCost <= 0)
            {
                return;
            }

            int currentMoney = GlobalData.PlayerMoney;
            int diff = currentMoney - money;
            if (diff == FreeTravelService.PendingFreeTravelCost)
            {
                money = currentMoney;
                FreeTravelService.PendingFreeTravelCost = 0;
            }
        }
    }

    internal static class StartingMoneyPatch
    {
        internal static bool Pending;
        internal static bool SceneLoaded;

        private static bool Prefix(ref int __result)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || config.StartingMoney <= 0)
            {
                return true;
            }

            __result = config.StartingMoney;
            MelonLogger.Msg("[Money] GetStartingMoney -> " + __result);
            return false;
        }
    }

    internal static class UpdateStartingMoneyPatch
    {
        private static void Postfix()
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || config.StartingMoney <= 0)
            {
                return;
            }

            GlobalData.PlayerMoney = config.StartingMoney;
            StartingMoneyPatch.Pending = true;
            StartingMoneyPatch.SceneLoaded = false;
            MelonLogger.Msg("[Money] UpdateStartingMoney called -> PlayerMoney=" + config.StartingMoney + " (holding until stable)");
        }
    }

    internal static class CarPriceCache
    {
        private static readonly System.Collections.Generic.Dictionary<string, int> BaseByModel = new System.Collections.Generic.Dictionary<string, int>();

        internal static void Remember(string model, int basePrice)
        {
            if (string.IsNullOrEmpty(model) || basePrice <= 0)
            {
                return;
            }

            int old;
            if (BaseByModel.TryGetValue(model, out old) && old == basePrice)
            {
                return;
            }

            BaseByModel[model] = basePrice;
            MelonLogger.Msg("[CarSale] cached baseCarPrice=" + basePrice + " model=" + model);
        }

        internal static bool TryGet(string model, out int basePrice)
        {
            return BaseByModel.TryGetValue(model, out basePrice);
        }
    }

    [HarmonyPatch(typeof(CMS.UI.Logic.Tabs.CarInfo.CarSummaryTab), "SetupForSell")]
    internal static class CarSaleConditionPatch
    {
        private static void Postfix(CMS.UI.Logic.Tabs.CarInfo.CarSummaryTab __instance)
        {
            CarSaleDisplay.Apply(__instance, "setup");
        }
    }

    internal static class CarSaleDisplay
    {
        internal static void Apply(CMS.UI.Logic.Tabs.CarInfo.CarSummaryTab tab, string tag)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || !config.EnableCarSaleCondition || tab == null)
            {
                return;
            }

            try
            {
                int basePrice = tab.baseCarPrice;
                if (basePrice <= 0)
                {
                    MelonLogger.Warning("[CarSale] tag=" + tag + " baseCarPrice=" + basePrice + "; cache skipped");
                    return;
                }

                CarPriceCache.Remember(tab.currentCarLoader != null ? tab.currentCarLoader.carToLoad : null, basePrice);
                MelonLogger.Msg("[CarSale] cache tag=" + tag + " base=" + basePrice + " cond=" + tab.globalCondition);
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[CarSale] failed tag=" + tag + ": " + e.Message);
            }
        }
    }
}
