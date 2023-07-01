using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace Cozyheim.UpgradeUnlimited {
    internal class ItemDrop_Patch {

        private static List<ItemDrop> _itemList = new List<ItemDrop>();

        [HarmonyPatch]
        private class Patch {

            [HarmonyPostfix]
            [HarmonyPatch(typeof(ItemDrop), "Awake")]
            public static void ItemDrop_Awake_Postfix(ref ItemDrop __instance) {
                if(__instance.m_itemData == null) {
                    return;
                }

                if(_itemList.Contains(__instance)) {
                    return;
                }

                _itemList.Add(__instance);

                if(Main.IsItemAllowed(__instance)) {
                    ItemDrop.ItemData.SharedData shared = __instance.m_itemData.m_shared;
                    float value = ConfigSettings.statsIncrease.Value;

                    // Round numbers to 2 decimals
                    shared.m_armorPerLevel = Mathf.Round(Mathf.Max(shared.m_armorPerLevel * value, 1f) * 10f) / 10f;
                    shared.m_blockPowerPerLevel = Mathf.Round(Mathf.Max(shared.m_blockPowerPerLevel * value, 1f) * 10f) / 10f;
                    shared.m_deflectionForcePerLevel = Mathf.Round(Mathf.Max(shared.m_deflectionForcePerLevel * value, 1f) * 10f) / 10f;
                    shared.m_durabilityPerLevel = Mathf.Round(Mathf.Max(shared.m_durabilityPerLevel * value, 1f) * 10f) / 10f;
                    shared.m_damagesPerLevel.Modify(value);
                    shared.m_maxQuality = Mathf.Max(shared.m_maxQuality, ConfigSettings.maxItemLevel.Value);
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(ObjectDB), "Awake")]
            private static void ObjectDB_Awake_Postfix(ref List<GameObject> ___m_items) {
                // Go through all Items in the database and adjust the upgrade stats for weapons and armor

                for(int i = 0; i < ___m_items.Count; i++) {
                    ItemDrop itemDrop = ___m_items[i].GetComponent<ItemDrop>();
                    if(itemDrop == null) {
                        continue;
                    }

                    if(_itemList.Contains(itemDrop)) {
                        continue;
                    }

                    _itemList.Add(itemDrop);

                    if(Main.IsItemAllowed(itemDrop)) {
                        ItemDrop.ItemData.SharedData shared = itemDrop.m_itemData.m_shared;
                        float value = ConfigSettings.statsIncrease.Value;

                        // Round numbers to 2 decimals
                        shared.m_armorPerLevel = Mathf.Round(Mathf.Max(shared.m_armorPerLevel * value, 1f) * 10f) / 10f;
                        shared.m_blockPowerPerLevel = Mathf.Round(Mathf.Max(shared.m_blockPowerPerLevel * value, 1f) * 10f) / 10f;
                        shared.m_deflectionForcePerLevel = Mathf.Round(Mathf.Max(shared.m_deflectionForcePerLevel * value, 1f) * 10f) / 10f;
                        shared.m_durabilityPerLevel = Mathf.Round(Mathf.Max(shared.m_durabilityPerLevel * value, 1f) * 10f) / 10f;
                        shared.m_damagesPerLevel.Modify(value);
                        shared.m_maxQuality = Mathf.Max(shared.m_maxQuality, ConfigSettings.maxItemLevel.Value);
                    }
                }
            }
        }
    }
}
