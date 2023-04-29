using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cozyheim.LevelingSystem
{
    internal class SkillStaminaRegen : SkillBase
    {
        public static SkillStaminaRegen Instance;

        public SkillStaminaRegen(int maxLevel, float bonusPerLevel, string iconName, string displayName, string unit = "", float baseBonus = 0f) : base(maxLevel, bonusPerLevel, iconName, displayName, unit, baseBonus)
        {
            skillType = SkillType.StaminaRegen;
            Instance = this;
        }


        [HarmonyPatch]
        private class PatchClass
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(SEMan), "ModifyStaminaRegen")]
            static void SEMan_ModifyStaminaRegen_Postfix(ref float staminaMultiplier)
            {
                if(Instance == null) {
                    return;
                }

                staminaMultiplier += (Instance.level * Instance.bonusPerLevel) / 100f;
            }
        }
    }
}
