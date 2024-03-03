using CMS.UI.Logic;
using Harmony;
using MelonLoader;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace RealShop
{
    public class RealParts : MelonMod
    {
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
                item.RepairGroup = 6;

                if (item.ShopName == "BodyShop" || item.ShopName == "RimsShop" || item.ShopName == "InteriorShop" ||
                    item.ShopName == "TuningShop" || item.ShopName == "BodyTuningShop" || item.ShopName == "MainShop" ||
                    item.ShopName == "GearboxShop" || item.ShopName == "TireShop")
                {
                    if (Helpers.CheckForSubstrings(item.CarID, Config.RetroParts1950_1959)
                        || Helpers.CheckForSubstrings(item.ID, Config.RetroParts1950_1959))
                    {
                        item.Price = item.Price * _config.PriceParts1950_1959;

                        item.ShopGroup = "Noinshop";
                        item.ShopName = "none";

                        continue;
                    }

                    if (Helpers.CheckForSubstrings(item.CarID, Config.RetroParts1960_1969)
                        || Helpers.CheckForSubstrings(item.ID, Config.RetroParts1960_1969))
                    {
                        item.Price = item.Price * _config.PriceParts1960_1969;


                        item.ShopGroup = "Noinshop";
                        item.ShopName = "none";

                        continue;
                    }

                    if (Helpers.CheckForSubstrings(item.CarID, Config.RetroParts1970_1979)
                        || Helpers.CheckForSubstrings(item.ID, Config.RetroParts1970_1979))
                    {
                        item.Price = item.Price * _config.PriceParts1970_1979;

                        item.ShopGroup = "Noinshop";
                        item.ShopName = "none";

                        continue;
                    }

                    if (Helpers.CheckForSubstrings(item.CarID, Config.RetroParts1980_1990)
                        || Helpers.CheckForSubstrings(item.ID, Config.RetroParts1980_1990))
                    {
                        item.Price = item.Price * _config.PriceParts1980_1990;

                        item.ShopGroup = "Noinshop";
                        item.ShopName = "none";

                        continue;
                    }

                    if (Helpers.CheckForSubstrings(item.CarID, Config.RetroParts1991_2000)
                        || Helpers.CheckForSubstrings(item.ID, Config.RetroParts1991_2000))
                    {
                        item.Price = item.Price * _config.PriceParts1991_2000;

                        item.ShopGroup = "Noinshop";
                        item.ShopName = "none";

                        continue;
                    }

                    if (Helpers.CheckForSubstrings(item.CarID, Config.RetroParts2001_2005)
                        || Helpers.CheckForSubstrings(item.ID, Config.RetroParts2001_2005))
                    {
                        item.Price = item.Price * _config.PriceParts2001_2005;

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

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            GameSettings.CanShowPopups = true;

            var gameSettingData = GameSettings.GameSettingsData;
            gameSettingData.Init();
            gameSettingData.TravelHaveCost = false;


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

    }

}

