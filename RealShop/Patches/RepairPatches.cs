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
    [HarmonyPatch(typeof(Inventory), "GetItemsForRepairTable")]
    internal static class AddRedPartsToRepairTablePatch
    {
        private static void Postfix(Inventory __instance, bool bodyParts, ref Il2CppSystem.Collections.Generic.List<ChoosePartDownItem> __result)
        {
            if (__result == null)
            {
                return;
            }

            var allItems = __instance.GetItems();
            if (allItems == null)
            {
                return;
            }

            RemoveRepairedParts(__result, allItems);
            UnlockRepairTableItems(__result);

            for (int i = 0; i < allItems.Count; i++)
            {
                Item item = allItems[i];
                if (!IsRedRepairablePart(item, bodyParts) || ContainsItem(__result, item))
                {
                    continue;
                }

                var choosePart = new ChoosePartDownItem(item)
                {
                    IsLocked = false
                };

                __result.Add(choosePart);
            }
        }

        private static bool IsRedRepairablePart(Item item, bool bodyParts)
        {
            if (item == null || !RealParts.IsDamagedRed(item.GetConditionToShow()))
            {
                return false;
            }

            PartProperty property = Singleton<GameInventory>.Instance.GetItemProperty(item.ID);
            if (property == null || property.RepairGroup <= 0)
            {
                return false;
            }

            return property.IsBody == bodyParts;
        }

        internal static void RemoveRepairedParts(Il2CppSystem.Collections.Generic.List<ChoosePartDownItem> items, Il2CppSystem.Collections.Generic.List<Item> inventoryItems)
        {
            for (int i = items.Count - 1; i >= 0; i--)
            {
                ChoosePartDownItem current = items[i];
                if (current?.BaseItem != null && IsRepairedItem(current.BaseItem, inventoryItems))
                {
                    items.RemoveAt(i);
                }
            }
        }

        private static bool IsRepairedItem(BaseItem baseItem, Il2CppSystem.Collections.Generic.List<Item> inventoryItems)
        {
            if (IsConditionFullyRepaired(baseItem.GetConditionToShow()))
            {
                return true;
            }

            return IsRepairedItem(baseItem.UID, inventoryItems);
        }

        private static bool IsRepairedItem(long uid, Il2CppSystem.Collections.Generic.List<Item> inventoryItems)
        {
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                Item item = inventoryItems[i];
                if (item != null && item.UID == uid)
                {
                    return IsConditionFullyRepaired(item);
                }
            }

            return false;
        }

        private static bool IsConditionFullyRepaired(Item item)
        {
            return MaxQualityService.IsRepairComplete(item);
        }

        private static bool IsConditionFullyRepaired(float conditionToShow)
        {
            return RealParts.IsFullyRepaired(conditionToShow);
        }

        private static void UnlockRepairTableItems(Il2CppSystem.Collections.Generic.List<ChoosePartDownItem> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                ChoosePartDownItem current = items[i];
                if (current != null)
                {
                    current.IsLocked = false;
                }
            }
        }

        private static bool ContainsItem(Il2CppSystem.Collections.Generic.List<ChoosePartDownItem> items, Item item)
        {
            for (int i = 0; i < items.Count; i++)
            {
                ChoosePartDownItem current = items[i];
                if (current?.BaseItem != null && current.BaseItem.UID == item.UID)
                {
                    return true;
                }
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(RepairPartWindow), "ProcessGameResult")]
    internal static class DestroyPartOnRepairFailPatch
    {
        private static void Prefix(RepairPartWindow __instance, BarType result, out Item __state)
        {
            Item item = __instance.currentItemInfo?.Item;
            __state = result == BarType.Fail ? item : null;
        }

        private static void Postfix(Item __state)
        {
            if (__state != null)
            {
                Inventory inventory = Singleton<Inventory>.Instance;
                if (inventory != null && inventory.FindItemIndex(__state.UID) >= 0)
                {
                    inventory.Delete(__state);
                }
            }
        }
    }

    [HarmonyPatch(typeof(RepairPartWindow), "GetItemsToRepair")]
    internal static class RemoveRepairedPartsFromRepairWindowPatch
    {
        private static void Postfix(ref Il2CppSystem.Collections.Generic.List<ChoosePartDownItem> __result)
        {
            if (__result == null)
            {
                return;
            }

            Inventory inventory = Singleton<Inventory>.Instance;
            if (inventory == null)
            {
                return;
            }

            var allItems = inventory.GetItems();
            if (allItems == null)
            {
                return;
            }

            AddRedPartsToRepairTablePatch.RemoveRepairedParts(__result, allItems);
            AddDamagedPartsToRepairWindow(__result, allItems);
        }

        private static void AddDamagedPartsToRepairWindow(Il2CppSystem.Collections.Generic.List<ChoosePartDownItem> items, Il2CppSystem.Collections.Generic.List<Item> allItems)
        {
            for (int i = 0; i < allItems.Count; i++)
            {
                Item item = allItems[i];
                if (item == null || MaxQualityService.IsRepairComplete(item))
                {
                    continue;
                }

                PartProperty property = Singleton<GameInventory>.Instance.GetItemProperty(item.ID);
                if (property == null || property.RepairGroup <= 0)
                {
                    continue;
                }

                bool present = false;
                for (int j = 0; j < items.Count; j++)
                {
                    if (items[j] != null && items[j].BaseItem != null && items[j].BaseItem.UID == item.UID)
                    {
                        present = true;
                        break;
                    }
                }
                if (present)
                {
                    continue;
                }

                items.Add(new ChoosePartDownItem(item) { IsLocked = false });
            }
        }
    }

    [HarmonyPatch(typeof(RepairPartWindow), "StartMiniGame")]
    internal static class BlockRepairedPartMiniGamePatch
    {
        private static bool Prefix(RepairPartWindow __instance)
        {
            Item item = __instance.currentItemInfo?.Item;
            return item == null || !MaxQualityService.IsRepairComplete(item);
        }
    }

    [HarmonyPatch(typeof(RepairPartWindow), "ProcessGameResult")]
    internal static class CapRepairResultOnProcessGameResultPatch
    {
        private static void Prefix(RepairPartWindow __instance)
        {
            PartInfo partInfo = __instance.currentItemInfo;
            if (partInfo == null || partInfo.Item == null)
            {
                return;
            }

            Config config = RealParts.ActiveConfig;
            if (config == null || !config.EnableMaxQuality)
            {
                return;
            }

            float maxFraction = MaxQualityService.MaxFraction(partInfo.Item);
            partInfo.SuccessCondition = ClampToCap(partInfo.SuccessCondition, maxFraction);
            partInfo.DentSuccessCondition = ClampToCap(partInfo.DentSuccessCondition, maxFraction);
        }

        private static void Postfix(RepairPartWindow __instance)
        {
            Item item = __instance.currentItemInfo?.Item;
            if (item == null)
            {
                return;
            }

            Config config = RealParts.ActiveConfig;
            if (config == null || !config.EnableMaxQuality)
            {
                return;
            }

            float maxFraction = MaxQualityService.MaxFraction(item);
            item.Condition = ClampToCap(item.Condition, maxFraction);
        }

        private static float ClampToCap(float value, float maxFraction)
        {
            if (value <= 0f)
            {
                return value;
            }

            float cap = value > 1f ? maxFraction * 100f : maxFraction;
            return value > cap ? cap : value;
        }
    }

    [HarmonyPatch(typeof(RepairPartWindow), "FillItemData")]
    internal static class FixRepairResultOnFillItemDataPatch
    {
        private static void Postfix(RepairPartWindow __instance)
        {
            RepairResultConditionFixer.Fix(__instance);
        }
    }

    [HarmonyPatch(typeof(RepairPartWindow), "FillItemDataWithAnimation")]
    internal static class FixRepairResultOnFillItemDataWithAnimationPatch
    {
        private static void Postfix(RepairPartWindow __instance)
        {
            RepairResultConditionFixer.Fix(__instance);
        }
    }

    [HarmonyPatch(typeof(RepairPartWindow), "OnDownWindowItemChange")]
    internal static class FixRepairResultOnDownWindowItemChangePatch
    {
        private static void Postfix(RepairPartWindow __instance)
        {
            RepairResultConditionFixer.Fix(__instance);
        }
    }

    internal static class RepairResultConditionFixer
    {
        internal static void Fix(RepairPartWindow window)
        {
            PartInfo partInfo = window.currentItemInfo;
            if (partInfo == null)
            {
                return;
            }

            float currentCondition = partInfo.CurrentCondition > 0f || partInfo.Item == null
                ? partInfo.CurrentCondition
                : partInfo.Item.GetConditionToShow();

            float maxFraction = partInfo.Item != null ? MaxQualityService.MaxFraction(partInfo.Item) : 1f;
            partInfo.SuccessCondition = FixSuccessCondition(currentCondition, partInfo.SuccessCondition, maxFraction);
            if (partInfo.DentCurrentCondition > 0f || partInfo.DentSuccessCondition > 0f)
            {
                partInfo.DentSuccessCondition = FixSuccessCondition(partInfo.DentCurrentCondition, partInfo.DentSuccessCondition, maxFraction);
            }
        }

        private static float FixSuccessCondition(float currentCondition, float successCondition, float maxFraction)
        {
            bool percentScale = currentCondition > 1f;
            float cap = percentScale ? maxFraction * 100f : maxFraction;
            float inc = percentScale ? RealParts.MinimumRepairConditionIncrease : RealParts.MinimumRepairConditionIncrease / 100f;
            if (successCondition > currentCondition)
            {
                return Mathf.Min(successCondition, cap);
            }

            return Mathf.Min(cap, currentCondition + inc);
        }
    }

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
