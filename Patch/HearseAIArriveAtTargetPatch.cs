using ColossalFramework;
using HarmonyLib;
using MoreOutsideInteraction.Util;
using System;
using System.Reflection;
using UnityEngine;

namespace MoreOutsideInteraction.Patch
{
    [HarmonyPatch]
    public static class HearseAIArriveAtTargetPatch
    {
        public static MethodBase TargetMethod()
        {
            return typeof(HearseAI).GetMethod("ArriveAtTarget", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Vehicle).MakeByRefType()}, null);
        }
        public static bool Prefix(ref HearseAI __instance, ushort vehicleID, ref Vehicle data, ref bool __result)
        {
            if (data.m_targetBuilding == 0)
            {
                Singleton<VehicleManager>.instance.ReleaseVehicle(vehicleID);
                __result = true;
                return false;
            }
            if (data.m_transferType == 42)
            {
                int num = 0;
                if ((data.m_flags & Vehicle.Flags.TransferToTarget) != (Vehicle.Flags)0)
                {
                    num = (int)data.m_transferSize;
                }
                if ((data.m_flags & Vehicle.Flags.TransferToSource) != (Vehicle.Flags)0)
                {
                    num = Mathf.Min(0, (int)data.m_transferSize - __instance.m_corpseCapacity);
                }
                BuildingManager instance = Singleton<BuildingManager>.instance;
                BuildingInfo info = instance.m_buildings.m_buffer[(int)data.m_targetBuilding].Info;
                info.m_buildingAI.ModifyMaterialBuffer(data.m_targetBuilding, ref instance.m_buildings.m_buffer[(int)data.m_targetBuilding], (TransferManager.TransferReason)data.m_transferType, ref num);
                ProcessDeadmoveIncomeArriveAtTarget(ref data, num);
                if ((data.m_flags & Vehicle.Flags.TransferToTarget) != (Vehicle.Flags)0)
                {
                    data.m_transferSize = (ushort)Mathf.Clamp((int)data.m_transferSize - num, 0, (int)data.m_transferSize);
                }
                if ((data.m_flags & Vehicle.Flags.TransferToSource) != (Vehicle.Flags)0)
                {
                    data.m_transferSize += (ushort)Mathf.Max(0, -num);
                }
                if (data.m_sourceBuilding != 0 && (instance.m_buildings.m_buffer[(int)data.m_sourceBuilding].m_flags & Building.Flags.IncomingOutgoing) == Building.Flags.Outgoing)
                {
                    BuildingInfo info2 = instance.m_buildings.m_buffer[(int)data.m_sourceBuilding].Info;
                    ushort num2 = instance.FindBuilding(instance.m_buildings.m_buffer[(int)data.m_sourceBuilding].m_position, 200f, info2.m_class.m_service, info2.m_class.m_subService, Building.Flags.Incoming, Building.Flags.Outgoing);
                    if (num2 != 0)
                    {
                        instance.m_buildings.m_buffer[(int)data.m_sourceBuilding].RemoveOwnVehicle(vehicleID, ref data);
                        data.m_sourceBuilding = num2;
                        instance.m_buildings.m_buffer[(int)data.m_sourceBuilding].AddOwnVehicle(vehicleID, ref data);
                    }
                }
                __instance.SetTarget(vehicleID, ref data, 0);
                __result = false;
                return false;
            }
            return true;
        }

        public static void ProcessDeadmoveIncomeArriveAtTarget(ref Vehicle data, int num)
        {
            BuildingManager instance = Singleton<BuildingManager>.instance;
            Building building = instance.m_buildings.m_buffer[(int)data.m_sourceBuilding];
            if ((data.m_flags & Vehicle.Flags.TransferToTarget) != (Vehicle.Flags)0)
            {
                if (building.m_flags.IsFlagSet(Building.Flags.Untouchable))
                {
                    if ((data.m_flags & Vehicle.Flags.Importing) != (Vehicle.Flags)0)
                    {
                        if (Loader.isRealCityRunning)
                        {
                            double x = instance.m_buildings.m_buffer[(int)data.m_targetBuilding].m_position.x - instance.m_buildings.m_buffer[(int)data.m_sourceBuilding].m_position.x;
                            double z = instance.m_buildings.m_buffer[(int)data.m_targetBuilding].m_position.z - instance.m_buildings.m_buffer[(int)data.m_sourceBuilding].m_position.z;
                            x = (x > 0) ? x : -x;
                            z = (z > 0) ? z : -z;
                            double distance = (x + z);
                            int money = (int)(num * distance);
                            Singleton<EconomyManager>.instance.AddPrivateIncome(money, ItemClass.Service.HealthCare, ItemClass.SubService.None, ItemClass.Level.Level3, 115333);
                            if (Loader.isRealCityV10)
                            {
                                RealCityUtil.InitDelegate();
                                if (RealCityUtil.GetRealCityV10())
                                    RealCityUtil.SetOutsideGovermentMoney(RealCityUtil.GetOutsideGovermentMoney() - money);
                            }
                        }
                    }
                }
            }
        }
    }
}
