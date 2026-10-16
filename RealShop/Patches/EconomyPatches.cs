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
    internal static class SellItemCapPatch
    {
        internal static float PendingSellCap = -1f;

        private static void Prefix(Item item)
        {
            PendingSellCap = -1f;

            Config config = RealParts.ActiveConfig;
            if (config == null || !config.EnableSellCap)
            {
                return;
            }

            MelonLogger.Msg("[SellCap] START: SellItem called for item=" + (item != null ? item.ID.ToString() : "<null>"));

            if (item == null)
            {
                MelonLogger.Msg("[SellCap] DONE: skipped (item is null)");
                return;
            }

            PartProperty property = Singleton<GameInventory>.Instance.GetItemProperty(item.ID);
            if (property == null || property.Price <= 0)
            {
                MelonLogger.Msg("[SellCap] DONE: skipped (no shop price; property=" + (property != null) + ")");
                return;
            }

            float factor = SellConditionFactor(item, config);
            PendingSellCap = property.Price * config.SellRatio * factor;
            MelonLogger.Msg("[SellCap] PROCESS: shopPrice=" + property.Price + " ratio=" + config.SellRatio + " factor=" + factor + " cap=" + PendingSellCap);
        }

        internal static float SellConditionFactor(Item item, Config config)
        {
            if (config == null || !config.EnableSellConditionFactor || item == null)
            {
                return 1f;
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

            return config.SellConditionFloor + config.SellConditionSpan * cond;
        }
    }

    internal static class SellPriceCapPatch
    {
        private static float ShopPriceOf(object instance)
        {
            float shopPrice = 0f;
            try
            {
                System.Type t = instance.GetType();
                var priceProp = t.GetProperty("Price");
                if (priceProp != null)
                {
                    shopPrice = System.Convert.ToSingle(priceProp.GetValue(instance, null));
                }

                if (shopPrice <= 0f)
                {
                    var idProp = t.GetProperty("ID") ?? t.GetProperty("Id");
                    if (idProp != null)
                    {
                        string id = idProp.GetValue(instance, null) as string;
                        if (!string.IsNullOrEmpty(id))
                        {
                            PartProperty pp = Singleton<GameInventory>.Instance.GetItemProperty(id);
                            if (pp != null)
                            {
                                shopPrice = pp.Price;
                            }
                        }
                    }
                }

                if (shopPrice <= 0f)
                {
                    foreach (var prop in t.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
                    {
                        if (prop.GetIndexParameters().Length > 0 || !prop.CanRead)
                        {
                            continue;
                        }

                        if (prop.Name.IndexOf("item", System.StringComparison.OrdinalIgnoreCase) < 0)
                        {
                            continue;
                        }

                        object val = prop.GetValue(instance, null);
                        if (val == null)
                        {
                            continue;
                        }

                        var valIdProp = val.GetType().GetProperty("ID") ?? val.GetType().GetProperty("Id");
                        string vid = valIdProp != null ? valIdProp.GetValue(val, null) as string : null;
                        if (!string.IsNullOrEmpty(vid))
                        {
                            PartProperty pp2 = Singleton<GameInventory>.Instance.GetItemProperty(vid);
                            if (pp2 != null && pp2.Price > 0)
                            {
                                shopPrice = pp2.Price;
                                break;
                            }
                        }
                    }
                }
            }
            catch
            {
                return 0f;
            }
            return shopPrice;
        }

        private static void Postfix(object __instance, ref float __result)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || !config.EnableSellCap || __instance == null)
            {
                return;
            }

            float shopPrice = ShopPriceOf(__instance);
            if (shopPrice <= 0f)
            {
                return;
            }

            float cap = shopPrice * config.SellRatio * ConditionFactor(__instance, config);
            if (__result > cap)
            {
                MelonLogger.Msg("[SellCap] display cap: sellPrice " + __result + " -> " + cap + " (shopPrice=" + shopPrice + ")");
                __result = cap;
            }
        }

        private static void PostfixInt(object __instance, ref int __result)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || !config.EnableSellCap || __instance == null)
            {
                return;
            }

            float shopPrice = ShopPriceOf(__instance);
            if (shopPrice <= 0f)
            {
                return;
            }

            int cap = Mathf.RoundToInt(shopPrice * config.SellRatio * ConditionFactor(__instance, config));
            if (__result > cap)
            {
                MelonLogger.Msg("[SellCap] display cap(int): sellPrice " + __result + " -> " + cap + " (shopPrice=" + shopPrice + ")");
                __result = cap;
            }
        }

        internal static float ConditionFactor(object instance, Config config)
        {
            if (config == null || !config.EnableSellConditionFactor)
            {
                return 1f;
            }

            float cond = ConditionOf(instance);
            if (cond < 0f)
            {
                return 1f;
            }

            if (cond > 1f)
            {
                cond = 1f;
            }

            return config.SellConditionFloor + config.SellConditionSpan * cond;
        }

        private static float ConditionOf(object instance)
        {
            try
            {
                if (instance is Item)
                {
                    return ((Item)instance).GetConditionToShow();
                }

                if (instance is BaseItem)
                {
                    return ((BaseItem)instance).GetConditionToShow();
                }

                System.Type t = instance.GetType();
                foreach (var prop in t.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
                {
                    if (!prop.CanRead || prop.GetIndexParameters().Length > 0)
                    {
                        continue;
                    }

                    if (prop.PropertyType != typeof(Item) && prop.PropertyType != typeof(BaseItem))
                    {
                        continue;
                    }

                    object val = prop.GetValue(instance, null);
                    if (val == null)
                    {
                        continue;
                    }

                    if (val is Item)
                    {
                        return ((Item)val).GetConditionToShow();
                    }

                    if (val is BaseItem)
                    {
                        return ((BaseItem)val).GetConditionToShow();
                    }
                }
            }
            catch
            {
                return -1f;
            }

            return -1f;
        }
    }

    internal static class SellProbePatch
    {
        private static void Postfix(object __0, object __1, ref int __result)
        {
            try
            {
                float arg = __1 != null ? System.Convert.ToSingle(__1) : -1f;
                if (arg < 0.4f || arg > 0.6f)
                {
                    return;
                }

                MelonLogger.Msg("[SellCap] PRICE-CALC arg=" + arg + " result=" + __result);

                Config config = RealParts.ActiveConfig;
                if (config == null || !config.EnableSellCap || __0 == null)
                {
                    return;
                }

                var idProp = __0.GetType().GetProperty("ID") ?? __0.GetType().GetProperty("Id");
                string id = idProp != null ? idProp.GetValue(__0, null) as string : null;
                if (string.IsNullOrEmpty(id))
                {
                    return;
                }

                PartProperty pp = Singleton<GameInventory>.Instance.GetItemProperty(id);
                if (pp == null || pp.Price <= 0)
                {
                    return;
                }

                int cap = Mathf.RoundToInt(pp.Price * config.SellRatio * SellPriceCapPatch.ConditionFactor(__0, config));
                if (__result > cap)
                {
                    MelonLogger.Msg("[SellCap] PRICE-CALC clamp " + __result + " -> " + cap + " (id=" + id + " shopPrice=" + pp.Price + ")");
                    __result = cap;
                }
            }
            catch
            {
            }
        }

        private static void PostfixBtn(object __instance)
        {
            try
            {
                MelonLogger.Msg("[SellCap] SELL-BTN owner=" + (__instance != null ? __instance.GetType().Name : "null"));
            }
            catch
            {
            }
        }

        internal static void Install(HarmonyInstance harmony)
        {
            var postfix = typeof(SellProbePatch).GetMethod("Postfix", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var postfixBtn = typeof(SellProbePatch).GetMethod("PostfixBtn", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            int calc = 0;
            int btn = 0;
            foreach (var t in GameTypes.All())
            {
                System.Reflection.MethodInfo[] methods;
                try
                {
                    methods = t.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static);
                }
                catch
                {
                    continue;
                }

                foreach (var m in methods)
                {
                    var ps = m.GetParameters();

                    if ((m.Name == "GetPrice" || m.Name == "GetItemPrice") && ps.Length == 2 && ps[1].ParameterType == typeof(float) && !ps[0].ParameterType.IsValueType && m.ReturnType == typeof(int))
                    {
                        try
                        {
                            harmony.Patch(m, null, new HarmonyLib.HarmonyMethod(postfix));
                            calc++;
                        }
                        catch
                        {
                        }

                        continue;
                    }

                    if ((m.Name == "SellItemAction" || m.Name == "SellPartsAction") && ps.Length == 0 && m.ReturnType == typeof(void))
                    {
                        try
                        {
                            harmony.Patch(m, null, new HarmonyLib.HarmonyMethod(postfixBtn));
                            btn++;
                        }
                        catch
                        {
                        }
                    }
                }
            }

            MelonLogger.Msg("[SellCap] sell probe patched calc=" + calc + " btn=" + btn);
        }
    }

    [HarmonyPatch(typeof(GlobalData), "AddPlayerMoney")]
    internal static class FreeMapTravelPaymentPatch
    {
        private static void Prefix(ref int money)
        {
            if (SellItemCapPatch.PendingSellCap >= 0f)
            {
                int before = money;
                bool clamped = false;
                if (money > 0 && money > SellItemCapPatch.PendingSellCap)
                {
                    money = (int)SellItemCapPatch.PendingSellCap;
                    clamped = true;
                }

                MelonLogger.Msg("[SellCap] DONE: AddPlayerMoney before=" + before + " cap=" + SellItemCapPatch.PendingSellCap + " after=" + money + " clamped=" + clamped);
                SellItemCapPatch.PendingSellCap = -1f;
                return;
            }

            if (FreeTravelService.PendingFreeTravelCost <= 0 || money >= 0)
            {
                return;
            }

            if (money == -FreeTravelService.PendingFreeTravelCost)
            {
                money = 0;
                FreeTravelService.PendingFreeTravelCost = 0;
            }
        }
    }

    [HarmonyPatch(typeof(GlobalData), "SetPlayerMoney")]
    internal static class FreeMapTravelSetMoneyPatch
    {
        private static void Prefix(ref int money)
        {
            if (FreeTravelService.PendingFreeTravelCost <= 0)
            {
                return;
            }

            int currentMoney = GlobalData.PlayerMoney;
            int diff = currentMoney - money;
            if (diff == FreeTravelService.PendingFreeTravelCost)
            {
                money = currentMoney;
                FreeTravelService.PendingFreeTravelCost = 0;
            }
        }
    }

    internal static class StartingMoneyPatch
    {
        internal static bool Pending;
        internal static bool SceneLoaded;

        private static bool Prefix(ref int __result)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || config.StartingMoney <= 0)
            {
                return true;
            }

            __result = config.StartingMoney;
            MelonLogger.Msg("[Money] GetStartingMoney -> " + __result);
            return false;
        }
    }

    internal static class UpdateStartingMoneyPatch
    {
        private static void Postfix()
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || config.StartingMoney <= 0)
            {
                return;
            }

            GlobalData.PlayerMoney = config.StartingMoney;
            StartingMoneyPatch.Pending = true;
            StartingMoneyPatch.SceneLoaded = false;
            MelonLogger.Msg("[Money] UpdateStartingMoney called -> PlayerMoney=" + config.StartingMoney + " (holding until stable)");
        }
    }

    internal static class CarPriceCache
    {
        private static readonly System.Collections.Generic.Dictionary<string, int> BaseByModel = new System.Collections.Generic.Dictionary<string, int>();

        internal static void Remember(string model, int basePrice)
        {
            if (string.IsNullOrEmpty(model) || basePrice <= 0)
            {
                return;
            }

            int old;
            if (BaseByModel.TryGetValue(model, out old) && old == basePrice)
            {
                return;
            }

            BaseByModel[model] = basePrice;
            MelonLogger.Msg("[CarSale] cached baseCarPrice=" + basePrice + " model=" + model);
        }

        internal static bool TryGet(string model, out int basePrice)
        {
            return BaseByModel.TryGetValue(model, out basePrice);
        }
    }

    [HarmonyPatch(typeof(CMS.UI.Logic.Tabs.CarInfo.CarSummaryTab), "SetupForSell")]
    internal static class CarSaleConditionPatch
    {
        private static void Postfix(CMS.UI.Logic.Tabs.CarInfo.CarSummaryTab __instance)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || !config.EnableCarSaleCondition || __instance == null)
            {
                return;
            }

            try
            {
                float cond = __instance.globalCondition;
                if (cond > 1f)
                {
                    cond = cond / 100f;
                }

                if (cond < 0f)
                {
                    cond = 0f;
                }

                if (cond > 1f)
                {
                    cond = 1f;
                }

                int basePrice = __instance.baseCarPrice;
                int gamePrice = __instance.sellPrice;
                if (basePrice <= 0)
                {
                    MelonLogger.Warning("[CarSale] baseCarPrice=" + basePrice + "; scaling skipped (game price kept)");
                    return;
                }

                int newPrice = Mathf.RoundToInt(basePrice * (config.CarSaleConditionFloor + config.CarSaleConditionSpan * cond));
                __instance.sellPrice = newPrice;
                CarPriceCache.Remember(__instance.currentCarLoader != null ? __instance.currentCarLoader.carToLoad : null, basePrice);

                Text sell = __instance.sellValue;
                if (sell != null && sell.text != null)
                {
                    sell.text = ReplaceNumber(sell.text, gamePrice, newPrice);
                }

                Text profit = __instance.profit;
                if (profit != null && profit.text != null)
                {
                    int oldProfit;
                    if (TryReadNumber(profit.text, out oldProfit))
                    {
                        profit.text = ReplaceNumber(profit.text, oldProfit, oldProfit + (newPrice - gamePrice));
                    }
                }

                MelonLogger.Msg("[CarSale] cond=" + cond + " base=" + basePrice + " game=" + gamePrice + " new=" + newPrice);
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[CarSale] failed: " + e.Message);
            }
        }

        private static bool TryReadNumber(string text, out int value)
        {
            value = 0;
            var matches = System.Text.RegularExpressions.Regex.Matches(text, @"\d[\d\s.,\u00A0]*\d|\d");
            foreach (System.Text.RegularExpressions.Match m in matches)
            {
                string digits = DigitsOnly(m.Value);
                if (int.TryParse(digits, out value))
                {
                    return true;
                }
            }

            return false;
        }

        private static string ReplaceNumber(string text, int oldValue, int newValue)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            var matches = System.Text.RegularExpressions.Regex.Matches(text, @"\d[\d\s.,\u00A0]*\d|\d");
            foreach (System.Text.RegularExpressions.Match m in matches)
            {
                int v;
                if (!int.TryParse(DigitsOnly(m.Value), out v) || v != oldValue)
                {
                    continue;
                }

                char sep = '\0';
                foreach (char c in m.Value)
                {
                    if (!char.IsDigit(c))
                    {
                        sep = c;
                        break;
                    }
                }

                return text.Substring(0, m.Index) + FormatWithSeparator(newValue, sep) + text.Substring(m.Index + m.Value.Length);
            }

            return text;
        }

        private static string DigitsOnly(string token)
        {
            var sb = new System.Text.StringBuilder();
            foreach (char c in token)
            {
                if (char.IsDigit(c))
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        private static string FormatWithSeparator(int value, char sep)
        {
            string digits = System.Math.Abs(value).ToString(System.Globalization.CultureInfo.InvariantCulture);
            string sign = value < 0 ? "-" : "";
            if (sep == '\0' || digits.Length <= 3)
            {
                return sign + digits;
            }

            var sb = new System.Text.StringBuilder();
            int count = 0;
            for (int i = digits.Length - 1; i >= 0; i--)
            {
                sb.Insert(0, digits[i]);
                count++;
                if (count % 3 == 0 && i > 0)
                {
                    sb.Insert(0, sep);
                }
            }

            return sign + sb.ToString();
        }
    }
}
