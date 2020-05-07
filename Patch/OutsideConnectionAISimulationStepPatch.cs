using ColossalFramework;
using HarmonyLib;
using MoreOutsideInteraction.CustomAI;
using MoreOutsideInteraction.Util;
using System;
using System.Reflection;
using UnityEngine;

namespace MoreOutsideInteraction.Patch
{
    [HarmonyPatch]
    public static class OutsideConnectionAISimulationStepPatch
    {
        public static MethodBase TargetMethod()
        {
            return typeof(OutsideConnectionAI).GetMethod("SimulationStep", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Building).MakeByRefType() }, null);
        }
        public static void Postfix(ushort buildingID, ref Building data)
        {
            if (data.Info.m_class.m_service == ItemClass.Service.Road)
            {
                bool canAddOffer = true;
                if (Loader.isRealCityV10)
                {
                    RealCityUtil.InitDelegate();
                    if (RealCityUtil.GetRealCityV10())
                    {
                        if (RealCityUtil.GetOutsideGovermentMoney() < 0)
                        {
                            canAddOffer = false;
                        }
                    }
                }

                if (canAddOffer)
                {
                    ProcessOutsideDemand(buildingID, ref data);
                    AddOffers(buildingID, ref data);
                }
            }
        }

        public static void ProcessOutsideDemand(ushort buildingID, ref Building data)
        {
            //Gabarge
            if (CustomPlayerBuildingAI.haveGarbageBuilding && (MoreOutsideInteraction.garbageFromOutside || MoreOutsideInteraction.garbageToOutside) && Singleton<UnlockManager>.instance.Unlocked(ItemClass.Service.Garbage))
            {
                if ((data.m_flags & Building.Flags.IncomingOutgoing) == Building.Flags.Incoming)
                {
                    data.m_garbageBuffer = (ushort)(data.m_garbageBuffer + 40);
                }
                else if ((data.m_flags & Building.Flags.IncomingOutgoing) == Building.Flags.IncomingOutgoing)
                {
                    data.m_garbageBuffer = (ushort)(data.m_garbageBuffer + 60);
                }
                else
                {
                    data.m_garbageBuffer = (ushort)(data.m_garbageBuffer + 20);
                }
            }
            else if (data.m_garbageBuffer != 0)
            {
                data.m_garbageBuffer = 0;
                TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);
                offer.Building = buildingID;
                Singleton<TransferManager>.instance.RemoveOutgoingOffer(TransferManager.TransferReason.Garbage, offer);
                Singleton<TransferManager>.instance.RemoveOutgoingOffer(TransferManager.TransferReason.GarbageMove, offer);
            }

            if (CustomPlayerBuildingAI.havePoliceBuilding && MoreOutsideInteraction.crimeToOutside && Singleton<UnlockManager>.instance.Unlocked(ItemClass.Service.PoliceDepartment))
            {
                if ((data.m_flags & Building.Flags.IncomingOutgoing) == Building.Flags.Incoming)
                {
                    data.m_crimeBuffer = (ushort)(data.m_crimeBuffer + 1);
                }
                else if ((data.m_flags & Building.Flags.IncomingOutgoing) == Building.Flags.IncomingOutgoing)
                {
                    data.m_crimeBuffer = (ushort)(data.m_crimeBuffer + 1);
                }
            }
            else if (data.m_crimeBuffer != 0)
            {
                data.m_crimeBuffer = 0;
                TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);
                offer.Building = buildingID;
                Singleton<TransferManager>.instance.RemoveOutgoingOffer(TransferManager.TransferReason.Crime, offer);
            }
            //sick
            if (CustomPlayerBuildingAI.haveHospitalBuilding && MoreOutsideInteraction.sickToOutside && Singleton<UnlockManager>.instance.Unlocked(ItemClass.Service.HealthCare))
            {
                if ((data.m_flags & Building.Flags.IncomingOutgoing) == Building.Flags.Incoming)
                {
                    data.m_customBuffer2 = (ushort)(data.m_customBuffer2 + 1);
                }
                else if ((data.m_flags & Building.Flags.IncomingOutgoing) == Building.Flags.IncomingOutgoing)
                {
                    data.m_customBuffer2 = (ushort)(data.m_customBuffer2 + 1);
                }
            }
            else if (data.m_customBuffer2 != 0)
            {
                data.m_customBuffer2 = 0;
                TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);
                offer.Building = buildingID;
                Singleton<TransferManager>.instance.RemoveOutgoingOffer(TransferManager.TransferReason.Sick, offer);
            }
            //fire
            if (CustomPlayerBuildingAI.haveFireBuilding && MoreOutsideInteraction.fireToOutside && Singleton<UnlockManager>.instance.Unlocked(ItemClass.Service.FireDepartment))
            {
                if ((data.m_flags & Building.Flags.IncomingOutgoing) == Building.Flags.Incoming)
                {
                    data.m_waterBuffer = (ushort)(data.m_waterBuffer + 1);
                }
                else if ((data.m_flags & Building.Flags.IncomingOutgoing) == Building.Flags.IncomingOutgoing)
                {
                    data.m_waterBuffer = (ushort)(data.m_waterBuffer + 1);
                }
                data.m_fireIntensity = 250;
            }
            else if (data.m_waterBuffer != 0)
            {
                data.m_waterBuffer = 0;
                data.m_fireIntensity = 0;
                TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);
                offer.Building = buildingID;
                Singleton<TransferManager>.instance.RemoveOutgoingOffer(TransferManager.TransferReason.Fire, offer);
            }
            //deadbuffer
            if (CustomPlayerBuildingAI.haveCemetryBuilding && MoreOutsideInteraction.deadFromOutside && Singleton<UnlockManager>.instance.Unlocked(UnlockManager.Feature.DeathCare))
            {
                if ((data.m_flags & Building.Flags.IncomingOutgoing) == Building.Flags.Outgoing)
                {
                    if (Can16timesUpdate(buildingID))
                    {
                        data.m_customBuffer1 = (ushort)(data.m_customBuffer1 + 1);
                    }
                }
                else if ((data.m_flags & Building.Flags.IncomingOutgoing) == Building.Flags.IncomingOutgoing)
                {
                    if (Can16timesUpdate(buildingID))
                    {
                        data.m_customBuffer1 = (ushort)(data.m_customBuffer1 + 1);
                    }
                }
            }
            else if (data.m_customBuffer1 != 0)
            {
                data.m_customBuffer1 = 0;
                TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);
                offer.Building = buildingID;
                Singleton<TransferManager>.instance.RemoveOutgoingOffer(TransferManager.TransferReason.DeadMove, offer);
            }

            if (data.m_customBuffer1 > 65000)
            {
                data.m_customBuffer1 = 65000;
            }

            if (data.m_customBuffer2 > 65000)
            {
                data.m_customBuffer2 = 65000;
            }

            if (data.m_crimeBuffer > 65000)
            {
                data.m_crimeBuffer = 65000;
            }

            if (data.m_garbageBuffer > 65000)
            {
                data.m_garbageBuffer = 65000;
            }

            if (data.m_waterBuffer > 65000)
            {
                data.m_waterBuffer = 65000;
            }
        }

        public static void AddGarbageOffers(ushort buildingID, ref Building data)
        {
            TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);

            if (CustomPlayerBuildingAI.haveGarbageBuilding && Singleton<UnlockManager>.instance.Unlocked(ItemClass.Service.Garbage) && (MoreOutsideInteraction.garbageFromOutside || MoreOutsideInteraction.garbageToOutside))
            {
                if ((data.m_flags & Building.Flags.IncomingOutgoing) == Building.Flags.Incoming)
                {
                    if (data.m_garbageBuffer > 200)
                    {
                        int car_valid_path = TickPathfindStatus(ref data.m_education3, ref data.m_adults);
                        SimulationManager instance1 = Singleton<SimulationManager>.instance;
                        if (car_valid_path + instance1.m_randomizer.Int32(256u) >> 8 == 0)
                        {
                            if (instance1.m_randomizer.Int32(16u) == 0)
                            {
                                int num24 = (int)data.m_garbageBuffer;
                                if (Singleton<SimulationManager>.instance.m_randomizer.Int32(5u) == 0 && Singleton<UnlockManager>.instance.Unlocked(ItemClass.Service.Garbage))
                                {
                                    int num25 = 0;
                                    int num26 = 0;
                                    int num27 = 0;
                                    int num28 = 0;
                                    CalculateGuestVehicles(buildingID, ref data, TransferManager.TransferReason.Garbage, ref num25, ref num26, ref num27, ref num28);
                                    num24 -= num27 - num26;
                                    if (num24 >= 200)
                                    {
                                        offer = default(TransferManager.TransferOffer);
                                        offer.Priority = 2;
                                        offer.Building = buildingID;
                                        offer.Position = data.m_position;
                                        offer.Amount = 1;
                                        offer.Active = false;
                                        Singleton<TransferManager>.instance.AddOutgoingOffer(TransferManager.TransferReason.Garbage, offer);
                                    }
                                }
                            }
                        }
                        else
                        {
                            int num24 = (int)data.m_garbageBuffer;
                            if (Singleton<SimulationManager>.instance.m_randomizer.Int32(5u) == 0 && Singleton<UnlockManager>.instance.Unlocked(ItemClass.Service.Garbage))
                            {
                                int num25 = 0;
                                int num26 = 0;
                                int num27 = 0;
                                int num28 = 0;
                                CalculateGuestVehicles(buildingID, ref data, TransferManager.TransferReason.Garbage, ref num25, ref num26, ref num27, ref num28);
                                num24 -= num27 - num26;
                                if (num24 >= 200)
                                {
                                    offer = default(TransferManager.TransferOffer);
                                    offer.Priority = 2;
                                    offer.Building = buildingID;
                                    offer.Position = data.m_position;
                                    offer.Amount = 1;
                                    offer.Active = false;
                                    Singleton<TransferManager>.instance.AddOutgoingOffer(TransferManager.TransferReason.Garbage, offer);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (data.m_garbageBuffer > 4000)
                    {
                        int car_valid_path = TickPathfindStatus(ref data.m_teens, ref data.m_serviceProblemTimer);
                        SimulationManager instance1 = Singleton<SimulationManager>.instance;
                        if (car_valid_path + instance1.m_randomizer.Int32(256u) >> 8 == 0)
                        {
                            if (instance1.m_randomizer.Int32(16u) == 0)
                            {
                                offer = default(TransferManager.TransferOffer);
                                offer.Priority = 2;
                                offer.Building = buildingID;
                                offer.Position = data.m_position;
                                offer.Amount = 1;
                                offer.Active = true;
                                Singleton<TransferManager>.instance.AddOutgoingOffer(TransferManager.TransferReason.GarbageMove, offer);
                            }
                        }
                        else
                        {
                            offer = default(TransferManager.TransferOffer);
                            offer.Priority = 2;
                            offer.Building = buildingID;
                            offer.Position = data.m_position;
                            offer.Amount = 1;
                            offer.Active = true;
                            Singleton<TransferManager>.instance.AddOutgoingOffer(TransferManager.TransferReason.GarbageMove, offer);
                        }
                    }
                }
            }
        }

        public static int TickPathfindStatus(ref byte success, ref byte failure)
        {
            int result = ((int)success << 8) / Mathf.Max(1, (int)(success + failure));
            if (success > failure)
            {
                success = (byte)(success + 1 >> 1);
                failure = (byte)(failure >> 1);
            }
            else
            {
                success = (byte)(success >> 1);
                failure = (byte)(failure + 1 >> 1);
            }
            return result;
        }

        public static void CalculateGuestVehicles(ushort buildingID, ref Building data, TransferManager.TransferReason material, ref int count, ref int cargo, ref int capacity, ref int outside)
        {
            VehicleManager instance = Singleton<VehicleManager>.instance;
            ushort num = data.m_guestVehicles;
            int num2 = 0;
            while (num != 0)
            {
                if ((TransferManager.TransferReason)instance.m_vehicles.m_buffer[(int)num].m_transferType == material)
                {
                    VehicleInfo info = instance.m_vehicles.m_buffer[(int)num].Info;
                    int a;
                    int num3;
                    info.m_vehicleAI.GetSize(num, ref instance.m_vehicles.m_buffer[(int)num], out a, out num3);
                    cargo += Mathf.Min(a, num3);
                    capacity += num3;
                    count++;
                    if ((instance.m_vehicles.m_buffer[(int)num].m_flags & (Vehicle.Flags.Importing | Vehicle.Flags.Exporting)) != (Vehicle.Flags)0)
                    {
                        outside++;
                    }
                }
                num = instance.m_vehicles.m_buffer[(int)num].m_nextGuestVehicle;
                if (++num2 > Singleton<VehicleManager>.instance.m_vehicles.m_size)
                {
                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                    break;
                }
            }
        }

        public static void AddDeadOffers(ushort buildingID, ref Building data)
        {
            TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);
            if (CustomPlayerBuildingAI.haveCemetryBuilding && MoreOutsideInteraction.deadFromOutside && Singleton<UnlockManager>.instance.Unlocked(UnlockManager.Feature.DeathCare))
            {
                int car_valid_path = TickPathfindStatus(ref data.m_teens, ref data.m_serviceProblemTimer);
                SimulationManager instance1 = Singleton<SimulationManager>.instance;
                if (car_valid_path + instance1.m_randomizer.Int32(256u) >> 8 == 0)
                {
                    if (instance1.m_randomizer.Int32(16u) == 0)
                    {
                        if (data.m_customBuffer1 > 10)
                        {
                            offer = default(TransferManager.TransferOffer);
                            offer.Priority = 2;
                            offer.Building = buildingID;
                            offer.Position = data.m_position;
                            offer.Amount = 1;
                            offer.Active = true;
                            Singleton<TransferManager>.instance.AddOutgoingOffer(TransferManager.TransferReason.DeadMove, offer);
                        }
                    }
                }
                else
                {
                    if (data.m_customBuffer1 > 10)
                    {
                        offer = default(TransferManager.TransferOffer);
                        offer.Priority = 2;
                        offer.Building = buildingID;
                        offer.Position = data.m_position;
                        offer.Amount = 1;
                        offer.Active = true;
                        Singleton<TransferManager>.instance.AddOutgoingOffer(TransferManager.TransferReason.DeadMove, offer);
                    }
                }
            }
        }

        public static void AddPoliceOffers(ushort buildingID, ref Building data)
        {
            TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);
            if (CustomPlayerBuildingAI.havePoliceBuilding && MoreOutsideInteraction.crimeToOutside && Singleton<UnlockManager>.instance.Unlocked(ItemClass.Service.PoliceDepartment))
            {
                int num25 = 0;
                int num26 = 0;
                int num27 = 0;
                int num28 = 0;
                CalculateGuestVehicles(buildingID, ref data, TransferManager.TransferReason.Crime, ref num25, ref num26, ref num27, ref num28);
                int car_valid_path = TickPathfindStatus(ref data.m_education3, ref data.m_adults);
                SimulationManager instance1 = Singleton<SimulationManager>.instance;
                if (car_valid_path + instance1.m_randomizer.Int32(256u) >> 8 == 0)
                {
                    if (instance1.m_randomizer.Int32(16u) == 0)
                    {
                        if ((data.m_crimeBuffer - (num27 - num26) * 100) > 200)
                        {
                            offer = default(TransferManager.TransferOffer);
                            offer.Priority = 2;
                            offer.Building = buildingID;
                            offer.Position = data.m_position;
                            offer.Amount = 1;
                            offer.Active = false;
                            Singleton<TransferManager>.instance.AddOutgoingOffer(TransferManager.TransferReason.Crime, offer);
                        }
                    }
                }
                else
                {
                    if ((data.m_crimeBuffer - (num27 - num26) * 100) > 200)
                    {
                        offer = default(TransferManager.TransferOffer);
                        offer.Priority = 2;
                        offer.Building = buildingID;
                        offer.Position = data.m_position;
                        offer.Amount = 1;
                        offer.Active = false;
                        Singleton<TransferManager>.instance.AddOutgoingOffer(TransferManager.TransferReason.Crime, offer);
                    }
                }
            }
        }

        public static void AddFireOffers(ushort buildingID, ref Building data)
        {
            TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);
            if (CustomPlayerBuildingAI.haveFireBuilding && MoreOutsideInteraction.fireToOutside && Singleton<UnlockManager>.instance.Unlocked(ItemClass.Service.FireDepartment))
            {
                int num25 = 0;
                int num26 = 0;
                int num27 = 0;
                int num28 = 0;
                CalculateGuestVehicles(buildingID, ref data, TransferManager.TransferReason.Fire, ref num25, ref num26, ref num27, ref num28);
                int car_valid_path = TickPathfindStatus(ref data.m_education3, ref data.m_adults);
                SimulationManager instance1 = Singleton<SimulationManager>.instance;
                if (car_valid_path + instance1.m_randomizer.Int32(256u) >> 8 == 0)
                {
                    if (instance1.m_randomizer.Int32(16u) == 0)
                    {
                        if ((data.m_waterBuffer - (num27 - num26) * 100) > 200)
                        {
                            offer = default(TransferManager.TransferOffer);
                            offer.Priority = 2;
                            offer.Building = buildingID;
                            offer.Position = data.m_position;
                            offer.Amount = 1;
                            offer.Active = false;
                            Singleton<TransferManager>.instance.AddOutgoingOffer(TransferManager.TransferReason.Fire, offer);
                        }
                    }
                }
                else
                {
                    if ((data.m_waterBuffer - (num27 - num26) * 100) > 200)
                    {
                        offer = default(TransferManager.TransferOffer);
                        offer.Priority = instance1.m_randomizer.Int32(3);
                        offer.Building = buildingID;
                        offer.Position = data.m_position;
                        offer.Amount = 1;
                        offer.Active = false;
                        Singleton<TransferManager>.instance.AddOutgoingOffer(TransferManager.TransferReason.Fire, offer);
                    }
                }
            }
        }

        public static void AddSickOffers(ushort buildingID, ref Building data)
        {
            TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);
            if (CustomPlayerBuildingAI.haveHospitalBuilding && MoreOutsideInteraction.sickToOutside && Singleton<UnlockManager>.instance.Unlocked(ItemClass.Service.HealthCare))
            {
                int num25 = 0;
                int num26 = 0;
                int num27 = 0;
                int num28 = 0;
                CalculateGuestVehicles(buildingID, ref data, TransferManager.TransferReason.Sick, ref num25, ref num26, ref num27, ref num28);
                int car_valid_path = TickPathfindStatus(ref data.m_education3, ref data.m_adults);
                SimulationManager instance1 = Singleton<SimulationManager>.instance;
                if (car_valid_path + instance1.m_randomizer.Int32(256u) >> 8 == 0)
                {
                    if (instance1.m_randomizer.Int32(128u) == 0)
                    {
                        if ((data.m_customBuffer2 - (num27 - num26) * 100) > 200)
                        {
                            offer = default(TransferManager.TransferOffer);
                            offer.Priority = 2;
                            offer.Building = buildingID;
                            offer.Position = data.m_position;
                            offer.Amount = 1;
                            offer.Active = false;
                            Singleton<TransferManager>.instance.AddOutgoingOffer(TransferManager.TransferReason.Sick, offer);
                        }
                    }
                }
                else
                {
                    if ((data.m_customBuffer2 - (num27 - num26) * 100) > 200)
                    {
                        offer = default(TransferManager.TransferOffer);
                        offer.Priority = instance1.m_randomizer.Int32(3);
                        offer.Building = buildingID;
                        offer.Position = data.m_position;
                        offer.Amount = 1;
                        offer.Active = false;
                        Singleton<TransferManager>.instance.AddOutgoingOffer(TransferManager.TransferReason.Sick, offer);
                    }
                }
            }
        }

        public static void AddOffers(ushort buildingID, ref Building data)
        {
            //gabarge
            if (data.Info.m_class.m_service == ItemClass.Service.Road)
            {
                AddGarbageOffers(buildingID, ref data);
                if ((data.m_flags & Building.Flags.IncomingOutgoing) == Building.Flags.Incoming)
                {
                    AddPoliceOffers(buildingID, ref data);
                    AddFireOffers(buildingID, ref data);
                    AddSickOffers(buildingID, ref data);
                }
                else if ((data.m_flags & Building.Flags.IncomingOutgoing) == Building.Flags.IncomingOutgoing)
                {
                    AddPoliceOffers(buildingID, ref data);
                    AddFireOffers(buildingID, ref data);
                    AddSickOffers(buildingID, ref data);
                    AddDeadOffers(buildingID, ref data);
                }
                else
                {
                    AddDeadOffers(buildingID, ref data);
                }
            }
        }

        public static bool Can16timesUpdate(ushort ID)
        {
            uint currentFrameIndex = Singleton<SimulationManager>.instance.m_currentFrameIndex;
            int frameIndex = (int)(currentFrameIndex & 4095u);
            if (((frameIndex >> 8) & 15u) == (ID & 15u))
            {
                return true;
            }
            return false;
        }
    }
}
