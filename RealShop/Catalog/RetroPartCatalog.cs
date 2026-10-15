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
    internal static class RetroPartCatalog
    {
        internal static void Initialize()
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

        private static void InitAllItems(Il2CppSystem.Collections.Generic.List<PartProperty> items)
        {
            Config config = RealParts.ActiveConfig;

            foreach (var item in items)
            {
                item.RepairGroup = RealParts.RepairGroupAvailableFromStart;

                if (item.ShopName == "BodyShop" || item.ShopName == "RimsShop" || item.ShopName == "InteriorShop" ||
                    item.ShopName == "TuningShop" || item.ShopName == "BodyTuningShop" || item.ShopName == "MainShop" ||
                    item.ShopName == "GearboxShop" || item.ShopName == "TireShop")
                {
                    float retroMultiplier = GetYearRetroMultiplier(item, (int)config.RemoveCarsOlderThanYear);
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
                        item.Price = item.Price * config.PricePartsSportGT;

                        item.ShopGroup = "Noinshop";
                        item.ShopName = "none";

                        continue;
                    }

                    if (Helpers.CheckForSubstrings(item.CarID, Config.SpecialRetro) || Helpers.CheckForSubstrings(item.ID, Config.SpecialRetro))
                    {
                        item.Price = item.Price * config.PricePartsSpecialRetro;

                        item.ShopGroup = "Noinshop";
                        item.ShopName = "none";

                        continue;
                    }
                }

                if (Helpers.CheckForSubstrings(item.LocalizedName, Config.RetroParts))
                {
                    item.ShopGroup = "Noinshop";
                    item.ShopName = "none";

                    item.Price = item.Price * config.PricePartsOther;
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
                    return PriceForYear(e.Year);
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
    }
}
