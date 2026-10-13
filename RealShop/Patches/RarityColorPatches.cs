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
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS0618

namespace RealShop
{
    internal static class RarityColorPatches
    {
        private static readonly Color RareColor = new Color(0.13f, 0.5f, 0.13f, 1f);
        private static readonly Color UniqueColor = new Color(0.5f, 0.15f, 0.65f, 1f);

        internal static bool TestAllRare = false;

        private static readonly System.Collections.Generic.HashSet<string> SeenRarity = new System.Collections.Generic.HashSet<string>();

        private static readonly System.Collections.Generic.Dictionary<System.IntPtr, Color> OriginalBgColor = new System.Collections.Generic.Dictionary<System.IntPtr, Color>();

        private static readonly System.Collections.Generic.HashSet<System.IntPtr> ColoredSlots = new System.Collections.Generic.HashSet<System.IntPtr>();

        internal static void Install(HarmonyInstance harmony)
        {
            MethodInfo postfix = typeof(RarityColorPatches).GetMethod("Postfix", BindingFlags.NonPublic | BindingFlags.Static);
            HarmonyLib.HarmonyMethod harmonyPostfix = new HarmonyLib.HarmonyMethod(postfix);

            var targets = new System.Collections.Generic.List<KeyValuePair<string, Type>>
            {
                new KeyValuePair<string, Type>("FillShopItem", typeof(PartProperty)),
                new KeyValuePair<string, Type>("FillInventoryItem", typeof(Item)),
                new KeyValuePair<string, Type>("FillBasicInventoryItem", typeof(Item)),
                new KeyValuePair<string, Type>("FillInventoryItemDetails", typeof(Item)),
                new KeyValuePair<string, Type>("FillChoosePartDownItem", typeof(ChoosePartDownItem)),
            };

            int patched = 0;
            foreach (var t in targets)
            {
                MethodInfo method = FindStatic(t.Key, t.Value);
                if (method == null)
                {
                    MelonLogger.Warning("[RarityColor] hook not found: " + t.Key);
                    continue;
                }

                try
                {
                    harmony.Patch(method, null, harmonyPostfix);
                    patched++;
                }
                catch (Exception e)
                {
                    MelonLogger.Warning("[RarityColor] patch failed for " + t.Key + ": " + e.Message);
                }
            }

            MelonLogger.Msg("[RarityColor] hooks patched: " + patched + "/" + targets.Count);
        }

        private static MethodInfo FindStatic(string name, Type firstParam)
        {
            foreach (var t in GameTypes.All())
            {
                MethodInfo[] methods;
                try
                {
                    methods = t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                }
                catch
                {
                    continue;
                }

                foreach (var m in methods)
                {
                    if (m.Name != name)
                    {
                        continue;
                    }

                    var ps = m.GetParameters();
                    if (ps.Length >= 2 && ps[0].ParameterType == firstParam)
                    {
                        return m;
                    }
                }
            }

            return null;
        }

        private static void Postfix(object __0, object __1)
        {
            try
            {
                string id = GetId(__0);
                if (string.IsNullOrEmpty(id))
                {
                    return;
                }

                string rarity = RetroPartCatalog.GetRarityForPart(id);
                if (TestAllRare)
                {
                    rarity = "rare";
                }

                if (SeenRarity.Add(id))
                {
                    MelonLogger.Msg("[RarityColor] RARITY id=" + id + " rarity=" + rarity);
                }

                bool isRare = rarity == "rare" || rarity == "unique";

                if (!isRare)
                {
                    GameObject slot = GetGameObject(__1);
                    if (slot != null && ColoredSlots.Contains(slot.Pointer))
                    {
                        foreach (var bg in FindBackgroundImages(__1))
                        {
                            Color original;
                            if (OriginalBgColor.TryGetValue(bg.Pointer, out original))
                            {
                                bg.color = original;
                            }
                        }

                        ColoredSlots.Remove(slot.Pointer);
                        MelonLogger.Msg("[RarityColor] RESET id=" + id + " ui=" + (__1 != null ? __1.GetType().Name : "null"));
                    }

                    return;
                }

                DumpStructure(__1);
                var backgrounds = FindBackgroundImages(__1);
                Color color = rarity == "unique" ? UniqueColor : RareColor;
                var applied = new System.Text.StringBuilder();
                foreach (var bg in backgrounds)
                {
                    if (!OriginalBgColor.ContainsKey(bg.Pointer))
                    {
                        OriginalBgColor[bg.Pointer] = bg.color;
                    }

                    bg.color = color;
                    if (applied.Length > 0)
                    {
                        applied.Append(',');
                    }

                    applied.Append(bg.gameObject.name);
                }

                GameObject coloredSlot = GetGameObject(__1);
                if (coloredSlot != null)
                {
                    ColoredSlots.Add(coloredSlot.Pointer);
                }

                MelonLogger.Msg("[RarityColor] FOUND rarity=" + rarity + " id=" + id + " ui=" + (__1 != null ? __1.GetType().Name : "null") + " color=" + (backgrounds.Count > 0 ? "applied(" + applied + ")" : "NOT_APPLIED"));
            }
            catch
            {
            }
        }

        private static string GetId(object source)
        {
            if (source == null)
            {
                return null;
            }

            string id = GetProp(source, "ID") as string;
            if (!string.IsNullOrEmpty(id))
            {
                return id;
            }

            object baseItem = GetProp(source, "BaseItem");
            if (baseItem != null)
            {
                return GetProp(baseItem, "ID") as string;
            }

            return null;
        }

        private static object GetProp(object target, string name)
        {
            try
            {
                PropertyInfo prop = target.GetType().GetProperty(name);
                if (prop != null && prop.CanRead && prop.GetIndexParameters().Length == 0)
                {
                    return prop.GetValue(target, null);
                }
            }
            catch
            {
            }

            return null;
        }

        private static readonly System.Collections.Generic.HashSet<string> _dumped = new System.Collections.Generic.HashSet<string>();

        private static void DumpStructure(object ui)
        {
            if (ui == null)
            {
                return;
            }

            Type t = ui.GetType();
            if (!_dumped.Add(t.FullName))
            {
                return;
            }

            var sb = new System.Text.StringBuilder();
            sb.Append("[RarityColor] DUMP ").Append(t.Name).Append(" isComponent=").Append(ui is UnityEngine.Component);

            foreach (var f in t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                sb.Append(" F.").Append(f.Name).Append(':').Append(f.FieldType.Name);
            }

            foreach (var p in t.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (p.GetIndexParameters().Length > 0)
                {
                    continue;
                }

                sb.Append(" P.").Append(p.Name).Append(':').Append(p.PropertyType.Name);
            }

            MelonLogger.Msg(sb.ToString());

            GameObject go = GetGameObject(ui);
            if (go == null)
            {
                MelonLogger.Msg("[RarityColor] DUMP " + t.Name + " -> no GameObject");
                return;
            }

            var sb2 = new System.Text.StringBuilder();
            sb2.Append("[RarityColor] IMAGES under ").Append(go.name).Append(": ");
            foreach (var img in go.GetComponentsInChildren<Image>(true))
            {
                var r = img.rectTransform.rect;
                sb2.Append(img.gameObject.name).Append('(').Append(Mathf.RoundToInt(r.width)).Append('x').Append(Mathf.RoundToInt(r.height)).Append(") ");
            }

            MelonLogger.Msg(sb2.ToString());
        }

        private static System.Collections.Generic.List<Image> FindBackgroundImages(object ui)
        {
            var result = new System.Collections.Generic.List<Image>();
            if (ui == null)
            {
                return result;
            }

            Type t = ui.GetType();

            foreach (var name in new[] { "bgSolid", "background", "bg" })
            {
                foreach (var f in t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (typeof(Image).IsAssignableFrom(f.FieldType) && f.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
                    {
                        var img = SafeGet(() => f.GetValue(ui) as Image);
                        if (img != null && !result.Contains(img))
                        {
                            result.Add(img);
                        }
                    }
                }

                foreach (var p in t.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (typeof(Image).IsAssignableFrom(p.PropertyType) && p.CanRead && p.GetIndexParameters().Length == 0 && p.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
                    {
                        var img = SafeGet(() => p.GetValue(ui, null) as Image);
                        if (img != null && !result.Contains(img))
                        {
                            result.Add(img);
                        }
                    }
                }
            }

            GameObject go = GetGameObject(ui);
            if (go != null)
            {
                foreach (var img in go.GetComponentsInChildren<Image>(true))
                {
                    string n = img.gameObject.name.ToLowerInvariant();
                    if (n == "bgsolid" || n == "bgimg" || n == "bg" || n == "bg2")
                    {
                        if (!result.Contains(img))
                        {
                            result.Add(img);
                        }
                    }
                }

                if (result.Count == 0)
                {
                    var root = go.GetComponent<Image>();
                    if (root != null)
                    {
                        result.Add(root);
                    }
                }
            }

            return result;
        }

        private static GameObject GetGameObject(object ui)
        {
            if (ui == null)
            {
                return null;
            }

            if (ui is GameObject)
            {
                return (GameObject)ui;
            }

            if (ui is UnityEngine.Component)
            {
                return ((UnityEngine.Component)ui).gameObject;
            }

            try
            {
                var goProp = ui.GetType().GetProperty("gameObject");
                if (goProp != null && typeof(GameObject).IsAssignableFrom(goProp.PropertyType))
                {
                    return goProp.GetValue(ui, null) as GameObject;
                }
            }
            catch
            {
            }

            return null;
        }

        private static Image SafeGet(Func<Image> getter)
        {
            try
            {
                return getter();
            }
            catch
            {
                return null;
            }
        }
    }
}

#pragma warning restore CS0618
