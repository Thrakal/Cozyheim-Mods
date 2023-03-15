using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cozyheim.LevelingSystem
{
    internal class SkillCarryWeight : SkillBase
    {
        public static SkillCarryWeight Instance;

        public SkillCarryWeight(int maxLevel, float bonusPerLevel, string iconName, string displayName, string unit = "") : base(maxLevel, bonusPerLevel, iconName, displayName, unit)
        {
            skillType = SkillType.CarryWeight;
            Instance = this;
        }


        [HarmonyPatch]
        private class SkillCarryWeight_Patch
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(SEMan), "ModifyMaxCarryWeight")]
            static void Player_ModifyMaxCarryWeight_Postfix(SEMan __instance, Character ___m_character, ref float limit)
            {
                if (Instance == null)
                {
                    return;
                }

                if (___m_character.IsPlayer())
                {
                    limit += Instance.level * Instance.bonusPerLevel;
                }
            }
        }
    }
}
