using MelonLoader;
using Harmony;
using UnityEngine;
using UnityEngine.UI;
using OrbCreationExtensions;
using System.Collections.Generic;
using System.Linq;
using CMS.UI.Windows;
using CMS.UI.Logic;
using System;
using CMS.Difficulty;

namespace RealShop
{
    public class RealParts : MelonMod
    {
        private Config _config;
        private bool _modIsEnable = true;
        private bool _runOnes = false;

        GameObject _blocker_Repair_Parts_2;
        GameObject _blocker_Repair_Parts_3;

        [Obsolete]
        public override void OnApplicationStart()
        {
            MelonLogger.Msg("initializing...");

            _config = new Config();
            _config.Reload();
        }

        public override void OnUpdate()
        {
            if  (Input.GetKeyDown(_config.IsEnableSwitchMode))
            {
                if (_modIsEnable)
                {
                    _modIsEnable = false;
                    UIManager.Get().ShowPopup("[RealShop]","has been stoped.", PopupType.GameMode);
                } else {
                    _modIsEnable = true;
                    UIManager.Get().ShowPopup("[RealShop]", "has been enabled.", PopupType.GameMode);
                }
            }

            if (Input.GetKeyDown(_config.Repair))
            {
                RepairItems(Config.TargetRepairParts, Config.RepairCost);
            }

            if (!_modIsEnable) {
                return;
            }

            if (_runOnes)
            {
                GlobalPref();
            }

            RemoveBlocks();

            GameObject engineRoom = GameObject.Find("EngineRoom");
            if (engineRoom != null)
            {
                var erw = engineRoom.transform.Find("Engine_Room_Workbench_2");
                if(erw != null)
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

            GameObject shopCarWindow = GameObject.Find("ShopCarWindow");
            if (shopCarWindow != null)
            {
                CheckAndDisableOldCarFromShop(shopCarWindow);

                return;
            }

            GameObject shopMenu = GameObject.Find("ShopMenu");
            if (shopMenu != null && _config.IsEnableBrandFilter)
            {
                CheckAndDisableBrandsFromShop(shopMenu);
            }
        }

        private void RepairItems(string[] repairParts, int[] repairCost)
        {
            Inventory inventory = Singleton<Inventory>.Instance;
            var cItems = inventory.GetItems();
            foreach (var baseItem in cItems)
            {
                if (baseItem.Condition == 1.0f)
                {
                    continue;
                }

                if (Helpers.CheckForSubstrings(baseItem.ID, repairParts))
                {
                    var money = GlobalData.PlayerMoney;
                    var currentCond = baseItem.Condition;

                    if (currentCond < 0.15f && money < repairCost[2])
                    {
                        UIManager.Get().ShowPopup(Config.ModName, "insufficient funds", PopupType.Normal);

                        return;
                    }

                    if (currentCond < 0.4f && money < repairCost[1])
                    {
                        UIManager.Get().ShowPopup(Config.ModName, "insufficient funds", PopupType.Normal);

                        return;
                    }

                    if (currentCond < 0.7f && money < repairCost[0])
                    {
                        UIManager.Get().ShowPopup(Config.ModName, "insufficient funds", PopupType.Normal);

                        return;
                    }

                    float finallyPrice = 0;

                    switch (currentCond)
                    {
                        case var cond when cond < 0.15f:
                            finallyPrice = repairCost[2] + (currentCond * repairCost[2]);
                            GlobalData.AddPlayerMoney(-(int)finallyPrice);
                            break;

                        case var cond when cond < 0.4f:
                            finallyPrice = repairCost[1] + (currentCond * repairCost[1]);
                            GlobalData.AddPlayerMoney(-(int)finallyPrice);
                            break;

                        case var cond when cond < 0.7f:
                            finallyPrice = repairCost[0] + (currentCond * repairCost[0]);
                            GlobalData.AddPlayerMoney(-(int)finallyPrice);
                            break;
                    }

                    baseItem.Condition = 1.0f;
                    baseItem.Dent = 1.0f;

                    UIManager.Get().ShowPopup(Config.ModName, $"item was repair! cost: {finallyPrice}", PopupType.Normal);
                } else
                {
                    MelonLogger.Msg($"{Helpers.ToPrettyString(baseItem)}");
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

                        if (_config.IsShowDebugInfo)
                        {
                            MelonLogger.Msg($"car year {carYear}");
                        }
                    }
                }
            }
        }

        private void CheckAndDisableBrandsFromShop(GameObject shopMenu)
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

                    var partBrand = part.Find("Brand");
                    if (partBrand != null)
                    {
                        var image = partBrand.GetComponent<Image>();
                        string imageName = image.sprite.name;

                        if (!Helpers.IsBrandExist(imageName))
                        {
                            GameObject ShowroomCarObj = part.gameObject;
                            ShowroomCarObj.SetActive(false);
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

        private void GlobalPref()
        {
            _runOnes = false;

            var itemsMain = Singleton<GameInventory>.Instance.GetItems(ShopType.Main);
            SetAllRepairs(itemsMain);

            var itemsBody = Singleton<GameInventory>.Instance.GetItems(ShopType.Body);
            foreach (var item in itemsBody)
            {
                item.RepairGroup = 6;
            }

            var itemsInterior = Singleton<GameInventory>.Instance.GetItems(ShopType.Interior);
            foreach (var item in itemsInterior)
            {
                item.RepairGroup = 6;
            }

            var itemsTire = Singleton<GameInventory>.Instance.GetItems(ShopType.Tire);
            SetAllRepairs(itemsTire);
            var itemsLicensePlate = Singleton<GameInventory>.Instance.GetItems(ShopType.LicensePlate);
            SetAllRepairs(itemsLicensePlate);
            var itemsTuning = Singleton<GameInventory>.Instance.GetItems(ShopType.Tuning);
            SetAllRepairs(itemsTuning);
            var itemsBodyTuning = Singleton<GameInventory>.Instance.GetItems(ShopType.BodyTuning);
            SetAllRepairs(itemsBodyTuning);
            var itemsRims = Singleton<GameInventory>.Instance.GetItems(ShopType.Rims);
            SetAllRepairs(itemsRims);
            var itemsGearbox = Singleton<GameInventory>.Instance.GetItems(ShopType.Gearbox);
            SetAllRepairs(itemsGearbox);
            var itemsElectronics = Singleton<GameInventory>.Instance.GetItems(ShopType.Electronics);
            SetAllRepairs(itemsElectronics);
            var itemsAddons = Singleton<GameInventory>.Instance.GetItems(ShopType.Addons);
            SetAllRepairs(itemsAddons);
        }

        private void SetAllRepairs(Il2CppSystem.Collections.Generic.List<PartProperty> items)
        {
            MelonLogger.Msg($"start set repair groups {items.Count}");
            foreach (var item in items)
            {
                if (!Helpers.IsBrandExist(item.Brand))
                {
                    item.RepairGroup = 6;
                }
            }

            MelonLogger.Msg($"finished repair groups");
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            LoggerInstance.Msg($"Scene {sceneName} with build index {buildIndex} has been loaded!");
            if (sceneName == "garage" && buildIndex == 10)
            {
                _runOnes = true;

                MelonLogger.Msg($"GlobalData.Cost_TravelJunkyard = {GlobalData.Cost_TravelJunkyard} ");
            }
        }

    }
}

