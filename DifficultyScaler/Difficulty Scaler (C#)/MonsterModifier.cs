using HarmonyLib;
using Cozyheim.API;

namespace Cozyheim.DifficultyScaler
{
    internal class MonsterModifier
    {
        [HarmonyPatch]
        private class Patch
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(Character), "Awake")]
            private static void Character_Awake_Postfix(ref Character __instance) {
                if(__instance.GetComponent<ZNetView>() == null) {
                    return;
                }

                DifficultyScalerComp dsComp = __instance.GetComponent<DifficultyScalerComp>();

                if(dsComp == null) {
                    dsComp = __instance.gameObject.AddComponent<DifficultyScalerComp>();
                }

                // Set the Biome Multiplier
                Heightmap.Biome biome = Heightmap.FindBiome(__instance.transform.position);
                switch(biome) {
                    case Heightmap.Biome.Meadows:
                        dsComp.SetBiomeMultiplier(1f + Main.meadowsMultiplier.Value); break;
                    case Heightmap.Biome.BlackForest:
                        dsComp.SetBiomeMultiplier(1f + Main.blackForestMultiplier.Value); break;
                    case Heightmap.Biome.Swamp:
                        dsComp.SetBiomeMultiplier(1f + Main.swampMultiplier.Value); break;
                    case Heightmap.Biome.Mountain:
                        dsComp.SetBiomeMultiplier(1f + Main.mountainMultiplier.Value); break;
                    case Heightmap.Biome.Plains:
                        dsComp.SetBiomeMultiplier(1f + Main.plainsMultiplier.Value); break;
                    case Heightmap.Biome.AshLands:
                        dsComp.SetBiomeMultiplier(1f + Main.ashlandsMultiplier.Value); break;
                    case Heightmap.Biome.DeepNorth:
                        dsComp.SetBiomeMultiplier(1f + Main.deepNorthMultiplier.Value); break;
                    case Heightmap.Biome.Mistlands:
                        dsComp.SetBiomeMultiplier(1f + Main.mistlandsMultiplier.Value); break;
                    case Heightmap.Biome.Ocean:
                        dsComp.SetBiomeMultiplier(1f + Main.oceanMultiplier.Value); break;
                    default:
                        dsComp.SetBiomeMultiplier(1f); break;
                }

                // Set the Boss Kill Multiplier
                string[] split = Main.bossGlobalKeys.Value.Split(',');
                int counter = 0;
                foreach(string key in split) {
                    if(key != "") {
                        if(ZoneSystem.instance.GetGlobalKey(key.Trim())) {
                            counter++;
                        }
                    }
                }
                dsComp.SetBossKillMultiplier(1f + Main.bossKillMultiplier.Value * counter);

                // Set the Night Multiplier
                dsComp.SetNightMultiplier(1f + Main.nightMultiplier.Value);

                // Set the Health Multiplier
                dsComp.SetHealthMultiplier(Main.overallHealthMultipler.Value);

                // Set the Damage Multiplier
                dsComp.SetDamageMultiplier(Main.overallDamageMultipler.Value);
            }

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

                        DifficultyScalerComp dsComp = __instance.GetComponent<DifficultyScalerComp>();

                        float multiplier = 1f;
                        multiplier *= dsComp.GetHealthMultiplier();
                        multiplier *= dsComp.GetBiomeMultiplier();
                        multiplier *= dsComp.GetBossKillMultiplier();
                        multiplier *= dsComp.GetNightMultiplier();

                        ___m_health *= multiplier;

                        // print instance name and biome multiplier
                        ConsoleLog.Print(__instance.name + " -> (SetupMaxHealth) Health Multiplier: " + dsComp.GetHealthMultiplier(), LogType.Info);
                        ConsoleLog.Print(__instance.name + " -> (SetupMaxHealth) Biome Multiplier (" + EnvMan.instance.GetCurrentBiome().ToString() + "): " + dsComp.GetBiomeMultiplier().ToString(), LogType.Info);
                        ConsoleLog.Print(__instance.name + " -> (SetupMaxHealth) Boss Kill Multiplier: " + dsComp.GetBossKillMultiplier().ToString(), LogType.Info);
                        ConsoleLog.Print(__instance.name + " -> (SetupMaxHealth) Night Multiplier (" + EnvMan.instance.IsNight() + "): " + dsComp.GetNightMultiplier().ToString(), LogType.Info);
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
                            ConsoleLog.Print("(ApplyDamage) Found: " + hit.GetAttacker().name + " = " + value);
                            multiplier *= value;
                        }

                        DifficultyScalerComp dsComp = __instance.GetComponent<DifficultyScalerComp>();

                        multiplier *= dsComp.GetDamageMultiplier();
                        multiplier *= dsComp.GetBiomeMultiplier();
                        multiplier *= dsComp.GetBossKillMultiplier();
                        multiplier *= dsComp.GetNightMultiplier();

                        hit.ApplyModifier(multiplier);

                        // print instance name and biome multiplier
                        ConsoleLog.Print(__instance.name + " -> (ApplyDamage) Damage Multiplier: " + dsComp.GetDamageMultiplier(), LogType.Info);
                        ConsoleLog.Print(__instance.name + " -> (ApplyDamage) Biome Multiplier (" + EnvMan.instance.GetCurrentBiome().ToString() + "): " + dsComp.GetBiomeMultiplier().ToString(), LogType.Info);
                        ConsoleLog.Print(__instance.name + " -> (ApplyDamage) Boss Kill Multiplier: " + dsComp.GetBossKillMultiplier().ToString(), LogType.Info);
                        ConsoleLog.Print(__instance.name + " -> (ApplyDamage) Night Multiplier (" + EnvMan.instance.IsNight() + "): " + dsComp.GetNightMultiplier().ToString(), LogType.Info);
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
                if (Main.monsterDamage.TryGetValue(hit.GetAttacker().name, out float value))
                {
                    ConsoleLog.Print("(RPC_Damage) Found: " + hit.GetAttacker().name + " = " + value);
                    multiplier *= value;
                }

                DifficultyScalerComp dsComp = hit.GetAttacker().GetComponent<DifficultyScalerComp>();

                multiplier *= dsComp.GetDamageMultiplier();
                multiplier *= dsComp.GetBiomeMultiplier();
                multiplier *= dsComp.GetBossKillMultiplier();
                multiplier *= dsComp.GetNightMultiplier();

                hit.ApplyModifier(1f / multiplier);

                // print instance name and biome multiplier
                ConsoleLog.Print(hit.GetAttacker().name + " -> (RPC_Damage) Damage Multiplier: " + dsComp.GetDamageMultiplier(), LogType.Info);
                ConsoleLog.Print(hit.GetAttacker().name + " -> (RPC_Damage) Biome Multiplier (" + EnvMan.instance.GetCurrentBiome().ToString() + "): " + dsComp.GetBiomeMultiplier().ToString(), LogType.Info);
                ConsoleLog.Print(hit.GetAttacker().name + " -> (RPC_Damage) Boss Kill Multiplier: " + dsComp.GetBossKillMultiplier().ToString(), LogType.Info);
                ConsoleLog.Print(hit.GetAttacker().name + " -> (RPC_Damage) Night Multiplier (" + EnvMan.instance.IsNight() + "): " + dsComp.GetNightMultiplier().ToString(), LogType.Info);
                ConsoleLog.Print(hit.GetAttacker().name + " -> (RPC_Damage) Total: " + multiplier.ToString("N4"), LogType.Info);
            }
        }
    }
}
