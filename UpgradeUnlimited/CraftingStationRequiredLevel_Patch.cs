using HarmonyLib;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Reflection;

namespace Cozyheim.UpgradeUnlimited
{
    internal class CraftingStationRequiredLevel_Patch
    {
        [HarmonyPatch]
        private class Patch
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(Recipe), "GetRequiredStationLevel")]
            private static bool Recipe_GetRequiredStationLevel_Prefix(ref int __result)
            {
                __result = 1;
                return false;
            }
        }
    }
}
