using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cozyheim.LevelingSystem
{
    internal class SkillEitr : SkillBase
    {
        public static SkillEitr Instance;

        public SkillEitr(int maxLevel, float bonusPerLevel, string iconName, string displayName, string unit = "", float baseBonus = 0f) : base(maxLevel, bonusPerLevel, iconName, displayName, unit, baseBonus)
        {
            skillType = SkillType.Eitr;
            Instance = this;
        }


        [HarmonyPatch]
        private class PatchClass
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(Player), "GetTotalFoodValue")]
            static void Player_GetTotalFoodValue_Postfix(ref float eitr)
            {
                if (Instance == null)
                {
                    return;
                }
            
                eitr += Instance.level * Instance.bonusPerLevel;
            }
        }
    }
}
