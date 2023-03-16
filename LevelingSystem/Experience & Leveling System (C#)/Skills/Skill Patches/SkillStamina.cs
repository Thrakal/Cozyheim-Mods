using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cozyheim.LevelingSystem
{
    internal class SkillStamina : SkillBase
    {
        public static SkillStamina Instance;

        public SkillStamina(int maxLevel, float bonusPerLevel, string iconName, string displayName, string unit = "", float baseBonus = 0f) : base(maxLevel, bonusPerLevel, iconName, displayName, unit, baseBonus)
        {
            skillType = SkillType.Stamina;
            Instance = this;
        }


        [HarmonyPatch]
        private class SkillStamina_Patch
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(Player), "GetTotalFoodValue")]
            static void Player_GetTotalFoodValue_Postfix(ref float stamina)
            {
                if (Instance == null)
                {
                    return;
                }

                float addValue = Instance.level * Instance.bonusPerLevel;
                stamina += addValue;
            }
        }
    }
}
