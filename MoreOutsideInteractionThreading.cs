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
        public const int HarmonyPatchNum = 16;

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
                    string error = "MoreOutsideInteraction detected an incompatibility with another mod! You can continue playing but it's NOT recommended. MoreOutsideInteraction will not work as expected. See MoreOutsideInteraction.txt for technical details.";
                    DebugLog.LogToFileOnly(error);
                    string text = "The following methods were overriden by another mod:";
                    foreach (string current2 in list)
                    {
                        text += string.Format("\n\t{0}", current2);
                    }
                    DebugLog.LogToFileOnly(text);
                    UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage("Incompatibility Issue", text, true);
                }

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

        //TODO, use harmony to add this function to outsideconnectionAI.
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
