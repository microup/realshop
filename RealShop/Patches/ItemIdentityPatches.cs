using CMS.Containers;
using Harmony;
using MelonLoader;
using System;

#pragma warning disable CS0618

namespace RealShop
{
    // The game regenerates an item's UID every time it is transferred between the
    // inventory and the warehouse (NotificationCenter.MoveItem -> BaseItem.MakeNewUID).
    // RealShop derives rarity / repair-cap / profit markers deterministically from the
    // item UID, so a regenerated UID would silently re-roll the item's quality (the cap
    // could go up or down) on every warehouse round-trip. Preserve an already-assigned
    // UID so the same physical item keeps the same quality across storage moves.
    //
    // Safe: MakeNewUID is only reachable from MoveItem (item moves, never item creation);
    // freshly created items get their UID at creation. A non-zero UID simply stays.
    [HarmonyPatch(typeof(BaseItem), "MakeNewUID")]
    internal static class PreserveItemUidOnMovePatch
    {
        private static bool _warnedOnce;

        private static bool Prefix(BaseItem __instance)
        {
            try
            {
                Config config = RealParts.ActiveConfig;
                if (config == null || !config.PreserveItemQualityOnMove)
                {
                    return true;
                }

                if (__instance == null)
                {
                    return true;
                }

                // Item already carries a stable identity -> keep it (skip the re-roll).
                if (__instance.UID != 0L)
                {
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                if (!_warnedOnce)
                {
                    _warnedOnce = true;
                    MelonLogger.Warning("[ItemIdentity] prefix failed: " + e.Message);
                }

                return true;
            }
        }
    }
}

#pragma warning restore CS0618
