using HarmonyLib;
using System.Threading;

namespace Cozyheim.DifficultyScaler
{
    internal class MonsterModifier
    {
        [HarmonyPatch]
        private class Patch
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(Character), "SetupMaxHealth")]
            private static void Character_SetupMaxHealth_Prefix(Character __instance, ref float ___m_health)
            {
                if (__instance != null)
                {
                    if (__instance.m_faction != Character.Faction.Players)
                    {
                        if (Main.monsterHealth.TryGetValue(__instance.name, out float value))
                        {
                            ___m_health = value;
                        }

                        ___m_health *= Main.overallHealthMultipler.Value;
                    }
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(Character), "ApplyDamage")]
            private static void Character_ApplyDamage_Prefix(Character __instance, ref HitData hit)
            {
                if (hit.HaveAttacker())
                {
                    if (__instance.m_faction == Character.Faction.Players)
                    {
                        float multiplier = 1f;

                        if (Main.monsterDamage.TryGetValue(hit.GetAttacker().name, out float value))
                        {
                            multiplier *= value;
                        }
                        multiplier *= Main.overallDamageMultipler.Value;

                        hit.ApplyModifier(multiplier);
                    }
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(WearNTear), "RPC_Damage")]
            private static void WearNTear_RPC_Damage_Prefix(ref HitData hit)
            {
                if(hit != null)
                {
                    if (hit.GetAttacker().m_faction != Character.Faction.Players)
                    {
                        float multiplier = 1f;
                        if (Main.monsterHealth.TryGetValue(hit.GetAttacker().name, out float value))
                        {
                            multiplier *= value;
                        }
                        multiplier *= Main.overallDamageMultipler.Value;

                        hit.ApplyModifier(1f / multiplier);
                    }
                }
            }
        }
    }
}
