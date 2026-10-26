using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace RealShop
{
    internal static partial class GarageNeedsRecon
    {
        private static void DumpSavedCars()
        {
            try
            {
                object pd = GetProfileData(true);
                if (pd == null)
                {
                    MelonLogger.Msg("[Recon] ProfileData null (both accessors failed)");
                    return;
                }

                DumpSavedArray(pd, "carsInGarage");
                DumpSavedArray(pd, "carsOnParking");
            }
            catch (Exception e)
            {
                MelonLogger.Warning("[Recon] saved dump err: " + e.Message);
            }
        }

        private static void DumpSavedArray(object pd, string field)
        {
            object cars = Get(pd.GetType(), pd, field);
            var arr = cars as IEnumerable;
            if (arr == null)
            {
                MelonLogger.Msg("[Recon] " + field + " null");
                return;
            }

            int c = 0;
            foreach (var car in arr)
            {
                if (car == null)
                {
                    continue;
                }
                c++;
                Type ct = car.GetType();
                string ctl = AsStr(Get(ct, car, "carToLoad"));
                string uid = AsStr(Get(ct, car, "UId"));
                MelonLogger.Msg("[Recon] SAVED[" + field + "] car[" + c + "] carToLoad=" + ctl + " UId=" + uid);
                DumpPartList(Get(ct, car, "PartData"), "Path");
                DumpPartList(Get(ct, car, "BodyPartsData"), "Id");
                if (!string.IsNullOrEmpty(ctl) && field == "carsInGarage")
                {
                    DumpAllProps(car);
                }
            }
            MelonLogger.Msg("[Recon] saved " + field + "=" + c);
        }

        private static void DumpPartList(object listObj, string idField)
        {
            if (listObj == null)
            {
                MelonLogger.Msg("[Recon]   " + idField + "-list=null");
                return;
            }
            var items = Enumerate(listObj);
            int n = 0;
            int un = 0;
            int shown = 0;
            foreach (var p in items)
            {
                if (p == null)
                {
                    continue;
                }
                n++;
                Type pt = p.GetType();
                string id = AsStr(Get(pt, p, idField));
                string cond = AsStr(Get(pt, p, "Condition"));
                string unb = AsStr(Get(pt, p, "Unmounted"));
                if (unb == "True")
                {
                    un++;
                }
                if (shown < MaxPerList)
                {
                    shown++;
                    MelonLogger.Msg("[Recon]    " + idField + "=" + (id ?? "<null>") + " cond=" + cond + " unmounted=" + unb);
                }
            }
            string more = n > shown ? " (+" + (n - shown) + " more)" : "";
            MelonLogger.Msg("[Recon]   " + idField + "-list total=" + n + " unmounted=" + un + more);
        }

        private static object GetProfileData(bool verbose)
        {
            try
            {
                Type pm = HarmonyLib.AccessTools.TypeByName("ProfileManager");
                if (verbose)
                {
                    MelonLogger.Msg("[Recon] ProfileManager type found=" + (pm != null));
                }
                if (pm != null)
                {
                    object pmi = CachedProfileManager;
                    string src = "cached(Awake)";
                    object pd = null;
                    if (pmi != null)
                    {
                        pd = Call(pm, pmi, "GetSelectedProfileData", false);
                        if (pd == null)
                        {
                            pmi = null;
                        }
                    }
                    if (pmi == null)
                    {
                        object ilType = GetIl2CppType(pm);
                        if (verbose)
                        {
                            MelonLogger.Msg("[Recon] Il2CppType found=" + (ilType != null));
                        }
                        if (ilType != null)
                        {
                            MethodInfo fo = typeof(UnityEngine.Object).GetMethod("FindObjectOfType", new[] { ilType.GetType() });
                            if (verbose)
                            {
                                MelonLogger.Msg("[Recon] FindObjectOfType(Il2CppSystem.Type) method=" + (fo != null));
                            }
                            if (fo != null)
                            {
                                pmi = fo.Invoke(null, new[] { ilType });
                                src = "FindObjectOfType";
                                if (pmi != null)
                                {
                                    CachedProfileManager = pmi;
                                    pd = Call(pm, pmi, "GetSelectedProfileData", false);
                                }
                            }
                        }
                    }
                    if (verbose)
                    {
                        MelonLogger.Msg("[Recon] ProfileManager instance via " + src + " = " + (pmi != null) + " data=" + (pd != null));
                    }
                    if (pd != null)
                    {
                        return pd;
                    }
                }
            }
            catch (Exception e)
            {
                if (verbose)
                {
                    MelonLogger.Msg("[Recon] ProfileManager path err: " + e.Message);
                }
            }

            try
            {
                Type gdm = HarmonyLib.AccessTools.TypeByName("GameDataManager");
                if (gdm != null)
                {
                    object gi = GetStatic(gdm, "get_Instance", "instance");
                    if (gi == null)
                    {
                        gi = GetStatic(gdm, "get_Instance", "m_instance");
                    }
                    if (gi != null)
                    {
                        object pd = Get(gdm, gi, "CurrentProfileData");
                        if (pd != null)
                        {
                            if (verbose)
                            {
                                MelonLogger.Msg("[Recon] ProfileData via GameDataManager.CurrentProfileData");
                            }
                            return pd;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (verbose)
                {
                    MelonLogger.Msg("[Recon] GameDataManager path err: " + e.Message);
                }
            }
            return null;
        }

        private static void DumpLiveCars()
        {
            try
            {
                Type clp = HarmonyLib.AccessTools.TypeByName("CarLoaderPlaces");
                if (clp == null)
                {
                    MelonLogger.Msg("[Recon] CarLoaderPlaces type not found");
                    return;
                }

                object places = GetStatic(clp, "Get", "m_instance");
                if (places == null)
                {
                    places = FindSceneObject(clp);
                    if (places != null)
                    {
                        MelonLogger.Msg("[Recon] CarLoaderPlaces via FindObjectOfType");
                    }
                }
                if (places == null)
                {
                    MelonLogger.Msg("[Recon] CarLoaderPlaces instance null (garage scene only)");
                    return;
                }

                int count = ToInt(Call(clp, places, "GetCarLoadersCount"));
                MelonLogger.Msg("[Recon] live carLoaders=" + count);
                for (int i = 0; i < count; i++)
                {
                    object cl = Call(clp, places, "GetCarLoaderByIndex", i);
                    if (cl == null)
                    {
                        continue;
                    }
                    Type clt = cl.GetType();
                    object partsObj = Call(clt, cl, "GetCarParts");
                    var list = partsObj as IEnumerable;
                    int n = 0;
                    int un = 0;
                    int shown = 0;
                    if (list != null)
                    {
                        foreach (var p in list)
                        {
                            if (p == null)
                            {
                                continue;
                            }
                            n++;
                            Type pt = p.GetType();
                            string unb = AsStr(Get(pt, p, "Unmounted"));
                            if (unb == "True")
                            {
                                un++;
                            }
                            if (shown < 12)
                            {
                                shown++;
                                MelonLogger.Msg("[Recon]    live name=" + (AsStr(Get(pt, p, "name")) ?? "<null>") + " cond=" + AsStr(Get(pt, p, "Condition")) + " unmounted=" + unb);
                            }
                        }
                    }
                    MelonLogger.Msg("[Recon]  live car[" + i + "] parts=" + (partsObj == null ? "null" : n.ToString()) + " unmounted=" + un);
                }
            }
            catch (Exception e)
            {
                MelonLogger.Warning("[Recon] live dump err: " + e.Message);
            }
        }

        private static List<object> Enumerate(object listObj)
        {
            var result = new List<object>();
            if (listObj == null)
            {
                return result;
            }
            if (listObj is IEnumerable en)
            {
                int k = 0;
                foreach (var x in en)
                {
                    result.Add(x);
                    if (++k >= 5000)
                    {
                        break;
                    }
                }
                return result;
            }
            try
            {
                int cnt = ToInt(Get(listObj.GetType(), listObj, "Count"));
                if (cnt > 5000)
                {
                    cnt = 5000;
                }
                PropertyInfo item = listObj.GetType().GetProperty("Item");
                for (int i = 0; i < cnt; i++)
                {
                    object el = item != null ? item.GetValue(listObj, new object[] { i }) : null;
                    result.Add(el);
                }
            }
            catch
            {
            }
            return result;
        }

        private static void DumpAllProps(object o)
        {
            try
            {
                Type t = o.GetType();
                var sb = new StringBuilder();
                foreach (var p in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    object v;
                    try
                    {
                        v = p.GetValue(o, null);
                    }
                    catch
                    {
                        v = "<err>";
                    }
                    string desc;
                    if (v == null)
                    {
                        desc = "null";
                    }
                    else if (!(v is string) && (v is IEnumerable || v.GetType().GetProperty("Count") != null))
                    {
                        desc = "list(" + Enumerate(v).Count + ")";
                    }
                    else
                    {
                        desc = v.ToString();
                    }
                    sb.Append(p.Name).Append("=").Append(desc).Append("  ");
                }
                MelonLogger.Msg("[Recon]   PROPS: " + sb.ToString());
            }
            catch (Exception e)
            {
                MelonLogger.Msg("[Recon]   PROPS err: " + e.Message);
            }
        }

        private static object GetStatic(Type t, string methodName, string fieldName)
        {
            try
            {
                PropertyInfo p = t.GetProperty(methodName == "get_Instance" ? "Instance" : methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                if (p != null)
                {
                    return p.GetValue(null, null);
                }
            }
            catch
            {
            }
            try
            {
                FieldInfo f = t.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                if (f != null)
                {
                    return f.GetValue(null);
                }
            }
            catch
            {
            }
            return null;
        }

        private static object Call(Type t, object inst, string name, params object[] args)
        {
            try
            {
                MethodInfo m = t.GetMethod(name);
                if (m == null)
                {
                    return null;
                }
                return m.Invoke(inst, args);
            }
            catch
            {
                return null;
            }
        }

        private static object Get(Type t, object inst, string name)
        {
            Dictionary<string, MemberInfo> byName;
            if (!MemberCache.TryGetValue(t, out byName))
            {
                byName = new Dictionary<string, MemberInfo>();
                MemberCache[t] = byName;
            }
            MemberInfo mi;
            if (!byName.TryGetValue(name, out mi))
            {
                mi = t.GetProperty(name);
                if (mi == null)
                {
                    mi = t.GetField(name);
                }
                byName[name] = mi;
            }
            if (mi == null)
            {
                return null;
            }
            try
            {
                PropertyInfo pi = mi as PropertyInfo;
                if (pi != null)
                {
                    return pi.GetValue(inst, null);
                }
                return ((FieldInfo)mi).GetValue(inst);
            }
            catch
            {
                return null;
            }
        }

        private static object FindSceneObject(Type gameType)
        {
            try
            {
                object ilType = GetIl2CppType(gameType);
                if (ilType == null)
                {
                    return null;
                }
                MethodInfo fo = typeof(UnityEngine.Object).GetMethod("FindObjectOfType", new[] { ilType.GetType() });
                if (fo == null)
                {
                    return null;
                }
                return fo.Invoke(null, new[] { ilType });
            }
            catch
            {
                return null;
            }
        }

        private static object GetIl2CppType(Type gameType)
        {
            try
            {
                PropertyInfo p = gameType.GetProperty("Il2CppType", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                if (p != null)
                {
                    return p.GetValue(null, null);
                }
            }
            catch
            {
            }
            return null;
        }

        private static string AsStr(object o)
        {
            return o == null ? null : o.ToString();
        }

        private static int ToInt(object o)
        {
            try
            {
                return Convert.ToInt32(o);
            }
            catch
            {
                return 0;
            }
        }

        private static float ToFloat(object o)
        {
            try
            {
                return Convert.ToSingle(o);
            }
            catch
            {
                return 0f;
            }
        }
    }
}
