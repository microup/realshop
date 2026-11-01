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
    internal static class RepairCostPreviewService
    {
        private static readonly Color ProfitGreen = new Color(0.2f, 0.75f, 0.3f, 1f);
        private static readonly Color LossRed = new Color(0.85f, 0.25f, 0.25f, 1f);

        internal static void Apply(RepairPartWindow window)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || !config.EnableRepairCostPreview || window == null)
            {
                return;
            }

            try
            {
                PartInfo partInfo = window.currentItemInfo;
                Item item = partInfo != null ? partInfo.Item : null;
                if (item == null || MaxQualityService.IsRepairComplete(item))
                {
                    return;
                }

                float cond = item.GetConditionToShow();
                if (cond < 0f)
                {
                    cond = 0f;
                }

                if (cond > 1f)
                {
                    cond = 1f;
                }

                float cap = MaxQualityService.MaxFraction(item);
                float current = partInfo.CurrentCondition > 1f ? partInfo.CurrentCondition / 100f : partInfo.CurrentCondition;
                float success = partInfo.SuccessCondition > 1f ? partInfo.SuccessCondition / 100f : partInfo.SuccessCondition;
                if (success > cap)
                {
                    success = cap;
                }

                float delta = success - current;
                if (delta <= 0f)
                {
                    return;
                }

                int fee = LaborCostService.Fee(config, MaxQualityService.GradeFor(item), cond);
                int total = fee;
                var priceMethod = window.GetType().GetMethod("CalculateRepairPrice", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                if (priceMethod != null)
                {
                    object raw = priceMethod.Invoke(window, new object[] { item });
                    if (raw != null)
                    {
                        total += System.Convert.ToInt32(raw);
                    }
                }

                Text button = window.startButtonText;
                if (button == null || string.IsNullOrEmpty(button.text))
                {
                    return;
                }

                int deltaPct = Mathf.RoundToInt(delta * 100f);
                button.text = button.text + "  +" + deltaPct + "%  -" + total;

                Color gameColor = button.color;
                string profitModel;
                float profitGain;
                bool profitable = MarginService.IsCycleProfitable(item, total, delta, out profitModel, out profitGain);

                int feeBase = LaborCostService.Fee(config, MaxQualityService.GradeFor(item), 0f);
                int baseCost = (total - fee) + feeBase;
                string baseModel;
                float baseGain;
                bool profitableAtBase = MarginService.IsCycleProfitable(item, baseCost, delta, out baseModel, out baseGain);

                string verdict;
                if (profitable)
                {
                    button.color = ProfitGreen;
                    verdict = "green";
                }
                else if (profitableAtBase)
                {
                    button.color = LossRed;
                    verdict = "red";
                }
                else
                {
                    button.color = gameColor;
                    verdict = "neutral";
                }

                MelonLogger.Msg("[RepairPreview] id=" + item.ID + " cond=" + cond + " cap=" + cap + " delta=" + deltaPct + "% fee=" + fee + " total=" + total + " gain=" + profitGain + " model=" + profitModel + " profitable=" + profitable + " feeBase=" + feeBase + " verdict=" + verdict);
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[RepairPreview] failed: " + e.Message);
            }
        }
    }

    [HarmonyPatch(typeof(RepairPartWindow), "FillItemData")]
    internal static class RepairCostPreviewOnFillItemDataPatch
    {
        private static void Postfix(RepairPartWindow __instance)
        {
            RepairCostPreviewService.Apply(__instance);
        }
    }

    [HarmonyPatch(typeof(RepairPartWindow), "FillItemDataWithAnimation")]
    internal static class RepairCostPreviewOnFillItemDataWithAnimationPatch
    {
        private static void Postfix(RepairPartWindow __instance)
        {
            RepairCostPreviewService.Apply(__instance);
        }
    }

    [HarmonyPatch(typeof(RepairPartWindow), "OnDownWindowItemChange")]
    internal static class RepairCostPreviewOnDownWindowItemChangePatch
    {
        private static void Postfix(RepairPartWindow __instance)
        {
            RepairCostPreviewService.Apply(__instance);
        }
    }
}
