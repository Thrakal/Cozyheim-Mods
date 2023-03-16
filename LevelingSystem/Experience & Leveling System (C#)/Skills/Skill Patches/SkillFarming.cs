using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cozyheim.LevelingSystem
{
    internal class SkillFarming : SkillBase
    {
        public static SkillFarming Instance;

        public SkillFarming(int maxLevel, float bonusPerLevel, string iconName, string displayName, string unit = "", float baseBonus = 0f) : base(maxLevel, bonusPerLevel, iconName, displayName, unit, baseBonus)
        {
            skillType = SkillType.Farming;
            Instance = this;
        }


        [HarmonyPatch]
        private class PatchClass
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(Player), "Awake")]
            static void Player_Awake_Prefix()
            {
                if (Instance == null)
                {
                    return;
                }

                // Patch code here
            }
        }
    }
}
