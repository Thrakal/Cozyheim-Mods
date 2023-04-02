using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Managers;
using Jotunn.Utils;
using ServerSync;
using System.Collections.Generic;
using UnityEngine;

namespace Cozyheim.DifficultyScaler
{
    [BepInPlugin(GUID, modName, version)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class Main : BaseUnityPlugin
    {
        // Mod information
        internal const string modName = "DifficultyScaler";
        internal const string version = "0.0.1";
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
                        monsterHealth.Add(monsterData[0] + "(Clone)", health);
                        ConsoleLog.Print(monsterData[0] + ": (" + originalHealth + " -> " + health + " HP)");
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
                        ConsoleLog.Print(monsterData[0] + ": " + damage + "% damage");
                        damage /= 100f;
                        monsterDamage.Add(monsterData[0] + "(Clone)", damage);
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
