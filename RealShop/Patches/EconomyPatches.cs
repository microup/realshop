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
}
