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
    internal static partial class RarityColorPatches
    {
        private static readonly Color RareColor = new Color(0.13f, 0.5f, 0.13f, 1f);
        private static readonly Color UniqueColor = new Color(0.5f, 0.15f, 0.65f, 1f);
        private static readonly Color MaxBarColor = new Color(0.55f, 0.55f, 0.55f, 1f);

        internal static bool TestAllRare = false;

        private static readonly System.Collections.Generic.HashSet<string> SeenRarity = new System.Collections.Generic.HashSet<string>();

        private static readonly System.Collections.Generic.Dictionary<System.IntPtr, Color> OriginalBgColor = new System.Collections.Generic.Dictionary<System.IntPtr, Color>();

        private static readonly System.Collections.Generic.HashSet<System.IntPtr> ColoredSlots = new System.Collections.Generic.HashSet<System.IntPtr>();

        private static readonly Color NeedColor = new Color(0.95f, 0.6f, 0.1f, 1f);
        private static readonly System.Collections.Generic.Dictionary<System.IntPtr, GameObject> MarkerCache = new System.Collections.Generic.Dictionary<System.IntPtr, GameObject>();
        private static readonly System.Collections.Generic.HashSet<string> SeenNeedLog = new System.Collections.Generic.HashSet<string>();
        private static Sprite _needSprite;
        private static Sprite _betterSprite;

        private static readonly Color ProfitColor = new Color(0.18f, 0.7f, 0.28f, 1f);
        private static readonly System.Collections.Generic.Dictionary<System.IntPtr, GameObject> ProfitMarkerCache = new System.Collections.Generic.Dictionary<System.IntPtr, GameObject>();
        private static readonly System.Collections.Generic.HashSet<string> SeenProfitLog = new System.Collections.Generic.HashSet<string>();
        private static Sprite _profitSprite;

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
                Item item = __0 as Item;
                if (item == null)
                {
                    return;
                }

                string id = GetId(__0);
                if (string.IsNullOrEmpty(id))
                {
                    return;
                }

                ApplyMaxQualityMarker(item, __1);
                ApplyNeededMarker(id, __1, item.GetConditionToShow());
                ApplyProfitMarker(item, __1);

                string rarity = MaxQualityService.GradeFor(item);
                if (TestAllRare)
                {
                    rarity = "rare";
                }

                if (SeenRarity.Add(id))
                {
                    MelonLogger.Msg("[RarityColor] RARITY id=" + id + " rarity=" + rarity);
                }

                if (rarity == "rare" && MaxQualityService.IsBody(item))
                {
                    rarity = "common";
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

        private static void ApplyNeededMarker(string id, object ui, float cond)
        {
            GameObject slot = GetGameObject(ui);
            if (slot == null)
            {
                return;
            }

            bool needed = GarageNeedsRecon.IsNeeded(id);
            bool better = !needed && GarageNeedsRecon.IsBetter(id, cond);

            if (!needed && !better)
            {
                GameObject off = GetCachedMarker(slot);
                if (off != null && off.activeSelf)
                {
                    off.SetActive(false);
                }

                return;
            }

            GameObject marker = GetOrCreateMarker(slot, ui);
            if (marker == null)
            {
                return;
            }

            Image img = marker.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = needed ? NeedSprite() : BetterSprite();
            }

            if (!marker.activeSelf)
            {
                marker.SetActive(true);
                marker.transform.SetAsLastSibling();
                if (SeenNeedLog.Add(id + "|" + (needed ? "need" : "better")))
                {
                    MelonLogger.Msg("[Needs] MARK id=" + id + " kind=" + (needed ? "need" : "better") + " cond=" + cond + " ui=" + (ui != null ? ui.GetType().Name : "null"));
                }
            }
        }

        private static void ApplyProfitMarker(Item item, object ui)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || !config.EnableProfitMarker || item == null || ui == null)
            {
                return;
            }

            GameObject slot = GetGameObject(ui);
            if (slot == null)
            {
                return;
            }

            float cond = item.GetConditionToShow();
            float cap = MaxQualityService.MaxFraction(item);
            bool show = false;
            string model = null;
            float gain = 0f;
            if (cond < cap - 0.0001f && cond < 0.995f)
            {
                float delta = Mathf.Min(cap - cond, MarginService.EstimatedCycleDelta);
                int fee = LaborCostService.Fee(config, MaxQualityService.GradeFor(item), cond);
                show = MarginService.IsCycleProfitable(item, fee, delta, out model, out gain);
            }

            if (!show)
            {
                GameObject off = GetCachedProfitMarker(slot);
                if (off != null && off.activeSelf)
                {
                    off.SetActive(false);
                }

                return;
            }

            GameObject marker = GetOrCreateProfitMarker(slot);
            if (marker == null)
            {
                return;
            }

            if (!marker.activeSelf)
            {
                marker.SetActive(true);
                marker.transform.SetAsLastSibling();
                if (SeenProfitLog.Add(item.ID))
                {
                    MelonLogger.Msg("[Profit] MARK $ id=" + item.ID + " cond=" + cond + " gain=" + gain + " model=" + model + " ui=" + (ui != null ? ui.GetType().Name : "null"));
                }
            }
        }

    }
}

#pragma warning restore CS0618
