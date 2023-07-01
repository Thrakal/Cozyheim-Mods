using HarmonyLib;

namespace Cozyheim.BuildMore
{
    internal class IsInsideNoBuildLocation_Patch
    {
        [HarmonyPatch]
        private class Patch
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(Location), "IsInsideNoBuildLocation")]
            static bool Location_IsInsideNoBuildLocation_Prefix(ref bool __result)
            {
                if (ConfigSettings.buildInsideNoBuild.Value)
                {
                    __result = false;
                    return false;
                }
                return true;
            }
        }
    }
}
