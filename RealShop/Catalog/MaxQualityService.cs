using CMS.UI.Logic;
using CMS.UI.Logic.Map;
using CMS.UI.Logic.RepairPart;
using CMS.UI.Windows;
using CMS.Containers;
using CMS.Helpers;
using CMS.Extensions;

namespace RealShop
{
    internal static class MaxQualityService
    {
        internal static string RarityFor(Item item)
        {
            if (item == null)
            {
                return "common";
            }

            Config config = RealParts.ActiveConfig;
            if (config == null)
            {
                return "common";
            }

            int common = config.MaxQualityCommonWeight;
            int rare = config.MaxQualityRareWeight;
            int unique = config.MaxQualityUniqueWeight;
            int total = common + rare + unique;
            if (total <= 0)
            {
                return "common";
            }

            uint roll = HashUid(item.UID) % (uint)total;
            if (roll < (uint)common)
            {
                return "common";
            }

            roll -= (uint)common;
            if (roll < (uint)rare)
            {
                return "rare";
            }

            return "unique";
        }

        internal static float MaxFraction(Item item)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || !config.EnableMaxQuality || item == null)
            {
                return 1f;
            }

            string rarity = RarityFor(item);
            int min;
            int max;
            if (rarity == "unique")
            {
                min = config.MaxQualityUniqueMin;
                max = config.MaxQualityUniqueMax;
            }
            else if (rarity == "rare")
            {
                min = config.MaxQualityRareMin;
                max = config.MaxQualityRareMax;
            }
            else
            {
                min = config.MaxQualityCommonMin;
                max = config.MaxQualityCommonMax;
            }

            if (max < min)
            {
                int t = min;
                min = max;
                max = t;
            }

            int span = max - min + 1;
            int value = min + (int)(HashUid(item.UID ^ 0x51ED2701L) % (uint)span);
            if (IsBody(item))
            {
                int bmin = config.MaxQualityBodyMin;
                int bmax = config.MaxQualityBodyMax;
                if (bmax < bmin)
                {
                    int t = bmin;
                    bmin = bmax;
                    bmax = t;
                }
                int bspan = bmax - bmin + 1;
                value = bmin + (int)(HashUid(item.UID ^ 0x51ED2701L) % (uint)bspan);
            }
            return value / 100f;
        }

        internal static string GradeFor(Item item)
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || !config.EnableMaxQuality || item == null)
            {
                return RarityFor(item);
            }

            int pct = (int)(MaxFraction(item) * 100f + 0.5f);
            if (pct <= config.MaxQualityCommonMax)
            {
                return "common";
            }

            if (pct <= config.MaxQualityRareMax)
            {
                return "rare";
            }

            return "unique";
        }

        internal static bool IsBody(Item item)
        {
            if (item == null)
            {
                return false;
            }

            PartProperty property = Singleton<GameInventory>.Instance.GetItemProperty(item.ID);
            return property != null && property.IsBody;
        }

        internal static bool IsRepairComplete(Item item)
        {
            if (item == null)
            {
                return true;
            }

            float show = item.GetConditionToShow();
            Config config = RealParts.ActiveConfig;
            if (config != null && config.EnableMaxQuality)
            {
                return show >= MaxFraction(item) - 0.0001f;
            }

            return RealParts.IsFullyRepaired(show);
        }

        private static uint HashUid(long uid)
        {
            unchecked
            {
                uint h = (uint)uid ^ (uint)(uid >> 32);
                h *= 2654435761u;
                h ^= h >> 16;
                h *= 2246822519u;
                h ^= h >> 13;
                return h;
            }
        }
    }
}
