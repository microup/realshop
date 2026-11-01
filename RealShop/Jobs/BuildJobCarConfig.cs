using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;

namespace RealShop
{
    internal static class BuildJobCarConfig
    {
        private const string InteriorSection = "[5_interior]";

        private static readonly string[] InteriorSlotKeys = new string[]
        {
            "seatLeft",
            "seatRight",
            "wheel",
            "bench",
            "bench_front"
        };

        private static readonly Dictionary<string, int[]> _scores = new Dictionary<string, int[]>();

        internal static int ChooseConfigVersion(string carFile, int variants)
        {
            if (variants <= 1)
            {
                return 0;
            }

            int[] scores = Scores(carFile, variants);
            int limit = variants < scores.Length ? variants : scores.Length;
            int best = 0;
            for (int i = 1; i < limit; i++)
            {
                if (IsBetter(carFile, scores, i, best))
                {
                    best = i;
                }
            }

            return best < 0 || best >= variants ? 0 : best;
        }

        internal static int InteriorScore(string carFile, int configIndex)
        {
            if (configIndex < 0)
            {
                return -1;
            }

            int[] scores = Scores(carFile, configIndex + 1);
            return configIndex < scores.Length ? scores[configIndex] : -1;
        }

        private static int[] Scores(string carFile, int variants)
        {
            string id = NormalizeCarId(carFile);
            if (string.IsNullOrEmpty(id) || variants <= 0)
            {
                return new int[0];
            }

            int[] cached;
            if (_scores.TryGetValue(id, out cached) && cached.Length == variants)
            {
                return cached;
            }

            int[] scores = new int[variants];
            for (int i = 0; i < variants; i++)
            {
                scores[i] = ReadInteriorScore(id, i);
            }

            _scores[id] = scores;
            return scores;
        }

        private static bool IsBetter(string carFile, int[] scores, int candidate, int current)
        {
            if (scores[candidate] != scores[current])
            {
                return scores[candidate] > scores[current];
            }

            return CarData.GetEngineSwapOptions(carFile, candidate).Length
                > CarData.GetEngineSwapOptions(carFile, current).Length;
        }

        private static int ReadInteriorScore(string id, int configIndex)
        {
            string path = ConfigPath(id, configIndex);
            if (path == null)
            {
                return -1;
            }

            int score = 0;
            bool inSection = false;
            bool[] counted = new bool[InteriorSlotKeys.Length];

            try
            {
                foreach (string line in File.ReadLines(path))
                {
                    string text = line.Trim();
                    if (text.StartsWith("["))
                    {
                        if (inSection)
                        {
                            break;
                        }

                        inSection = text == InteriorSection;
                        continue;
                    }

                    if (!inSection)
                    {
                        continue;
                    }

                    int split = text.IndexOf('=');
                    if (split <= 0)
                    {
                        continue;
                    }

                    string key = text.Substring(0, split).Trim();
                    string value = text.Substring(split + 1).Trim();
                    for (int i = 0; i < InteriorSlotKeys.Length; i++)
                    {
                        if (counted[i] || !string.Equals(InteriorSlotKeys[i], key, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        counted[i] = true;
                        if (value.Length > 0 && value[0] != '#')
                        {
                            score++;
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] car config: car=" + id + " config=" + configIndex
                    + " interior slots unreadable " + e.Message);
                return -1;
            }

            return score;
        }

        private static string ConfigPath(string id, int configIndex)
        {
            try
            {
                string folder = Path.Combine(Path.Combine(UnityEngine.Application.streamingAssetsPath, "Cars"), id);
                string file = configIndex == 0 ? "config.txt" : ("config" + configIndex + ".txt");
                string path = Path.Combine(folder, file);
                return File.Exists(path) ? path : null;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        private static string NormalizeCarId(string carFile)
        {
            if (string.IsNullOrEmpty(carFile))
            {
                return string.Empty;
            }

            string id = carFile.ToLowerInvariant();
            return id.StartsWith("car_") ? id.Substring(4) : id;
        }
    }
}
