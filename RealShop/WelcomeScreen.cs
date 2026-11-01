using MelonLoader;
using System;
using System.Reflection;
using UnityEngine;

#pragma warning disable CS0618

namespace RealShop
{
    internal static class WelcomeScreen
    {
        private const string ProfileKeyPrefix = "RealShop.WelcomeShown.";

        private static readonly string EnTitle = "RealShop v0.3.11";
        private static readonly string RuTitle = "RealShop v0.3.11";

        private static readonly string EnBody =
            "Realism mod for CMS 2021:\n" +
            "cars older than 2005 are built only from\n" +
            "junkyard / auction / barn parts.\n\n" +
            "No part hits 100% \u2014 hunt with a ceiling!\n" +
            "Ordinary ~30\u201350%, good ~50\u201370% (green),\n" +
            "rare 70\u2013100% (purple). Body can be welded.\n\n" +
            "Extremely hard jobs: build a whole car\n" +
            "from a bare chassis.\n\n" +
            "Latest: salon no longer pre-2005 cars,\n" +
            "build-orders arrive stripped & dirty,\n" +
            "starting money fixed, auction rating capped.";

        private static readonly string RuBody =
            "\u041c\u043e\u0434 \u0440\u0435\u0430\u043b\u0438\u0437\u043c\u0430 \u0434\u043b\u044f CMS 2021:\n" +
            "\u043c\u0430\u0448\u0438\u043d\u044b \u0441\u0442\u0430\u0440\u0448\u0435 2005 \u0441\u043e\u0431\u0438\u0440\u0430\u044e\u0442\u0441\u044f \u0442\u043e\u043b\u044c\u043a\u043e \u0438\u0437\n" +
            "\u0434\u0435\u0442\u0430\u043b\u0435\u0439 \u0441\u043e \u0441\u0432\u0430\u043b\u043a\u0438 / \u0430\u0443\u043a\u0446\u0438\u043e\u043d\u0430 / \u0430\u043c\u0431\u0430\u0440\u0430.\n\n" +
            "\u0414\u0435\u0442\u0430\u043b\u044c \u043d\u0435 \u0434\u043e\u0445\u043e\u0434\u0438\u0442 \u0434\u043e 100% \u2014 \u0443 \u043e\u0445\u043e\u0442\u044b \u0435\u0441\u0442\u044c \u043f\u0440\u0435\u0434\u0435\u043b!\n" +
            "\u041e\u0431\u044b\u0447\u043d\u044b\u0435 ~30\u201350%, \u0445\u043e\u0440\u043e\u0448\u0438\u0435 ~50\u201370% (\u0437\u0435\u043b\u0451\u043d\u044b\u0435),\n" +
            "\u0440\u0435\u0434\u043a\u0438\u0435 70\u2013100% (\u0444\u0438\u043e\u043b\u0435\u0442\u043e\u0432\u044b\u0435). \u041a\u0443\u0437\u043e\u0432 \u0432\u0430\u0440\u0438\u0442\u0441\u044f.\n\n" +
            "\u042d\u043a\u0441\u0442\u0440\u0435\u043c\u0430\u043b\u044c\u043d\u044b\u0435 \u0437\u0430\u043a\u0430\u0437\u044b: \u043f\u043e\u043b\u043d\u0430\u044f \u0441\u0431\u043e\u0440\u043a\u0430 \u043c\u0430\u0448\u0438\u043d\u044b\n" +
            "\u0438\u0437 \u0433\u043e\u043b\u043e\u0433\u043e \u043a\u0430\u0440\u043a\u0430\u0441\u0430.\n\n" +
            "\u041f\u0440\u0430\u0432\u043a\u0438: \u0441\u0430\u043b\u043e\u043d \u0431\u0435\u0437 \u043c\u0430\u0448\u0438\u043d \u0434\u043e 2005,\n" +
            "\u0437\u0430\u043a\u0430\u0437\u044b \u043f\u0440\u0438\u0445\u043e\u0434\u044f\u0442 \u0440\u0430\u0437\u043e\u0431\u0440\u0430\u043d\u043d\u044b\u043c\u0438 \u0438 \u0433\u0440\u044f\u0437\u043d\u044b\u043c\u0438,\n" +
            "\u0441\u0442\u0430\u0440\u0442\u043e\u0432\u044b\u0435 \u0434\u0435\u043d\u044c\u0433\u0438 \u0438 \u043b\u0438\u043c\u0438\u0442 \u0430\u0443\u043a\u0446\u0438\u043e\u043d\u0430 \u0438\u0441\u043f\u0440\u0430\u0432\u043b\u0435\u043d\u044b.";

        private static bool _pending;
        private static bool _active;
        private static bool _fallbackShown;
        private static bool _inGame;
        private static string _persistKey;
        private static int _retryFrames;

        private static MethodInfo _selectedProfileMethod;
        private static MethodInfo _profileNameMethod;
        private static GUIStyle _titleStyle;
        private static GUIStyle _bodyStyle;
        private static GUIStyle _closeStyle;

        internal static void OnNewGameDetected()
        {
            _pending = true;
        }

        internal static void OnSceneLoaded(int buildIndex)
        {
            if (buildIndex == 1)
            {
                _inGame = true;
            }
        }

        internal static void Tick()
        {
            if (_active)
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape))
                {
                    Dismiss();
                }

                return;
            }

            if (!_pending || !_inGame || !Allow())
            {
                return;
            }

            string key = ProfileKey();
            if (string.IsNullOrEmpty(key))
            {
                _retryFrames++;
                if (_retryFrames < 2400)
                {
                    return;
                }

                if (_fallbackShown)
                {
                    _pending = false;
                    return;
                }

                _persistKey = string.Empty;
            }
            else
            {
                if (ShouldSuppress(key))
                {
                    _pending = false;
                    MelonLogger.Msg("[Welcome] already shown for this profile; skipped");
                    return;
                }

                _persistKey = key;
            }

            _pending = false;
            _active = true;
            MelonLogger.Msg("[Welcome] new-game welcome overlay shown");
        }

        internal static void OnGui()
        {
            if (!_active || !Allow())
            {
                return;
            }

            try
            {
                bool russian = BuildJobText.Russian;
                string title = russian ? RuTitle : EnTitle;
                string body = russian ? RuBody : EnBody;
                string close = russian ? "\u0417\u0430\u043a\u0440\u044b\u0442\u044c" : "Close";

                Rect full = new Rect(0f, 0f, Screen.width, Screen.height);
                Color prev = GUI.color;
                GUI.color = new Color(0f, 0f, 0f, 0.88f);
                GUI.DrawTexture(full, Texture2D.whiteTexture);
                GUI.color = new Color(1f, 1f, 1f, 1f);

                float boxW = 820f;
                float boxH = 580f;
                Rect box = new Rect((Screen.width - boxW) / 2f, (Screen.height - boxH) / 2f, boxW, boxH);

                if (_titleStyle == null)
                {
                    _titleStyle = new GUIStyle(GUI.skin.label);
                    _titleStyle.fontSize = 34;
                    _titleStyle.fontStyle = FontStyle.Bold;
                    _titleStyle.alignment = TextAnchor.MiddleCenter;

                    _bodyStyle = new GUIStyle(GUI.skin.label);
                    _bodyStyle.fontSize = 21;
                    _bodyStyle.wordWrap = true;
                    _bodyStyle.alignment = TextAnchor.UpperLeft;

                    _closeStyle = new GUIStyle(GUI.skin.button);
                    _closeStyle.fontSize = 22;
                }

                GUI.Label(new Rect(box.x, box.y + 20f, box.width, 56f), title, _titleStyle);
                GUI.Label(new Rect(box.x + 48f, box.y + 104f, box.width - 96f, box.height - 220f), body, _bodyStyle);
                Rect ok = new Rect(box.x + box.width / 2f - 110f, box.y + box.height - 76f, 220f, 52f);
                if (GUI.Button(ok, close, _closeStyle))
                {
                    Dismiss();
                }

                GUI.color = prev;
            }
            catch (Exception e)
            {
                MelonLogger.Warning("[Welcome] draw failed: " + e.Message);
                _active = false;
            }
        }

        private static bool Allow()
        {
            Config config = RealParts.ActiveConfig;
            return config != null && config.ShowWelcomeOnNewGame && RealParts.ModIsEnabled;
        }

        private static string ProfileKey()
        {
            object pm = GarageNeedsRecon.CachedProfileManager;
            if (pm == null)
            {
                return string.Empty;
            }

            try
            {
                if (_selectedProfileMethod == null)
                {
                    _selectedProfileMethod = pm.GetType().GetMethod("GetSelectedProfileName", BindingFlags.Public | BindingFlags.Instance);
                }

                if (_profileNameMethod == null)
                {
                    _profileNameMethod = pm.GetType().GetMethod("GetProfileName", BindingFlags.Public | BindingFlags.Instance);
                }

                MethodInfo method = _selectedProfileMethod != null ? _selectedProfileMethod : _profileNameMethod;
                if (method != null)
                {
                    object value = method.Invoke(pm, null);
                    if (value is string name && !string.IsNullOrEmpty(name))
                    {
                        return ProfileKeyPrefix + Sanitize(name);
                    }
                }
            }
            catch (Exception e)
            {
                MelonLogger.Warning("[Welcome] profile key resolve failed: " + e.Message);
            }

            return string.Empty;
        }

        private static string Sanitize(string name)
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder(name.Length);
            for (int i = 0; i < name.Length; i++)
            {
                char c = name[i];
                if (char.IsLetterOrDigit(c) || c == '_' || c == '-')
                {
                    builder.Append(c);
                }
            }

            return builder.ToString();
        }

        private static bool ShouldSuppress(string key)
        {
            try
            {
                return PlayerPrefs.GetInt(key, 0) == 1;
            }
            catch
            {
                return false;
            }
        }

        private static void Dismiss()
        {
            _active = false;

            if (!string.IsNullOrEmpty(_persistKey))
            {
                try
                {
                    PlayerPrefs.SetInt(_persistKey, 1);
                    PlayerPrefs.Save();
                    MelonLogger.Msg("[Welcome] marked shown: " + _persistKey);
                }
                catch (Exception e)
                {
                    MelonLogger.Warning("[Welcome] persist failed: " + e.Message);
                }
            }
            else
            {
                _fallbackShown = true;
            }
        }
    }
}

#pragma warning restore CS0618
