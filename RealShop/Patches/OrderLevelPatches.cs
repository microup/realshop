using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using CMS.UI.Logic;
using Harmony;
using MelonLoader;
using UnhollowerBaseLib;

namespace RealShop
{
    [HarmonyPatch(typeof(OrderGenerator), "GenerateNewJob")]
    internal static class OrderLevelScopePatch
    {
        private const int SpoofRawLevel = 11;

        private const int ClassStaticFieldsOffset = 0xb8;

        private const int LevelStaticFieldOffset = 0x2c8;

        private const int OwnerDataFieldOffset = 0xb8;

        private const int DirectLevelFieldOffset = 0x2c8;

        private const int ProfileDataFieldOffset = 0xe8;

        private const int ProfileDataLevelOffset = 0x14;

        private const int Il2CppClassNameOffset = 0x10;

        private const int LevelKlassSlotRva = 0x2807f98;

        private const int MaxResolveAttempts = 100;

        private const int MaxReasonableRawLevel = 500;

        private struct SavedLevel
        {
            internal readonly IntPtr Ptr;

            internal readonly int Original;

            internal SavedLevel(IntPtr ptr, int original)
            {
                Ptr = ptr;
                Original = original;
            }
        }

        private struct Candidate
        {
            internal readonly IntPtr Ptr;

            internal readonly string Source;

            internal readonly object Keeper;

            internal Candidate(IntPtr ptr, string source, object keeper)
            {
                Ptr = ptr;
                Source = source;
                Keeper = keeper;
            }
        }

        private static readonly Stack<SavedLevel> _saved = new Stack<SavedLevel>();

        private static object _levelKeeper;

        private static IntPtr _levelPtr = IntPtr.Zero;

        private static bool _activeLogged;

        private static bool _resolveFailed;

        private static bool _restoreWarned;

        private static int _resolveAttempts;

        [DllImport("kernel32", SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private static void Prefix()
        {
            Config config = RealParts.ActiveConfig;
            if (config == null || !config.OrderIgnoreJobLevel)
            {
                return;
            }

            if (!_activeLogged)
            {
                _activeLogged = true;
                MelonLogger.Msg("[Orders] hook active: GenerateNewJob level-scope="
                    + config.OrderIgnoreJobLevel + " spoof=" + SpoofRawLevel);
            }

            try
            {
                if (_resolveFailed)
                {
                    return;
                }

                if (_levelPtr == IntPtr.Zero)
                {
                    _resolveAttempts++;
                    if (_resolveAttempts > MaxResolveAttempts)
                    {
                        _resolveFailed = true;
                        MelonLogger.Warning("[Orders] job level field never resolved; level scope disabled");
                        return;
                    }

                    if (!TryResolveLevelPtr())
                    {
                        return;
                    }
                }

                int original = Marshal.ReadInt32(_levelPtr);
                _saved.Push(new SavedLevel(_levelPtr, original));
                Marshal.WriteInt32(_levelPtr, SpoofRawLevel);
            }
            catch (Exception e)
            {
                _resolveFailed = true;
                MelonLogger.Warning("[Orders] job level scope failed: " + e.Message);
            }
        }

        private static void Postfix()
        {
            if (_saved.Count == 0)
            {
                return;
            }

            try
            {
                SavedLevel saved = _saved.Pop();
                int current = Marshal.ReadInt32(saved.Ptr);
                if (current == SpoofRawLevel)
                {
                    Marshal.WriteInt32(saved.Ptr, saved.Original);
                    int restored;
                    try
                    {
                        restored = GlobalData.RealPlayerLevel;
                    }
                    catch
                    {
                        restored = -1;
                    }

                    if (restored != saved.Original + 1 && !_restoreWarned)
                    {
                        _restoreWarned = true;
                        MelonLogger.Warning("[Orders] job level restore mismatch: RealPlayerLevel="
                            + restored + " expected=" + (saved.Original + 1));
                    }
                }
                else if (current != saved.Original && !_restoreWarned)
                {
                    _restoreWarned = true;
                    MelonLogger.Warning("[Orders] job level changed inside generation ("
                        + current + " != spoof " + SpoofRawLevel + "); restore skipped");
                }
            }
            catch (Exception e)
            {
                if (!_restoreWarned)
                {
                    _restoreWarned = true;
                    MelonLogger.Warning("[Orders] job level restore failed: " + e.Message);
                }
            }
        }

        private static bool TryResolveLevelPtr()
        {
            int expectedRaw;
            try
            {
                expectedRaw = GlobalData.RealPlayerLevel - 1;
            }
            catch
            {
                return false;
            }

            if (expectedRaw < 0 || expectedRaw > MaxReasonableRawLevel)
            {
                return false;
            }

            List<Candidate> candidates = new List<Candidate>();
            AddKlassSlotCandidate(candidates, LevelKlassSlotRva, "GenerateNewJob klass slot");
            AddOwnerCandidates(candidates);
            AddProfileDataCandidates(candidates);

            for (int i = 0; i < candidates.Count; i++)
            {
                if (!ProbeCandidate(candidates[i].Ptr, expectedRaw))
                {
                    continue;
                }

                _levelPtr = candidates[i].Ptr;
                _levelKeeper = candidates[i].Keeper;
                MelonLogger.Msg("[Orders] job level field resolved via " + candidates[i].Source
                    + " (level=" + expectedRaw + ", spoof=" + SpoofRawLevel + ", flip=confirmed)");
                return true;
            }

            return false;
        }

        private static bool ProbeCandidate(IntPtr ptr, int expectedRaw)
        {
            if (ptr == IntPtr.Zero)
            {
                return false;
            }

            try
            {
                if (Marshal.ReadInt32(ptr) != expectedRaw)
                {
                    return false;
                }

                int original = expectedRaw;
                Marshal.WriteInt32(ptr, SpoofRawLevel);
                int flipped;
                try
                {
                    flipped = GlobalData.RealPlayerLevel;
                }
                catch
                {
                    Marshal.WriteInt32(ptr, original);
                    return false;
                }

                Marshal.WriteInt32(ptr, original);
                return flipped == SpoofRawLevel + 1;
            }
            catch
            {
                return false;
            }
        }

        private static void AddKlassSlotCandidate(List<Candidate> candidates, int slotRva, string source)
        {
            try
            {
                IntPtr module = GetModuleHandle("GameAssembly.dll");
                if (module == IntPtr.Zero)
                {
                    return;
                }

                IntPtr klass = Marshal.ReadIntPtr(IntPtr.Add(module, slotRva));
                if (klass == IntPtr.Zero)
                {
                    return;
                }

                IntPtr staticFields = Marshal.ReadIntPtr(klass, ClassStaticFieldsOffset);
                if (staticFields == IntPtr.Zero)
                {
                    return;
                }

                IntPtr levelPtr = IntPtr.Add(staticFields, LevelStaticFieldOffset);
                int value = Marshal.ReadInt32(levelPtr);
                if (value < 0 || value > MaxReasonableRawLevel)
                {
                    return;
                }

                string className = null;
                try
                {
                    IntPtr namePtr = Marshal.ReadIntPtr(klass, Il2CppClassNameOffset);
                    if (namePtr != IntPtr.Zero)
                    {
                        className = Marshal.PtrToStringAnsi(namePtr);
                    }
                }
                catch
                {
                }

                candidates.Add(new Candidate(levelPtr, source + " (" + className + ")", null));
            }
            catch
            {
            }
        }

        private static void AddOwnerCandidates(List<Candidate> candidates)
        {
            AddOwnerCandidate(candidates, GarageNeedsRecon.CachedProfileManager, "ProfileManager+0xb8");
            AddOwnerCandidate(candidates, GetGameDataManagerInstance(), "GameDataManager+0xb8");
        }

        private static void AddOwnerCandidate(List<Candidate> candidates, object owner, string source)
        {
            try
            {
                IntPtr ownerPtr = ObjectPtr(owner);
                if (ownerPtr == IntPtr.Zero)
                {
                    return;
                }

                IntPtr dataPtr = Marshal.ReadIntPtr(ownerPtr, OwnerDataFieldOffset);
                if (dataPtr == IntPtr.Zero)
                {
                    return;
                }

                int value = Marshal.ReadInt32(dataPtr, DirectLevelFieldOffset);
                if (value < 0 || value > MaxReasonableRawLevel)
                {
                    return;
                }

                candidates.Add(new Candidate(IntPtr.Add(dataPtr, DirectLevelFieldOffset), source, owner));
            }
            catch
            {
            }
        }

        private static void AddProfileDataCandidates(List<Candidate> candidates)
        {
            try
            {
                object pd = GetSelectedProfileData();
                IntPtr pdPtr = ObjectPtr(pd);
                if (pdPtr == IntPtr.Zero)
                {
                    return;
                }

                int direct = Marshal.ReadInt32(pdPtr, DirectLevelFieldOffset);
                if (direct >= 0 && direct <= MaxReasonableRawLevel)
                {
                    candidates.Add(new Candidate(IntPtr.Add(pdPtr, DirectLevelFieldOffset),
                        "ProfileData+0x2c8", pd));
                }

                IntPtr inner = Marshal.ReadIntPtr(pdPtr, ProfileDataFieldOffset);
                if (inner != IntPtr.Zero)
                {
                    int innerLevel = Marshal.ReadInt32(inner, ProfileDataLevelOffset);
                    if (innerLevel >= 0 && innerLevel <= MaxReasonableRawLevel)
                    {
                        candidates.Add(new Candidate(IntPtr.Add(inner, ProfileDataLevelOffset),
                            "ProfileData+0xe8+0x14", pd));
                    }
                }
            }
            catch
            {
            }
        }

        private static object GetGameDataManagerInstance()
        {
            try
            {
                Type gdm = HarmonyLib.AccessTools.TypeByName("GameDataManager");
                if (gdm == null)
                {
                    return null;
                }

                PropertyInfo pi = gdm.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
                if (pi != null)
                {
                    return pi.GetValue(null, null);
                }

                FieldInfo fi = gdm.GetField("m_instance",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                if (fi != null)
                {
                    return fi.GetValue(null);
                }

                fi = gdm.GetField("instance",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                return fi != null ? fi.GetValue(null) : null;
            }
            catch
            {
                return null;
            }
        }

        private static object GetSelectedProfileData()
        {
            try
            {
                object pmi = GarageNeedsRecon.CachedProfileManager;
                if (pmi == null)
                {
                    return null;
                }

                Type pm = HarmonyLib.AccessTools.TypeByName("ProfileManager");
                if (pm == null)
                {
                    return null;
                }

                MethodInfo mi = pm.GetMethod("GetSelectedProfileData");
                return mi != null ? mi.Invoke(pmi, null) : null;
            }
            catch
            {
                return null;
            }
        }

        private static IntPtr ObjectPtr(object value)
        {
            Il2CppObjectBase baseObj = value as Il2CppObjectBase;
            return baseObj != null ? baseObj.Pointer : IntPtr.Zero;
        }
    }
}
