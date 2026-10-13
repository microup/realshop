using CMS.UI.Logic;
using CMS.UI.Logic.Map;
using CMS.UI.Logic.RepairPart;
using CMS.UI.Windows;
using CMS.Containers;
using CMS.Helpers;
using CMS.Extensions;
using Harmony;
using MelonLoader;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace RealShop
{
    internal static class GameTypes
    {
        private static System.Type[] _cache;

        internal static System.Type[] All()
        {
            if (_cache != null)
            {
                return _cache;
            }

            System.Type[] allTypes;
            try
            {
                allTypes = typeof(Item).Assembly.GetTypes();
            }
            catch (System.Reflection.ReflectionTypeLoadException rtle)
            {
                var list = new System.Collections.Generic.List<System.Type>();
                foreach (var t in rtle.Types)
                {
                    if (t != null)
                    {
                        list.Add(t);
                    }
                }
                allTypes = list.ToArray();
            }

            _cache = allTypes;
            return _cache;
        }
    }
}
