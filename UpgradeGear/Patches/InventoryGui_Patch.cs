using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace Cozyheim.UpgradeUnlimited {

    internal class InventoryGui_Patch {

        [HarmonyPatch]
        private class Patch {
            [HarmonyBefore(new string[] { "randyknapp.mods.auga" })]
            [HarmonyPrefix]
            [HarmonyPatch(typeof(InventoryGui), "DoCrafting")]
            private static bool InventoryGui_DoCrafting_Prefix(ref Recipe ___m_craftRecipe) {
                if(___m_craftRecipe == null) {
                    return true;
                }

                if(Main.IsItemAllowed(___m_craftRecipe.m_item)) {
                    ___m_craftRecipe.m_item.m_itemData.m_shared.m_maxQuality = Mathf.Max(___m_craftRecipe.m_item.m_itemData.m_shared.m_maxQuality, ConfigSettings.maxItemLevel.Value);
                }

                return true;
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(InventoryGui), "SetRecipe")]
            private static void InventoryGui_SetRecipe_Postfix(ref KeyValuePair<Recipe, ItemDrop.ItemData> ___m_selectedRecipe) {
                if(___m_selectedRecipe.Key == null) {
                    return;
                }

                if(___m_selectedRecipe.Key.m_item.m_itemData.m_shared.m_maxQuality <= 1) {
                    return;
                }

                ___m_selectedRecipe.Key.m_item.m_itemData.m_shared.m_maxQuality = ConfigSettings.maxItemLevel.Value;
            }


            [HarmonyPrefix]
            [HarmonyPatch(typeof(InventoryGui), "SetupUpgradeItem")]
            private static void InventoryGui_SetupUpgradeItem_Prefix(ref Recipe recipe, ref ItemDrop.ItemData item) {
                if(item == null) {
                    recipe.m_item.m_itemData.m_shared.m_maxQuality = ConfigSettings.maxItemLevel.Value;
                } else {
                    item.m_shared.m_maxQuality = ConfigSettings.maxItemLevel.Value;
                }
            }
        }
    }
}