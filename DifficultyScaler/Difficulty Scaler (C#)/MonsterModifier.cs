using HarmonyLib;
using Cozyheim.API;

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
                float nightMultiplier = EnvMan.instance.IsNight() ? 1f + Main.nightMultiplier.Value : 1f;
                dsComp.SetNightMultiplier(nightMultiplier);

                // Set the Health Multiplier
                dsComp.SetHealthMultiplier(Main.overallHealthMultipler.Value);

                // Set the Damage Multiplier
                dsComp.SetDamageMultiplier(Main.overallDamageMultipler.Value);
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(Character), "Awake")]
            private static void Character_Awake_Prefix(ref Character __instance) {
                ZNetView nview = __instance.GetComponent<ZNetView>();
                if(nview == null) {
                    return;
                }

                if(nview.GetZDO() == null) {
                    return;
                }

                if(__instance == null) {
                    return;
                }

                if(ZNet.instance == null) {
                    return;
                }

                if(__instance.m_faction == Character.Faction.Players) {
                    return;
                }

                DifficultyScalerBase dsComp = __instance.GetComponent<DifficultyScalerBase>();
                if(dsComp == null) {
                    dsComp = __instance.gameObject.AddComponent<DifficultyScalerBase>();
                    SetupDifficultyScalerComponent(__instance, ref dsComp);
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(Character), "SetupMaxHealth")]
            private static void Character_SetupMaxHealth_Prefix(ref Character __instance, ref float ___m_health)
            {
                if (__instance != null)
                {
                    if (__instance.m_faction != Character.Faction.Players)
                    {
                        DifficultyScalerBase dsComp = __instance.GetComponent<DifficultyScalerBase>();

                        if(dsComp == null) {
                            ConsoleLog.Print("DSComp is Null!", LogType.Warning);
                            return;
                        }

                        if(Main.monsterHealth.TryGetValue(__instance.name, out float value))
                        {
                            ___m_health = value;
                        }

                        float startHealth = ___m_health;

                        float multiplier = 1f;
                        multiplier *= dsComp.GetHealthMultiplier();
                        multiplier *= dsComp.GetBiomeMultiplier();
                        multiplier *= dsComp.GetBossKillMultiplier();
                        multiplier *= dsComp.GetNightMultiplier();

                        ___m_health *= multiplier;

                        // print instance name and biome multiplier
                        ConsoleLog.Print(__instance.name + " -> (SetupMaxHealth) Health Multiplier: " + dsComp.GetHealthMultiplier(), LogType.Info);
                        ConsoleLog.Print(__instance.name + " -> (SetupMaxHealth) Biome Multiplier: " + dsComp.GetBiomeMultiplier().ToString(), LogType.Info);
                        ConsoleLog.Print(__instance.name + " -> (SetupMaxHealth) Boss Kill Multiplier: " + dsComp.GetBossKillMultiplier().ToString(), LogType.Info);
                        ConsoleLog.Print(__instance.name + " -> (SetupMaxHealth) Night Multiplier (" + EnvMan.instance.IsNight() + "): " + dsComp.GetNightMultiplier().ToString(), LogType.Info);
                        ConsoleLog.Print(__instance.name + " -> (SetupMaxHealth) Total: " + multiplier.ToString("N4"), LogType.Info);
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

                        DifficultyScalerBase dsComp = hit.GetAttacker().GetComponent<DifficultyScalerBase>();

                        if(dsComp != null) {
                            multiplier *= dsComp.GetDamageMultiplier();
                            multiplier *= dsComp.GetBiomeMultiplier();
                            multiplier *= dsComp.GetBossKillMultiplier();
                            multiplier *= dsComp.GetNightMultiplier();

                            // print instance name and biome multiplier
                            ConsoleLog.Print(__instance.name + " -> (ApplyDamage) Damage Multiplier: " + dsComp.GetDamageMultiplier(), LogType.Info);
                            ConsoleLog.Print(__instance.name + " -> (ApplyDamage) Biome Multiplier: " + dsComp.GetBiomeMultiplier().ToString(), LogType.Info);
                            ConsoleLog.Print(__instance.name + " -> (ApplyDamage) Boss Kill Multiplier: " + dsComp.GetBossKillMultiplier().ToString(), LogType.Info);
                            ConsoleLog.Print(__instance.name + " -> (ApplyDamage) Night Multiplier (" + EnvMan.instance.IsNight() + "): " + dsComp.GetNightMultiplier().ToString(), LogType.Info);
                            ConsoleLog.Print(__instance.name + " -> (ApplyDamage) Total: " + multiplier.ToString("N4"), LogType.Info);
                        }

                        hit.ApplyModifier(multiplier);
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

                DifficultyScalerBase dsComp = hit.GetAttacker().GetComponent<DifficultyScalerBase>();

                if(dsComp != null) {
                    multiplier *= dsComp.GetDamageMultiplier();
                    multiplier *= dsComp.GetBiomeMultiplier();
                    multiplier *= dsComp.GetBossKillMultiplier();
                    multiplier *= dsComp.GetNightMultiplier();

                    // print instance name and biome multiplier
                    ConsoleLog.Print(hit.GetAttacker().name + " -> (RPC_Damage) Damage Multiplier: " + dsComp.GetDamageMultiplier(), LogType.Info);
                    ConsoleLog.Print(hit.GetAttacker().name + " -> (RPC_Damage) Biome Multiplier: " + dsComp.GetBiomeMultiplier().ToString(), LogType.Info);
                    ConsoleLog.Print(hit.GetAttacker().name + " -> (RPC_Damage) Boss Kill Multiplier: " + dsComp.GetBossKillMultiplier().ToString(), LogType.Info);
                    ConsoleLog.Print(hit.GetAttacker().name + " -> (RPC_Damage) Night Multiplier (" + EnvMan.instance.IsNight() + "): " + dsComp.GetNightMultiplier().ToString(), LogType.Info);
                    ConsoleLog.Print(hit.GetAttacker().name + " -> (RPC_Damage) Total: " + multiplier.ToString("N4"), LogType.Info);
                }

                hit.ApplyModifier(1f / multiplier);

            }
        }
    }
}
