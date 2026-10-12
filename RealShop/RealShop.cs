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
    public class RealParts : MelonMod
    {
        private const string JunkyardMapDestinationButtonName = "MapDestinationButton (5)";
        internal const float RedPartConditionThreshold = 15f;
        internal const int FullyRepairedPartConditionToShow = 100;
        internal const float FullyRepairedPartConditionThreshold = 99.5f;
        internal const float MinimumRepairConditionIncrease = 1f;
        internal const int RepairGroupAvailableFromStart = 1;
        internal const int DefaultJunkyardTravelCost = 500;

        internal static Config ActiveConfig { get; private set; }
        internal static int PendingFreeTravelCost { get; set; }

        private Config _config;
        private bool _modIsEnable = true;
        private bool _isRunOnce = false;

        GameObject _blocker_Repair_Parts_2;
        GameObject _blocker_Repair_Parts_3;

        [Obsolete]
        public override void OnApplicationStart()
        {
            MelonLogger.Msg("initializing...");


            HarmonyInstance harmony = this.Harmony;
            harmony.PatchAll();

            _config = new Config();
            _config.Reload();
            ActiveConfig = _config;
        }


        public override void OnUpdate()
        {
            if (Input.GetKeyDown(_config.IsKeyBindEnableSwitchMode))
            {
                if (_modIsEnable)
                {
                    _modIsEnable = false;
                }
                else
                {
                    _modIsEnable = true;
                }
                UIManager.Get().ShowPopup("[RealShop]", $"the mod has been status: {_modIsEnable}.", PopupType.Normal);

                MelonLogger.Msg($"the mod has been {_modIsEnable}");
            }

            if (!_modIsEnable)
            {
                return;
            }

            RemoveBlocks();
            UnlockJunkyardOnMap();

            GameObject engineRoom = GameObject.Find("EngineRoom");
            if (engineRoom != null)
            {
                var erw = engineRoom.transform.Find("Engine_Room_Workbench_2");
                if (erw != null)
                {
                    var gameobj = erw.gameObject;
                    gameobj?.SetActive(true);
                }

                var erbt = engineRoom.transform.Find("Engine_Room_Big_Table");
                if (erbt != null)
                {
                    var gameobj = erbt.gameObject;
                    gameobj?.SetActive(true);
                }

            }

            GameObject mapDestShop = GameObject.Find("MapDestinationButton (6)");
            if (mapDestShop != null && !_config.IsEnableShop)
            {
                mapDestShop?.SetActive(false);
            }

            GameObject auctionsButton = GameObject.Find("AuctionsButton");
            if (auctionsButton != null && !_config.IsEnableAdditionalAuction)
            {
                auctionsButton?.SetActive(false);
            }

            GameObject shopCarWindow = GameObject.Find("ShopCarWindow");
            if (shopCarWindow != null)
            {
                CheckAndDisableOldCarFromShop(shopCarWindow);

                return;
            }

            GameObject shopMenu = GameObject.Find("ShopMenu");
            if (shopMenu != null && _config.IsEnableBrandFilter)
            {

                var mainLabel = shopMenu.transform.Find("MainLabel");
                if (mainLabel != null)
                {
                    var labelText = mainLabel.GetComponent<Text>();
                    if (!Helpers.CheckForSubstrings(labelText.text, Config.CarNames))
                    {
                        CheckAndDisableICarsFromShop(shopMenu);
                    }
                }
            }
        }

        private void CheckAndDisableOldCarFromShop(GameObject shopCarWindow)
        {

            var itemsCars = shopCarWindow.transform.Find("Items");
            if (itemsCars != null)
            {
                for (int i = 0; i < itemsCars.childCount; i++)
                {
                    Transform ShowroomCarItem21Shop = itemsCars.GetChild(i);
                    var carName = ShowroomCarItem21Shop.Find("CarName").GetComponent<Text>();
                    var carYear = Helpers.GetYearFromString(carName.text);

                    if (carYear < _config.RemoveCarsOlderThanYear)
                    {
                        GameObject ShowroomCarObj = ShowroomCarItem21Shop.gameObject;
                        ShowroomCarObj.SetActive(false);
                    }
                }
            }
        }

        private void CheckAndDisableICarsFromShop(GameObject shopMenu)
        {
            var items = shopMenu.transform.Find("Items");
            if (items != null)
            {
                for (int i = 0; i < items.childCount; i++)
                {
                    Transform part = items.GetChild(i);
                    if (part == null)
                    {
                        continue;
                    }

                    var partText = part.Find("Text");
                    if (partText != null)
                    {
                        var itemText = partText.GetComponent<Text>();

                        if (Helpers.CheckForSubstrings(itemText.text, Config.ListWhiteCarParts))
                        {
                            continue;
                        }

                        if (Helpers.CheckForSubstrings(itemText.text, Config.RetroParts))
                        {
                            GameObject ShowroomCarObj = part.gameObject;
                            ShowroomCarObj.SetActive(false);

                            continue;
                        }
                    }
                }
            }
        }

        private void RemoveBlocks()
        {
            var engineRoomBlocker = GameObject.Find("EngineRoomBlocker");
            engineRoomBlocker?.SetActive(false);

            _blocker_Repair_Parts_2 = GameObject.Find("Blocker_Repair_Parts_2");
            _blocker_Repair_Parts_2?.SetActive(false);

            _blocker_Repair_Parts_3 = GameObject.Find("Blocker_Repair_Parts_3");
            _blocker_Repair_Parts_3?.SetActive(false);
        }

        private void UnlockJunkyardOnMap()
        {
            GameObject junkyardButton = GameObject.Find(JunkyardMapDestinationButtonName);
            if (junkyardButton == null)
            {
                return;
            }

            junkyardButton.SetActive(true);

            var button = junkyardButton.GetComponent<Button>();
            if (button != null)
            {
                button.interactable = true;
            }

            DisableLockObjects(junkyardButton.transform);
        }

        private void DisableLockObjects(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                string childName = child.gameObject.name;

                if (childName.IndexOf("Lock", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    childName.IndexOf("Blocker", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    childName.IndexOf("Required", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    child.gameObject.SetActive(false);
                    continue;
                }

                DisableLockObjects(child);
            }
        }

        private void GlobalInits()
        {
            var itemsMain = Singleton<GameInventory>.Instance.GetItems(ShopType.Main);
            InitAllItems(itemsMain);
            var itemsBody = Singleton<GameInventory>.Instance.GetItems(ShopType.Body);
            InitAllItems(itemsBody);
            var itemsInterior = Singleton<GameInventory>.Instance.GetItems(ShopType.Interior);
            InitAllItems(itemsInterior);
            var itemsTire = Singleton<GameInventory>.Instance.GetItems(ShopType.Tire);
            InitAllItems(itemsTire);
            var itemsLicensePlate = Singleton<GameInventory>.Instance.GetItems(ShopType.LicensePlate);
            InitAllItems(itemsLicensePlate);
            var itemsTuning = Singleton<GameInventory>.Instance.GetItems(ShopType.Tuning);
            InitAllItems(itemsTuning);
            var itemsBodyTuning = Singleton<GameInventory>.Instance.GetItems(ShopType.BodyTuning);
            InitAllItems(itemsBodyTuning);
            var itemsRims = Singleton<GameInventory>.Instance.GetItems(ShopType.Rims);
            InitAllItems(itemsRims);
            var itemsGearbox = Singleton<GameInventory>.Instance.GetItems(ShopType.Gearbox);
            InitAllItems(itemsGearbox);
            var itemsElectronics = Singleton<GameInventory>.Instance.GetItems(ShopType.Electronics);
            InitAllItems(itemsElectronics);
            var itemsAddons = Singleton<GameInventory>.Instance.GetItems(ShopType.Addons);
            InitAllItems(itemsAddons);
        }

        private void InitAllItems(Il2CppSystem.Collections.Generic.List<PartProperty> items)
        {

            foreach (var item in items)
            {
                item.RepairGroup = RepairGroupAvailableFromStart;

                if (item.ShopName == "BodyShop" || item.ShopName == "RimsShop" || item.ShopName == "InteriorShop" ||
                    item.ShopName == "TuningShop" || item.ShopName == "BodyTuningShop" || item.ShopName == "MainShop" ||
                    item.ShopName == "GearboxShop" || item.ShopName == "TireShop")
                {
                    float retroMultiplier = GetYearRetroMultiplier(item, (int)_config.RemoveCarsOlderThanYear);
                    if (retroMultiplier != 0f)
                    {
                        item.Price = Mathf.RoundToInt(item.Price * retroMultiplier);

                        item.ShopGroup = "Noinshop";
                        item.ShopName = "none";

                        continue;
                    }

                    if (Helpers.CheckForSubstrings(item.CarID, Config.SportGT)
                        || Helpers.CheckForSubstrings(item.ID, Config.SportGT))
                    {
                        item.Price = item.Price * _config.PricePartsSportGT;

                        item.ShopGroup = "Noinshop";
                        item.ShopName = "none";

                        continue;
                    }

                    if (Helpers.CheckForSubstrings(item.CarID, Config.SpecialRetro) || Helpers.CheckForSubstrings(item.ID, Config.SpecialRetro))
                    {
                        item.Price = item.Price * _config.PricePartsSpecialRetro;

                        item.ShopGroup = "Noinshop";
                        item.ShopName = "none";

                        continue;
                    }

                }

                if (Helpers.CheckForSubstrings(item.LocalizedName, Config.RetroParts))
                {
                    item.ShopGroup = "Noinshop";
                    item.ShopName = "none";

                    item.Price = item.Price * _config.PricePartsOther;
                }

            }
        }

        private struct RetroEntry
        {
            public int Year;
            public string Token;
            public string Rarity;
        }

        private static System.Collections.Generic.List<RetroEntry> _retroEntries;
        private static string[] _retroSource;

        private static System.Collections.Generic.List<RetroEntry> GetRetroEntries()
        {
            Config config = RealParts.ActiveConfig;
            string[] src = config != null ? config.ListRetroPartYears : null;
            if (_retroEntries != null && ReferenceEquals(_retroSource, src))
            {
                return _retroEntries;
            }

            var list = new System.Collections.Generic.List<RetroEntry>();
            if (src != null)
            {
                foreach (var s in src)
                {
                    if (string.IsNullOrEmpty(s))
                    {
                        continue;
                    }

                    int bar = s.IndexOf('|');
                    if (bar <= 0)
                    {
                        continue;
                    }

                    if (!int.TryParse(s.Substring(0, bar), out int year))
                    {
                        continue;
                    }

                    string token = s.Substring(bar + 1);
                    if (string.IsNullOrEmpty(token))
                    {
                        continue;
                    }

                    string rarity = "common";
                    int bar2 = token.IndexOf('|');
                    if (bar2 >= 0)
                    {
                        rarity = token.Substring(bar2 + 1).Trim().ToLowerInvariant();
                        token = token.Substring(0, bar2);
                        if (rarity != "uncommon" && rarity != "rare" && rarity != "unique")
                        {
                            rarity = "common";
                        }
                    }

                    if (string.IsNullOrEmpty(token))
                    {
                        continue;
                    }

                    list.Add(new RetroEntry { Year = year, Token = token, Rarity = rarity });
                }
            }

            _retroSource = src;
            _retroEntries = list;
            return list;
        }

        private static float GetYearRetroMultiplier(PartProperty item, int threshold)
        {
            foreach (var e in GetRetroEntries())
            {
                if (e.Year >= threshold)
                {
                    continue;
                }

                if (ContainsToken(item.CarID, e.Token) || ContainsToken(item.ID, e.Token))
                {
                    return PriceForYear(e.Year) * RarityFactor(e.Rarity);
                }
            }

            if (CarData.TryGetBodyYear(item.ID, out int bodyYear) && bodyYear < threshold)
            {
                return PriceForYear(bodyYear);
            }

            return 0f;
        }

        internal static string GetRarityForPart(string partId)
        {
            if (string.IsNullOrEmpty(partId))
            {
                return "common";
            }

            foreach (var e in GetRetroEntries())
            {
                if (ContainsToken(partId, e.Token))
                {
                    return e.Rarity;
                }
            }

            return "common";
        }

        private static float RarityFactor(string rarity)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null)
            {
                return 1f;
            }

            switch (rarity)
            {
                case "uncommon":
                    return config.PriceRarityUncommon;
                case "rare":
                    return config.PriceRarityRare;
                case "unique":
                    return config.PriceRarityUnique;
                default:
                    return config.PriceRarityCommon;
            }
        }

        private static bool ContainsToken(string input, string token)
        {
            return !string.IsNullOrEmpty(input) && input.Contains(token);
        }

        private static int PriceForYear(int year)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null)
            {
                return 1;
            }

            if (year < 1960)
            {
                return config.PriceParts1950_1959;
            }

            if (year < 1970)
            {
                return config.PriceParts1960_1969;
            }

            if (year < 1980)
            {
                return config.PriceParts1970_1979;
            }

            if (year <= 1990)
            {
                return config.PriceParts1980_1990;
            }

            if (year <= 2000)
            {
                return config.PriceParts1991_2000;
            }

            return config.PriceParts2001_2005;
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            GameSettings.CanShowPopups = true;
            PendingFreeTravelCost = 0;

            if (buildIndex == 1)
            {
                var buildVersion = GameSettings.BuildVersion;

                MelonLogger.Msg($"GameSettings.BuildVersion = {buildVersion} ");
            }

            if (buildIndex == 10 && !_isRunOnce)
            {
                GlobalInits();

                _isRunOnce = true;
            }
        }

        internal static void MarkFreeTravelPayment(MapDestinationID destinationID)
        {
            int cost = GetFreeTravelCost(destinationID);
            if (cost > 0)
            {
                PendingFreeTravelCost = cost;
            }
        }

        internal static int GetFreeTravelCost(MapDestinationID destinationID)
        {
            if (destinationID == MapDestinationID.Junkyard)
            {
                int cost = GlobalData.Cost_TravelJunkyard;
                return cost > 0 ? cost : DefaultJunkyardTravelCost;
            }

            if (destinationID == MapDestinationID.Auction)
            {
                return GlobalData.Cost_TravelAuction;
            }

            return 0;
        }

        internal static bool IsFreeTravelDestination(MapDestinationID destinationID)
        {
            return destinationID == MapDestinationID.Junkyard || destinationID == MapDestinationID.Auction;
        }

    }

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
            if (item == null || item.Condition <= 0f || item.Condition >= RealParts.RedPartConditionThreshold)
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
            return item.Condition >= RealParts.FullyRepairedPartConditionThreshold ||
                   IsConditionFullyRepaired(item.GetConditionToShow());
        }

        private static bool IsConditionFullyRepaired(float conditionToShow)
        {
            return Mathf.RoundToInt(conditionToShow) >= RealParts.FullyRepairedPartConditionToShow;
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
        }
    }

    [HarmonyPatch(typeof(RepairPartWindow), "StartMiniGame")]
    internal static class BlockRepairedPartMiniGamePatch
    {
        private static bool Prefix(RepairPartWindow __instance)
        {
            Item item = __instance.currentItemInfo?.Item;
            return item == null ||
                   item.Condition < RealParts.FullyRepairedPartConditionThreshold &&
                   Mathf.RoundToInt(item.GetConditionToShow()) < RealParts.FullyRepairedPartConditionToShow;
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

            return Mathf.Min(RealParts.FullyRepairedPartConditionToShow, currentCondition + RealParts.MinimumRepairConditionIncrease);
        }
    }

    [HarmonyPatch(typeof(MapWindow), "SubmitItem")]
    internal static class MarkFreeTravelFromMapSubmitPatch
    {
        private static void Prefix(MapDestinationButton item)
        {
            if (item != null && RealParts.IsFreeTravelDestination(item.ID))
            {
                RealParts.MarkFreeTravelPayment(item.ID);
            }
        }
    }

    [HarmonyPatch(typeof(MapWindow), "SubmitItemAction")]
    internal static class MarkFreeTravelFromMapActionPatch
    {
        private static void Prefix(MapWindow __instance)
        {
            if (RealParts.IsFreeTravelDestination(__instance.currentSelectedDestination))
            {
                RealParts.MarkFreeTravelPayment(__instance.currentSelectedDestination);
            }
        }
    }

    [HarmonyPatch(typeof(GlobalData), "AddPlayerMoney")]
    internal static class FreeMapTravelPaymentPatch
    {
        private static void Prefix(ref int money)
        {
            if (SellItemCapPatch.PendingSellCap >= 0f)
            {
                if (money > 0 && money > SellItemCapPatch.PendingSellCap)
                {
                    money = (int)SellItemCapPatch.PendingSellCap;
                }

                SellItemCapPatch.PendingSellCap = -1f;
                return;
            }

            if (RealParts.PendingFreeTravelCost <= 0 || money >= 0)
            {
                return;
            }

            if (money == -RealParts.PendingFreeTravelCost)
            {
                money = 0;
                RealParts.PendingFreeTravelCost = 0;
            }
        }
    }

    [HarmonyPatch(typeof(GlobalData), "SetPlayerMoney")]
    internal static class FreeMapTravelSetMoneyPatch
    {
        private static void Prefix(ref int money)
        {
            if (RealParts.PendingFreeTravelCost <= 0)
            {
                return;
            }

            int currentMoney = GlobalData.PlayerMoney;
            int diff = currentMoney - money;
            if (diff == RealParts.PendingFreeTravelCost)
            {
                money = currentMoney;
                RealParts.PendingFreeTravelCost = 0;
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
                if (RealParts.IsFreeTravelDestination(destinationID))
                {
                    price = 0;
                }

                __result = true;
            }
        }
    }

    [HarmonyPatch(typeof(CarBundleLoader), "GetCarsForScene")]
    internal static class FilterJunkyardCarsByYearPatch
    {
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
        }

        internal static readonly System.Collections.Generic.HashSet<string> JunkyardCarIds = new System.Collections.Generic.HashSet<string>();

        internal static bool IsJunkyardCar(CarLoader loader)
        {
            if (loader == null || JunkyardCarIds.Count == 0)
            {
                return false;
            }

            string id = GetCarLoaderId(loader);
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }

            return JunkyardCarIds.Contains(NormalizeCarId(id));
        }

        private static string GetCarLoaderId(CarLoader loader)
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

    [HarmonyPatch(typeof(Inventory), "SellItem", new[] { typeof(Item), typeof(bool), typeof(bool) })]
    internal static class SellItemCapPatch
    {
        internal static float PendingSellCap = -1f;

        private static void Prefix(Item item)
        {
            PendingSellCap = -1f;

            Config config = RealParts.ActiveConfig;
            if (config == null || !config.EnableSellCap || item == null)
            {
                return;
            }

            PartProperty property = Singleton<GameInventory>.Instance.GetItemProperty(item.ID);
            if (property == null || property.Price <= 0)
            {
                return;
            }

            PendingSellCap = property.Price * config.SellRatio;
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
            if (item == null)
            {
                return;
            }

            string rarity = RealParts.GetRarityForPart(item.ID);
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
            if (fee > 0)
            {
                GlobalData.AddPlayerMoney(-fee);
            }
        }
    }

    [HarmonyPatch(typeof(CarLoader), "SetConditionOnDetails", new[] { typeof(float) })]
    internal static class JunkyardDonorConditionOnDetailsPatch
    {
        private static void Prefix(CarLoader __instance, ref float condition)
        {
            JunkyardDonorConditionCore.Apply(__instance, ref condition);
        }
    }

    [HarmonyPatch(typeof(CarLoader), "SetConditionAll")]
    internal static class JunkyardDonorConditionAllPatch
    {
        private static void Prefix(CarLoader __instance, ref float condition)
        {
            JunkyardDonorConditionCore.Apply(__instance, ref condition);
        }
    }

    internal static class JunkyardDonorConditionCore
    {
        internal static void Apply(CarLoader loader, ref float condition)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || !config.EnableDonorCondition || condition <= 0f)
            {
                return;
            }

            if (!FilterJunkyardCarsByYearPatch.IsJunkyardCar(loader))
            {
                return;
            }

            condition = Mathf.Clamp01(condition * config.DonorConditionScale);
        }
    }

}

#pragma warning restore CS0618

