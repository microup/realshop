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
    }
}
