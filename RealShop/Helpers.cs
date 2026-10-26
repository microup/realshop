using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        private static Dictionary<string, int> _configVariants;
        private static Dictionary<string, string[]> _engineOptions;
        private static Dictionary<string, string> _engineNames;
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

        public static int GetConfigVariantCount(string carFile)
        {
            if (string.IsNullOrEmpty(carFile))
            {
                return 0;
            }

            EnsureLoaded();
            if (_carYear == null || _carYear.Count == 0)
            {
                return 0;
            }

            string id = carFile.ToLowerInvariant();
            if (id.StartsWith("car_"))
            {
                id = id.Substring(4);
            }

            if (!_carYear.ContainsKey(id))
            {
                return 0;
            }

            if (_configVariants == null)
            {
                _configVariants = new Dictionary<string, int>();
            }

            int cached;
            if (_configVariants.TryGetValue(id, out cached))
            {
                return cached;
            }

            int count = 0;
            try
            {
                string folder = Path.Combine(Path.Combine(UnityEngine.Application.streamingAssetsPath, "Cars"), id);
                while (File.Exists(Path.Combine(folder, count == 0 ? "config.txt" : ("config" + count + ".txt"))))
                {
                    count++;
                }
            }
            catch
            {
                count = 0;
            }

            _configVariants[id] = count;
            return count;
        }

        public static string[] GetEngineSwapOptions(string carFile, int configIndex)
        {
            if (string.IsNullOrEmpty(carFile))
            {
                return new string[0];
            }

            EnsureLoaded();
            if (_carYear == null || _carYear.Count == 0)
            {
                return new string[0];
            }

            string id = carFile.ToLowerInvariant();
            if (id.StartsWith("car_"))
            {
                id = id.Substring(4);
            }

            if (!_carYear.ContainsKey(id))
            {
                return new string[0];
            }

            if (_engineOptions == null)
            {
                _engineOptions = new Dictionary<string, string[]>();
            }

            string cacheKey = id + "|" + configIndex;
            string[] cached;
            if (_engineOptions.TryGetValue(cacheKey, out cached))
            {
                return cached;
            }

            string folder = Path.Combine(Path.Combine(UnityEngine.Application.streamingAssetsPath, "Cars"), id);
            string[] options = ReadSwapOptions(folder, configIndex);
            _engineOptions[cacheKey] = options;
            return options;
        }

        public static string GetEngineDisplayName(string engineId)
        {
            if (string.IsNullOrEmpty(engineId))
            {
                return string.Empty;
            }

            if (_engineNames == null)
            {
                _engineNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }

            string cacheKey = (BuildJobText.Russian ? "ru|" : "en|") + engineId;
            string cached;
            if (_engineNames.TryGetValue(cacheKey, out cached))
            {
                return cached;
            }

            cached = ReadLocalizedName(engineId);
            _engineNames[cacheKey] = cached;
            return cached;
        }

        private static string[] ReadSwapOptions(string folder, int configIndex)
        {
            List<string> files = new List<string>();
            if (configIndex > 0)
            {
                files.Add(Path.Combine(folder, "config" + configIndex + ".txt"));
            }

            files.Add(Path.Combine(folder, "config.txt"));

            for (int f = 0; f < files.Count; f++)
            {
                try
                {
                    string file = files[f];
                    if (!File.Exists(file))
                    {
                        continue;
                    }

                    foreach (var line in File.ReadAllLines(file))
                    {
                        string s = line.Trim();
                        if (!s.StartsWith("swapoptions="))
                        {
                            continue;
                        }

                        string raw = s.Substring("swapoptions=".Length).Trim();
                        List<string> list = new List<string>();
                        foreach (var token in raw.Split(','))
                        {
                            string name = token.Trim();
                            if (!string.IsNullOrEmpty(name))
                            {
                                list.Add(name);
                            }
                        }

                        if (list.Count > 0)
                        {
                            return list.ToArray();
                        }
                    }
                }
                catch
                {
                }
            }

            return new string[0];
        }

        private static string ReadLocalizedName(string engineId)
        {
            string[] languages = BuildJobText.Russian
                ? new[] { "Russian.txt", "English.txt" }
                : new[] { "English.txt" };
            string key = "!" + engineId + "=";

            for (int i = 0; i < languages.Length; i++)
            {
                try
                {
                    string file = Path.Combine(Path.Combine(UnityEngine.Application.streamingAssetsPath, "Localizations"),
                        languages[i]);
                    if (!File.Exists(file))
                    {
                        continue;
                    }

                    foreach (var line in File.ReadAllLines(file, Encoding.UTF8))
                    {
                        string s = line.TrimStart('\uFEFF').Trim();
                        if (!s.StartsWith(key, StringComparison.Ordinal))
                        {
                            continue;
                        }

                        string value = s.Substring(key.Length).Trim();
                        if (value.Length > 0)
                        {
                            return value;
                        }
                    }
                }
                catch
                {
                }
            }

            return engineId;
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
