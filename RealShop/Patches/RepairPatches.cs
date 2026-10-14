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
            return RealParts.IsFullyRepaired(item.GetConditionToShow());
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
                if (item == null || !(item.GetConditionToShow() < 0.995f))
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
            return item == null || !RealParts.IsFullyRepaired(item.GetConditionToShow());
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

            partInfo.SuccessCondition = FixSuccessCondition(currentCondition, partInfo.SuccessCondition);
            if (partInfo.DentCurrentCondition > 0f || partInfo.DentSuccessCondition > 0f)
            {
                partInfo.DentSuccessCondition = FixSuccessCondition(partInfo.DentCurrentCondition, partInfo.DentSuccessCondition);
            }
        }

        private static float FixSuccessCondition(float currentCondition, float successCondition)
        {
            if (successCondition > currentCondition)
            {
                return successCondition;
            }

            float cap = currentCondition <= 1f ? 1f : (float)RealParts.FullyRepairedPartConditionToShow;
            float inc = currentCondition <= 1f ? RealParts.MinimumRepairConditionIncrease / 100f : RealParts.MinimumRepairConditionIncrease;
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

    [HarmonyPatch(typeof(RepairPartWindow), "ProcessGameResult")]
    internal static class LaborCostOnRepairPatch
    {
        private static void Postfix(RepairPartWindow __instance, BarType result)
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

            string rarity = RetroPartCatalog.GetRarityForPart(item.ID);
            float mult = config.PriceRarityCommon;
            if (rarity == "unique")
            {
                mult = config.PriceRarityUnique;
            }
            else if (rarity == "rare")
            {
                mult = config.PriceRarityRare;
            }
            else if (rarity == "uncommon")
            {
                mult = config.PriceRarityUncommon;
            }

            int fee = Mathf.RoundToInt(config.LaborCostBase * mult);
            MelonLogger.Msg("[Labor] PROCESS: rarity=" + rarity + " base=" + config.LaborCostBase + " mult=" + mult + " fee=" + fee);
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
