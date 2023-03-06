using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cozyheim.LevelingSystem
{
    internal class SkillHPRegen : SkillBase
    {
        public static SkillHPRegen Instance;

        public SkillHPRegen(int maxLevel, float bonusPerLevel, string iconName, string displayName, string unit = "") : base(maxLevel, bonusPerLevel, iconName, displayName, unit)
        {
            skillType = SkillType.HPRegen;
            Instance = this;
        }


        [HarmonyPatch]
        private class PatchClass
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(SEMan), "ModifyHealthRegen")]
            static void SEMan_ModifyHealthRegen_Postfix(Character ___m_character, ref float regenMultiplier)
            {
                if (Instance == null)
                {
                    return;
                }

                if (___m_character.IsPlayer())
                {
                    regenMultiplier += (Instance.level * Instance.bonusPerLevel) / 100f / 2f;
                }
            }
        }
    }
}
