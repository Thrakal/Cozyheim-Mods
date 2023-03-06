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

        public SkillMovementSpeed(int maxLevel, float bonusPerLevel, string iconName, string displayName, string unit = "") : base(maxLevel, bonusPerLevel, iconName, displayName, unit)
        {
            skillType = SkillType.MovementSpeed;
            Instance = this;
        }


        [HarmonyPatch]
        private class PatchClass
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(Player), "UpdateMovementModifier")]
            static void Player_UpdateMovementModifier_Postfix(ref float ___m_equipmentMovementModifier)
            {
                if (Instance == null)
                {
                    return;
                }

                ___m_equipmentMovementModifier += (Instance.level * Instance.bonusPerLevel) / 100f;
            }
        }
    }
}
