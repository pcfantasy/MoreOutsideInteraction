﻿using HarmonyLib;
using MoreOutsideInteraction.CustomAI;
using System;
using System.Reflection;

namespace MoreOutsideInteraction.Patch
{
    [HarmonyPatch]
    public static class GarbageTruckAIShouldReturnToSourcePatch
    {
        public static MethodBase TargetMethod()
        {
            return typeof(GarbageTruckAI).GetMethod("ShouldReturnToSource", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Vehicle).MakeByRefType() }, null);
        }
        public static bool Prefix(ushort vehicleID, ref bool __result)
        {
            if (CustomPlayerBuildingAI.canReturn[vehicleID])
            {
                __result = true;
                return false;
            }
            return true ;
        }
    }
}
