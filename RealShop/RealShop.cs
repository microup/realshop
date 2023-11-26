using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using System;
using CMS.UI.Logic;
using Harmony;
using UnhollowerRuntimeLib;
using HarmonyLib;
using Il2CppSystem.Reflection;

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
                    // MelonLogger.Msg($"ITEM TEXT {labelText.text}");
                    if (!Helpers.CheckForSubstrings(labelText.text, Config.CarNames))
                    {
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

                        if (Helpers.CheckForSubstrings(itemText.text, Config.RetroParts))
                        {
                            GameObject ShowroomCarObj = part.gameObject;
                            ShowroomCarObj.SetActive(false);

                            continue;
                        }
                    }

/*                    var partBrand = part.Find("Brand");
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
                    }*/
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

            // MelonLogger.Msg($"TMP, ID, IsBody, Brand, CarID, LocalizedName, PartGroup, Price, RepairGroup, ShopGroup, ShopName, BrakesValue");

            foreach (var item in items)
            {
                /* MelonLogger.Msg($",{item.ID}, {item.IsBody}, {item.Brand}, {item.CarID}, " +
                    $"{item.LocalizedName}, {item.PartGroup}, " +
                    $"{item.Price}, {item.RepairGroup}, {item.ShopGroup}, " +
                    $"{item.ShopName}, {item.BrakesValue}");*/
                /*
                MelonLogger.Msg($"(Item) ID: {item.ID}, IsBody: {item.IsBody}, Brand: {item.Brand}, " +
                    $"CarID: {item.CarID}, LocalizedName: {item.LocalizedName}, PartGroup: {item.PartGroup}, " +
                    $"Price: {item.Price}, RepairGroup: {item.RepairGroup}, ShopGroup: {item.ShopGroup}, " +
                    $"ShopName: {item.ShopName}, BrakesValue: {item.BrakesValue}");*/

                item.RepairGroup = 6;

                if (item.ShopName == "BodyShop" || item.ShopName == "RimsShop" || item.ShopName == "InteriorShop" ||
                    item.ShopName == "TuningShop" || item.ShopName == "BodyTuningShop" || item.ShopName == "MainShop" ||
                    item.ShopName == "GearboxShop" || item.ShopName == "TireShop")
                {
                    if (Helpers.CheckForSubstrings(item.CarID, Config.RetroParts1950_1959)
                        || Helpers.CheckForSubstrings(item.ID, Config.RetroParts1950_1959))
                    {
                        item.Price = item.Price * 30;

                        item.ShopGroup = "Noinshop";
                        item.ShopName = "none";

                        continue;
                    }

                    if (Helpers.CheckForSubstrings(item.CarID, Config.RetroParts1960_1969)
                        || Helpers.CheckForSubstrings(item.ID, Config.RetroParts1960_1969))
                    {
                        item.Price = item.Price * 24;


                        item.ShopGroup = "Noinshop";
                        item.ShopName = "none";

                        continue;
                    }

                    if (Helpers.CheckForSubstrings(item.CarID, Config.RetroParts1970_1979)
                        || Helpers.CheckForSubstrings(item.ID, Config.RetroParts1970_1979))
                    {
                        item.Price = item.Price * 20;

                        item.ShopGroup = "Noinshop";
                        item.ShopName = "none";

                        continue;
                    }

                    if (Helpers.CheckForSubstrings(item.CarID, Config.RetroParts1980_1990)
                        || Helpers.CheckForSubstrings(item.ID, Config.RetroParts1980_1990))
                    {
                        item.Price = item.Price * 17;

                        item.ShopGroup = "Noinshop";
                        item.ShopName = "none";

                        continue;
                    }

                    if (Helpers.CheckForSubstrings(item.CarID, Config.RetroParts1991_2000)
                        || Helpers.CheckForSubstrings(item.ID, Config.RetroParts1991_2000))
                    {
                        item.Price = item.Price * 14;

                        item.ShopGroup = "Noinshop";
                        item.ShopName = "none";

                        continue;
                    }

                    if (Helpers.CheckForSubstrings(item.CarID, Config.RetroParts2001_2005)
                        || Helpers.CheckForSubstrings(item.ID, Config.RetroParts2001_2005))
                    {
                        item.Price = item.Price * 10;

                        item.ShopGroup = "Noinshop";
                        item.ShopName = "none";

                        continue;
                    }

                    if (Helpers.CheckForSubstrings(item.CarID, Config.SportGT)
                        || Helpers.CheckForSubstrings(item.ID, Config.SportGT))
                    {
                        item.Price = item.Price * 22;

                        item.ShopGroup = "Noinshop";
                        item.ShopName = "none";

                        continue;
                    }

                    if (Helpers.CheckForSubstrings(item.CarID, Config.SpecialRetro) || Helpers.CheckForSubstrings(item.ID, Config.SpecialRetro))
                    {
                        item.Price = item.Price * 50;

                        item.ShopGroup = "Noinshop";
                        item.ShopName = "none";

                        continue;
                    }

                }

                if (Helpers.CheckForSubstrings(item.LocalizedName, Config.RetroParts))
                {
                    item.ShopGroup = "Noinshop";
                    item.ShopName = "none";

                    item.Price = item.Price * 5;
                }

            }
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (buildIndex == 1)
            {
                MelonLogger.Msg($"GlobalData.Cost_TravelJunkyard = {GlobalData.Cost_TravelJunkyard} ");
                MelonLogger.Msg($"GlobalData.JunkCondition = {GlobalData.JunkCondition} ");
                MelonLogger.Msg($"GlobalData.GetCommissionForScene(Garage) = {GlobalData.GetCommissionForScene(SceneType.Garage)} ");
                MelonLogger.Msg($"GlobalData.GetCommissionForScene(Auction) = {GlobalData.GetCommissionForScene(SceneType.Auction)} ");
                MelonLogger.Msg($"GlobalData.GetCommissionForScene(Barn) = {GlobalData.GetCommissionForScene(SceneType.Barn)} ");
                MelonLogger.Msg($"GlobalData.GetCommissionForScene(Junkyard) = {GlobalData.GetCommissionForScene(SceneType.Junkyard)} ");
            }

            if (buildIndex == 10 && !_isRunOnce)
            {

/*                var profileIndex = Singleton<GameManager>.Instance.ProfileManager.GetSelectedProfile();
                var playerLevel = Singleton<GameManager>.Instance.ProfileManager.GetProfileLevel(profileIndex);
                var profileName = Singleton<GameManager>.Instance.ProfileManager.GetProfileName(profileIndex);

                MelonLogger.Msg($"player level: {playerLevel} level {GlobalData.PlayerLevel} name: {profileName}");
                if (playerLevel < 11)
                {
                    var difficulty = Singleton<GameManager>.Instance.ProfileManager.GetSelectedProfileDifficulty();

                    if (difficulty == DifficultyLevel.Normal)
                    {
                        GlobalData.AddPlayerExp(7250);
                    }

                    if (difficulty == DifficultyLevel.Expert)
                    {
                        GlobalData.AddPlayerExp(3600);
                    }

                    GlobalData.AddPlayerMoney(5000);

                    Singleton<GameManager>.Instance.GameDataManager.Save(profileIndex);
                    //Singleton<GameManager>.Instance.GameDataManager.
                }*/

                GlobalPref();

                _isRunOnce = true;
            }
        }

    }

}

