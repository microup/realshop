using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace RealShop
{
    internal class Helpers
    {
        public static int GetYearFromString(string input)
        {
            string pattern = @"\((\d{4})\)\s*";
            Match match = Regex.Match(input, pattern);

            if (match.Success)
            {
                return int.Parse(match.Groups[1].Value);
            }
            else
            {
                return -1;
            }
        }

        public static bool IsBrandExist(string brand)
        {
            foreach (string name in Config.ListActiveBrands)
            {
                if (brand.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsCarExist(string checkCar)
        {
            foreach (string car in Config.CarNames)
            {
                if (checkCar.Equals(car, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool CheckForSubstrings(string input, string[] targets)
        {
            return targets.Any(target => input.Contains(target));
        }

        public static string ToPrettyString(Item item)
        {
            if (item == null) return "";

            return ($"(Item) ID: {item.ID}, UID: {item.UID}, Condition: {item.Condition}, Dent: {item.Dent}, " +
                    $"Quality: {item.Quality}, IsExamined: {item.IsExamined}, RepairAmount: {item.RepairAmount}, " +
                    $"WashFactor: {item.WashFactor}, OutsideRustEnabled: {item.OutsideRustEnabled}, Livery: {item.Livery}, " +
                    $"LiveryStrength: {item.LiveryStrength}, NormalID: {item.NormalID}, ConditionToShow: {item.ConditionToShow}," +
                    $"LPData: {item.LPData}");
        }

        public static string ToPrettyString(GroupItem groupItem)
        {
            if (groupItem == null) return "null";

            return ($"(GroupItem) ID: {groupItem.ID}, UID: {groupItem.UID}, " +
                            $"Condition: {groupItem.GetCondition()}, ConditionToShow: {groupItem.GetConditionToShow()}, " +
                            $"Size: {groupItem.Size}, IsNormalGroup: {groupItem.IsNormalGroup}");
        }

        public static string ToPrettyString(BaseItem baseItem)
        {
            if (baseItem == null) return "null";
            return ($"(BaseItem) ID: {baseItem.ID}, UID: {baseItem.UID}, " +
                            $"Condition: {baseItem.GetCondition()}, ConditionToShow: {baseItem.GetConditionToShow()}");
        }
    }
}
