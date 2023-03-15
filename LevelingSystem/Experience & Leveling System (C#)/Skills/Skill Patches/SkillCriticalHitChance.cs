using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cozyheim.LevelingSystem
{
    internal class SkillCriticalHitChance : SkillBase
    {
        public static SkillCriticalHitChance Instance;

        public SkillCriticalHitChance(int maxLevel, float bonusPerLevel, string iconName, string displayName, string unit = "") : base(maxLevel, bonusPerLevel, iconName, displayName, unit)
        {
            skillType = SkillType.CriticalChance;
            Instance = this;
        }

        [HarmonyPatch]
        private class PatchClass
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(Character), "ApplyDamage")]
            private static void Character_ApplyDamage_Prefix(Character __instance, ref HitData hit)
            {
                if (Instance == null)
                {
                    return;
                }

                if(SkillCriticalHitDamage.Instance == null)
                {
                    return;
                }

                if (hit.HaveAttacker())
                {
                    if (__instance.m_faction != Character.Faction.Players && hit.GetAttacker().m_faction == Character.Faction.Players)
                    {
                        float chance = UnityEngine.Random.Range(0f, 100f);
                        if (chance < Instance.GetBonus())
                        {
                            float critDamageMultiplier = 1 + (SkillCriticalHitDamage.Instance.GetBonus() / 100f);

                            hit.m_damage.m_blunt *= critDamageMultiplier;
                            hit.m_damage.m_slash *= critDamageMultiplier;
                            hit.m_damage.m_pierce *= critDamageMultiplier;
                            hit.m_damage.m_chop *= critDamageMultiplier;
                            hit.m_damage.m_pickaxe *= critDamageMultiplier;
                            hit.m_damage.m_fire *= critDamageMultiplier;
                            hit.m_damage.m_frost *= critDamageMultiplier;
                            hit.m_damage.m_lightning *= critDamageMultiplier;
                            hit.m_damage.m_poison *= critDamageMultiplier;
                            hit.m_damage.m_spirit *= critDamageMultiplier;

                            SkillManager.Instance.SpawnCriticalHitVFX(hit.m_point);
                        }
                    }
                }
            }
        }
    }
}
