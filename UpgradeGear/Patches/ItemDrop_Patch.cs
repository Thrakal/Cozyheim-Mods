using HarmonyLib;
using UnityEngine;

namespace Cozyheim.UpgradeUnlimited {
    internal class ItemDrop_Patch {
        [HarmonyPatch]
        private class Patch {

            [HarmonyPostfix]
            [HarmonyPatch(typeof(ItemDrop), "Awake")]
            public static void ItemDrop_Awake_Postfix(ref ItemDrop __instance) {
                if(__instance.m_itemData == null) {
                    return;
                }

                if(Main.IsItemAllowed(__instance)) {
                    ItemDrop.ItemData.SharedData shared = __instance.m_itemData.m_shared;
                    float value = ConfigSettings.statsIncrease.Value;

                    // Round numbers to 2 decimals
                    shared.m_armorPerLevel = Mathf.Round(Mathf.Max(shared.m_armorPerLevel * value, 1f) * 10f) / 10f;
                    shared.m_blockPowerPerLevel = Mathf.Round(Mathf.Max(shared.m_blockPowerPerLevel * value, 1f) * 10f) / 10f;
                    shared.m_deflectionForcePerLevel = Mathf.Round(Mathf.Max(shared.m_deflectionForcePerLevel * value, 1f) * 10f) / 10f;
                    shared.m_durabilityPerLevel = Mathf.Round(Mathf.Max(shared.m_durabilityPerLevel * value, 1f) * 10f) / 10f;
                    shared.m_damagesPerLevel.Modify(value);
                }
            }

        }
    }
}
