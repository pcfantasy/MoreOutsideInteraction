using System;
using ColossalFramework;
using System.Reflection;

namespace MoreOutsideInteraction
{
    public class CustomTransferManager
    {
        private void StartTransfer(TransferManager.TransferReason material, TransferManager.TransferOffer offerOut, TransferManager.TransferOffer offerIn, int delta)
        {
            bool active = offerIn.Active;
            bool active2 = offerOut.Active;
            if (active && offerIn.Vehicle != 0)
            {
                Array16<Vehicle> vehicles = Singleton<VehicleManager>.instance.m_vehicles;
                ushort vehicle = offerIn.Vehicle;
                VehicleInfo info = vehicles.m_buffer[(int)vehicle].Info;
                offerOut.Amount = delta;
                info.m_vehicleAI.StartTransfer(vehicle, ref vehicles.m_buffer[(int)vehicle], material, offerOut);
            }
            else if (active2 && offerOut.Vehicle != 0)
            {
                Array16<Vehicle> vehicles2 = Singleton<VehicleManager>.instance.m_vehicles;
                ushort vehicle2 = offerOut.Vehicle;
                VehicleInfo info2 = vehicles2.m_buffer[(int)vehicle2].Info;
                offerIn.Amount = delta;
                info2.m_vehicleAI.StartTransfer(vehicle2, ref vehicles2.m_buffer[(int)vehicle2], material, offerIn);
            }
            else if (active && offerIn.Citizen != 0u)
            {
                Array32<Citizen> citizens = Singleton<CitizenManager>.instance.m_citizens;
                uint citizen = offerIn.Citizen;
                CitizenInfo citizenInfo = citizens.m_buffer[(int)((UIntPtr)citizen)].GetCitizenInfo(citizen);
                if (citizenInfo != null)
                {
                    offerOut.Amount = delta;
                    citizenInfo.m_citizenAI.StartTransfer(citizen, ref citizens.m_buffer[(int)((UIntPtr)citizen)], material, offerOut);
                }
            }
            else if (active2 && offerOut.Citizen != 0u)
            {
                Array32<Citizen> citizens2 = Singleton<CitizenManager>.instance.m_citizens;
                uint citizen2 = offerOut.Citizen;
                CitizenInfo citizenInfo2 = citizens2.m_buffer[(int)((UIntPtr)citizen2)].GetCitizenInfo(citizen2);
                if (citizenInfo2 != null)
                {
                    offerIn.Amount = delta;
                    citizenInfo2.m_citizenAI.StartTransfer(citizen2, ref citizens2.m_buffer[(int)((UIntPtr)citizen2)], material, offerIn);
                }
            }
            else if (active2 && offerOut.Building != 0)
            {
                Array16<Building> buildings = Singleton<BuildingManager>.instance.m_buildings;
                ushort building = offerOut.Building;
                ushort building1 = offerIn.Building;
                BuildingInfo info3 = buildings.m_buffer[(int)building].Info;
                offerIn.Amount = delta;
                // NON-STOCK CODE START
                // New Outside Interaction
                if ((material == TransferManager.TransferReason.DeadMove || material == TransferManager.TransferReason.GarbageMove) && Singleton<BuildingManager>.instance.m_buildings.m_buffer[offerOut.Building].m_flags.IsFlagSet(Building.Flags.Untouchable))
                {
                    DebugLog.LogToFileOnly("Find StartMoreTransfer");
                    StartMoreTransfer(building, ref buildings.m_buffer[(int)building], material, offerIn);
                }
                else
                {
                    // NON-STOCK CODE END
                    info3.m_buildingAI.StartTransfer(building, ref buildings.m_buffer[(int)building], material, offerIn);
                }
            }
            else if (active && offerIn.Building != 0)
            {
                Array16<Building> buildings2 = Singleton<BuildingManager>.instance.m_buildings;
                ushort building2 = offerIn.Building;
                BuildingInfo info4 = buildings2.m_buffer[(int)building2].Info;
                offerOut.Amount = delta;
                info4.m_buildingAI.StartTransfer(building2, ref buildings2.m_buffer[(int)building2], material, offerOut);
            }
        }

        public void StartMoreTransfer(ushort buildingID, ref Building data, TransferManager.TransferReason material, TransferManager.TransferOffer offer)
        {
            if (material == TransferManager.TransferReason.GarbageMove)
            {
                DebugLog.LogToFileOnly("Find StartMoreTransfer GarbageMove");
                VehicleInfo randomVehicleInfo2 = Singleton<VehicleManager>.instance.GetRandomVehicleInfo(ref Singleton<SimulationManager>.instance.m_randomizer, ItemClass.Service.Garbage, ItemClass.SubService.None, ItemClass.Level.Level1);
                if (randomVehicleInfo2 != null)
                {
                    Array16<Vehicle> vehicles2 = Singleton<VehicleManager>.instance.m_vehicles;
                    ushort num2;
                    if (Singleton<VehicleManager>.instance.CreateVehicle(out num2, ref Singleton<SimulationManager>.instance.m_randomizer, randomVehicleInfo2, data.m_position, material, false, true))
                    {
                        randomVehicleInfo2.m_vehicleAI.SetSource(num2, ref vehicles2.m_buffer[(int)num2], buildingID);
                        randomVehicleInfo2.m_vehicleAI.StartTransfer(num2, ref vehicles2.m_buffer[(int)num2], material, offer);
                        vehicles2.m_buffer[num2].m_flags |= (Vehicle.Flags.Importing);
                    }
                }
            }
            else if (material == TransferManager.TransferReason.DeadMove)
            {
                DebugLog.LogToFileOnly("Find StartMoreTransfer DeadMove");
                VehicleInfo randomVehicleInfo2 = Singleton<VehicleManager>.instance.GetRandomVehicleInfo(ref Singleton<SimulationManager>.instance.m_randomizer, ItemClass.Service.HealthCare, ItemClass.SubService.None, ItemClass.Level.Level2);
                if (randomVehicleInfo2 != null)
                {
                    Array16<Vehicle> vehicles2 = Singleton<VehicleManager>.instance.m_vehicles;
                    ushort num2;
                    if (Singleton<VehicleManager>.instance.CreateVehicle(out num2, ref Singleton<SimulationManager>.instance.m_randomizer, randomVehicleInfo2, data.m_position, material, false, true))
                    {
                        randomVehicleInfo2.m_vehicleAI.SetSource(num2, ref vehicles2.m_buffer[(int)num2], buildingID);
                        randomVehicleInfo2.m_vehicleAI.StartTransfer(num2, ref vehicles2.m_buffer[(int)num2], material, offer);
                        vehicles2.m_buffer[num2].m_flags |= (Vehicle.Flags.Importing);
                    }
                }
            }
        }
    }
}//end naming space 