using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using static ItemDrop.ItemData;

namespace Cozyheim.AdjustUpgradeStats {
    internal class ObjectDB_Awake_Patch {

        private static List<ItemDrop> _itemList = new List<ItemDrop>();

        [HarmonyPatch]
        private class Patch {
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

                    ItemType itemType = itemDrop.m_itemData.m_shared.m_itemType;

                    // Get the original damage values
                    float totalDamageOriginal = itemDrop.m_itemData.m_shared.m_damages.GetTotalDamage();
                    float totalUpgradeOriginal = itemDrop.m_itemData.m_shared.m_damagesPerLevel.GetTotalDamage();

                    Stats stats = null;

                    foreach(Stats stat in ConfigSettings.gearStats) {
                        if(stat.types.Contains(itemType)) {
                            stats = stat;
                            break;
                        }
                    }

                    if(stats == null) {
                        continue;
                    }

                    // Exclude two handed knives
                    if(stats.types.Contains(ItemType.TwoHandedWeapon) && itemDrop.m_itemData.m_shared.m_skillType == Skills.SkillType.Knives) {
                        continue;
                    }

                    // Modify the base damage
                    itemDrop.m_itemData.m_shared.m_damages.Modify(stats.damageMultiplier.Value);

                    // Adjust the damage per level
                    if(stats.upgradeDamageType.Value != UpgradeType.None) {
                        if(stats.upgradeDamageType.Value == UpgradeType.PercentageOfBase) {
                            itemDrop.m_itemData.m_shared.m_damagesPerLevel = itemDrop.m_itemData.m_shared.m_damages.Clone();
                        }
                        itemDrop.m_itemData.m_shared.m_damagesPerLevel.Modify(stats.upgradeDamageValue.Value);
                    }

                    // Get the adjusted damage values
                    float totalDamageAdjusted = itemDrop.m_itemData.m_shared.m_damages.GetTotalDamage();
                    float totalUpgradeAdjusted = itemDrop.m_itemData.m_shared.m_damagesPerLevel.GetTotalDamage();

                    // Display debug message
                    if(totalDamageOriginal != totalDamageAdjusted) {
                        ConsoleLog.Print($"Adjusted damage for {itemDrop.m_itemData.m_shared.m_name} from {totalDamageOriginal} to {totalDamageAdjusted}");
                    }
                    if(totalUpgradeOriginal != totalUpgradeAdjusted) {
                        ConsoleLog.Print($"-> Adjusted upgrade damage for {itemDrop.m_itemData.m_shared.m_name} from {totalUpgradeOriginal} to {totalUpgradeAdjusted}");
                    }
                }
            }
        }

    }
}
