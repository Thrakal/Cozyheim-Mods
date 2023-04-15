using HarmonyLib;
using UnityEngine;

namespace Cozyheim.UpgradeUnlimited {
    internal class Recipe_Patch {

        [HarmonyPatch]
        private class Patch {

            [HarmonyAfter(new string[] { "randyknapp.mods.auga" })]
            [HarmonyPostfix]
            [HarmonyPatch(typeof(Recipe), "GetRequiredStationLevel")]
            private static void Recipe_GetRequiredStationLevel_Prefix(Recipe __instance, ref int __result, CraftingStation ___m_craftingStation) {
                if(___m_craftingStation == null) {
                    __result = Mathf.Min(__result, Mathf.Min(ConfigSettings.forgeUpgradeLevelLimit.Value, ConfigSettings.workbenchUpgradeLevelLimit.Value));
                    return;
                }

                string name = ___m_craftingStation.name;

                if(name.StartsWith("piece_workbench") || name.StartsWith("CraftingPole")) {
                    __result = Mathf.Min(__result, ConfigSettings.workbenchUpgradeLevelLimit.Value);
                } else if(name.StartsWith("forge")) {
                    __result = Mathf.Min(__result, ConfigSettings.forgeUpgradeLevelLimit.Value);
                } else {
                    __result = Mathf.Min(__result, ConfigSettings.otherCraftingStationLevelLimit.Value);
                }

                return;
            }
            
        }
    }
}
