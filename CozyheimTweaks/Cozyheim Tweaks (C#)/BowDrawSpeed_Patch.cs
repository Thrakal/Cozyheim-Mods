using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CozyheimTweaks {

    [HarmonyPatch]
    internal class BowDrawSpeed_Patch {

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ItemDrop.ItemData), "GetDrawStaminaDrain")]
        static bool ItemData_GetDrawStaminaDrain_Prefix(ref ItemDrop.ItemData __instance, ref float __result) {
            float drawStaminaDrain = 6f; // Adjust this number

            if(__instance.m_shared.m_attack.m_drawStaminaDrain <= 0f) {
                __result = 0f;
                return false;
            }

            float skillFactor = Player.m_localPlayer.GetSkillFactor(__instance.m_shared.m_skillType);
            __result = drawStaminaDrain - drawStaminaDrain * 0.33f * skillFactor;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Humanoid), "GetAttackDrawPercentage")]
        static bool Humanoid_GetAttackDrawPercentage_Prefix(ref Humanoid __instance, ref float __result, float ___m_attackDrawTime) {
            float minDrawSpeed = 1f; // Adjust this number

            ItemDrop.ItemData currentWeapon = __instance.GetCurrentWeapon();

            if(currentWeapon != null && currentWeapon.m_shared.m_attack.m_bowDraw && ___m_attackDrawTime > 0f) {
                float skillFactor = __instance.GetSkillFactor(currentWeapon.m_shared.m_skillType);
                float num = Mathf.Lerp(minDrawSpeed, minDrawSpeed * 0.2f, skillFactor);
                if(!(num > 0f)) {
                    __result = 1f;
                }
                __result = Mathf.Clamp01(___m_attackDrawTime / num);
            } else {
                __result = 0f;
            }

            return false;
        }
    }
}
