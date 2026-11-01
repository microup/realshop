using MelonLoader;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace RealShop
{
    internal static class BuildJobPartPropertyProbe
    {
        private const int MembersCap = 900;
        private const string TypeMemberName = "type";
        private const int NativeTypeOffset = 0x30;
        private const int MaxPlausibleType = 32;

        private static bool _nativeAvailable;
        private static int _nativeFallbacks;

        private static readonly HashSet<Type> ResolvedTypes = new HashSet<Type>();

        private static MemberInfo _typeMember;
        private static bool _exactMatch;
        private static bool _membersLogged;
        private static string _members = "";

        internal static string Members => _members;

        internal static string SelectedMemberName
        {
            get
            {
                MemberInfo member = _typeMember;
                return member == null ? "<none>" : member.Name;
            }
        }

        internal static string SelectedMemberKind
        {
            get
            {
                MemberInfo member = _typeMember;
                if (member == null)
                {
                    return "<none>";
                }

                return member is FieldInfo ? "field" : "property";
            }
        }

        internal static string TypeSource
        {
            get
            {
                MemberInfo member = _typeMember;
                if (member != null)
                {
                    string baseName = member.Name + "/" + SelectedMemberKind;
                    return _nativeFallbacks > 0
                        ? baseName + "+native(" + _nativeFallbacks + ")"
                        : baseName;
                }

                return _nativeAvailable ? "native+0x30/native" : "<none>/<none>";
            }
        }

        internal static bool TryType(PartProperty property, out int type)
        {
            type = 0;
            if (property == null)
            {
                return false;
            }

            MemberInfo member = ResolveTypeMember(property);
            if (member != null)
            {
                try
                {
                    object value = Read(member, property);
                    if (value != null)
                    {
                        int managed = Convert.ToInt32(value);
                        if (managed > 0 && managed <= MaxPlausibleType)
                        {
                            type = managed;
                            return true;
                        }
                    }
                }
                catch (System.Exception)
                {
                }
            }

            return TryNativeType(property, member != null, out type);
        }

        private static bool TryNativeType(PartProperty property, bool memberExists, out int type)
        {
            type = 0;
            try
            {
                IntPtr pointer = property.Pointer;
                if (pointer == IntPtr.Zero)
                {
                    return false;
                }

                int value = Marshal.ReadByte(pointer, NativeTypeOffset);
                if (value <= 0 || value > MaxPlausibleType)
                {
                    return false;
                }

                type = value;
                _nativeAvailable = true;
                if (memberExists)
                {
                    _nativeFallbacks++;
                }

                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        internal static int TypeOf(GameInventory inventory, string id)
        {
            int type;
            if (inventory == null || string.IsNullOrEmpty(id))
            {
                return -1;
            }

            try
            {
                PartProperty property = inventory.GetItemProperty(id);
                if (property == null)
                {
                    return -1;
                }

                return TryType(property, out type) ? type : -1;
            }
            catch (System.Exception)
            {
                return -1;
            }
        }

        internal static string PartId(PartScript part)
        {
            if (part == null)
            {
                return null;
            }

            try
            {
                string tuned = part.GetIDWithTuned();
                if (!string.IsNullOrEmpty(tuned))
                {
                    return tuned;
                }
            }
            catch (System.Exception)
            {
            }

            try
            {
                return part.GetID();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        internal static bool IsBody(GameInventory inventory, string id)
        {
            if (inventory == null || string.IsNullOrEmpty(id))
            {
                return false;
            }

            try
            {
                PartProperty property = inventory.GetItemProperty(id);
                return property != null && property.IsBody;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        internal static bool IsInterior(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }

            try
            {
                return GameInventory.IsInteriorPart(id);
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        private static MemberInfo ResolveTypeMember(PartProperty property)
        {
            Type runtimeType = property.GetType();
            lock (ResolvedTypes)
            {
                if (ResolvedTypes.Contains(runtimeType))
                {
                    return _typeMember;
                }
            }

            FieldInfo[] fields = null;
            PropertyInfo[] properties = null;

            try
            {
                fields = runtimeType.GetFields();
                properties = runtimeType.GetProperties();
            }
            catch (System.Exception e)
            {
                MelonLogger.Warning("[BuildJob] prop members: type=" + runtimeType.Name + " failed " + e.Message);
                return null;
            }

            MemberInfo exactField = null;
            MemberInfo looseField = null;
            MemberInfo exactProperty = null;
            MemberInfo looseProperty = null;
            StringBuilder names = new StringBuilder();

            if (fields != null)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    FieldInfo field = fields[i];
                    if (field == null)
                    {
                        continue;
                    }

                    AppendMember(names, field.Name, field.FieldType);
                    if (!IsNumeric(field.FieldType))
                    {
                        continue;
                    }

                    if (string.Equals(field.Name, TypeMemberName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (exactField == null)
                        {
                            exactField = field;
                        }
                    }
                    else if (looseField == null && field.Name.IndexOf(TypeMemberName, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        looseField = field;
                    }
                }
            }

            if (properties != null)
            {
                for (int i = 0; i < properties.Length; i++)
                {
                    PropertyInfo prop = properties[i];
                    if (prop == null || prop.GetIndexParameters().Length > 0)
                    {
                        continue;
                    }

                    AppendMember(names, prop.Name, prop.PropertyType);
                    if (!prop.CanRead || !IsNumeric(prop.PropertyType))
                    {
                        continue;
                    }

                    if (string.Equals(prop.Name, TypeMemberName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (exactProperty == null)
                        {
                            exactProperty = prop;
                        }
                    }
                    else if (looseProperty == null
                        && prop.Name.IndexOf(TypeMemberName, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        looseProperty = prop;
                    }
                }
            }

            MemberInfo selected = exactField;
            bool exact = selected != null;
            if (selected == null)
            {
                selected = looseField;
            }

            if (selected == null)
            {
                selected = exactProperty;
                exact = selected != null;
            }

            if (selected == null)
            {
                selected = looseProperty;
            }

            lock (ResolvedTypes)
            {
                _typeMember = selected;
                _exactMatch = exact;
                _members = names.ToString();
                ResolvedTypes.Add(runtimeType);
            }

            if (!_membersLogged)
            {
                _membersLogged = true;
                MelonLogger.Msg("[BuildJob] prop members: type=" + runtimeType.Name
                    + " fields=" + (fields == null ? 0 : fields.Length)
                    + " props=" + (properties == null ? 0 : properties.Length)
                    + " typeMember=" + SelectedMemberName + " kind=" + SelectedMemberKind
                    + " exact=" + _exactMatch + " names=" + _members);
            }

            return _typeMember;
        }

        private static object Read(MemberInfo member, object target)
        {
            FieldInfo field = member as FieldInfo;
            if (field != null)
            {
                return field.GetValue(target);
            }

            PropertyInfo property = member as PropertyInfo;
            if (property != null)
            {
                return property.GetValue(target, null);
            }

            return null;
        }

        private static void AppendMember(StringBuilder names, string name, Type type)
        {
            if (string.IsNullOrEmpty(name) || names.Length >= MembersCap)
            {
                return;
            }

            if (names.Length > 0)
            {
                names.Append('|');
            }

            names.Append(name).Append(':').Append(ItemName(type));
        }

        private static bool IsNumeric(Type type)
        {
            if (type == null)
            {
                return false;
            }

            return type == typeof(int) || type == typeof(uint) || type == typeof(byte)
                || type == typeof(sbyte) || type == typeof(short) || type == typeof(ushort)
                || type == typeof(long) || type == typeof(ulong) || type.IsEnum;
        }

        private static string ItemName(Type type)
        {
            return type == null ? "?" : type.Name;
        }
    }
}
