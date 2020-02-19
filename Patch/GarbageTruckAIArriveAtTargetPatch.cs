using ColossalFramework;
using ColossalFramework.Math;
using Harmony;
using MoreOutsideInteraction.CustomAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace MoreOutsideInteraction.Patch
{
    [HarmonyPatch]
    public static class GarbageTruckAIArriveAtTargetPatch
    {
        public static MethodBase TargetMethod()
        {
            return typeof(GarbageTruckAI).GetMethod("ArriveAtTarget", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Vehicle).MakeByRefType()}, null);
        }
        public static bool Prefix(ref GarbageTruckAI __instance, ushort vehicleID, ref Vehicle data, ref bool __result)
        {
            if (data.m_targetBuilding == 0)
            {
                __result = true;
                return false;
            }
            int num = 0;
            if ((data.m_flags & Vehicle.Flags.TransferToTarget) != (Vehicle.Flags)0)
            {
                num = (int)data.m_transferSize;
            }
            if ((data.m_flags & Vehicle.Flags.TransferToSource) != (Vehicle.Flags)0)
            {
                num = Mathf.Min(0, (int)data.m_transferSize - __instance.m_cargoCapacity);
            }
            BuildingManager instance = Singleton<BuildingManager>.instance;
            BuildingInfo info = instance.m_buildings.m_buffer[(int)data.m_targetBuilding].Info;
            info.m_buildingAI.ModifyMaterialBuffer(data.m_targetBuilding, ref instance.m_buildings.m_buffer[(int)data.m_targetBuilding], (TransferManager.TransferReason)data.m_transferType, ref num);
            ProcessGarbageIncomeArriveAtTarget(vehicleID, ref data, num);
            if ((data.m_flags & Vehicle.Flags.TransferToTarget) != (Vehicle.Flags)0)
            {
                data.m_transferSize = (ushort)Mathf.Clamp((int)data.m_transferSize - num, 0, (int)data.m_transferSize);
            }
            if ((data.m_flags & Vehicle.Flags.TransferToSource) != (Vehicle.Flags)0)
            {
                data.m_transferSize += (ushort)Mathf.Max(0, -num);
            }

            // NON-STOCK CODE START
            //Go back
            if (data.m_sourceBuilding != 0 && (instance.m_buildings.m_buffer[(int)data.m_sourceBuilding].m_flags & Building.Flags.IncomingOutgoing) == Building.Flags.Outgoing)
            {
                BuildingInfo info2 = instance.m_buildings.m_buffer[(int)data.m_sourceBuilding].Info;
                ushort num2 = instance.FindBuilding(instance.m_buildings.m_buffer[(int)data.m_sourceBuilding].m_position, 200f, info2.m_class.m_service, ItemClass.SubService.None, Building.Flags.Incoming, Building.Flags.Outgoing);
                if (num2 != 0)
                {
                    instance.m_buildings.m_buffer[(int)data.m_sourceBuilding].RemoveOwnVehicle(vehicleID, ref data);
                    data.m_sourceBuilding = num2;
                    instance.m_buildings.m_buffer[(int)data.m_sourceBuilding].AddOwnVehicle(vehicleID, ref data);
                }
            }

            //Turn around
            if ((instance.m_buildings.m_buffer[(int)data.m_targetBuilding].m_flags & Building.Flags.IncomingOutgoing) == Building.Flags.Incoming)
            {
                if (Loader.isRealCityRunning)
                {
                    double x = instance.m_buildings.m_buffer[(int)data.m_targetBuilding].m_position.x - instance.m_buildings.m_buffer[(int)data.m_sourceBuilding].m_position.x;
                    double z = instance.m_buildings.m_buffer[(int)data.m_targetBuilding].m_position.z - instance.m_buildings.m_buffer[(int)data.m_sourceBuilding].m_position.z;
                    x = (x > 0) ? x : -x;
                    z = (z > 0) ? z : -z;
                    double distance = (x + z);
                    Singleton<EconomyManager>.instance.AddPrivateIncome((int)(-num * (distance / 4000f)), ItemClass.Service.Garbage, ItemClass.SubService.None, ItemClass.Level.Level3, 115);
                    CustomPlayerBuildingAI.canReturn[vehicleID] = true;
                }
                ushort num3 = instance.FindBuilding(instance.m_buildings.m_buffer[(int)data.m_targetBuilding].m_position, 200f, info.m_class.m_service, ItemClass.SubService.None, Building.Flags.Outgoing, Building.Flags.Incoming);
                if (num3 != 0)
                {
                    BuildingInfo info3 = instance.m_buildings.m_buffer[(int)num3].Info;
                    Randomizer randomizer = new Randomizer((int)vehicleID);
                    Vector3 vector;
                    Vector3 vector2;
                    info3.m_buildingAI.CalculateSpawnPosition(num3, ref instance.m_buildings.m_buffer[(int)num3], ref randomizer, __instance.m_info, out vector, out vector2);
                    Quaternion rotation = Quaternion.identity;
                    Vector3 forward = vector2 - vector;
                    if (forward.sqrMagnitude > 0.01f)
                    {
                        rotation = Quaternion.LookRotation(forward);
                    }
                    data.m_frame0 = new Vehicle.Frame(vector, rotation);
                    data.m_frame1 = data.m_frame0;
                    data.m_frame2 = data.m_frame0;
                    data.m_frame3 = data.m_frame0;
                    data.m_targetPos0 = vector;
                    data.m_targetPos0.w = 2f;
                    data.m_targetPos1 = vector2;
                    data.m_targetPos1.w = 2f;
                    data.m_targetPos2 = data.m_targetPos1;
                    data.m_targetPos3 = data.m_targetPos1;
                    __instance.FrameDataUpdated(vehicleID, ref data, ref data.m_frame0);
                    __instance.SetTarget(vehicleID, ref data, 0);
                    __result = false;
                    return false;
                }
            }
            /// NON-STOCK CODE END ///
            __instance.SetTarget(vehicleID, ref data, 0);
            __result = false;
            return false;
        }

        public static void ProcessGarbageIncomeArriveAtTarget(ushort vehicleID, ref Vehicle data, int num)
        {
            BuildingManager instance = Singleton<BuildingManager>.instance;
            Building building = instance.m_buildings.m_buffer[(int)data.m_sourceBuilding];
            Building building1 = instance.m_buildings.m_buffer[(int)data.m_targetBuilding];
            BuildingInfo info = instance.m_buildings.m_buffer[(int)data.m_targetBuilding].Info;

            if ((data.m_flags & Vehicle.Flags.TransferToTarget) != (Vehicle.Flags)0)
            {
                if (building.m_flags.IsFlagSet(Building.Flags.Untouchable))
                {
                    // Garbage income is based on distance.
                    if ((data.m_flags & Vehicle.Flags.Importing) != (Vehicle.Flags)0)
                    {
                        if (Loader.isRealCityRunning)
                        {
                            double x = instance.m_buildings.m_buffer[(int)data.m_targetBuilding].m_position.x - instance.m_buildings.m_buffer[(int)data.m_sourceBuilding].m_position.x;
                            double z = instance.m_buildings.m_buffer[(int)data.m_targetBuilding].m_position.z - instance.m_buildings.m_buffer[(int)data.m_sourceBuilding].m_position.z;
                            x = (x > 0) ? x : -x;
                            z = (z > 0) ? z : -z;
                            double distance = (x + z);
                            Singleton<EconomyManager>.instance.AddPrivateIncome((int)(num * (distance / 4000f)), ItemClass.Service.Garbage, ItemClass.SubService.None, ItemClass.Level.Level3, 115);
                        }
                    }
                }
            }
        }
    }
}
