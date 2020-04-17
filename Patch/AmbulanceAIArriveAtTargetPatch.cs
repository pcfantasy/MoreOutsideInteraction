using ColossalFramework;
using ColossalFramework.Math;
using HarmonyLib;
using MoreOutsideInteraction.CustomAI;
using System;
using System.Reflection;
using UnityEngine;

namespace MoreOutsideInteraction.Patch
{
    [HarmonyPatch]
    public static class AmbulanceAIArriveAtTargetPatch
    {
        public static MethodBase TargetMethod()
        {
            return typeof(AmbulanceAI).GetMethod("ArriveAtTarget", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Vehicle).MakeByRefType()}, null);
        }
        public static bool Prefix(ref AmbulanceAI __instance, ushort vehicleID, ref Vehicle data, ref bool __result)
        {
            if (Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int)data.m_targetBuilding].m_flags.IsFlagSet(Building.Flags.Untouchable))
            {
                if (data.m_targetBuilding == 0)
                {
                    Singleton<VehicleManager>.instance.ReleaseVehicle(vehicleID);
                    __result = true;
                }

                int num = Mathf.Min(0, (int)data.m_transferSize - __instance.m_patientCapacity);
                BuildingInfo info = Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int)data.m_targetBuilding].Info;
                info.m_buildingAI.ModifyMaterialBuffer(data.m_targetBuilding, ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int)data.m_targetBuilding], (TransferManager.TransferReason)data.m_transferType, ref num);
                var instance = Singleton<BuildingManager>.instance;
                if ((instance.m_buildings.m_buffer[(int)data.m_targetBuilding].m_flags & Building.Flags.IncomingOutgoing) == Building.Flags.Incoming)
                {
                    if (Loader.isRealCityRunning)
                    {
                        double x = instance.m_buildings.m_buffer[(int)data.m_targetBuilding].m_position.x - instance.m_buildings.m_buffer[(int)data.m_sourceBuilding].m_position.x;
                        double z = instance.m_buildings.m_buffer[(int)data.m_targetBuilding].m_position.z - instance.m_buildings.m_buffer[(int)data.m_sourceBuilding].m_position.z;
                        x = (x > 0) ? x : -x;
                        z = (z > 0) ? z : -z;
                        double distance = (x + z);
                        Singleton<EconomyManager>.instance.AddPrivateIncome((int)(-num * distance * 1.5f), ItemClass.Service.HealthCare, ItemClass.SubService.None, ItemClass.Level.Level3, 115333);
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
                    }
                }
                return false;
            }
            return true;
        }
    }
}
