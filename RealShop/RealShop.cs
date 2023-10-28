using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace RealShop
{
    public class RealParts : MelonMod
    {

        private Config _config;

        [Obsolete]
        public override void OnApplicationStart()
        {
            MelonLogger.Msg("initializing...");
            _config = new Config();
            _config.Reload();
        }

        public override void OnUpdate()
        {

            GameObject shopCarWindow = GameObject.Find("ShopCarWindow");
            if (shopCarWindow != null)
            {
                var itemsCars = shopCarWindow.transform.Find("Items");
                if (itemsCars != null)
                {
                    for (int i = 0; i < itemsCars.childCount; i++)
                    {
                        Transform ShowroomCarItem21Shop = itemsCars.GetChild(i);
                        GameObject ShowroomCarObj = ShowroomCarItem21Shop.gameObject;
                        var carName = ShowroomCarItem21Shop.Find("CarName").GetComponent<Text>();
                        var carYear = Helpers.GetYearFromString(carName.text);
                        if (carYear < _config.RemoveCarsOlderThanYear)
                        {
                            ShowroomCarObj.SetActive(false);
                            MelonLogger.Msg($"Car year {carYear}");
                        }
                    }

                }
            }
        }
    }
}

