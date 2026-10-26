using MelonLoader;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RealShop
{
    internal static class BuildJobText
    {
        private const int Unknown = 0;
        private const int RussianCode = 1;
        private const int EnglishCode = 2;

        private static int _resolved = Unknown;

        private static readonly Dictionary<string, string> English = new Dictionary<string, string>
        {
            { "difficulty", "EXTREMELY DIFFICULT" },
            { "body", "BODY" },
            { "parts", "PARTS" },
            { "frame", "FRAME" },
            { "interior", "INTERIOR" },
            { "reward", "finished car price" },
            { "advance", "ADVANCE" },
            { "advanceSuffix", "on accept; deducted from the final payout" },
            { "advancePopup", "Advance for the order" },
            { "takePopup", "accepted: build the car from scratch. Body, frame, parts and interior must reach the minimum quality." },
            { "notReadyPrefix", "the car does not meet the order requirements" },
            { "modOff", "the mod is disabled" },
            { "jobMissing", "order not found" },
            { "carMissing", "the order car is not in the garage" },
            { "unmounted", "unmounted parts" },
            { "mounted", "mounted parts" },
        };

        internal static bool Russian
        {
            get
            {
                if (_resolved == Unknown)
                {
                    Resolve();
                }

                return _resolved == RussianCode;
            }
        }

        internal static string Pick(string key, string russianValue)
        {
            if (Russian)
            {
                return russianValue;
            }

            string english;
            if (English.TryGetValue(key, out english))
            {
                return english;
            }

            return russianValue;
        }

        private static void Resolve()
        {
            string mode = "auto";
            Config config = RealParts.ActiveConfig;
            if (config != null && !string.IsNullOrEmpty(config.BuildJobTextLanguage))
            {
                mode = config.BuildJobTextLanguage.Trim().ToLowerInvariant();
            }

            if (mode == "ru" || mode == "russian")
            {
                _resolved = RussianCode;
                MelonLogger.Msg("[BuildJob] text language: russian (BuildJobTextLanguage = " + mode + ")");
                return;
            }

            if (mode == "en" || mode == "english")
            {
                _resolved = EnglishCode;
                MelonLogger.Msg("[BuildJob] text language: english (BuildJobTextLanguage = " + mode + ")");
                return;
            }

            string detected = ReadPlayerPrefLanguage();
            string source = "PlayerPrefs[Language]";
            if (string.IsNullOrEmpty(detected))
            {
                detected = ReadSystemLanguage();
                source = "Application.systemLanguage";
            }

            _resolved = LooksRussian(detected) ? RussianCode : EnglishCode;
            MelonLogger.Msg("[BuildJob] text language: " + (_resolved == RussianCode ? "russian" : "english")
                + " (" + source + "=" + detected + ")");
        }

        private static string ReadPlayerPrefLanguage()
        {
            string[] keys = { "Language", "language", "SelectedLanguage", "CurrentLanguage" };
            for (int i = 0; i < keys.Length; i++)
            {
                try
                {
                    string value = PlayerPrefs.GetString(keys[i], string.Empty);
                    if (!string.IsNullOrEmpty(value))
                    {
                        return value;
                    }
                }
                catch
                {
                }
            }

            return string.Empty;
        }

        private static string ReadSystemLanguage()
        {
            try
            {
                return Application.systemLanguage.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        private static bool LooksRussian(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            string v = value.Trim().ToLowerInvariant();
            return v == "ru" || v.StartsWith("ru-") || v.StartsWith("ru_")
                || v.Contains("russian") || v.Contains("русск");
        }
    }
}
