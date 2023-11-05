using MelonLoader;
using UnityEngine;

namespace RealShop
{
    internal class Inspect
    {
        public static void UnityObject(GameObject parent)
        {
            if (parent == null) return;

            int childCount = parent.transform.childCount;

            for (int i = 0; i < childCount; i++)
            {
                Transform currentChild = parent.transform.GetChild(i);
                var currentChildType = currentChild.gameObject.GetType();
                MelonLogger.Msg($"Child {i}: {currentChild.gameObject.name} - {currentChildType}");
            }
        }

        public static void UnityObject(Transform obj)
        {
            if (obj == null) return;

            int childCount = obj.childCount;

            for (int i = 0; i < childCount; i++)
            {
                Transform currentChild = obj.GetChild(i);
                var currentChildType = currentChild.gameObject.GetType();
                MelonLogger.Msg($"Child {i}: {currentChild.gameObject.name} - {currentChildType}");
            }
        }

    }
}
