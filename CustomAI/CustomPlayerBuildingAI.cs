using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreOutsideInteraction.CustomAI
{
    public class CustomPlayerBuildingAI
    {
        public static bool haveGarbageBuilding = true;
        public static bool haveFireBuilding = true;
        public static bool haveCemetryBuilding = true;
        public static bool haveHospitalBuilding = true;
        public static bool havePoliceBuilding = true;
        public static bool haveGarbageBuildingTemp = true;
        public static bool haveFireBuildingTemp = true;
        public static bool haveCemetryBuildingTemp = true;
        public static bool haveHospitalBuildingTemp = true;
        public static bool havePoliceBuildingTemp = true;
        public static void PlayerBuildingAISimulationStepPostFix(ushort buildingID, ref Building buildingData, ref Building.Frame frameData)
        {
            if ((buildingData.Info.m_class.m_service == ItemClass.Service.HealthCare) && (buildingData.Info.m_class.m_level == ItemClass.Level.Level2))
            {
                haveCemetryBuildingTemp = true;
            }
            else if (buildingData.Info.m_class.m_service == ItemClass.Service.Garbage)
            {
                haveGarbageBuildingTemp = true;
            }
            else if (buildingData.Info.m_class.m_service == ItemClass.Service.PoliceDepartment)
            {
                havePoliceBuildingTemp = true;
            }
            else if ((buildingData.Info.m_class.m_service == ItemClass.Service.HealthCare) && (buildingData.Info.m_class.m_level == ItemClass.Level.Level1))
            {
                haveHospitalBuildingTemp = true;
            }
            else if (buildingData.Info.m_class.m_service == ItemClass.Service.FireDepartment)
            {
                haveFireBuildingTemp = true;
            }
        }
    }
}
