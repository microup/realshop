using MelonLoader;
using System;
using System.Reflection;
using System.Text;

namespace RealShop
{
    internal static class BuildJobPartMemberProbe
    {
        private const int MembersCap = 900;

        private static readonly string[] IdMemberNames = { "id", "realId", "partId", "itemId" };

        private static readonly string[] NameMemberNames = { "name", "localizedName", "partName", "itemName" };

        private static readonly object ResolveLock = new object();

        private static MemberInfo _idMember;

        private static MemberInfo _nameMember;

        private static bool _membersLogged;

        internal static bool IdentityUnresolved
        {
            get
            {
                return _idMember == null && _nameMember == null;
            }
        }

        internal static string IdMemberName
        {
            get
            {
                return Name(_idMember);
            }
        }

        internal static string NameMemberName
        {
            get
            {
                return Name(_nameMember);
            }
        }

        internal static void Resolve(JobPart sample)
        {
            if (sample == null || (_idMember != null && _nameMember != null))
            {
                return;
            }

            Type type = sample.GetType();
            string describe = null;

            lock (ResolveLock)
            {
                if (_idMember == null)
                {
                    _idMember = ResolveMember(type, IdMemberNames, out describe);
                }

                if (_nameMember == null)
                {
                    _nameMember = ResolveMember(type, NameMemberNames, out describe);
                }
            }

            if (describe != null && !_membersLogged)
            {
                _membersLogged = true;
                MelonLogger.Msg("[BuildJob] jobpart members: type=" + type.Name + " " + describe);
            }
        }

        internal static string IdOf(JobPart part)
        {
            return Text(part, _idMember);
        }

        internal static string NameOf(JobPart part)
        {
            string label = Text(part, _nameMember);
            return string.IsNullOrEmpty(label) ? Text(part, _idMember) : label;
        }

        private static string Text(JobPart part, MemberInfo member)
        {
            if (part == null || member == null)
            {
                return null;
            }

            try
            {
                object value = ReadMember(part, member);
                return value == null ? null : value.ToString();
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        private static object ReadMember(object target, MemberInfo member)
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

        private static MemberInfo ResolveMember(Type type, string[] names, out string describe)
        {
            describe = null;
            FieldInfo[] fields = null;
            PropertyInfo[] properties = null;
            MemberInfo selected = null;
            StringBuilder failures = new StringBuilder();

            try
            {
                fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            }
            catch (System.Exception e)
            {
                failures.Append(" fields:").Append(e.Message);
            }

            try
            {
                properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public
                    | BindingFlags.NonPublic);
            }
            catch (System.Exception e)
            {
                failures.Append(" props:").Append(e.Message);
            }

            for (int i = 0; i < names.Length && selected == null; i++)
            {
                try
                {
                    selected = Find(fields, properties, names[i]);
                }
                catch (System.Exception e)
                {
                    failures.Append(" find ").Append(names[i]).Append(":").Append(e.Message);
                }
            }

            try
            {
                describe = "fields=" + Count(fields) + " props=" + Count(properties)
                    + " names=" + Describe(fields, properties);
            }
            catch (System.Exception e)
            {
                failures.Append(" describe:").Append(e.Message);
            }

            if (failures.Length > 0)
            {
                MelonLogger.Warning("[BuildJob] jobpart members: type=" + type.Name + " failed" + failures);
            }

            return selected;
        }

        private static int Count<T>(T[] values)
        {
            return values == null ? -1 : values.Length;
        }

        private static MemberInfo Find(FieldInfo[] fields, PropertyInfo[] properties, string name)
        {
            if (fields != null)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    FieldInfo field = fields[i];
                    if (field != null && !field.IsStatic && !field.IsLiteral
                        && string.Equals(field.Name, name, StringComparison.OrdinalIgnoreCase))
                    {
                        return field;
                    }
                }
            }

            if (properties != null)
            {
                for (int i = 0; i < properties.Length; i++)
                {
                    PropertyInfo property = properties[i];
                    if (property == null)
                    {
                        continue;
                    }

                    MethodInfo getter = property.GetMethod;
                    if (property.CanRead && getter != null && !getter.IsStatic
                        && property.GetIndexParameters().Length == 0
                        && string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase))
                    {
                        return property;
                    }
                }
            }

            return null;
        }

        private static string Describe(FieldInfo[] fields, PropertyInfo[] properties)
        {
            StringBuilder names = new StringBuilder();
            int dropped = 0;

            if (fields != null)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    FieldInfo field = fields[i];
                    if (field != null)
                    {
                        Append(names, field.Name + ":" + field.FieldType.Name, ref dropped);
                    }
                }
            }

            if (properties != null)
            {
                for (int i = 0; i < properties.Length; i++)
                {
                    PropertyInfo property = properties[i];
                    if (property != null && property.GetIndexParameters().Length == 0)
                    {
                        Append(names, property.Name + ":" + property.PropertyType.Name, ref dropped);
                    }
                }
            }

            return names + (dropped > 0 ? " (+" + dropped + " more)" : "");
        }

        private static void Append(StringBuilder names, string name, ref int dropped)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            if (names.Length >= MembersCap)
            {
                dropped++;
                return;
            }

            if (names.Length > 0)
            {
                names.Append('|');
            }

            names.Append(name);
        }

        private static string Name(MemberInfo member)
        {
            return member == null ? "<none>" : member.Name;
        }
    }
}
