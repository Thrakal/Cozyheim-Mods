using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cozyheim.LevelingSystem
{
    internal class SkillEitrRegen : SkillBase
    {
        public static SkillEitrRegen Instance;

        public SkillEitrRegen(int maxLevel, float bonusPerLevel, string iconName, string displayName, string unit = "", float baseBonus = 0f) : base(maxLevel, bonusPerLevel, iconName, displayName, unit, baseBonus)
        {
            skillType = SkillType.EitrRegen;
            Instance = this;
        }


        [HarmonyPatch]
        private class PatchClass
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(SEMan), "ModifyEitrRegen")]
            static void SEMan_ModifyEitrRegen_Postfix(Character ___m_character, ref float eitrMultiplier)
            {
                if (Instance == null)
                {
                    return;
                }

                if (___m_character.IsPlayer())
                {
                    eitrMultiplier += (Instance.level * Instance.bonusPerLevel) / 100f / 2f;
                }
            }
        }
    }
}
