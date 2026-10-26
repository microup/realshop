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
        private static GameObject GetCachedProfitMarker(GameObject slot)
        {
            GameObject cached;
            if (ProfitMarkerCache.TryGetValue(slot.Pointer, out cached) && cached != null)
            {
                return cached;
            }

            return null;
        }

        private static GameObject GetOrCreateProfitMarker(GameObject slot)
        {
            GameObject cached;
            if (ProfitMarkerCache.TryGetValue(slot.Pointer, out cached) && cached != null)
            {
                return cached;
            }

            Transform existing = slot.transform.Find("RealShopProfit");
            if (existing != null)
            {
                ProfitMarkerCache[slot.Pointer] = existing.gameObject;
                return existing.gameObject;
            }

            GameObject root = new GameObject("RealShopProfit");
            root.transform.SetParent(slot.transform, false);
            Image bg = root.AddComponent<Image>();
            bg.sprite = ProfitSprite();
            bg.color = Color.white;
            bg.raycastTarget = false;
            RectTransform rt = bg.rectTransform;
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);
            rt.anchoredPosition = new Vector2(6f, -36f);
            rt.sizeDelta = new Vector2(26f, 26f);

            root.SetActive(false);
            root.transform.SetAsLastSibling();
            ProfitMarkerCache[slot.Pointer] = root;
            return root;
        }

        private static Sprite ProfitSprite()
        {
            if (_profitSprite == null)
            {
                int s = 32;
                Texture2D tex = new Texture2D(s, s, TextureFormat.RGBA32, false);
                Color[] px = new Color[s * s];
                for (int i = 0; i < px.Length; i++)
                {
                    px[i] = ProfitColor;
                }

                for (int y = 0; y < s; y++)
                {
                    for (int x = 0; x < s; x++)
                    {
                        bool stem = x >= 15 && x <= 16 && y >= 3 && y <= 28;
                        bool topBar = y >= 22 && y <= 24 && x >= 11 && x <= 20;
                        bool upperLeft = x >= 10 && x <= 12 && y >= 16 && y <= 24;
                        bool midBar = y >= 14 && y <= 16 && x >= 10 && x <= 21;
                        bool lowerRight = x >= 19 && x <= 21 && y >= 7 && y <= 16;
                        bool bottomBar = y >= 6 && y <= 8 && x >= 11 && x <= 20;
                        if (stem || topBar || upperLeft || midBar || lowerRight || bottomBar)
                        {
                            px[y * s + x] = Color.white;
                        }
                    }
                }

                tex.SetPixels(px);
                tex.Apply();
                _profitSprite = Sprite.Create(tex, new Rect(0f, 0f, s, s), new Vector2(0.5f, 0.5f));
            }

            return _profitSprite;
        }

        private static GameObject GetCachedMarker(GameObject slot)
        {
            GameObject cached;
            if (MarkerCache.TryGetValue(slot.Pointer, out cached) && cached != null)
            {
                return cached;
            }

            return null;
        }

        private static GameObject GetOrCreateMarker(GameObject slot, object ui)
        {
            GameObject cached;
            if (MarkerCache.TryGetValue(slot.Pointer, out cached) && cached != null)
            {
                return cached;
            }

            Transform existing = slot.transform.Find("RealShopNeed");
            if (existing != null)
            {
                MarkerCache[slot.Pointer] = existing.gameObject;
                return existing.gameObject;
            }

            GameObject root = new GameObject("RealShopNeed");
            root.transform.SetParent(slot.transform, false);
            Image bg = root.AddComponent<Image>();
            bg.sprite = NeedSprite();
            bg.color = Color.white;
            bg.raycastTarget = false;
            RectTransform rt = bg.rectTransform;
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);
            rt.anchoredPosition = new Vector2(6f, -6f);
            rt.sizeDelta = new Vector2(26f, 26f);

            root.SetActive(false);
            root.transform.SetAsLastSibling();
            MarkerCache[slot.Pointer] = root;
            return root;
        }

        private static Sprite NeedSprite()
        {
            if (_needSprite == null)
            {
                int s = 32;
                Texture2D tex = new Texture2D(s, s, TextureFormat.RGBA32, false);
                Color[] px = new Color[s * s];
                for (int i = 0; i < px.Length; i++)
                {
                    px[i] = NeedColor;
                }

                for (int y = 0; y < s; y++)
                {
                    for (int x = 0; x < s; x++)
                    {
                        bool stroke = x >= 15 && x <= 17;
                        bool bar = stroke && y >= 13 && y <= 25;
                        bool dot = stroke && y >= 6 && y <= 9;
                        if (bar || dot)
                        {
                            px[y * s + x] = Color.white;
                        }
                    }
                }

                tex.SetPixels(px);
                tex.Apply();
                _needSprite = Sprite.Create(tex, new Rect(0f, 0f, s, s), new Vector2(0.5f, 0.5f));
            }

            return _needSprite;
        }

        private static Sprite BetterSprite()
        {
            if (_betterSprite == null)
            {
                int s = 32;
                Texture2D tex = new Texture2D(s, s, TextureFormat.RGBA32, false);
                Color green = new Color(0.18f, 0.7f, 0.28f, 1f);
                Color[] px = new Color[s * s];
                for (int i = 0; i < px.Length; i++)
                {
                    px[i] = green;
                }

                int apex = 22;
                int baseY = 10;
                int maxHalf = 9;
                int span = apex - baseY;
                for (int y = baseY; y <= apex; y++)
                {
                    int t = apex - y;
                    int halfw = 1 + (t * maxHalf) / span;
                    for (int x = 16 - halfw; x <= 16 + halfw; x++)
                    {
                        if (x >= 0 && x < s)
                        {
                            px[y * s + x] = Color.white;
                        }
                    }
                }

                tex.SetPixels(px);
                tex.Apply();
                _betterSprite = Sprite.Create(tex, new Rect(0f, 0f, s, s), new Vector2(0.5f, 0.5f));
            }

            return _betterSprite;
        }

        private static void ApplyMaxQualityMarker(object source, object ui)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || !config.EnableMaxQuality)
            {
                return;
            }

            Item item = source as Item;
            if (item == null || ui == null)
            {
                return;
            }

            Image bar = GetPropAny(ui, "condition") as Image;
            Text text = GetPropAny(ui, "conditionPercentage") as Text;
            if (bar == null && text == null)
            {
                return;
            }

            float max = MaxQualityService.MaxFraction(item);
            float show = item.GetConditionToShow();

            if (show >= 0.995f)
            {
                return;
            }

            if (show >= max - 0.0001f)
            {
                if (bar != null)
                {
                    bar.color = MaxBarColor;
                }

                if (text != null)
                {
                    text.text = "MAX " + Mathf.RoundToInt(max * 100f) + "%";
                }
            }
            else if (text != null)
            {
                text.text = Mathf.RoundToInt(show * 100f) + "-" + Mathf.RoundToInt(max * 100f) + "%";
            }
        }

        private static object GetPropAny(object target, string name)
        {
            try
            {
                PropertyInfo prop = target.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
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
