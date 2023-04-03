using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace Cozyheim.CustomItems
{
    internal class RemoveVanillaItems_Patch
    {
        private static bool recipesDisabled = false;
        private static List<PieceTable> pieceTablesDisabled = new List<PieceTable>();

        private static List<string> disabledVanillaRecipes;
        private static List<string> disabledVanillaPieces;

        [HarmonyPatch]
        private class PatchClass
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(CraftingStation), "Interact")]
            static void CraftingStation_Interact_Prefix()
            {
                if (ConfigSettings.modEnabled.Value && !recipesDisabled)
                {
                    if(disabledVanillaRecipes == null)
                    {
                        disabledVanillaRecipes = AddRemoveItemsRecipesConfig.removedRecipes;
                    }

                    int disableCount = 0;
                    List<Recipe> allRecipes = ObjectDB.instance.m_recipes;

                    ConsoleLog.Print("Updating Recipes List (" + allRecipes.Count + " checked):", LogType.Message);

                    foreach (Recipe recipe in allRecipes)
                    {
                        if (disabledVanillaRecipes.Contains(recipe.name))
                        {
                            disableCount++;
                            recipe.m_enabled = false;
                            ConsoleLog.Print(recipe.name + " -> Disabled", LogType.Error);
                        }
                    }

                    ConsoleLog.Print("* " + disableCount + " was disabled *", LogType.Warning);
                        
                    recipesDisabled = true;
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(PieceTable), "UpdateAvailable")]
            static void PieceTable_UpdateAvailable_Prefix(ref List<GameObject> ___m_pieces, PieceTable __instance)
            {
                if (ConfigSettings.modEnabled.Value && !pieceTablesDisabled.Contains(__instance))
                {
                    if (disabledVanillaPieces == null)
                    {
                        disabledVanillaPieces = AddRemoveItemsRecipesConfig.removedItems;
                    }

                    int disableCount = 0;

                    ConsoleLog.Print("Updating " + __instance.name + " (" + ___m_pieces.Count + " checked):", LogType.Message);

                    foreach (GameObject item in ___m_pieces)
                    {
                        if (disabledVanillaPieces.Contains(item.name))
                        {
                            Piece itemPiece = item.GetComponent<Piece>();
                            if (itemPiece != null)
                            {
                                itemPiece.m_enabled = false;
                                disableCount++;
                                ConsoleLog.Print(item.name + " -> Disabled", LogType.Error);
                            }
                        }
                    }

                    ConsoleLog.Print("* " + disableCount + " was disabled *", LogType.Warning);

                    pieceTablesDisabled.Add(__instance);
                }
            }
        }
    }
}
