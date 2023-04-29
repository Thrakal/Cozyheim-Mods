using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace Cozyheim.UpgradeUnlimited {
    internal class Recipe_Patch {

        private static Dictionary<string, int> craftingStationList = new Dictionary<string, int>();

        // Generate a Dictionary with custom crafting station name and level limit, with the format -> craftingStationName:levelLimit
        public static void CreateCustomStationsList() {
            string[] customStationArray = ConfigSettings.customCraftingStationUpgradeLevelLimit.Value.Split(',');

            foreach(string station in customStationArray) {
                string[] stationSplit = station.Split(':');

                if(stationSplit.Length != 2) {
                    ConsoleLog.Print($"Invalid customCraftingStationUpgradeLevelLimit entry: {station}", LogType.Warning);
                    continue;
                }

                string stationName = stationSplit[0];
                int stationLevelLimit = int.Parse(stationSplit[1]);

                if(craftingStationList.ContainsKey(stationName)) {
                    ConsoleLog.Print($"Duplicate customCraftingStationUpgradeLevelLimit entry: {station}", LogType.Warning);
                    continue;
                }

                ConsoleLog.Print($"Added customCraftingStationUpgradeLevelLimit entry: {stationName}: {stationLevelLimit}");

                AddStationToList(stationName, stationLevelLimit);
            }
        }

        public static void AddStationToList(string stationName, int stationLevelLimit) {
            craftingStationList.Add(stationName, stationLevelLimit);
        }

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
                name = name.Replace("(Clone)", "");

                if(craftingStationList.ContainsKey(name)) {
                    __result = Mathf.Min(__result, craftingStationList[name]);
                    return;
                } else {
                    __result = Mathf.Min(__result, ConfigSettings.unknownCraftingStationLevelLimit.Value);
                }
            }
            
        }
    }
}