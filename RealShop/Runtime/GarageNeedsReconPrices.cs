using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace RealShop
{
    internal static partial class GarageNeedsRecon
    {
        internal static bool TryGetModelBuyPrice(string model, out int buyPrice)
        {
            return ModelBuyPrice.TryGetValue(model, out buyPrice);
        }

        internal static bool TryGetNeedModels(string id, out List<string> models)
        {
            models = null;
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }

            MaybeRefresh();
            List<string> direct;
            if (NeedModelsByComponent.TryGetValue(id, out direct) && direct.Count > 0)
            {
                models = direct;
                return true;
            }

            if (id.StartsWith("car_"))
            {
                foreach (var m in OwnedModels)
                {
                    if (id.StartsWith(m + "-"))
                    {
                        string rest = id.Substring(m.Length + 1);
                        HashSet<string> slots;
                        if (NeededBodyByModel.TryGetValue(m, out slots) && slots.Contains(BaseSlot(rest)))
                        {
                            models = new List<string> { m };
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        internal static int PartCountFor(string model)
        {
            int n;
            if (PartCountByModel.TryGetValue(model, out n))
            {
                return n;
            }

            return 0;
        }

        private static void CollectComponents(object listObj, string model)
        {
            foreach (var p in Enumerate(listObj))
            {
                if (p == null)
                {
                    continue;
                }
                Type pt = p.GetType();
                string leaf = Leaf(AsStr(Get(pt, p, "Path")));
                if (string.IsNullOrEmpty(leaf))
                {
                    continue;
                }

                int count;
                PartCountByModel.TryGetValue(model, out count);
                PartCountByModel[model] = count + 1;

                if (AsStr(Get(pt, p, "Unmounted")) == "True")
                {
                    NeededComponents.Add(leaf);
                    List<string> models;
                    if (!NeedModelsByComponent.TryGetValue(leaf, out models))
                    {
                        models = new List<string>();
                        NeedModelsByComponent[leaf] = models;
                    }

                    if (!models.Contains(model))
                    {
                        models.Add(model);
                    }
                }
                else
                {
                    float cond = ToFloat(Get(pt, p, "Condition"));
                    float best;
                    if (!OwnedBestCond.TryGetValue(leaf, out best) || cond > best)
                    {
                        OwnedBestCond[leaf] = cond;
                    }
                }
            }
        }

        private static void CollectBody(object listObj, string model)
        {
            HashSet<string> slots;
            if (!NeededBodyByModel.TryGetValue(model, out slots))
            {
                slots = new HashSet<string>();
                NeededBodyByModel[model] = slots;
            }
            Dictionary<string, float> bestSlots;
            if (!OwnedBodyBestCond.TryGetValue(model, out bestSlots))
            {
                bestSlots = new Dictionary<string, float>();
                OwnedBodyBestCond[model] = bestSlots;
            }
            foreach (var p in Enumerate(listObj))
            {
                if (p == null)
                {
                    continue;
                }
                Type pt = p.GetType();
                string id = AsStr(Get(pt, p, "Id"));
                if (string.IsNullOrEmpty(id))
                {
                    continue;
                }

                int count;
                PartCountByModel.TryGetValue(model, out count);
                PartCountByModel[model] = count + 1;

                string baseSlot = BaseSlot(id);
                if (AsStr(Get(pt, p, "Unmounted")) == "True")
                {
                    slots.Add(baseSlot);
                }
                else
                {
                    float cond = ToFloat(Get(pt, p, "Condition"));
                    float best;
                    if (!bestSlots.TryGetValue(baseSlot, out best) || cond > best)
                    {
                        bestSlots[baseSlot] = cond;
                    }
                }
            }
        }

        private static string Leaf(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            int slash = path.LastIndexOf('/');
            string seg = slash >= 0 ? path.Substring(slash + 1) : path;
            int paren = seg.IndexOf('(');
            if (paren >= 0)
            {
                seg = seg.Substring(0, paren);
            }
            return seg;
        }

        private static string BaseSlot(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }
            int i = s.Length;
            while (i > 0 && char.IsDigit(s[i - 1]))
            {
                i--;
            }
            if (i < s.Length && i > 0 && s[i - 1] != '_')
            {
                return s.Substring(0, i);
            }
            return s;
        }

    }
}
