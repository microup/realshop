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
    internal static class PatchBootstrap
    {
        internal static void Install(HarmonyInstance harmony)
        {
            TryPatchSellItem(harmony);
            TryPatchSellPrice(harmony);
            SellProbePatch.Install(harmony);
            RarityColorPatches.Install(harmony);
            TryPatchStartingMoney(harmony);
            LogDiagnostics();
        }

        private static void TryPatchSellItem(HarmonyInstance harmony)
        {
            try
            {
                System.Reflection.MethodInfo target = null;
                string ownerName = null;

                string[] candidates = { "GameScript", "InventoryWindow", "SellPerConditionWindow", "Inventory" };
                foreach (var name in candidates)
                {
                    System.Type owner = HarmonyLib.AccessTools.TypeByName(name);
                    if (TryFindSellItem(owner, out target))
                    {
                        ownerName = owner.FullName;
                        break;
                    }
                }

                if (target == null)
                {
                    foreach (var owner in GameTypes.All())
                    {
                        if (TryFindSellItem(owner, out target))
                        {
                            ownerName = owner.FullName;
                            break;
                        }
                    }
                }

                if (target == null)
                {
                    MelonLogger.Warning("[SellCap] SellItem(Item,bool,bool) not found; sell-cap disabled");
                    return;
                }

                MelonLogger.Msg("[SellCap] found SellItem on " + ownerName);
                var prefixMethod = typeof(SellItemCapPatch).GetMethod("Prefix", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                harmony.Patch(target, new HarmonyLib.HarmonyMethod(prefixMethod));
                MelonLogger.Msg("[SellCap] patched OK");
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[SellCap] patch failed: " + e.Message);
            }
        }

        private static bool TryFindSellItem(System.Type owner, out System.Reflection.MethodInfo target)
        {
            target = null;
            if (owner == null)
            {
                return false;
            }

            System.Reflection.MethodInfo[] methods;
            try
            {
                methods = owner.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static);
            }
            catch
            {
                return false;
            }

            foreach (var m in methods)
            {
                if (m.Name != "SellItem")
                {
                    continue;
                }

                var ps = m.GetParameters();
                if (ps.Length == 3 && ps[0].ParameterType == typeof(Item) && ps[1].ParameterType == typeof(bool) && ps[2].ParameterType == typeof(bool))
                {
                    target = m;
                    return true;
                }
            }

            return false;
        }

        private static void TryPatchSellPrice(HarmonyInstance harmony)
        {
            try
            {
                var owners = new System.Text.StringBuilder();
                var clean = new System.Collections.Generic.List<System.Type>();
                foreach (var t in GameTypes.All())
                {
                    System.Reflection.PropertyInfo p;
                    try
                    {
                        p = t.GetProperty("sellPrice");
                    }
                    catch
                    {
                        p = null;
                    }

                    if (p == null || !p.CanRead)
                    {
                        continue;
                    }

                    owners.Append(t.Name).Append(":").Append(p.PropertyType.Name).Append(" ");

                    string n = t.Name;
                    if (n.IndexOf('<') >= 0 || n.IndexOf('+') >= 0 || n.Contains("DisplayClass") || n.Contains("d__"))
                    {
                        continue;
                    }

                    clean.Add(t);
                }

                MelonLogger.Msg("[SellCap] sellPrice owners: " + owners.ToString());

                System.Type owner = null;
                string[] prefer = { "Item", "BaseItem", "PartProperty", "GroupItem", "InventoryItem" };
                foreach (var pref in prefer)
                {
                    foreach (var t in clean)
                    {
                        if (t.Name == pref)
                        {
                            owner = t;
                            break;
                        }
                    }
                    if (owner != null)
                    {
                        break;
                    }
                }

                if (owner == null)
                {
                    MelonLogger.Msg("[SellCap] no item sellPrice owner (car-only); item display handled via sellValue");
                    return;
                }

                System.Reflection.PropertyInfo prop = owner.GetProperty("sellPrice");
                System.Reflection.MethodInfo getter = prop.GetGetMethod();
                if (getter == null)
                {
                    MelonLogger.Warning("[SellCap] sellPrice getter null; display cap skipped");
                    return;
                }

                System.Reflection.MethodInfo postfix;
                if (prop.PropertyType == typeof(int))
                {
                    postfix = typeof(SellPriceCapPatch).GetMethod("PostfixInt", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                }
                else if (prop.PropertyType == typeof(float))
                {
                    postfix = typeof(SellPriceCapPatch).GetMethod("Postfix", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                }
                else
                {
                    MelonLogger.Warning("[SellCap] sellPrice type unsupported: " + prop.PropertyType.Name);
                    return;
                }

                MelonLogger.Msg("[SellCap] chosen sellPrice owner=" + owner.FullName + " type=" + prop.PropertyType.Name);
                harmony.Patch(getter, null, new HarmonyLib.HarmonyMethod(postfix));
                MelonLogger.Msg("[SellCap] sellPrice getter patched OK");
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[SellCap] sellPrice patch failed: " + e.Message);
            }
        }

        private static void TryPatchStartingMoney(HarmonyInstance harmony)
        {
            try
            {
                System.Reflection.MethodInfo getStart = typeof(GlobalData).GetMethod("GetStartingMoney", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                if (getStart != null)
                {
                    var pre = typeof(StartingMoneyPatch).GetMethod("Prefix", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                    harmony.Patch(getStart, new HarmonyLib.HarmonyMethod(pre));
                }

                System.Type pm = HarmonyLib.AccessTools.TypeByName("ProfileManager");
                System.Reflection.MethodInfo upd = pm != null ? pm.GetMethod("UpdateStartingMoney", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance) : null;
                if (upd != null)
                {
                    var post = typeof(UpdateStartingMoneyPatch).GetMethod("Postfix", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                    harmony.Patch(upd, null, new HarmonyLib.HarmonyMethod(post));
                }
                else
                {
                    MelonLogger.Warning("[Money] UpdateStartingMoney not found");
                }

                int sm = RealParts.ActiveConfig != null ? RealParts.ActiveConfig.StartingMoney : 0;
                MelonLogger.Msg("[Money] starting-money hooks OK (StartingMoney=" + sm + ", upd=" + (upd != null) + ")");
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[Money] GetStartingMoney patch failed: " + e.Message);
            }
        }

        private static void LogDiagnostics()
        {
            try
            {
                System.Type rpw = HarmonyLib.AccessTools.TypeByName("RepairPartWindow");
                if (rpw == null)
                {
                    MelonLogger.Msg("[Diag] RepairPartWindow type not found");
                    return;
                }

                var sb = new System.Text.StringBuilder();
                foreach (var m in rpw.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static))
                {
                    string n = m.Name;
                    if (n.IndexOf("Update", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                        n.IndexOf("Refresh", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                        n.IndexOf("Item", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                        n.IndexOf("List", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                        n.IndexOf("Prepare", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                        n.IndexOf("Next", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                        n.IndexOf("Select", System.StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        sb.Append(n).Append("(").Append(m.GetParameters().Length).Append(") ");
                    }
                }

                MelonLogger.Msg("[Diag] RepairPartWindow methods: " + sb.ToString());
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[Diag] failed: " + e.Message);
            }
        }
    }
}

#pragma warning restore CS0618
