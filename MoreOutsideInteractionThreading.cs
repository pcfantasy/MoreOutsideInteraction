using ColossalFramework;
using ColossalFramework.UI;
using Harmony;
using ICities;
using MoreOutsideInteraction.CustomAI;
using MoreOutsideInteraction.Util;
using System.Collections.Generic;

namespace MoreOutsideInteraction
{
    public class MoreOutsideInteractionThreading : ThreadingExtensionBase
    {
        public static bool isFirstTime = true;
        public const int HarmonyPatchNum = 17;

        public override void OnBeforeSimulationFrame()
        {
            base.OnBeforeSimulationFrame();
            if (Loader.CurrentLoadMode == LoadMode.LoadGame || Loader.CurrentLoadMode == LoadMode.NewGame)
            {
                if (MoreOutsideInteraction.IsEnabled)
                {
                    CheckDetour();
                }
            }
        }

        public void CheckDetour()
        {
            if (isFirstTime && Loader.DetourInited)
            {
                isFirstTime = false;
                DebugLog.LogToFileOnly("ThreadingExtension.OnBeforeSimulationFrame: First frame detected. Checking detours.");

                if (!Loader.HarmonyDetourInited)
                {
                    string error = "MoreOutsideInteraction HarmonyDetourInit is failed, Send MoreOutsideInteraction.txt to Author.";
                    DebugLog.LogToFileOnly(error);
                    UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage("Incompatibility Issue", error, true);
                }
                else
                {
                    var harmony = HarmonyInstance.Create(HarmonyDetours.ID);
                    var methods = harmony.GetPatchedMethods();
                    int i = 0;
                    foreach (var method in methods)
                    {
                        var info = harmony.GetPatchInfo(method);
                        if (info.Owners?.Contains(harmony.Id) == true)
                        {
                            DebugLog.LogToFileOnly("Harmony patch method = " + method.Name.ToString());
                            if (info.Prefixes.Count != 0)
                            {
                                DebugLog.LogToFileOnly("Harmony patch method has PreFix");
                            }
                            if (info.Postfixes.Count != 0)
                            {
                                DebugLog.LogToFileOnly("Harmony patch method has PostFix");
                            }
                            i++;
                        }
                    }

                    if (i != HarmonyPatchNum)
                    {
                        string error = $"MoreOutsideInteraction HarmonyDetour Patch Num is {i}, Right Num is {HarmonyPatchNum} Send MoreOutsideInteraction.txt to Author.";
                        DebugLog.LogToFileOnly(error);
                        UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage("Incompatibility Issue", error, true);
                    }
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

                    if (num4 == 255)
                    {
                        CustomPlayerBuildingAI.haveCemetryBuilding = CustomPlayerBuildingAI.haveCemetryBuildingTemp;
                        CustomPlayerBuildingAI.haveFireBuilding = CustomPlayerBuildingAI.haveFireBuildingTemp;
                        CustomPlayerBuildingAI.havePoliceBuilding = CustomPlayerBuildingAI.havePoliceBuildingTemp;
                        CustomPlayerBuildingAI.haveHospitalBuilding = CustomPlayerBuildingAI.haveHospitalBuildingTemp;
                        CustomPlayerBuildingAI.haveGarbageBuilding = CustomPlayerBuildingAI.haveGarbageBuildingTemp;
                        CustomPlayerBuildingAI.haveCemetryBuildingTemp = false;
                        CustomPlayerBuildingAI.haveFireBuildingTemp = false;
                        CustomPlayerBuildingAI.havePoliceBuildingTemp = false;
                        CustomPlayerBuildingAI.haveHospitalBuildingTemp = false;
                        CustomPlayerBuildingAI.haveGarbageBuildingTemp = false;
                    }
                }
            }
        }
    }
}
