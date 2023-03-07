using BepInEx.Configuration;
using CozyheimTweaks.Resources;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CozyheimTweaks
{
    [HarmonyPatch]
    internal class RemoveVanillaItems
    {
        internal static ConfigFile patchConfig;

        internal static ConfigEntry<bool> enable;
        internal static ConfigEntry<bool> debugMode;

        internal const string patchName = "RemoveVanillaItems";

        private static bool recipesDisabled = false;
        private static List<PieceTable> pieceTablesDisabled = new List<PieceTable>();

        private static List<string> disabledVanillaRecipes;
        private static List<string> disabledVanillaPieces;

        internal static void Init()
        {
            patchConfig = new ConfigFile(BepInEx.Paths.ConfigPath + "/Cozyheim/" + patchName + ".cfg", true);
            patchConfig.SaveOnConfigSet = true;

            enable = Main.CreateConfigEntry("General", "Enable this mod", true, "Allows this mod to be used", patchConfig);
            debugMode = Main.CreateConfigEntry("General", "enableDebug", false, "Print debug messages in the console. (mainConfig debugmode must also be enabled)", patchConfig);
        }

        [HarmonyPatch]
        private class PatchClass
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(CraftingStation), "Interact")]
            static void CraftingStation_Interact_Prefix()
            {
                if (enable.Value && !recipesDisabled)
                {
                    if(disabledVanillaRecipes == null)
                    {
                        disabledVanillaRecipes = AddRemoveItemsRecipesConfig.removedRecipes;
                    }

                    int disableCount = 0;
                    List<Recipe> allRecipes = ObjectDB.instance.m_recipes;

                    ToppLog.Print("Updating Recipes List (" + allRecipes.Count + " checked):", LogType.Message, debugMode.Value);

                    foreach (Recipe recipe in allRecipes)
                    {
                        if (disabledVanillaRecipes.Contains(recipe.name))
                        {
                            disableCount++;
                            recipe.m_enabled = false;
                            ToppLog.Print(recipe.name + " -> Disabled", LogType.Error, debugMode.Value);
                        }
                    }
                        
                    ToppLog.Print("* " + disableCount + " was disabled *", LogType.Warning, debugMode.Value);
                        
                    recipesDisabled = true;
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(PieceTable), "UpdateAvailable")]
            static void PieceTable_UpdateAvailable_Prefix(ref List<GameObject> ___m_pieces, PieceTable __instance)
            {
                if (enable.Value && !pieceTablesDisabled.Contains(__instance))
                {
                    if (disabledVanillaPieces == null)
                    {
                        disabledVanillaPieces = AddRemoveItemsRecipesConfig.removedItems;
                    }

                    int disableCount = 0;

                    ToppLog.Print("Updating " + __instance.name + " (" + ___m_pieces.Count + " checked):", LogType.Message, debugMode.Value);

                    foreach (GameObject item in ___m_pieces)
                    {
                        if (disabledVanillaPieces.Contains(item.name))
                        {
                            Piece itemPiece = item.GetComponent<Piece>();
                            if (itemPiece != null)
                            {
                                itemPiece.m_enabled = false;
                                disableCount++;
                                ToppLog.Print(item.name + " -> Disabled", LogType.Error, debugMode.Value);
                            }
                        }
                    }

                    ToppLog.Print("* " + disableCount + " was disabled *", LogType.Warning, debugMode.Value);

                    pieceTablesDisabled.Add(__instance);
                }
            }
        }
    }
}
