using Harmony;

namespace MoreOutsideInteraction.Util
{
    internal static class HarmonyDetours
    {
        public const string ID = "pcfantasy.moreoutsideinteraction";
        public static void Apply()
        {
            var harmony = new Harmony.Harmony(ID);
            harmony.PatchAll(typeof(HarmonyDetours).Assembly);
            Loader.HarmonyDetourFailed = false;
            DebugLog.LogToFileOnly("Harmony patches applied");
        }

        public static void DeApply()
        {
            var harmony = new Harmony.Harmony(ID);
            harmony.UnpatchAll(ID);
            DebugLog.LogToFileOnly("Harmony patches DeApplied");
        }
    }
}
