using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MoreOutsideInteraction
{
    public class MoreOutsideInteractionThreading : ThreadingExtensionBase
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
        public static bool isFirstTime = true;

        public override void OnBeforeSimulationFrame()
        {
            base.OnBeforeSimulationFrame();
            if (Loader.CurrentLoadMode == LoadMode.LoadGame || Loader.CurrentLoadMode == LoadMode.NewGame)
            {
                if (MoreOutsideInteraction.IsEnabled)
                {
                    //Check language
                    CheckLanguage();
                    CheckDetour();
                }
            }
        }

        public static void CheckLanguage()
        {
            if (SingletonLite<LocaleManager>.instance.language.Contains("zh") && (MoreOutsideInteraction.languageIdex == 1))
            {
            }
            else if (!SingletonLite<LocaleManager>.instance.language.Contains("zh") && (MoreOutsideInteraction.languageIdex != 1))
            {
            }
            else
            {
                MoreOutsideInteraction.languageIdex = (byte)(SingletonLite<LocaleManager>.instance.language.Contains("zh") ? 1 : 0);
                Language.LanguageSwitch((byte)MoreOutsideInteraction.languageIdex);
            }
        }

        public void CheckDetour()
        {
            if (isFirstTime && Loader.DetourInited)
            {
                isFirstTime = false;
                DebugLog.LogToFileOnly("ThreadingExtension.OnBeforeSimulationFrame: First frame detected. Checking detours.");
                List<string> list = new List<string>();
                foreach (Loader.Detour current in Loader.Detours)
                {
                    if (!RedirectionHelper.IsRedirected(current.OriginalMethod, current.CustomMethod))
                    {
                        list.Add(string.Format("{0}.{1} with {2} parameters ({3})", new object[]
                        {
                    current.OriginalMethod.DeclaringType.Name,
                    current.OriginalMethod.Name,
                    current.OriginalMethod.GetParameters().Length,
                    current.OriginalMethod.DeclaringType.AssemblyQualifiedName
                        }));
                    }
                }
                DebugLog.LogToFileOnly(string.Format("ThreadingExtension.OnBeforeSimulationFrame: First frame detected. Detours checked. Result: {0} missing detours", list.Count));
                if (list.Count > 0)
                {
                    string error = "More Outside Interaction detected an incompatibility with another mod! You can continue playing but it's NOT recommended. RealCity will not work as expected. See RealCity.log for technical details.";
                    DebugLog.LogToFileOnly(error);
                    string text = "The following methods were overriden by another mod:";
                    foreach (string current2 in list)
                    {
                        text += string.Format("\n\t{0}", current2);
                    }
                    DebugLog.LogToFileOnly(text);
                    UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage("Incompatibility Issue", text, true);
                }
            }
        }

        public override void OnAfterSimulationFrame()
        {
            base.OnAfterSimulationFrame();
            if (Loader.CurrentLoadMode == LoadMode.LoadGame || Loader.CurrentLoadMode == LoadMode.NewGame)
            {
                if (MoreOutsideInteraction.IsEnabled)
                {
                    uint currentFrameIndex = Singleton<SimulationManager>.instance.m_currentFrameIndex;
                    int num4 = (int)(currentFrameIndex & 255u);
                    int num5 = num4 * 192;
                    int num6 = (num4 + 1) * 192 - 1;
                    BuildingManager instance = Singleton<BuildingManager>.instance;

                    for (int i = num5; i <= num6; i = i + 1)
                    {
                        if (instance.m_buildings.m_buffer[i].m_flags.IsFlagSet(Building.Flags.Created) && (!instance.m_buildings.m_buffer[i].m_flags.IsFlagSet(Building.Flags.Deleted)))
                        {
                            if (instance.m_buildings.m_buffer[i].m_flags.IsFlagSet(Building.Flags.Untouchable))
                            {
                                if (instance.m_buildings.m_buffer[i].Info.m_buildingAI is OutsideConnectionAI)
                                {
                                    if (instance.m_buildings.m_buffer[i].Info.m_class.m_service == ItemClass.Service.Road)
                                    {
                                        ProcessOutsideDemand((ushort)i, ref instance.m_buildings.m_buffer[i]);
                                        AddOffers((ushort)i, ref instance.m_buildings.m_buffer[i]);
                                    }
                                }
                            }
                            else
                            {
                                if ((instance.m_buildings.m_buffer[i].Info.m_class.m_service == ItemClass.Service.HealthCare) && (instance.m_buildings.m_buffer[i].Info.m_class.m_level == ItemClass.Level.Level2))
                                {
                                    haveCemetryBuildingTemp = true;
                                }
                                else if (instance.m_buildings.m_buffer[i].Info.m_class.m_service == ItemClass.Service.Garbage)
                                {
                                    haveGarbageBuildingTemp = true;
                                }
                                else if (instance.m_buildings.m_buffer[i].Info.m_class.m_service == ItemClass.Service.PoliceDepartment)
                                {
                                    havePoliceBuildingTemp = true;
                                }
                                else if ((instance.m_buildings.m_buffer[i].Info.m_class.m_service == ItemClass.Service.HealthCare) && (instance.m_buildings.m_buffer[i].Info.m_class.m_level == ItemClass.Level.Level1))
                                {
                                    haveHospitalBuildingTemp = true;
                                }
                                else if (instance.m_buildings.m_buffer[i].Info.m_class.m_service == ItemClass.Service.FireDepartment)
                                {
                                    haveFireBuildingTemp = true;
                                }
                            }
                        }
                    }

                    if (num4 == 255)
                    {
                        haveCemetryBuilding = haveCemetryBuildingTemp;
                        haveFireBuilding = haveFireBuildingTemp;
                        havePoliceBuilding = havePoliceBuildingTemp;
                        haveHospitalBuilding = haveHospitalBuildingTemp;
                        haveGarbageBuilding = haveGarbageBuildingTemp;
                        haveCemetryBuildingTemp = false;
                        haveFireBuildingTemp = false;
                        havePoliceBuildingTemp = false;
                        haveHospitalBuildingTemp = false;
                        haveGarbageBuildingTemp = false;
                    }
                }
            }
        }

        public void ProcessOutsideDemand(ushort buildingID, ref Building data)
        {
            //Gabarge
            if (haveGarbageBuilding && (MoreOutsideInteraction.garbageFromOutside || MoreOutsideInteraction.garbageToOutside) && Singleton<UnlockManager>.instance.Unlocked(ItemClass.Service.Garbage))
            {
                if ((data.m_flags & Building.Flags.IncomingOutgoing) == Building.Flags.Incoming)
                {
                    data.m_garbageBuffer = (ushort)(data.m_garbageBuffer + 40);
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

            if (havePoliceBuilding && MoreOutsideInteraction.crimeToOutside && Singleton<UnlockManager>.instance.Unlocked(ItemClass.Service.PoliceDepartment))
            {
                if ((data.m_flags & Building.Flags.IncomingOutgoing) == Building.Flags.Incoming)
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
            if (haveHospitalBuilding && MoreOutsideInteraction.sickToOutside && Singleton<UnlockManager>.instance.Unlocked(ItemClass.Service.HealthCare))
            {
                if ((data.m_flags & Building.Flags.IncomingOutgoing) == Building.Flags.Incoming)
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
            if (haveFireBuilding && MoreOutsideInteraction.fireToOutside && Singleton<UnlockManager>.instance.Unlocked(ItemClass.Service.FireDepartment))
            {
                if ((data.m_flags & Building.Flags.IncomingOutgoing) == Building.Flags.Incoming)
                {
                    data.m_electricityBuffer = (ushort)(data.m_electricityBuffer + 1);
                }
                data.m_fireIntensity = 250;
            }
            else if (data.m_electricityBuffer != 0)
            {
                data.m_electricityBuffer = 0;
                data.m_fireIntensity = 0;
                TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);
                offer.Building = buildingID;
                Singleton<TransferManager>.instance.RemoveOutgoingOffer(TransferManager.TransferReason.Fire, offer);
            }
            //deadbuffer
            if (haveCemetryBuilding && MoreOutsideInteraction.deadFromOutside && Singleton<UnlockManager>.instance.Unlocked(UnlockManager.Feature.DeathCare))
            {
                if ((data.m_flags & Building.Flags.IncomingOutgoing) == Building.Flags.Outgoing)
                {
                    if (Singleton<SimulationManager>.instance.m_randomizer.Int32(64u) == 0)
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

            if (data.m_electricityBuffer > 65000)
            {
                data.m_electricityBuffer = 65000;
            }
        }

        public void AddGarbageOffers(ushort buildingID, ref Building data)
        {
            TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);

            if (haveGarbageBuilding && Singleton<UnlockManager>.instance.Unlocked(ItemClass.Service.Garbage) && (MoreOutsideInteraction.garbageFromOutside || MoreOutsideInteraction.garbageToOutside))
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
                                    this.CalculateGuestVehicles(buildingID, ref data, TransferManager.TransferReason.Garbage, ref num25, ref num26, ref num27, ref num28);
                                    num24 -= num27 - num26;
                                    if (num24 >= 200)
                                    {
                                        offer = default(TransferManager.TransferOffer);
                                        offer.Priority = instance1.m_randomizer.Int32(3);
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
                                    offer.Priority = instance1.m_randomizer.Int32(3);
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
                                offer.Priority = instance1.m_randomizer.Int32(3);
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
                            offer.Priority = instance1.m_randomizer.Int32(3);
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

        private static int TickPathfindStatus(ref byte success, ref byte failure)
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

        protected void CalculateGuestVehicles(ushort buildingID, ref Building data, TransferManager.TransferReason material, ref int count, ref int cargo, ref int capacity, ref int outside)
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
                if (++num2 > 16384)
                {
                    CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                    break;
                }
            }
        }

        public void AddDeadOffers(ushort buildingID, ref Building data)
        {
            TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);
            if (haveCemetryBuilding && MoreOutsideInteraction.deadFromOutside && Singleton<UnlockManager>.instance.Unlocked(UnlockManager.Feature.DeathCare))
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
                            offer.Priority = instance1.m_randomizer.Int32(3);
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
                        offer.Priority = instance1.m_randomizer.Int32(3);
                        offer.Building = buildingID;
                        offer.Position = data.m_position;
                        offer.Amount = 1;
                        offer.Active = true;
                        Singleton<TransferManager>.instance.AddOutgoingOffer(TransferManager.TransferReason.DeadMove, offer);
                    }
                }
            }
        }

        public void AddPoliceOffers(ushort buildingID, ref Building data)
        {
            TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);
            if (havePoliceBuilding && MoreOutsideInteraction.crimeToOutside && Singleton<UnlockManager>.instance.Unlocked(ItemClass.Service.PoliceDepartment))
            {
                int num25 = 0;
                int num26 = 0;
                int num27 = 0;
                int num28 = 0;
                this.CalculateGuestVehicles(buildingID, ref data, TransferManager.TransferReason.Crime, ref num25, ref num26, ref num27, ref num28);
                int car_valid_path = TickPathfindStatus(ref data.m_education3, ref data.m_adults);
                SimulationManager instance1 = Singleton<SimulationManager>.instance;
                if (car_valid_path + instance1.m_randomizer.Int32(256u) >> 8 == 0)
                {
                    if (instance1.m_randomizer.Int32(16u) == 0)
                    {
                        if ((data.m_crimeBuffer - (num27 - num26) * 100) > 200)
                        {
                            offer = default(TransferManager.TransferOffer);
                            offer.Priority = instance1.m_randomizer.Int32(3);
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
                        offer.Priority = instance1.m_randomizer.Int32(3);
                        offer.Building = buildingID;
                        offer.Position = data.m_position;
                        offer.Amount = 1;
                        offer.Active = false;
                        Singleton<TransferManager>.instance.AddOutgoingOffer(TransferManager.TransferReason.Crime, offer);
                    }
                }
            }
        }

        public void AddFireOffers(ushort buildingID, ref Building data)
        {
            TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);
            if (haveFireBuilding && MoreOutsideInteraction.fireToOutside && Singleton<UnlockManager>.instance.Unlocked(ItemClass.Service.FireDepartment))
            {
                int num25 = 0;
                int num26 = 0;
                int num27 = 0;
                int num28 = 0;
                this.CalculateGuestVehicles(buildingID, ref data, TransferManager.TransferReason.Fire, ref num25, ref num26, ref num27, ref num28);
                int car_valid_path = TickPathfindStatus(ref data.m_education3, ref data.m_adults);
                SimulationManager instance1 = Singleton<SimulationManager>.instance;
                if (car_valid_path + instance1.m_randomizer.Int32(256u) >> 8 == 0)
                {
                    if (instance1.m_randomizer.Int32(16u) == 0)
                    {
                        if ((data.m_electricityBuffer - (num27 - num26) * 100) > 200)
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
                else
                {
                    if ((data.m_electricityBuffer - (num27 - num26) * 100) > 200)
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

        public void AddSickOffers(ushort buildingID, ref Building data)
        {
            TransferManager.TransferOffer offer = default(TransferManager.TransferOffer);
            if (haveHospitalBuilding && MoreOutsideInteraction.sickToOutside && Singleton<UnlockManager>.instance.Unlocked(ItemClass.Service.HealthCare))
            {
                int num25 = 0;
                int num26 = 0;
                int num27 = 0;
                int num28 = 0;
                this.CalculateGuestVehicles(buildingID, ref data, TransferManager.TransferReason.Sick, ref num25, ref num26, ref num27, ref num28);
                int car_valid_path = TickPathfindStatus(ref data.m_education3, ref data.m_adults);
                SimulationManager instance1 = Singleton<SimulationManager>.instance;
                if (car_valid_path + instance1.m_randomizer.Int32(256u) >> 8 == 0)
                {
                    if (instance1.m_randomizer.Int32(128u) == 0)
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

        public void AddOffers(ushort buildingID, ref Building data)
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
                else
                {
                    AddDeadOffers(buildingID, ref data);
                }
            }
        }

    }
}
