using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Managers;
using Jotunn.Utils;
using ServerSync;
using System.Collections.Generic;

namespace Cozyheim.BurnBabyBurn
{
    [BepInPlugin(GUID, modName, version)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class Main : BaseUnityPlugin
    {
        // Mod information
        internal const string modName = "BurnBabyBurn";
        internal const string version = "0.0.1";
        internal const string GUID = "dk.thrakal." + modName;

        // Core objects that is required to patch and configure the mod
        private readonly Harmony harmony = new Harmony(GUID);
        internal static ConfigSync configSync = new ConfigSync(GUID) { DisplayName = modName, CurrentVersion = version, MinimumRequiredVersion = version };
        internal static ConfigFile configFile;

        // Config entries
        internal static ConfigEntry<bool> modEnabled;
        internal static ConfigEntry<bool> debugEnabled;

        internal static ConfigEntry<string> excludedFireplaces;

        internal static List<string> excludedFireplacesList = new List<string>();

        void Awake()
        {
            configFile = new ConfigFile(Config.ConfigFilePath, true);
            configFile.SaveOnConfigSet = true;

            // Assigning config entries
            modEnabled = CreateConfigEntry("General", "ModEnabled", true, "Enable this mod", false); // false = non-synced (client)
            debugEnabled = CreateConfigEntry("General", "DebugEnabled", false, "Display debug messages in the console", true); // true = synced (server)
            excludedFireplaces = CreateConfigEntry("General", "ExcludedFireplaces", "", "Add name of fireplaces which must be excluded. Seperate by comma", true); // true = synced 

            // Removes this object if the mod is not enabled
            if (!modEnabled.Value)
            {
                Destroy(gameObject);
            }

            string[] fireplaces = excludedFireplaces.Value.Split(',');
            excludedFireplacesList = new List<string>();
            foreach (string s in fireplaces)
            {
                excludedFireplacesList.Add(s.Replace(" ", ""));
            }

            harmony.PatchAll();
            CommandManager.Instance.AddConsoleCommand(new ConsoleLog());
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
