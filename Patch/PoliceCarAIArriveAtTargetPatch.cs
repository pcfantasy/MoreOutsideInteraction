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
    public static class PoliceCarAIArriveAtTargetPatch
    {
        public static ushort instance = 0;
        public static MethodBase TargetMethod()
        {
            return typeof(PoliceCarAI).GetMethod("ArriveAtTarget", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Vehicle).MakeByRefType() }, null);
        }
        public static void Prefix(ushort instanceID, ref CitizenInstance citizenData, float minSqrDistance, out byte __state)
        {
            instance = 0;
            __state = 0;
            BuildingManager instance2 = Singleton<BuildingManager>.instance;
            if (instance2.m_buildings.m_buffer[(int)citizenData.m_targetBuilding].m_fireIntensity != 0)
            {
                // NON-STOCK CODE START
                // New added outside fire, so ignore panicking
                if (instance2.m_buildings.m_buffer[(int)citizenData.m_targetBuilding].m_flags.IsFlagSet(Building.Flags.Untouchable))
                {
                    __state = instance2.m_buildings.m_buffer[(int)citizenData.m_targetBuilding].m_fireIntensity;
                    instance = instanceID;
                    instance2.m_buildings.m_buffer[(int)citizenData.m_targetBuilding].m_fireIntensity = 0;
                }
            }
        }

        public static void Postfix(ushort instanceID, ref CitizenInstance citizenData, float minSqrDistance, ref byte __state)
        {
            if (instance == instanceID)
            {
                if (instanceID != 0)
                {
                    BuildingManager instance2 = Singleton<BuildingManager>.instance;
                    instance2.m_buildings.m_buffer[(int)citizenData.m_targetBuilding].m_fireIntensity = __state;
                }
            }
        }
    }
}
