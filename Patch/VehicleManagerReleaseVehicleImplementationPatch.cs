using Harmony;
using MoreOutsideInteraction.CustomAI;
using System;
using System.Reflection;

namespace MoreOutsideInteraction.Patch
{
    [HarmonyPatch]
    public static class VehicleManagerReleaseVehicleImplementationPatch
    {
        public static MethodBase TargetMethod()
        {
            return typeof(VehicleManager).GetMethod("ReleaseVehicleImplementation", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Vehicle).MakeByRefType() }, null);
        }
        public static void Prefix(ushort vehicle)
        {
            CustomPlayerBuildingAI.canReturn[vehicle] = false;
        }
    }
}
