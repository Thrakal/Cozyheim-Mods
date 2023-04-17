using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Managers;
using Jotunn.Utils;
using ServerSync;
using System.Collections.Generic;

namespace Cozyheim.DifficultyScaler
{
    [BepInPlugin(GUID, modName, version)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class Main : BaseUnityPlugin
    {
        // Mod information
        internal const string modName = "DifficultyScaler";
        internal const string version = "0.1.1";
        internal const string GUID = "dk.thrakal." + modName;

        // Core objects that is required to patch and configure the mod
        private readonly Harmony harmony = new Harmony(GUID);
        internal static ConfigSync configSync = new ConfigSync(GUID) { DisplayName = modName, CurrentVersion = version, MinimumRequiredVersion = version };
        internal static ConfigFile configFile;

        // Config entries
        internal static ConfigEntry<bool> modEnabled;
        internal static ConfigEntry<bool> debugEnabled;

        internal static ConfigEntry<float> overallHealthMultipler;
        internal static ConfigEntry<float> overallDamageMultipler;

        internal static ConfigEntry<string> monsterHealthModifier;
        internal static ConfigEntry<string> monsterDamageModifier;

        internal static Dictionary<string, float> monsterHealth;
        internal static Dictionary<string, float> monsterDamage;

        internal static ConfigEntry<bool> enableBossKillDifficulty;
        internal static ConfigEntry<float> bossKillMultiplier;
        internal static ConfigEntry<string> bossGlobalKeys;

        internal static ConfigEntry<bool> enableBiomeDifficulty;
        internal static ConfigEntry<float> meadowsMultiplier;
        internal static ConfigEntry<float> blackForestMultiplier;
        internal static ConfigEntry<float> swampMultiplier;
        internal static ConfigEntry<float> mountainMultiplier;
        internal static ConfigEntry<float> plainsMultiplier;
        internal static ConfigEntry<float> mistlandsMultiplier;
        internal static ConfigEntry<float> oceanMultiplier;
        internal static ConfigEntry<float> deepNorthMultiplier;
        internal static ConfigEntry<float> ashlandsMultiplier;

        internal static ConfigEntry<bool> enableNightDifficulty;
        internal static ConfigEntry<float> nightMultiplier;


        void Awake()
        {
            harmony.PatchAll();
            configFile = new ConfigFile(Config.ConfigFilePath, true);
            configFile.SaveOnConfigSet = true;

            // Assigning config entries
            modEnabled = CreateConfigEntry("00_General", "ModEnabled", true, "Enable this mod", false);
            debugEnabled = CreateConfigEntry("00_General", "DebugEnabled", false, "Display debug messages in the console", true);

            overallHealthMultipler = CreateConfigEntry("01_Monsters", "overallHealthMultipler", 1f, "Increases the base health of all monsters in the game. (1 = No change, 1.5 = 50% more health). This stack with the individual monster modifiers.", true);
            overallDamageMultipler = CreateConfigEntry("01_Monsters", "overallDamageMultipler", 1f, "Increases the base damage of all monsters in the game. (1 = No change, 1.5 = 50% more damage). This stack with the individual monster modifiers.", true);
            monsterHealthModifier = CreateConfigEntry("01_Monsters", "monsterBaseHealthModifier", "", "Set the base health (60 = 60 base health) of individual monsters. Format must follow: Monstername:Health (example: Skeleton:60,Greyling:25)", true);
            monsterDamageModifier = CreateConfigEntry("01_Monsters", "monsterDamageModifier", "", "Set a damage multiplier (110 = +10% increased damage) of individual monsters. Format must follow: Monstername:Damage (example: Skeleton:110,Greyling:120)", true);


            enableBossKillDifficulty = CreateConfigEntry("02_Boss", "enableBossKillDifficulty", false, "Enables difficulty scaling after killing bosses", true);
            bossKillMultiplier = CreateConfigEntry("02_Boss", "bossKillMultiplier", 0.2f, "Increases (multiplies) the health & damage of all monsters by this value, per boss you have killed. (0 = 0% increase per boss, 0.5 = 50% increase per boss)", true);
            bossGlobalKeys = CreateConfigEntry("02_Boss", "bossGlobalKeys", "defeated_eikthyr, defeated_gdking, defeated_bonemass, defeated_dragon, defeated_goblinking, defeated_queen", "Global keys to check against for registering boss kills. Add custom boss keys to this list if you are using custom bosses.", true);


            enableBiomeDifficulty = CreateConfigEntry("03_Biomes", "enableBiomeDifficulty", false, "Enables difficulty scaling for biomes", true);
            meadowsMultiplier = CreateConfigEntry("03_Biomes", "meadowsMultiplier", 0f, "Increases (multiplies) the health & damage of all monsters in this Biome (0 = 0% increase, 0.5 = 50% increase)", true);
            blackForestMultiplier = CreateConfigEntry("03_Biomes", "blackForestMultiplier", 0.2f, "Increases (multiplies) the health & damage of all monsters in this Biome (0 = 0% increase, 0.5 = 50% increase)", true);
            swampMultiplier = CreateConfigEntry("03_Biomes", "swampMultiplier", 0.4f, "Increases (multiplies) the health & damage of all monsters in this Biome (0 = 0% increase, 0.5 = 50% increase)", true);
            oceanMultiplier = CreateConfigEntry("03_Biomes", "oceanMultiplier", 0.5f, "Increases (multiplies) the health & damage of all monsters in this Biome (0 = 0% increase, 0.5 = 50% increase)", true);
            mountainMultiplier = CreateConfigEntry("03_Biomes", "mountainMultiplier", 0.6f, "Increases (multiplies) the health & damage of all monsters in this Biome (0 = 0% increase, 0.5 = 50% increase)", true);
            plainsMultiplier = CreateConfigEntry("03_Biomes", "plainsMultiplier", 0.8f, "Increases (multiplies) the health & damage of all monsters in this Biome (0 = 0% increase, 0.5 = 50% increase)", true);
            mistlandsMultiplier = CreateConfigEntry("03_Biomes", "mistlandsMultiplier", 1f, "Increases (multiplies) the health & damage of all monsters in this Biome (0 = 0% increase, 0.5 = 50% increase)", true);
            deepNorthMultiplier = CreateConfigEntry("03_Biomes", "deepNorthMultiplier", 1f, "Increases (multiplies) the health & damage of all monsters in this Biome (0 = 0% increase, 0.5 = 50% increase)", true);
            ashlandsMultiplier = CreateConfigEntry("03_Biomes", "ashlandsMultiplier", 1f, "Increases (multiplies) the health & damage of all monsters in this Biome (0 = 0% increase, 0.5 = 50% increase)", true);

            enableNightDifficulty = CreateConfigEntry("04_Night", "enableNightDifficulty", false, "Enables difficulty scaling during night", true);
            nightMultiplier = CreateConfigEntry("04_Night", "nightMultiplier", 0.5f, "Increases (multiplies) the health & damage of all monsters during night (0 = 0% increase, 0.5 = 50% increase)", true);


            CommandManager.Instance.AddConsoleCommand(new ConsoleLog());

            PrefabManager.OnVanillaPrefabsAvailable += CreateMonsterHealthDictionary;
        }

        public static void CreateMonsterHealthDictionary()
        {
            monsterHealth = new Dictionary<string, float>();
            monsterHealth.Clear();
            ConsoleLog.Print("-- Changing base HEALTH for monsters: --", LogType.Message);

            string[] monsterArray = monsterHealthModifier.Value.Split(',');
            for (int i = 0; i < monsterArray.Length; i++)
            {
                string[] monsterData = monsterArray[i].Split(':');
                if (monsterData.Length >= 2)
                {
                    if (float.TryParse(monsterData[1], out float health))
                    {
                        float originalHealth = PrefabManager.Instance.GetPrefab(monsterData[0]).GetComponent<Humanoid>().m_health;
                        monsterHealth.Add(monsterData[0].Trim() + "(Clone)", health);
                        ConsoleLog.Print(monsterData[0].Trim() + ": (" + originalHealth + " -> " + health + " HP)");
                    }
                }
            }

            CreateMonsterDamageDictionary();
            PrefabManager.OnVanillaPrefabsAvailable -= CreateMonsterHealthDictionary;
        }

        public static void CreateMonsterDamageDictionary()
        {
            monsterDamage = new Dictionary<string, float>();
            monsterDamage.Clear();
            ConsoleLog.Print("-- Changing base DAMAGE for monsters: --", LogType.Message);

            string[] monsterArray = monsterDamageModifier.Value.Split(',');
            for (int i = 0; i < monsterArray.Length; i++)
            {
                string[] monsterData = monsterArray[i].Split(':');
                if (monsterData.Length >= 2)
                {
                    if (float.TryParse(monsterData[1], out float damage))
                    {
                        ConsoleLog.Print(monsterData[0].Trim() + ": " + damage + "% damage");
                        damage /= 100f;
                        monsterDamage.Add(monsterData[0].Trim() + "(Clone)", damage);
                    }
                }
            }

            ConsoleLog.Print("-- Adjusting ALL monster's base damage and health: --", LogType.Message);
            ConsoleLog.Print("Base health set to " + (overallHealthMultipler.Value * 100).ToString("N0") + "%");
            ConsoleLog.Print("Base damage set to " + (overallDamageMultipler.Value * 100).ToString("N0") + "%");
        }

        void OnDestroy()
        {
            harmony.UnpatchSelf();
        }

        #region CreateConfigEntry Wrapper
        ConfigEntry<T> CreateConfigEntry<T>(string group, string name, T value, ConfigDescription description, bool synchronizedSetting = true)
        {
            ConfigEntry<T> configEntry = configFile.Bind(group, name, value, description);

            SyncedConfigEntry<T> syncedConfigEntry = configSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }

        ConfigEntry<T> CreateConfigEntry<T>(string group, string name, T value, string description, bool synchronizedSetting = true) => CreateConfigEntry(group, name, value, new ConfigDescription(description), synchronizedSetting);
        #endregion
    }
}
