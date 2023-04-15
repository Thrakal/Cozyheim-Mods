namespace Cozyheim.DifficultyScaler
{
    public class DifficultyScalerAPI
    {
        public static float GetBiomeMultiplier() {
            if(!Main.enableBiomeDifficulty.Value) {
                ConsoleLog.Print("Biome Multiplier not enabled! - Returning 1f");
                return 1f;
            }

            Heightmap.Biome biome = EnvMan.instance.GetCurrentBiome();
            switch(biome) {
                case Heightmap.Biome.Meadows:
                    return Main.meadowsMultiplier.Value + 1f;
                case Heightmap.Biome.BlackForest:
                    return Main.blackForestMultiplier.Value + 1f;
                case Heightmap.Biome.Swamp:
                    return Main.swampMultiplier.Value + 1f;
                case Heightmap.Biome.Mountain:
                    return Main.mountainMultiplier.Value + 1f;
                case Heightmap.Biome.Plains:
                    return Main.plainsMultiplier.Value + 1f;
                case Heightmap.Biome.AshLands:
                    return Main.ashlandsMultiplier.Value + 1f;
                case Heightmap.Biome.DeepNorth:
                    return Main.deepNorthMultiplier.Value + 1f;
                case Heightmap.Biome.Mistlands:
                    return Main.mistlandsMultiplier.Value + 1f;
                case Heightmap.Biome.Ocean:
                    return Main.oceanMultiplier.Value + 1f;
                default:
                    return 1f;
            }
        }

        public static float GetBossKillMultiplier() {
            if(!Main.enableBossKillDifficulty.Value) {
                ConsoleLog.Print("Boss Multiplier not enabled! - Returning 1f");
                return 1f;
            }

            return 1f + Main.bossKillMultiplier.Value * GetBossKillCount();
        }

        public static float GetNightMultiplier() {
            if(!Main.enableNightDifficulty.Value) {
                ConsoleLog.Print("Night Multiplier not enabled! - Returning 1f");
                return 1f;
            }

            if(!EnvMan.instance.IsNight()) {
                ConsoleLog.Print("It's not night! - Returning 1f");
                return 1f;
            }

            return 1f + Main.nightMultiplier.Value;
        }

        public static int GetBossKillCount() {
            int bossKills = 0;

            string[] globalBossKeys = Main.bossGlobalKeys.Value.Split(',');

            foreach(string key in globalBossKeys) {
                if(ZoneSystem.instance.GetGlobalKey(key.Trim())) {
                    bossKills++;
                }
            }

            return bossKills;
        }

        public static float GetOverallHealthMultiplier() {
            return Main.overallHealthMultipler.Value;
        }

        public static float GetOverallDamageMultiplier() {
            return Main.overallDamageMultipler.Value;
        }
    }
}
