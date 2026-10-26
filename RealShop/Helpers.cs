using System;
using System.Collections.Generic;
using System.IO;
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

        public static bool CheckForSubstrings(string input, string[] targets)
        {
            return targets.Any(target => input.Contains(target));
        }
    }

    internal static class CarData
    {
        private static Dictionary<string, int> _carYear;
        private static Dictionary<string, string> _carPlaces;
        private static string[] _foldersByLengthDesc;
        private static bool _loaded;

        public static string[] GetOrderCars(int maxYearExclusive)
        {
            EnsureLoaded();
            if (_carYear == null || _carYear.Count == 0)
            {
                return new string[0];
            }

            var list = new List<string>();
            foreach (var pair in _carYear)
            {
                int year = pair.Value;
                if (maxYearExclusive > 0 && year >= maxYearExclusive)
                {
                    continue;
                }

                string places;
                if (_carPlaces == null || !_carPlaces.TryGetValue(pair.Key, out places))
                {
                    continue;
                }

                if (!PlacesAllowOrder(places))
                {
                    continue;
                }

                list.Add("car_" + pair.Key.ToLowerInvariant());
            }

            list.Sort(StringComparer.Ordinal);
            return list.ToArray();
        }

        private static bool PlacesAllowOrder(string places)
        {
            if (string.IsNullOrEmpty(places))
            {
                return false;
            }

            foreach (var token in places.Split(','))
            {
                string name = token.Trim();
                int dot = name.IndexOf('.');
                if (dot >= 0)
                {
                    name = name.Substring(0, dot);
                }

                if (string.Equals(name, "Order", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool TryGetBodyYear(string partId, out int year)
        {
            year = 0;
            if (string.IsNullOrEmpty(partId) || !partId.StartsWith("car_"))
            {
                return false;
            }

            EnsureLoaded();
            if (_carYear == null || _carYear.Count == 0)
            {
                return false;
            }

            string rest = partId.Substring(4);
            foreach (var folder in _foldersByLengthDesc)
            {
                if (rest == folder || rest.StartsWith(folder + "-"))
                {
                    year = _carYear[folder];
                    return true;
                }
            }

            return false;
        }

        public static bool TryGetYearByCarId(string carId, out int year)
        {
            year = 0;
            if (string.IsNullOrEmpty(carId))
            {
                return false;
            }

            EnsureLoaded();
            if (_carYear == null || _carYear.Count == 0)
            {
                return false;
            }

            string id = carId.ToLowerInvariant();
            if (id.StartsWith("car_"))
            {
                id = id.Substring(4);
            }

            if (_carYear.TryGetValue(id, out year))
            {
                return true;
            }

            foreach (var folder in _foldersByLengthDesc)
            {
                if (id == folder || id.StartsWith(folder + "-"))
                {
                    year = _carYear[folder];
                    return true;
                }
            }

            return false;
        }

        private static void EnsureLoaded()
        {
            if (_loaded)
            {
                return;
            }

            _loaded = true;
            _carYear = new Dictionary<string, int>();
            _carPlaces = new Dictionary<string, string>();

            try
            {
                string carsPath = Path.Combine(UnityEngine.Application.streamingAssetsPath, "Cars");
                if (!Directory.Exists(carsPath))
                {
                    return;
                }

                foreach (var dir in Directory.GetDirectories(carsPath))
                {
                    string folder = Path.GetFileName(dir).ToLowerInvariant();
                    int best = 0;
                    foreach (var name in new[] { "config.txt", "config1.txt" })
                    {
                        string file = Path.Combine(dir, name);
                        if (!File.Exists(file))
                        {
                            continue;
                        }

                        int y = ReadYear(file);
                        if (y > best)
                        {
                            best = y;
                        }
                    }

                    string primary = Path.Combine(dir, "config.txt");
                    string secondary = Path.Combine(dir, "config1.txt");
                    string chosen = primary;
                    if (!File.Exists(primary))
                    {
                        chosen = secondary;
                    }

                    if (chosen != null && File.Exists(chosen))
                    {
                        string places = ReadAllowedPlaces(chosen);
                        if (!string.IsNullOrEmpty(places))
                        {
                            _carPlaces[folder] = places;
                        }
                    }

                    if (best > 0)
                    {
                        _carYear[folder] = best;
                    }
                }
            }
            catch
            {
            }

            var keys = new List<string>(_carYear.Keys);
            keys.Sort((a, b) => b.Length.CompareTo(a.Length));
            _foldersByLengthDesc = keys.ToArray();
        }

        private static int ReadYear(string file)
        {
            foreach (var line in File.ReadAllLines(file))
            {
                string s = line.Trim();
                if (s.StartsWith("year=") && int.TryParse(s.Substring(5), out int y))
                {
                    return y;
                }
            }

            return 0;
        }

        private static string ReadAllowedPlaces(string file)
        {
            foreach (var line in File.ReadAllLines(file))
            {
                string s = line.Trim();
                if (s.StartsWith("allowedPlaces="))
                {
                    return s.Substring("allowedPlaces=".Length).Trim();
                }
            }

            return null;
        }
    }
}
