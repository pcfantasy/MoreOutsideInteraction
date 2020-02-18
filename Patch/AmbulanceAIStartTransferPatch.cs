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
    public static class AmbulanceAIStartTransferPatch
    {
        public static MethodBase TargetMethod()
        {
            return typeof(AmbulanceAI).GetMethod("StartTransfer", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Vehicle).MakeByRefType(), typeof(TransferManager.TransferReason), typeof(TransferManager.TransferOffer) }, null);
        }
        public static bool Prefix(ref AmbulanceAI __instance, ushort vehicleID, ref Vehicle data, TransferManager.TransferReason material, TransferManager.TransferOffer offer)
        {
            if (material == (TransferManager.TransferReason)data.m_transferType)
            {
                if ((data.m_flags & Vehicle.Flags.WaitingTarget) != (Vehicle.Flags)0)
                {
                    if (offer.Building != 0)
                    {
                        __instance.SetTarget(vehicleID, ref data, offer.Building);
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
