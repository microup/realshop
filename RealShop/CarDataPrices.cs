using System;
using System.Collections.Generic;
using System.IO;

namespace RealShop
{
    internal static partial class CarData
    {
        private static Dictionary<string, int> _carValue;
        private static readonly HashSet<string> _carValueMiss = new HashSet<string>();

        public static bool TryGetCarValue(string carFile, out int value)
        {
            value = 0;
            if (string.IsNullOrEmpty(carFile))
            {
                return false;
            }

            EnsureLoaded();
            if (_carYear == null || _carYear.Count == 0)
            {
                return false;
            }

            string id = carFile.ToLowerInvariant();
            if (id.StartsWith("car_"))
            {
                id = id.Substring(4);
            }

            if (!_carYear.ContainsKey(id))
            {
                return false;
            }

            if (_carValue == null)
            {
                _carValue = new Dictionary<string, int>();
            }

            int cached;
            if (_carValue.TryGetValue(id, out cached) || _carValueMiss.Contains(id))
            {
                value = cached;
                return value > 0;
            }

            try
            {
                string folder = Path.Combine(Path.Combine(UnityEngine.Application.streamingAssetsPath, "Cars"), id);
                string path = Path.Combine(folder, "config.txt");
                if (!File.Exists(path))
                {
                    _carValueMiss.Add(id);
                    return false;
                }

                foreach (string raw in File.ReadAllLines(path))
                {
                    string line = raw.Trim();
                    int eq = line.IndexOf('=');
                    if (eq <= 0)
                    {
                        continue;
                    }

                    string key = line.Substring(0, eq).Trim();
                    if (!string.Equals(key, "carValue", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    string valStr = line.Substring(eq + 1).Trim();
                    int parsed;
                    if (!int.TryParse(valStr, out parsed))
                    {
                        _carValueMiss.Add(id);
                        return false;
                    }

                    _carValue[id] = parsed;
                    return parsed > 0;
                }
            }
            catch
            {
                value = 0;
            }

            _carValueMiss.Add(id);
            return false;
        }
    }
}
