using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static ItemDrop;

namespace CozyheimTweaks {

    [HarmonyPatch]
    internal class CrossbowReloadSpeed_Patch {

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ItemDrop.ItemData), "GetWeaponLoadingTime")]
        static bool ItemData_GetWeaponLoadingTime_Prefix(ref ItemDrop.ItemData __instance, ref float __result) {
            if(__instance.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Bow) {
                if(__instance.m_shared.m_attack.m_requiresReload) {
                    __result = 0.2f;
                    return false;
                }
            }

            return true;            
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ItemDrop), "Start")]
        static void ItemData_Awake_Postfix(ref ItemDrop __instance) {
            float scaleSize = 0.65f; // Adjust this number

            if(__instance.m_itemData == null) {
                return;
            }

            ItemDrop.ItemData itemData = __instance.m_itemData;

            if(itemData.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Bow) {
                if(itemData.m_shared.m_skillType == Skills.SkillType.Crossbows) {
                    __instance.transform.localScale = Vector3.one * scaleSize;
                    ToppLog.Print("I got scaled: " + __instance.gameObject.name);
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Humanoid), "EquipItem")]
        static void Humanoid_EquipItem_Postfix(ref Humanoid __instance, ItemDrop.ItemData item) {
            float scaleSize = 0.65f; // Adjust this number
            VisEquipment visEquipment = __instance.GetComponent<VisEquipment>();

            if(visEquipment == null) {
                return;
            }

            if(item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Bow) {
                if(item.m_shared.m_skillType == Skills.SkillType.Crossbows) {
                    visEquipment.m_leftHand.localScale *= scaleSize;
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Attack), "GetAttackStamina")]
        static bool Attack_GetAttackStamina_Prefix(Attack __instance, ref float __result, Humanoid ___m_character) {
            float staminaCost = 10f; // Adjust this number

            if(__instance.m_attackAnimation == "crossbow_fire" && __instance.m_requiresReload) {
                float skillFactor = ___m_character.GetSkillFactor(Skills.SkillType.Crossbows);
                __result = staminaCost - staminaCost * skillFactor * 0.33f;
                return false;
            }

            return true;
        }

    }
}
