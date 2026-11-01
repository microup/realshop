using System;
using System.Text;
using UnhollowerBaseLib;
using UnityEngine;

namespace RealShop
{
    internal static class BuildJobMountIndex
    {
        internal const int KindHidden = 0;

        internal const int KindMountable = 1;

        internal const int KindInstalled = 2;

        internal const int KindUnknown = 3;

        internal const int NamesCap = 900;

        internal static System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<int>>
            Build(PartScript[] parts, ref int keyFailures)
        {
            System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<int>> index =
                new System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<int>>(
                    StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < parts.Length; i++)
            {
                PartScript part = parts[i];
                if (part == null)
                {
                    continue;
                }

                string rawName = SafeName(parts, i);
                AddKey(index, rawName, i);
                AddKey(index, NormalizeKeepCounter(rawName), i);
                AddKey(index, Normalize(rawName), i);

                try
                {
                    AddKey(index, BuildJobPartPropertyProbe.PartId(part), i);
                }
                catch (System.Exception)
                {
                    keyFailures++;
                }

                try
                {
                    AddKey(index, part.GetID(), i);
                }
                catch (System.Exception)
                {
                    keyFailures++;
                }
            }

            return index;
        }

        internal static void AddKey(
            System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<int>> index,
            string key, int slot)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            System.Collections.Generic.List<int> slots;
            if (!index.TryGetValue(key, out slots))
            {
                slots = new System.Collections.Generic.List<int>();
                index.Add(key, slots);
            }

            if (!slots.Contains(slot))
            {
                slots.Add(slot);
            }
        }

        internal static int Resolve(
            System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<int>> index,
            bool[] referenced, string id, string label)
        {
            int slot = Pick(index, referenced, id);
            if (slot < 0)
            {
                slot = Pick(index, referenced, label);
            }

            if (slot < 0)
            {
                slot = Pick(index, referenced, NormalizeKeepCounter(label));
            }

            if (slot < 0)
            {
                slot = Pick(index, referenced, Normalize(label));
            }

            return slot;
        }

        internal static int Pick(
            System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<int>> index,
            bool[] referenced, string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return -1;
            }

            System.Collections.Generic.List<int> slots;
            if (!index.TryGetValue(key, out slots) || slots.Count == 0)
            {
                return -1;
            }

            for (int i = 0; i < slots.Count; i++)
            {
                if (!referenced[slots[i]])
                {
                    return slots[i];
                }
            }

            return -1;
        }

        internal static int Classify(PartScript part)
        {
            if (part == null)
            {
                return KindUnknown;
            }

            try
            {
                GameObject go = part.gameObject;
                if (go == null || !go.activeInHierarchy)
                {
                    return KindHidden;
                }

                return part.IsUnmounted ? KindMountable : KindInstalled;
            }
            catch (System.Exception)
            {
                return KindUnknown;
            }
        }

        internal static string SafeName(PartScript[] parts, int slot)
        {
            try
            {
                return parts[slot].name;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        internal static JobPart FirstPart(Il2CppReferenceArray<JobTask> tasks)
        {
            for (int i = 0; i < tasks.Length; i++)
            {
                JobTask task = tasks[i];
                if (task == null || task.Parts == null)
                {
                    continue;
                }

                for (int k = 0; k < task.Parts.Count; k++)
                {
                    JobPart part = task.Parts[k];
                    if (part != null)
                    {
                        return part;
                    }
                }
            }

            return null;
        }

        internal static void Append(StringBuilder names, string name, ref int dropped)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            if (names.Length >= NamesCap)
            {
                dropped++;
                return;
            }

            if (names.Length > 0)
            {
                names.Append('|');
            }

            names.Append(name);
        }

        internal static string Normalize(string raw)
        {
            if (string.IsNullOrEmpty(raw))
            {
                return null;
            }

            string s = raw;
            int slash = s.LastIndexOf('/');
            if (slash >= 0)
            {
                s = s.Substring(slash + 1);
            }

            s = s.Replace("(Clone)", string.Empty).Trim();
            s = StripTrailingCounter(s);
            s = s.Trim();
            return s.Length == 0 ? null : s;
        }

        internal static string NormalizeKeepCounter(string raw)
        {
            if (string.IsNullOrEmpty(raw))
            {
                return null;
            }

            string s = raw;
            int slash = s.LastIndexOf('/');
            if (slash >= 0)
            {
                s = s.Substring(slash + 1);
            }

            s = s.Replace("(Clone)", string.Empty);
            s = s.Replace(" (", "(");
            s = s.Trim();
            return s.Length == 0 ? null : s;
        }

        private static string StripTrailingCounter(string s)
        {
            int len = s.Length;
            if (len < 4 || s[len - 1] != ')')
            {
                return s;
            }

            int open = s.LastIndexOf('(');
            if (open <= 0 || open >= len - 2)
            {
                return s;
            }

            for (int i = open + 1; i < len - 1; i++)
            {
                if (!char.IsDigit(s[i]))
                {
                    return s;
                }
            }

            return s.Substring(0, open);
        }
    }
}
