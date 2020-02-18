using ColossalFramework;
using ColossalFramework.Math;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace MoreOutsideInteraction.Patch
{
    [HarmonyPatch]
    public static class FireTruckAIStartTransferPatch
    {
        public static MethodBase TargetMethod()
        {
            return typeof(FireTruckAI).GetMethod("StartTransfer", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Vehicle).MakeByRefType(), typeof(TransferManager.TransferReason), typeof(TransferManager.TransferOffer) }, null);
        }
        public static bool Prefix(ref FireTruckAI __instance, ushort vehicleID, ref Vehicle data, TransferManager.TransferReason material, TransferManager.TransferOffer offer)
        {
            if (material == (TransferManager.TransferReason)data.m_transferType)
            {
                if (Singleton<BuildingManager>.instance.m_buildings.m_buffer[offer.Building].m_flags.IsFlagSet(Building.Flags.Untouchable))
                {
                    if (offer.Building != 0)
                    {
                        if ((data.m_flags & Vehicle.Flags.WaitingTarget) != (Vehicle.Flags)0)
                        {
                            __instance.SetTarget(vehicleID, ref data, offer.Building);
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }
}
