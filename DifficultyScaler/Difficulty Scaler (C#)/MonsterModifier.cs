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

                        float startHealth = ___m_health;

                        float multiplier = Main.overallHealthMultipler.Value;
                        multiplier *= DifficultyScalerAPI.GetBiomeMultiplier();
                        multiplier *= DifficultyScalerAPI.GetBossKillMultiplier();
                        multiplier *= DifficultyScalerAPI.GetNightMultiplier();

                        ___m_health *= multiplier;

                        // print instance name and biome multiplier
                        ConsoleLog.Print(__instance.name + " -> (SetupMaxHealth) Overall Multiplier: " + Main.overallHealthMultipler.Value, LogType.Info);
                        ConsoleLog.Print(__instance.name + " -> (SetupMaxHealth) Biome Multiplier (" + EnvMan.instance.GetCurrentBiome().ToString() + "): " + DifficultyScalerAPI.GetBiomeMultiplier().ToString(), LogType.Info);
                        ConsoleLog.Print(__instance.name + " -> (SetupMaxHealth) Boss Kill Multiplier (Kills: " + DifficultyScalerAPI.GetBossKillCount().ToString() + "): " + DifficultyScalerAPI.GetBossKillMultiplier().ToString(), LogType.Info);
                        ConsoleLog.Print(__instance.name + " -> (SetupMaxHealth) Night Multiplier (" + EnvMan.instance.IsNight() + "): " + DifficultyScalerAPI.GetNightMultiplier().ToString(), LogType.Info);
                        ConsoleLog.Print(__instance.name + " -> (SetupMaxHealth) Health: " + startHealth + " -> " + ___m_health, LogType.Info);
                    }
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(Character), "ApplyDamage")]
            private static void Character_ApplyDamage_Prefix(Character __instance, ref HitData hit)
            {
                if (hit.HaveAttacker())
                {
                    if(__instance.m_faction == Character.Faction.Players)
                    {
                        float multiplier = 1f;

                        if (Main.monsterDamage.TryGetValue(hit.GetAttacker().name, out float value))
                        {
                            multiplier *= value;
                        }

                        multiplier *= Main.overallDamageMultipler.Value;
                        multiplier *= DifficultyScalerAPI.GetBiomeMultiplier();
                        multiplier *= DifficultyScalerAPI.GetBossKillMultiplier();
                        multiplier *= DifficultyScalerAPI.GetNightMultiplier();

                        hit.ApplyModifier(multiplier);

                        // print instance name and biome multiplier
                        ConsoleLog.Print(__instance.name + " -> (ApplyDamage) Overall Multiplier: " + Main.overallDamageMultipler.Value, LogType.Info);
                        ConsoleLog.Print(__instance.name + " -> (ApplyDamage) Biome Multiplier (" + EnvMan.instance.GetCurrentBiome().ToString() + "): " + DifficultyScalerAPI.GetBiomeMultiplier().ToString(), LogType.Info);
                        ConsoleLog.Print(__instance.name + " -> (ApplyDamage) Boss Kill Multiplier (Kills: " + DifficultyScalerAPI.GetBossKillCount().ToString() + "): " + DifficultyScalerAPI.GetBossKillMultiplier().ToString(), LogType.Info);
                        ConsoleLog.Print(__instance.name + " -> (ApplyDamage) Night Multiplier (" + EnvMan.instance.IsNight() + "): " + DifficultyScalerAPI.GetNightMultiplier().ToString(), LogType.Info);
                        ConsoleLog.Print(__instance.name + " -> (ApplyDamage) Total: " + multiplier.ToString("N4"), LogType.Info);
                    }
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(WearNTear), "RPC_Damage")]
            private static void WearNTear_RPC_Damage_Prefix(ref HitData hit)
            {
                ConsoleLog.Print("RPC_Damage called!", LogType.Info);

                if(hit == null) {
                    ConsoleLog.Print("Hit is null", LogType.Warning);
                    return;
                }

                if(!hit.GetAttacker().IsMonsterFaction() || !hit.GetAttacker().IsBoss()) {
                    return;
                }

                float multiplier = 1f;
                if (Main.monsterHealth.TryGetValue(hit.GetAttacker().name, out float value))
                {
                    multiplier *= value;
                }

                multiplier *= Main.overallDamageMultipler.Value;
                multiplier *= DifficultyScalerAPI.GetBiomeMultiplier();
                multiplier *= DifficultyScalerAPI.GetBossKillMultiplier();
                multiplier *= DifficultyScalerAPI.GetNightMultiplier();

                hit.ApplyModifier(1f / multiplier);

                // print instance name and biome multiplier
                ConsoleLog.Print(hit.GetAttacker().name + " -> (RPC_Damage) Overall Multiplier: " + Main.overallDamageMultipler.Value, LogType.Info);
                ConsoleLog.Print(hit.GetAttacker().name + " -> (RPC_Damage) Biome Multiplier (" + EnvMan.instance.GetCurrentBiome().ToString() + "): " + DifficultyScalerAPI.GetBiomeMultiplier().ToString(), LogType.Info);
                ConsoleLog.Print(hit.GetAttacker().name + " -> (RPC_Damage) Boss Kill Multiplier (Kills: " + DifficultyScalerAPI.GetBossKillCount().ToString() + "): " + DifficultyScalerAPI.GetBossKillMultiplier().ToString(), LogType.Info);
                ConsoleLog.Print(hit.GetAttacker().name + " -> (RPC_Damage) Night Multiplier (" + EnvMan.instance.IsNight() + "): " + DifficultyScalerAPI.GetNightMultiplier().ToString(), LogType.Info);
                ConsoleLog.Print(hit.GetAttacker().name + " -> (RPC_Damage) Total: " + multiplier.ToString("N4"), LogType.Info);
            }
        }
    }
}
