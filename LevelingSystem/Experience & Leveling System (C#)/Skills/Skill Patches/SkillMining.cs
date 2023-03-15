using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cozyheim.LevelingSystem
{
    internal class SkillMining : SkillBase
    {
        public static SkillMining Instance;

        public SkillMining(int maxLevel, float bonusPerLevel, string iconName, string displayName, string unit = "") : base(maxLevel, bonusPerLevel, iconName, displayName, unit)
        {
            skillType = SkillType.Mining;
            Instance = this;
        }


        [HarmonyPatch]
        private class PatchClass
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(ItemDrop.ItemData), "GetDamage", new Type[] { typeof(int) })]
            static void ItemData_GetDamage_Mining_Postfix(ItemDrop.ItemData __instance, ref HitData.DamageTypes __result)
            {
                if (Instance == null)
                {
                    return;
                }

                if (__instance.m_shared.m_skillType == Skills.SkillType.Pickaxes)
                {
                    float multiplier = 1 + ((Instance.level * Instance.bonusPerLevel) / 100);
                    __result.m_pickaxe *= multiplier;
                }
            }
        }
    }
}
