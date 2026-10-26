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
    [HarmonyPatch(typeof(RepairPartWindow), "ProcessGameResult")]
    internal static class RefreshRepairListAfterRepairPatch
    {
        private static void Postfix(RepairPartWindow __instance, BarType result)
        {
            if (result != BarType.Success || __instance == null)
            {
                return;
            }

            try
            {
                Item item = __instance.currentItemInfo?.Item;
                string id = item != null ? item.ID : "<null>";
                float cond = item != null ? item.Condition : -1f;
                float show = item != null ? item.GetConditionToShow() : -1f;

                var update = typeof(RepairPartWindow).GetMethod("UpdateItems", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                if (update != null)
                {
                    update.Invoke(__instance, null);
                    MelonLogger.Msg("[RepairList] UpdateItems() after success: id=" + id + " cond=" + cond + " show=" + show);
                }
                else
                {
                    MelonLogger.Warning("[RepairList] UpdateItems not found");
                }
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[RepairList] refresh failed: " + e.Message);
            }
        }
    }

    internal static class LaborCostService
    {
        internal static float MultFor(Config config, string rarity)
        {
            if (rarity == "unique")
            {
                return config.PriceRarityUnique;
            }

            if (rarity == "rare")
            {
                return config.PriceRarityRare;
            }

            if (rarity == "uncommon")
            {
                return config.PriceRarityUncommon;
            }

            return config.PriceRarityCommon;
        }

        internal static int Fee(Config config, string rarity, float cond)
        {
            if (config == null || config.LaborCostBase <= 0)
            {
                return 0;
            }

            float mult = MultFor(config, rarity);
            float curve = 1f;
            if (config.EnableLaborCurve)
            {
                if (cond < 0f)
                {
                    cond = 0f;
                }

                if (cond > config.LaborCurveMaxCond)
                {
                    cond = config.LaborCurveMaxCond;
                }

                curve = 1f + config.LaborCurveK * cond / (1f - cond);
            }

            return Mathf.RoundToInt(config.LaborCostBase * mult * curve);
        }
    }

    [HarmonyPatch(typeof(RepairPartWindow), "ProcessGameResult")]
    internal static class LaborCostOnRepairPatch
    {
        private static void Prefix(RepairPartWindow __instance, out float __state)
        {
            Item item = __instance != null ? __instance.currentItemInfo?.Item : null;
            __state = item != null ? item.GetConditionToShow() : -1f;
        }

        private static void Postfix(RepairPartWindow __instance, BarType result, float __state)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || !config.EnableLaborCost || config.LaborCostBase <= 0)
            {
                return;
            }

            if (result != BarType.Success)
            {
                return;
            }

            Item item = __instance.currentItemInfo?.Item;
            MelonLogger.Msg("[Labor] START: repair success, item=" + (item != null ? item.ID.ToString() : "<null>"));
            if (item == null)
            {
                MelonLogger.Msg("[Labor] DONE: skipped (no item)");
                return;
            }

            string rarity = MaxQualityService.GradeFor(item);
            float cond = __state >= 0f ? __state : item.GetConditionToShow();
            int fee = LaborCostService.Fee(config, rarity, cond);
            MelonLogger.Msg("[Labor] PROCESS: rarity=" + rarity + " base=" + config.LaborCostBase + " cond=" + cond + " fee=" + fee);
            if (fee > 0)
            {
                GlobalData.AddPlayerMoney(-fee);
                MelonLogger.Msg("[Labor] DONE: charged " + fee);
            }
            else
            {
                MelonLogger.Msg("[Labor] DONE: fee=0, nothing charged");
            }
        }
    }
}
