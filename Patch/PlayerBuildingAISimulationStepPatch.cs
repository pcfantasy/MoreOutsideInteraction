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
    public static class PlayerBuildingAISimulationStepPatch
    {
        public static MethodBase TargetMethod()
        {
            return typeof(PlayerBuildingAI).GetMethod("SimulationStep", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Building).MakeByRefType(), typeof(Building.Frame).MakeByRefType() }, null);
        }
        public static void Postfix(ushort buildingID, ref Building buildingData, ref Building.Frame frameData)
        {
            if ((buildingData.Info.m_class.m_service == ItemClass.Service.HealthCare) && (buildingData.Info.m_class.m_level == ItemClass.Level.Level2))
            {
                CustomPlayerBuildingAI.haveCemetryBuildingTemp = true;
            }
            else if (buildingData.Info.m_class.m_service == ItemClass.Service.Garbage)
            {
                CustomPlayerBuildingAI.haveGarbageBuildingTemp = true;
            }
            else if (buildingData.Info.m_class.m_service == ItemClass.Service.PoliceDepartment)
            {
                CustomPlayerBuildingAI.havePoliceBuildingTemp = true;
            }
            else if ((buildingData.Info.m_class.m_service == ItemClass.Service.HealthCare) && (buildingData.Info.m_class.m_level == ItemClass.Level.Level1))
            {
                CustomPlayerBuildingAI.haveHospitalBuildingTemp = true;
            }
            else if (buildingData.Info.m_class.m_service == ItemClass.Service.FireDepartment)
            {
                CustomPlayerBuildingAI.haveFireBuildingTemp = true;
            }
        }
    }
}
