using MelonLoader;
using System;
using System.Text;
using UnhollowerBaseLib;

namespace RealShop
{
    internal static class BuildJobInteriorStrip
    {
        private const int DetailCap = 240;

        private static readonly int[] SlotOffsets =
        {
            0x3d0,
            0x3d8,
            0x3e0,
            0x3e8,
            0x3f0
        };

        private static readonly string[] SlotNames =
        {
            "seatLeft",
            "seatRight",
            "steeringWheel",
            "bench",
            "benchFront"
        };

        private static int _appliedJobId = -1;

        private static int _retryLoggedJobId = -1;

        internal static void Apply(CarLoader loader, int jobId, string tag)
        {
            try
            {
                Config config = RealParts.ActiveConfig;
                if (config == null || !RealParts.ModIsEnabled || !config.EnableBuildFromScratchJobs
                    || !config.BuildJobStripInteriorUnmount
                    || !BuildJobState.IsOursJobId(jobId)
                    || string.Equals(config.BuildJobStripMode, "off", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                if (jobId < 0 || _appliedJobId == jobId || config.BuildJobInteriorStrippedJobId == jobId)
                {
                    return;
                }

                if (loader == null || !BuildJobStripTarget.MatchesExact(loader))
                {
                    return;
                }

                string[] slotRes = SlotResourceIds(loader);
                int real = 0;
                int done = 0;
                int carWritten = 0;
                int hidden = 0;
                int missing = 0;
                int disagree = 0;
                StringBuilder detail = new StringBuilder();

                for (int i = 0; i < SlotNames.Length && i < slotRes.Length; i++)
                {
                    string slot = SlotNames[i];
                    string res = slotRes[i];
                    if (!IsRealRes(res))
                    {
                        Append(detail, slot + ":dummy");
                        continue;
                    }

                    CarPart part = null;
                    try
                    {
                        part = loader.GetCarPart(slot);
                    }
                    catch (System.Exception)
                    {
                    }

                    if (part == null)
                    {
                        missing++;
                        Append(detail, slot + ":" + res + ":no-entry");
                        continue;
                    }

                    real++;
                    if (!part.Unmounted)
                    {
                        part.Unmounted = true;
                        carWritten++;
                    }

                    bool carOff = part.Unmounted;
                    if (!carOff)
                    {
                        disagree++;
                    }

                    hidden += HideSlotRenderers(loader, i);

                    if (carOff)
                    {
                        done++;
                    }

                    Append(detail, slot + ":" + res + ":car" + (carOff ? "off" : "on"));
                }

                bool complete = real > 0 && hidden > 0 && missing == 0 && disagree == 0 && done == real;
                MelonLogger.Msg("[BuildJob] interior strip: job=" + jobId
                    + " real=" + real + " done=" + done + " carWritten=" + carWritten
                    + " hidden=" + hidden + " missing=" + missing + " disagree=" + disagree
                    + " persist=" + (complete ? "yes" : "no")
                    + " via=" + tag + " detail=" + detail);

                if (complete)
                {
                    _appliedJobId = jobId;
                    config.RememberInteriorStripped(jobId);
                }
                else if (_retryLoggedJobId != jobId)
                {
                    _retryLoggedJobId = jobId;
                    MelonLogger.Msg("[BuildJob] interior strip retry pending: job=" + jobId
                        + " real=" + real + " done=" + done + " via=" + tag);
                }
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] interior strip: job=" + jobId + " failed: " + e.Message
                    + " via=" + tag);
            }
        }

        internal static void ClearAppliedMarker(int jobId)
        {
            if (_appliedJobId == jobId)
            {
                _appliedJobId = -1;
            }

            if (_retryLoggedJobId == jobId)
            {
                _retryLoggedJobId = -1;
            }

            Config config = RealParts.ActiveConfig;
            if (config == null || config.BuildJobInteriorStrippedJobId != jobId)
            {
                return;
            }

            try
            {
                config.ClearInteriorStripped();
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] interior marker clear failed: job=" + jobId + " " + e.Message);
            }
        }

        internal static bool IsStrippedFor(int jobId)
        {
            if (jobId < 0)
            {
                return false;
            }

            if (_appliedJobId == jobId)
            {
                return true;
            }

            Config config = RealParts.ActiveConfig;
            return config != null && config.BuildJobInteriorStrippedJobId == jobId;
        }

        internal static int CountUnmountedInterior(CarLoader loader)
        {
            if (loader == null)
            {
                return 0;
            }

            try
            {
                Il2CppSystem.Collections.Generic.List<CarPart> parts = loader.carParts;
                if (parts == null)
                {
                    return 0;
                }

                int unmounted = 0;
                for (int i = 0; i < parts.Count; i++)
                {
                    CarPart part = parts[i];
                    if (part == null)
                    {
                        continue;
                    }

                    if (BuildJobMissingInteractiveObjects.IsInteriorName(part.name) && part.Unmounted)
                    {
                        unmounted++;
                    }
                }

                return unmounted;
            }
            catch
            {
                return 0;
            }
        }

        internal static int CountUnmountedFor(CarLoader loader, int jobId)
        {
            if (!IsStrippedFor(jobId))
            {
                return 0;
            }

            return CountUnmountedInterior(loader);
        }

        private static string[] SlotResourceIds(CarLoader loader)
        {
            string[] res = { null, null, null, null, null };
            try
            {
                InteriorParams slots = loader.InteriorParams;
                if (slots != null)
                {
                    res[0] = SlotValue(ReadSlot(() => slots.SeatLeft));
                    res[1] = SlotValue(ReadSlot(() => slots.SeatRight));
                    res[2] = SlotValue(ReadSlot(() => slots.Wheel));
                    res[3] = SlotValue(ReadSlot(() => slots.Bench));
                    res[4] = SlotValue(ReadSlot(() => slots.Bench_front));
                }
            }
            catch (System.Exception)
            {
            }

            return res;
        }

        private static string ReadSlot(Func<string> getter)
        {
            try
            {
                return getter();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        private static string SlotValue(string value)
        {
            return string.IsNullOrEmpty(value) ? string.Empty : value.Trim();
        }

        private static bool IsRealRes(string res)
        {
            if (string.IsNullOrEmpty(res))
            {
                return false;
            }

            string t = res.Trim();
            if (t.Length == 0 || t[0] == '#')
            {
                return false;
            }

            return !t.StartsWith("Dummy", StringComparison.OrdinalIgnoreCase)
                && !t.StartsWith("None", StringComparison.OrdinalIgnoreCase);
        }

        private static IntPtr ReadSlotPointer(CarLoader loader, int i)
        {
            try
            {
                if (loader == null || loader.Pointer == IntPtr.Zero || i < 0 || i >= SlotOffsets.Length)
                {
                    return IntPtr.Zero;
                }

                return System.Runtime.InteropServices.Marshal.ReadIntPtr(loader.Pointer, SlotOffsets[i]);
            }
            catch (System.Exception)
            {
                return IntPtr.Zero;
            }
        }

        private static int HideSlotRenderers(CarLoader loader, int i)
        {
            IntPtr goPtr = ReadSlotPointer(loader, i);
            if (goPtr == IntPtr.Zero)
            {
                return 0;
            }

            try
            {
                UnityEngine.GameObject go = new UnityEngine.GameObject(goPtr);
                return HideRenderers(go);
            }
            catch (System.Exception)
            {
                return 0;
            }
        }

        private static int HideRenderers(UnityEngine.GameObject root)
        {
            if (root == null)
            {
                return 0;
            }

            int hidden = 0;
            try
            {
                Il2CppArrayBase<UnityEngine.Renderer> renderers = root.GetComponentsInChildren<UnityEngine.Renderer>(true);
                if (renderers == null)
                {
                    return 0;
                }

                for (int i = 0; i < renderers.Length; i++)
                {
                    UnityEngine.Renderer renderer = renderers[i];
                    if (renderer == null || !renderer.enabled)
                    {
                        continue;
                    }

                    renderer.enabled = false;
                    hidden++;
                }
            }
            catch (System.Exception)
            {
            }

            return hidden;
        }

        private static void Append(StringBuilder detail, string text)
        {
            if (string.IsNullOrEmpty(text) || detail.Length >= DetailCap)
            {
                return;
            }

            if (detail.Length > 0)
            {
                detail.Append('|');
            }

            detail.Append(text);
        }
    }
}
