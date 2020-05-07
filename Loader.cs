using ICities;
using ColossalFramework;
using System.Reflection;
using System.Collections.Generic;
using MoreOutsideInteraction.Util;
using MoreOutsideInteraction.CustomAI;
using CitiesHarmony.API;
using System;

namespace MoreOutsideInteraction
{
    public class Loader : LoadingExtensionBase
    {
        public static LoadMode CurrentLoadMode;
        public static bool DetourInited = false;
        public static bool HarmonyDetourInited = false;
        public static bool HarmonyDetourFailed = true;
        public static bool isRealCityRunning = false;
        public static bool isRealCityV10 = false;

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            Loader.CurrentLoadMode = mode;
            if (MoreOutsideInteraction.IsEnabled)
            {
                if (mode == LoadMode.LoadGame || mode == LoadMode.NewGame)
                {
                    DebugLog.LogToFileOnly("OnLevelLoaded");
                    InitDetour();
                    for (int i = 0; i < 65536; i ++)
                    {
                        CustomPlayerBuildingAI.canReturn[i] = false;
                    }
                    HarmonyInitDetour();
                    MoreOutsideInteraction.LoadSetting();
                    if (mode == LoadMode.NewGame)
                    {
                        DebugLog.LogToFileOnly("New Game");
                    }
                }
            }
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            if (MoreOutsideInteraction.IsEnabled)
            {
                if (CurrentLoadMode == LoadMode.LoadGame || CurrentLoadMode == LoadMode.NewGame)
                {
                    RevertDetour();
                    HarmonyRevertDetour();
                }
            }

            MoreOutsideInteraction.SaveSetting();
        }

        public override void OnReleased()
        {
            base.OnReleased();
        }

        private bool CheckRealCityIsLoaded()
        {
            return this.Check3rdPartyModLoaded("RealCity", false);
        }

        public void HarmonyInitDetour()
        {
            if (HarmonyHelper.IsHarmonyInstalled)
            {
                if (!HarmonyDetourInited)
                {
                    DebugLog.LogToFileOnly("Init harmony detours");
                    HarmonyDetours.Apply();
                    HarmonyDetourInited = true;
                }
            }
        }

        public void HarmonyRevertDetour()
        {
            if (HarmonyHelper.IsHarmonyInstalled)
            {
                if (HarmonyDetourInited)
                {
                    DebugLog.LogToFileOnly("Revert harmony detours");
                    HarmonyDetours.DeApply();
                    HarmonyDetourInited = false;
                    HarmonyDetourFailed = true;
                }
            }
        }

        public void InitDetour()
        {
            isRealCityRunning = CheckRealCityIsLoaded();
            if (isRealCityRunning)
            {
                Version RealCity_Version = Assembly.Load("RealCity").GetName().Version;
                if (RealCity_Version > new Version(10, 0, 0, 0))
                {
                    DebugLog.LogToFileOnly($"Detect RealCity V10, version = {RealCity_Version}");
                    isRealCityV10 = true;
                }
                else
                {
                    isRealCityV10 = false;
                }
            }
            DetourInited = true;
        }

        public void RevertDetour()
        {
            DetourInited = false;
        }

        private bool Check3rdPartyModLoaded(string namespaceStr, bool printAll = false)
        {
            bool thirdPartyModLoaded = false;

            var loadingWrapperLoadingExtensionsField = typeof(LoadingWrapper).GetField("m_LoadingExtensions", BindingFlags.NonPublic | BindingFlags.Instance);
            List<ILoadingExtension> loadingExtensions = (List<ILoadingExtension>)loadingWrapperLoadingExtensionsField.GetValue(Singleton<LoadingManager>.instance.m_LoadingWrapper);

            if (loadingExtensions != null)
            {
                foreach (ILoadingExtension extension in loadingExtensions)
                {
                    if (printAll)
                        DebugLog.LogToFileOnly($"Detected extension: {extension.GetType().Name} in namespace {extension.GetType().Namespace}");
                    if (extension.GetType().Namespace == null)
                        continue;

                    var nsStr = extension.GetType().Namespace.ToString();
                    if (namespaceStr.Equals(nsStr))
                    {
                        DebugLog.LogToFileOnly($"The mod '{namespaceStr}' has been detected.");
                        thirdPartyModLoaded = true;
                        break;
                    }
                }
            }
            else
            {
                DebugLog.LogToFileOnly("Could not get loading extensions");
            }

            return thirdPartyModLoaded;
        }
    }
}

