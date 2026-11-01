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

namespace RealShop
{
    internal sealed class SceneUiController
    {
        private GameObject _blocker_Repair_Parts_2;
        private GameObject _blocker_Repair_Parts_3;

        internal void Update(Config config)
        {
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
            if (mapDestShop != null && !config.IsEnableShop)
            {
                mapDestShop?.SetActive(false);
            }

            GameObject auctionsButton = GameObject.Find("AuctionsButton");
            if (auctionsButton != null && !config.IsEnableAdditionalAuction)
            {
                auctionsButton?.SetActive(false);
            }

            GameObject shopCarWindow = GameObject.Find("ShopCarWindow");
            if (shopCarWindow != null)
            {
                CheckAndDisableOldCarFromShop(shopCarWindow, config);

                return;
            }

            GameObject shopMenu = GameObject.Find("ShopMenu");
            if (shopMenu != null && config.IsEnableBrandFilter)
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

        private void CheckAndDisableOldCarFromShop(GameObject shopCarWindow, Config config)
        {
            var itemsCars = shopCarWindow.transform.Find("Items");
            if (itemsCars == null)
            {
                return;
            }

            int cutoff = (int)config.RemoveCarsOlderThanYear;

            for (int i = 0; i < itemsCars.childCount; i++)
            {
                Transform showroomItem = itemsCars.GetChild(i);
                int carYear = ResolveShopCarYear(showroomItem);

                if (carYear >= 0 && carYear < cutoff)
                {
                    showroomItem.gameObject.SetActive(false);
                }
            }
        }

        private static int ResolveShopCarYear(Transform showroomItem)
        {
            var showroom = showroomItem.GetComponent<ShowroomCarItem>();
            if (showroom == null)
            {
                return -1;
            }

            int year;
            if (!string.IsNullOrEmpty(showroom.ID) && CarData.TryGetYearByCarId(showroom.ID, out year))
            {
                return year;
            }

            if (showroom.carName != null)
            {
                return Helpers.GetYearFromString(showroom.carName.text);
            }

            return -1;
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
            GameObject junkyardButton = GameObject.Find(RealParts.JunkyardMapDestinationButtonName);
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
    }
}
