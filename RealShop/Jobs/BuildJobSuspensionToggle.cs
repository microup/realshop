using MelonLoader;
using System.Text;
using UnhollowerBaseLib;
using UnityEngine;

namespace RealShop
{
    internal static class BuildJobSuspensionToggle
    {
        internal static void Update()
        {
            Config config = RealParts.ActiveConfig;
            KeyCode key = ParseKey(config != null ? config.BuildJobSuspensionToggleKey : null);
            if (!Input.GetKeyDown(key))
            {
                return;
            }

            int jobId = BuildJobCondition.GuardJobId;
            string source = "guard";
            if (jobId < 0)
            {
                jobId = config != null ? config.BuildJobConditionJobId : -1;
                source = "cfg marker";
            }

            CarLoader loader = BuildJobService.FindCarLoaderByOrder(jobId);
            if (loader == null)
            {
                MelonLogger.Warning("[BuildJob] suspension toggle: " + key + " no car for job=" + jobId
                    + " (source=" + source + ")");
                return;
            }

            string raw = config != null ? config.BuildJobStripSuspensionNames : null;
            if (string.IsNullOrWhiteSpace(raw))
            {
                raw = Config.ValueBuildJobStripSuspensionNames;
            }

            string[] names = raw.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
            int changed = 0;
            int matched = 0;
            int active = 0;
            {
                Transform root = loader.GetRootTransform();
                if (root == null)
                {
                    MelonLogger.Warning("[BuildJob] suspension toggle: " + key + " job=" + jobId + " root transform is null");
                    return;
                }

                Il2CppArrayBase<Transform> all = root.GetComponentsInChildren<Transform>(true);
                if (all == null)
                {
                    return;
                }

                for (int i = 0; i < all.Length; i++)
                {
                    Transform node = all[i];
                    string name = node != null ? node.name : null;
                    if (string.IsNullOrEmpty(name))
                    {
                        continue;
                    }

                    for (int n = 0; n < names.Length; n++)
                    {
                        if (!string.Equals(name, names[n].Trim(), System.StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        matched++;
                        if (node.gameObject.activeSelf)
                        {
                            active++;
                            continue;
                        }

                        node.gameObject.SetActive(true);
                        changed++;
                        break;
                    }
                }
            }

            MelonLogger.Msg("[BuildJob] suspension toggle: " + key + " matched=" + matched + " activeBefore=" + active
                + " enabled=" + changed + (matched == 0 ? " (no suspension objects in the scene)"
                    : active == matched ? " (the game keeps them active: the mount mode is blocked by something else)"
                    : " (mount mode should work now)"));
        }

        private static KeyCode ParseKey(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                raw = Config.ValueBuildJobSuspensionToggleKey;
            }

            try
            {
                return (KeyCode)System.Enum.Parse(typeof(KeyCode), raw.Trim(), true);
            }
            catch
            {
                MelonLogger.Warning("[BuildJob] suspension toggle: unknown key \"" + raw + "\", using F8");
                return KeyCode.F8;
            }
        }
    }
}
