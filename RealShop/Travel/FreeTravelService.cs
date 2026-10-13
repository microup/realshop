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
    internal static class FreeTravelService
    {
        internal static int PendingFreeTravelCost { get; set; }

        internal static void MarkFreeTravelPayment(MapDestinationID destinationID)
        {
            int cost = GetFreeTravelCost(destinationID);
            if (cost > 0)
            {
                PendingFreeTravelCost = cost;
            }
        }

        internal static int GetFreeTravelCost(MapDestinationID destinationID)
        {
            if (destinationID == MapDestinationID.Junkyard)
            {
                int cost = GlobalData.Cost_TravelJunkyard;
                return cost > 0 ? cost : RealParts.DefaultJunkyardTravelCost;
            }

            if (destinationID == MapDestinationID.Auction)
            {
                return GlobalData.Cost_TravelAuction;
            }

            return 0;
        }

        internal static bool IsFreeTravelDestination(MapDestinationID destinationID)
        {
            return destinationID == MapDestinationID.Junkyard || destinationID == MapDestinationID.Auction;
        }
    }
}
