using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cozyheim.LevelingSystem
{
    internal class SkillMovementSpeed : SkillBase
    {
        public static SkillMovementSpeed Instance;

        public SkillMovementSpeed(int maxLevel, float bonusPerLevel, string iconName, string displayName, string unit = "", float baseBonus = 0f) : base(maxLevel, bonusPerLevel, iconName, displayName, unit, baseBonus)
        {
            skillType = SkillType.MovementSpeed;
            Instance = this;
        }


        [HarmonyPatch]
        private class PatchClass
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(Player), "GetJogSpeedFactor")]
            static void Player_GetJogSpeedFactor_Postfix(Player __instance, ref float __result)
            {
                if (Instance == null)
                {
                    return;
                }

                if (__instance == Player.m_localPlayer)
                {
                    float bonusValue = (Instance.level * Instance.bonusPerLevel) / 100f;
                    __result += bonusValue;
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(Player), "GetRunSpeedFactor")]
            static void Player_GetRunSpeedFactor_Postfix(Player __instance, ref float __result, float ___m_equipmentMovementModifier)
            {
                if (Instance == null)
                {
                    return;
                }

                if (__instance == Player.m_localPlayer)
                {
                    float bonusValue = (Instance.level * Instance.bonusPerLevel) / 100f;
                    float threshold = bonusValue * 0.25f;

                    float runValue = __result + (bonusValue * 0.25f);
                    float jogValue = 1f + ___m_equipmentMovementModifier + bonusValue;

                    __result = runValue > jogValue + threshold ? runValue : runValue + threshold;
                }
            }

        }
    }
}
