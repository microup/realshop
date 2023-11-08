using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using System;
using Il2Cpp;
using Il2CppCMS.UI.Logic;

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
            if  (Input.GetKeyDown(_config.IsKeyBindEnableSwitchMode))
            {
                if (_modIsEnable)
                {
                    _modIsEnable = false;
                } else {
                    _modIsEnable = true;
                }
                UIManager.Get().ShowPopup("[RealShop]", $"the mod has been status: {_modIsEnable}.", PopupType.Normal);

                MelonLogger.Msg($"the mod has been {_modIsEnable}");
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
                    // MelonLogger.Msg($"ITEM TEXT {labelText.text}");
                    if (!Helpers.CheckForSubstrings(labelText.text, Config.CarNames)) {
                        CheckAndDisableItemsFromShop(shopMenu);
                    }
                }
            }
        }

        private void CheckAndDisableOldCarFromShop(GameObject shopCarWindow)
        {

            var mainLableText = shopCarWindow.transform.Find("MainLabel").GetComponent<Text>();
            if (mainLableText != null)
            {
                //MelonLogger.Msg($"main label {mainLableText.text} name {mainLableText.name} tostr {mainLableText.ToString()}");
            }

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

        private void CheckAndDisableItemsFromShop(GameObject shopMenu)
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
                        //string pt = $"{itemText.text.LocalizeItem()}";
                        //MelonLogger.Msg($"PART TEXT {pt} || {Helpers.ExtractEnglishWords(pt)}");

                        if (Helpers.CheckForSubstrings(itemText.text, Config.ListWhiteCarParts))
                        {
                            continue;
                        }

                        if (Helpers.CheckForSubstrings(itemText.text, Config.OldEngine))
                        {
                            GameObject ShowroomCarObj = part.gameObject;
                            ShowroomCarObj.SetActive(false);

                            continue;
                        }
                    }

                    var partBrand = part.Find("Brand");
                    if (partBrand != null)
                    {
                        var image = partBrand.GetComponent<Image>();
                        string imageName = image.sprite.name;
                        //MelonLogger.Msg($"BRAND NAME {imageName}");
                        if (!Helpers.CheckForSubstrings(imageName, Config.ListActiveBrands))
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

            MelonLogger.Msg($"GlobalData.Cost_TravelJunkyard = {GlobalData.Cost_TravelJunkyard} ");
            MelonLogger.Msg($"GlobalData.JunkCondition = {GlobalData.JunkCondition} ");
            MelonLogger.Msg($"GlobalData.GetCommissionForScene(Garage) = {GlobalData.GetCommissionForScene(SceneType.Garage)} ");
            MelonLogger.Msg($"GlobalData.GetCommissionForScene(Auction) = {GlobalData.GetCommissionForScene(SceneType.Auction)} ");

            var itemsMain = Singleton<GameInventory>.Instance.GetItems(ShopType.Main);
            SetAllRepairs(itemsMain);
            var itemsBody = Singleton<GameInventory>.Instance.GetItems(ShopType.Body);
            SetAllRepairs(itemsBody);
            var itemsInterior = Singleton<GameInventory>.Instance.GetItems(ShopType.Interior);
            SetAllRepairs(itemsInterior);
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
            MelonLogger.Msg($"begin set repair groups {items.Count}");
            foreach (var item in items)
            {
                item.RepairGroup = 6;
            }

            MelonLogger.Msg($"finished repair groups");
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            LoggerInstance.Msg($"Scene {sceneName} with build index {buildIndex} has been loaded!");
            if (sceneName == "garage" && buildIndex == 10)
            {
                _runOnes = true;
            }
        }

    }
}

