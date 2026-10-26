using MelonLoader;
using System;
using System.Text;
using UnhollowerBaseLib;
using UnityEngine;

namespace RealShop
{
    internal static partial class BuildJobCondition
    {
        internal static int UnmountByPrefix(CarLoader loader, string tag)
        {
            int unmounted = 0;
            try
            {
                if (!GuardApplies(loader))
                {
                    return 0;
                }

                Config config = RealParts.ActiveConfig;
                string raw = config != null
                    ? config.BuildJobStripUnmountPrefixes
                    : Config.ValueBuildJobStripUnmountPrefixes;
                if (string.IsNullOrEmpty(raw) || raw.Trim().Length == 0)
                {
                    return 0;
                }

                Il2CppSystem.Collections.Generic.List<CarPart> parts = loader.carParts;
                if (parts == null)
                {
                    return 0;
                }

                string[] prefixes = raw.Split(',');
                LogPartNames(loader, tag);
                for (int i = 0; i < parts.Count; i++)
                {
                    try
                    {
                        CarPart part = parts[i];
                        if (part == null || part.Unmounted)
                        {
                            continue;
                        }

                        string name = part.name;
                        if (string.IsNullOrEmpty(name))
                        {
                            continue;
                        }

                        string lower = name.ToLowerInvariant();
                        for (int p = 0; p < prefixes.Length; p++)
                        {
                            string prefix = prefixes[p].Trim().ToLowerInvariant();
                            if (prefix.Length == 0 || !lower.StartsWith(prefix, StringComparison.Ordinal))
                            {
                                continue;
                            }

                            part.Unmounted = true;
                            unmounted++;
                            break;
                        }
                    }
                    catch
                    {
                    }
                }

                return unmounted;
            }
            catch (System.Exception e)
            {
                LogOverrideFailure(tag, e);
                return unmounted;
            }
        }

        internal static int HideWheels(CarLoader loader, string tag)
        {
            int hidden = 0;
            try
            {
                if (!GuardApplies(loader))
                {
                    return 0;
                }

                Config config = RealParts.ActiveConfig;
                if (config != null && !config.BuildJobStripHideWheels)
                {
                    return 0;
                }

                for (int i = 0; i < 4; i++)
                {
                    try
                    {
                        GameObject handle = i == 0
                            ? loader.GetWheelFLHandle()
                            : (i == 1
                                ? loader.GetWheelFRHandle()
                                : (i == 2 ? loader.GetWheelRLHandle() : loader.GetWheelRRHandle()));
                        if (handle == null)
                        {
                            continue;
                        }

                        hidden += HideRenderers(handle);
                    }
                    catch
                    {
                    }
                }

                return hidden;
            }
            catch (System.Exception e)
            {
                LogOverrideFailure(tag, e);
                return hidden;
            }
        }

        private static int HideRenderers(GameObject handle)
        {
            int hidden = 0;
            Il2CppArrayBase<MeshRenderer> renderers = handle.GetComponentsInChildren<MeshRenderer>(true);
            if (renderers == null)
            {
                return 0;
            }

            for (int i = 0; i < renderers.Length; i++)
            {
                MeshRenderer renderer = renderers[i];
                if (renderer == null || !renderer.enabled)
                {
                    continue;
                }

                renderer.enabled = false;
                hidden++;
            }

            return hidden;
        }

        internal static int StripSuspension(CarLoader loader, string tag)
        {
            try
            {
                if (!GuardApplies(loader))
                {
                    return -1;
                }

                Config config = RealParts.ActiveConfig;
                int count = config != null
                    ? config.BuildJobStripSuspensionParts
                    : Config.ValueBuildJobStripSuspensionParts;
                if (count <= 0)
                {
                    return 0;
                }

                loader.SetRandomMissingSuspension(0, 100, count, count);
                return count;
            }
            catch (System.Exception e)
            {
                LogOverrideFailure(tag, e);
                return -1;
            }
        }
        internal static int HideSuspensionObjects(CarLoader loader, string tag)
        {
            int hidden = 0;
            try
            {
                if (!GuardApplies(loader))
                {
                    return 0;
                }

                Config config = RealParts.ActiveConfig;
                if (config != null && !config.BuildJobStripSuspensionObjects)
                {
                    return 0;
                }

                Transform root = loader.GetRootTransform();
                if (root == null)
                {
                    return 0;
                }

                string raw = config != null ? config.BuildJobStripSuspensionNames : null;
                if (string.IsNullOrWhiteSpace(raw))
                {
                    raw = Config.ValueBuildJobStripSuspensionNames;
                }

                string[] names = raw.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                Il2CppArrayBase<Transform> all = root.GetComponentsInChildren<Transform>(true);
                if (all == null)
                {
                    return 0;
                }

                for (int i = 0; i < all.Length; i++)
                {
                    Transform node = all[i];
                    string name = node != null ? node.name : null;
                    if (string.IsNullOrEmpty(name) || !node.gameObject.activeSelf)
                    {
                        continue;
                    }

                    for (int n = 0; n < names.Length; n++)
                    {
                        if (string.Equals(name, names[n].Trim(), System.StringComparison.OrdinalIgnoreCase))
                        {
                            node.gameObject.SetActive(false);
                            hidden++;
                            break;
                        }
                    }
                }

                return hidden;
            }
            catch (System.Exception e)
            {
                LogOverrideFailure(tag, e);
                return hidden;
            }
        }

        internal static string WheelState(CarLoader loader)
        {
            try
            {
                return "missing=" + loader.GetMissingWheelTypes() + " mounted=" + loader.CheckCarHaveMountedWheel();
            }
            catch (System.Exception e)
            {
                return "missing=? (" + e.Message + ")";
            }
        }

        private static int _namesLoggedJobId = -1;

        private static void LogPartNames(CarLoader loader, string tag)
        {
            int jobId = loader.orderConnection;
            if (jobId == _namesLoggedJobId)
            {
                return;
            }

            _namesLoggedJobId = jobId;
            Il2CppSystem.Collections.Generic.List<CarPart> parts = loader.carParts;
            if (parts == null)
            {
                return;
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < parts.Count && sb.Length < 400; i++)
            {
                CarPart part = parts[i];
                string name = part != null ? part.name : null;
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                if (sb.Length > 0)
                {
                    sb.Append('|');
                }

                sb.Append(name);
            }

            MelonLogger.Msg("[BuildJob] part names: job=" + jobId + " count=" + parts.Count
                + " via=" + tag + " names=" + sb);
            LogSuspensionNames(jobId, loader);
            LogFrameOwner(jobId, loader);
        }

        private static void LogFrameOwner(int jobId, CarLoader loader)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                Il2CppSystem.Collections.Generic.List<CarPart> parts = loader.carParts;
                if (parts != null)
                {
                    for (int i = 0; i < parts.Count; i++)
                    {
                        CarPart part = parts[i];
                        string name = part != null ? part.name : null;
                        if (!string.Equals(name, "body", StringComparison.OrdinalIgnoreCase)
                            && !string.Equals(name, "details", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        sb.Append(name).Append('=')
                          .Append(Mathf.RoundToInt(part.Condition * 100f)).Append('%')
                          .Append("/struct").Append(Mathf.RoundToInt(part.StructureCondition * 100f)).Append("% ");
                    }
                }

                StringBuilder types = new StringBuilder();
                Transform root = loader.GetRootTransform();
                Il2CppArrayBase<Component> components = root != null ? root.GetComponents<Component>() : null;
                if (components != null)
                {
                    for (int i = 0; i < components.Length && types.Length < 220; i++)
                    {
                        Component component = components[i];
                        if (component == null)
                        {
                            continue;
                        }

                        if (types.Length > 0)
                        {
                            types.Append('|');
                        }

                        types.Append(component.GetType().Name);
                    }
                }

                MelonLogger.Msg("[BuildJob] frame owner: job=" + jobId + " " + sb + " rootComponents=" + types);
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] frame owner failed: job=" + jobId + " " + e.Message);
            }
        }

        private static void LogSuspensionNames(int jobId, CarLoader loader)
        {
            try
            {
                Transform root = loader.GetRootTransform();
                if (root == null)
                {
                    return;
                }

                Il2CppArrayBase<Transform> all = root.GetComponentsInChildren<Transform>(true);
                if (all == null)
                {
                    return;
                }

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < all.Length && sb.Length < 300; i++)
                {
                    Transform node = all[i];
                    string name = node != null ? node.name : null;
                    if (string.IsNullOrEmpty(name))
                    {
                        continue;
                    }

                    string lower = name.ToLowerInvariant();
                    if (!lower.Contains("susp") && !lower.Contains("shock") && !lower.Contains("spring")
                        && !lower.Contains("arm") && !lower.Contains("brake") && !lower.Contains("wheel"))
                    {
                        continue;
                    }

                    if (sb.Length > 0)
                    {
                        sb.Append('|');
                    }

                    sb.Append(name);
                }

                MelonLogger.Msg("[BuildJob] suspension objects: job=" + jobId + " transforms=" + all.Length
                    + " names=" + sb);
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] suspension objects failed: job=" + jobId + " " + e.Message);
            }
        }

        internal static bool StripWritesRequested()
        {
            Config config = RealParts.ActiveConfig;
            if (config == null)
            {
                return true;
            }

            string raw = config.BuildJobStripUnmountPrefixes;
            return (!string.IsNullOrEmpty(raw) && raw.Trim().Length > 0) || config.BuildJobStripHideWheels;
        }
    }
}
