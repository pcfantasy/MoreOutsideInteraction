using ColossalFramework;
using ColossalFramework.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MoreOutsideInteraction.CustomAI
{
    public class CustomHumanAI
    {
        protected virtual void GetBuildingTargetPosition(ushort instanceID, ref CitizenInstance citizenData, float minSqrDistance)
        {
            if (citizenData.m_targetBuilding == 0)
            {
                citizenData.m_flags &= ~(CitizenInstance.Flags.HangAround | CitizenInstance.Flags.Panicking | CitizenInstance.Flags.SittingDown | CitizenInstance.Flags.Cheering);
                citizenData.m_targetDir = Vector2.zero;
                return;
            }
            if ((citizenData.m_flags & CitizenInstance.Flags.TargetIsNode) != CitizenInstance.Flags.None)
            {
                NetManager instance = Singleton<NetManager>.instance;
                Vector3 vector = instance.m_nodes.m_buffer[(int)citizenData.m_targetBuilding].m_position;
                uint lane = instance.m_nodes.m_buffer[(int)citizenData.m_targetBuilding].m_lane;
                if (lane != 0u)
                {
                    ushort segment = instance.m_lanes.m_buffer[(int)((UIntPtr)lane)].m_segment;
                    NetInfo.Lane lane2;
                    if (instance.m_segments.m_buffer[(int)segment].GetClosestLane(lane, NetInfo.LaneType.Pedestrian, VehicleInfo.VehicleType.None, out lane, out lane2))
                    {
                        int laneOffset = (int)instance.m_nodes.m_buffer[(int)citizenData.m_targetBuilding].m_laneOffset;
                        vector = instance.m_lanes.m_buffer[(int)((UIntPtr)lane)].CalculatePosition((float)laneOffset * 0.003921569f);
                    }
                }
                citizenData.m_flags = ((citizenData.m_flags & ~(CitizenInstance.Flags.HangAround | CitizenInstance.Flags.Panicking | CitizenInstance.Flags.SittingDown | CitizenInstance.Flags.Cheering)) | CitizenInstance.Flags.HangAround);
                citizenData.m_targetPos = new Vector4(vector.x, vector.y, vector.z, 1f);
                citizenData.m_targetDir = Vector2.zero;
                return;
            }
            BuildingManager instance2 = Singleton<BuildingManager>.instance;
            if (instance2.m_buildings.m_buffer[(int)citizenData.m_targetBuilding].m_fireIntensity != 0)
            {
                // NON-STOCK CODE START
                // New added outside fire, so ignore panicking
                if (!instance2.m_buildings.m_buffer[(int)citizenData.m_targetBuilding].m_flags.IsFlagSet(Building.Flags.Untouchable))
                {
                    // NON-STOCK CODE END
                    citizenData.m_flags |= CitizenInstance.Flags.Panicking;
                    citizenData.m_targetDir = Vector2.zero;
                    return;
                }
            }
            BuildingInfo info = instance2.m_buildings.m_buffer[(int)citizenData.m_targetBuilding].Info;
            Randomizer randomizer = new Randomizer((int)instanceID << 8 | (int)citizenData.m_targetSeed);
            Vector3 vector2;
            Vector3 vector3;
            Vector2 targetDir;
            CitizenInstance.Flags flags;
            info.m_buildingAI.CalculateUnspawnPosition(citizenData.m_targetBuilding, ref instance2.m_buildings.m_buffer[(int)citizenData.m_targetBuilding], ref randomizer, citizenData.Info, instanceID, out vector2, out vector3, out targetDir, out flags);
            citizenData.m_flags = ((citizenData.m_flags & ~(CitizenInstance.Flags.HangAround | CitizenInstance.Flags.Panicking | CitizenInstance.Flags.SittingDown | CitizenInstance.Flags.Cheering)) | flags);
            citizenData.m_targetPos = new Vector4(vector2.x, vector2.y, vector2.z, 1f);
            citizenData.m_targetDir = targetDir;
        }
    }
}
