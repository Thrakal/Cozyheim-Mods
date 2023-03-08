using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Managers;
using Jotunn.Utils;
using ServerSync;
using UnityEngine;

namespace Cozyheim.TestMod
{
    [BepInPlugin(GUID, modName, version)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class Main : BaseUnityPlugin
    {
        // Mod information
        internal const string modName = "TestMod";
        internal const string version = "0.0.1";
        internal const string GUID = "dk.thrakal." + modName;

        // Core objects that is required to patch and configure the mod
        private readonly Harmony harmony = new Harmony(GUID);
        internal static ConfigSync configSync = new ConfigSync(GUID) { DisplayName = modName, CurrentVersion = version, MinimumRequiredVersion = version };
        internal static ConfigFile configFile;

        // Config entries
        internal static ConfigEntry<bool> modEnabled;
        internal static ConfigEntry<bool> debugEnabled;

        void Awake()
        {
            harmony.PatchAll();
            configFile = new ConfigFile(BepInEx.Paths.ConfigPath + "/Cozyheim/" + modName + "_Config.cfg", true);
            configFile.SaveOnConfigSet = true;

            // Assigning config entries
            modEnabled = CreateConfigEntry("General", "ModEnabled", true, "Enable this mod", false); // false = non-synced (client)
            debugEnabled = CreateConfigEntry("General", "DebugEnabled", true, "Display debug messages in the console", true); // true = synced (server)

            CommandManager.Instance.AddConsoleCommand(new ConsoleLog());
        }

        public static void SpawnTamedAnimal(GameObject animal, int monsterLevel)
        {
            GameObject newAnimal = Instantiate(animal, Player.m_localPlayer.transform.position + Player.m_localPlayer.transform.forward * 2f, Quaternion.identity);

            newAnimal.GetComponent<MonsterAI>().MakeTame();
            newAnimal.GetComponent<MonsterAI>().SetDespawnInDay(false);
            newAnimal.GetComponent<Humanoid>().SetLevel(monsterLevel);
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
