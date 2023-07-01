using HarmonyLib;
using Cozyheim.API;
using System.Diagnostics;

namespace Cozyheim.DifficultyScaler
{
    internal class MonsterModifier
    {
        [HarmonyPatch]
        private class Patch
        {
            private static void SetupDifficultyScalerComponent(Character __instance, ref DifficultyScalerBase dsComp) {
                // Set the Biome Multiplier
                Heightmap.Biome biome = Heightmap.FindBiome(__instance.transform.position);
                switch(biome) {
                    case Heightmap.Biome.Meadows:
                        dsComp.SetBiomeMultiplier(Main.meadowsMultiplier.Value); break;
                    case Heightmap.Biome.BlackForest:
                        dsComp.SetBiomeMultiplier(Main.blackForestMultiplier.Value); break;
                    case Heightmap.Biome.Swamp:
                        dsComp.SetBiomeMultiplier(Main.swampMultiplier.Value); break;
                    case Heightmap.Biome.Mountain:
                        dsComp.SetBiomeMultiplier( Main.mountainMultiplier.Value); break;
                    case Heightmap.Biome.Plains:
                        dsComp.SetBiomeMultiplier(Main.plainsMultiplier.Value); break;
                    case Heightmap.Biome.AshLands:
                        dsComp.SetBiomeMultiplier(Main.ashlandsMultiplier.Value); break;
                    case Heightmap.Biome.DeepNorth:
                        dsComp.SetBiomeMultiplier(Main.deepNorthMultiplier.Value); break;
                    case Heightmap.Biome.Mistlands:
                        dsComp.SetBiomeMultiplier(Main.mistlandsMultiplier.Value); break;
                    case Heightmap.Biome.Ocean:
                        dsComp.SetBiomeMultiplier(Main.oceanMultiplier.Value); break;
                    default:
                        dsComp.SetBiomeMultiplier(0f); break;
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
                dsComp.SetBossKillMultiplier(Main.bossKillMultiplier.Value * counter);

                // Set the Night Multiplier
                float nightMultiplier = EnvMan.instance.IsNight() ? Main.nightMultiplier.Value : 0f;
                dsComp.SetNightMultiplier(nightMultiplier);

                // Set the Health Multiplier
                dsComp.SetHealthMultiplier(Main.overallHealthMultipler.Value);

                // Set the Damage Multiplier
                dsComp.SetDamageMultiplier(Main.overallDamageMultipler.Value);

                // Set the Star Multiplier
                dsComp.SetStarMultiplier((__instance.GetLevel() - 1) * Main.starMultiplier.Value);

                ConsoleLog.Print(__instance.name + ": Level = " + __instance.GetLevel() + " - (bonus: +" + (dsComp.GetStarMultiplier() * 100f).ToString("N0") + "%)");
            }


            [HarmonyPrefix]
            [HarmonyPatch(typeof(Character), "SetupMaxHealth")]
            private static bool Character_SetupMaxHealth_Prefix(ref Character __instance, ref float ___m_health)
            {
                ZNetView nview = __instance.GetComponent<ZNetView>();
                if(nview == null) {
                    return true;
                }

                if(nview.GetZDO() == null) {
                    return true;
                }

                if(__instance == null) {
                    return true;
                }

                if(ZNet.instance == null) {
                    return true;
                }

                if(__instance.m_faction == Character.Faction.Players) {
                    return true;
                }

                if(__instance.IsTamed()) {
                    return true;
                }

                DifficultyScalerBase dsComp = __instance.GetComponent<DifficultyScalerBase>();
                if(dsComp == null) {
                    dsComp = __instance.gameObject.AddComponent<DifficultyScalerBase>();
                }

                if(dsComp == null) {
                    ConsoleLog.Print("DSComp is Null!", LogType.Warning);
                    return true;
                }

                if(Main.monsterHealth.TryGetValue(__instance.name, out float value))
                {
                    ___m_health = value;
                }

                if(dsComp.GetStartHealth() <= 0f) {
                    dsComp.SetStartHealth(___m_health);
                }

                if(dsComp.GetLevel() != __instance.GetLevel()) {
                    dsComp.SetLevel(__instance.GetLevel());
                }

                SetupDifficultyScalerComponent(__instance, ref dsComp);

                float multiplier = 1f;

                multiplier += dsComp.GetHealthMultiplier();
                ConsoleLog.Print(__instance.name + " -> (SetupMaxHealth) Health Bonus: +" + (dsComp.GetHealthMultiplier() * 100f).ToString("N0") + "%", LogType.Info);

                if(Main.enableBiomeDifficulty.Value) {
                    multiplier += dsComp.GetBiomeMultiplier();
                    ConsoleLog.Print(__instance.name + " -> (SetupMaxHealth) Biome Bonus: +" + (dsComp.GetBiomeMultiplier() * 100f).ToString("N0") + "%", LogType.Info);
                }
                if(Main.enableBossKillDifficulty.Value) {
                    multiplier += dsComp.GetBossKillMultiplier();
                    ConsoleLog.Print(__instance.name + " -> (SetupMaxHealth) Boss Kill Bonus: +" + (dsComp.GetBossKillMultiplier() * 100f).ToString("N0") + "%", LogType.Info);
                }
                if(Main.enableNightDifficulty.Value) {
                    multiplier += dsComp.GetNightMultiplier();
                    ConsoleLog.Print(__instance.name + " -> (SetupMaxHealth) Night Bonus: +" + (dsComp.GetNightMultiplier() * 100f).ToString("N0") + "%", LogType.Info);
                }
                if(Main.enableStarDifficulty.Value) {
                    multiplier += dsComp.GetStarMultiplier();
                    ConsoleLog.Print(__instance.name + " -> (SetupMaxHealth) Star Bonus: +" + (dsComp.GetStarMultiplier() * 100f).ToString("N0") + "%", LogType.Info);
                }

                ___m_health = dsComp.GetStartHealth() * multiplier;

                ConsoleLog.Print(__instance.name + " -> (SetupMaxHealth) Total bonus: +" + ((multiplier - 1f) * 100f).ToString("N0") + "%", LogType.Info);
                ConsoleLog.Print(__instance.name + " -> (SetupMaxHealth) Health: " + dsComp.GetStartHealth() + " -> " + ___m_health, LogType.Info);

                if(Main.enableStarDifficulty.Value) {
                    __instance.SetMaxHealth(___m_health);
                    return false;
                }
              
                return true;
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(Character), "ApplyDamage")]
            private static void Character_ApplyDamage_Prefix(Character __instance, ref HitData hit)
            {
                if (hit.HaveAttacker())
                {
                    if(__instance.m_faction == Character.Faction.Players)
                    {
                        float startDamage = hit.GetTotalDamage();
                        float multiplier = 1f;

                        if (Main.monsterDamage.TryGetValue(hit.GetAttacker().name, out float value))
                        {
                            float oldBaseDamage = hit.GetTotalDamage();
                            hit.ApplyModifier(value);
                            ConsoleLog.Print("(ApplyDamage) " + hit.GetAttacker().name + " base damage adjusted to " + (value * 100f).ToString("N0") + "% of normal damage.");
                            ConsoleLog.Print($"-> Base damage: {oldBaseDamage} -> {hit.GetTotalDamage()}", LogType.Info);

                        }

                        DifficultyScalerBase dsComp = hit.GetAttacker().GetComponent<DifficultyScalerBase>();

                        if(dsComp != null) {
                            multiplier += dsComp.GetDamageMultiplier();
                            ConsoleLog.Print(__instance.name + " -> (ApplyDamage) Damage Bonus: +" + (dsComp.GetDamageMultiplier() * 100f).ToString("N0") + "%", LogType.Info);

                            if(Main.enableBiomeDifficulty.Value) {
                                multiplier += dsComp.GetBiomeMultiplier();
                                ConsoleLog.Print(__instance.name + " -> (ApplyDamage) Biome Bonus: +" + (dsComp.GetBiomeMultiplier() * 100f).ToString("N0") + "%", LogType.Info);
                            }
                            if(Main.enableBossKillDifficulty.Value) {
                                multiplier += dsComp.GetBossKillMultiplier();
                                ConsoleLog.Print(__instance.name + " -> (ApplyDamage) Boss Kill Bonus: +" + (dsComp.GetBossKillMultiplier() * 100f).ToString("N0") + "%", LogType.Info);
                            }
                            if(Main.enableNightDifficulty.Value) {
                                multiplier += dsComp.GetNightMultiplier();
                                ConsoleLog.Print(__instance.name + " -> (ApplyDamage) Night Bonus: +" + (dsComp.GetNightMultiplier() * 100f).ToString("N0") + "%", LogType.Info);
                            }
                            if(Main.enableStarDifficulty.Value) {
                                multiplier += dsComp.GetStarMultiplier();
                                ConsoleLog.Print(__instance.name + " -> (ApplyDamage) Star Bonus: +" + (dsComp.GetStarMultiplier() * 100f).ToString("N0") + "%", LogType.Info);
                            }
                        }

                        hit.ApplyModifier(multiplier);

                        ConsoleLog.Print(__instance.name + " -> (ApplyDamage) Total bonus: +" + ((multiplier - 1f) * 100f).ToString("N0") + "%", LogType.Info);
                        ConsoleLog.Print(__instance.name + " -> (ApplyDamage) Damage: " + startDamage + " -> " + hit.GetTotalDamage(), LogType.Info);
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

                if(hit.GetAttacker() == null) {
                    ConsoleLog.Print("Hit attacker is null", LogType.Warning);
                    return;
                }

                if(!hit.GetAttacker().IsMonsterFaction() || !hit.GetAttacker().IsBoss()) {
                    return;
                }

                float multiplier = 1f;
                if (Main.monsterDamage.TryGetValue(hit.GetAttacker().name, out float value))
                {
                    ConsoleLog.Print("(RPC_Damage) Found: " + hit.GetAttacker().name + " = " + value);
                    multiplier += value;
                }

                DifficultyScalerBase dsComp = hit.GetAttacker().GetComponent<DifficultyScalerBase>();

                if(dsComp != null) {
                    multiplier += dsComp.GetDamageMultiplier();
                    ConsoleLog.Print(hit.GetAttacker().name + " -> (RPC_Damage) Damage Multiplier: " + dsComp.GetDamageMultiplier(), LogType.Info);

                    if(Main.enableBiomeDifficulty.Value) {
                        multiplier += dsComp.GetBiomeMultiplier();
                        ConsoleLog.Print(hit.GetAttacker().name + " -> (RPC_Damage) Biome Multiplier: " + dsComp.GetBiomeMultiplier().ToString(), LogType.Info);
                    }
                    if(Main.enableBossKillDifficulty.Value) {
                        multiplier += dsComp.GetBossKillMultiplier();
                        ConsoleLog.Print(hit.GetAttacker().name + " -> (RPC_Damage) Boss Kill Multiplier: " + dsComp.GetBossKillMultiplier().ToString(), LogType.Info);
                    }
                    if(Main.enableNightDifficulty.Value) {
                        multiplier += dsComp.GetNightMultiplier();
                        ConsoleLog.Print(hit.GetAttacker().name + " -> (RPC_Damage) Night Multiplier: " + dsComp.GetNightMultiplier().ToString(), LogType.Info);
                    }
                    if(Main.enableStarDifficulty.Value) {
                        multiplier += dsComp.GetStarMultiplier();
                        ConsoleLog.Print(hit.GetAttacker().name + " -> (RPC_Damage) Star Multiplier: " + dsComp.GetStarMultiplier().ToString(), LogType.Info);
                    }
                }

                hit.ApplyModifier(1f / multiplier);
            }
        }
    }
}
