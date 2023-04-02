using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Managers;
using Jotunn.Utils;
using ServerSync;

namespace Cozyheim.LessSmoke
{
    [BepInPlugin(GUID, modName, version)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class Main : BaseUnityPlugin
    {
        // Mod information
        internal const string modName = "LessSmoke";
        internal const string version = "0.0.1";
        internal const string GUID = "dk.thrakal." + modName;

        // Core objects that is required to patch and configure the mod
        private readonly Harmony harmony = new Harmony(GUID);
        internal static ConfigSync configSync = new ConfigSync(GUID) { DisplayName = modName, CurrentVersion = version, MinimumRequiredVersion = version };
        internal static ConfigFile configFile;

        // Config entries
        internal static ConfigEntry<bool> modEnabled;
        internal static ConfigEntry<bool> debugEnabled;

        internal static ConfigEntry<float> timeUntilFade;
        internal static ConfigEntry<float> fadeTime;
        internal static ConfigEntry<float> sizeMin;
        internal static ConfigEntry<float> sizeMax;
        internal static ConfigEntry<float> moveSpeedMin;
        internal static ConfigEntry<float> moveSpeedMax;
        internal static ConfigEntry<float> spawnIntervalMin;
        internal static ConfigEntry<float> spawnIntervalMax;

        void Awake()
        {
            harmony.PatchAll();
            configFile = new ConfigFile(Config.ConfigFilePath, true);
            configFile.SaveOnConfigSet = true;

            // Assigning config entries
            modEnabled = CreateConfigEntry("General", "ModEnabled", true, "Enable this mod", false); // false = non-synced (client)
            debugEnabled = CreateConfigEntry("General", "DebugEnabled", true, "Display debug messages in the console", true); // true = synced (server)

            modEnabled = CreateConfigEntry("General", "Enable this mod", true, "Allows this mod to be used");
            timeUntilFade = CreateConfigEntry("General", "timeUntilFade", 3f, "Default value for vanilla valheim is '10'");
            fadeTime = CreateConfigEntry("General", "fadeTime", 3f, "Default value for vanilla valheim is '3'");
            sizeMin = CreateConfigEntry("General", "sizeMin", 0.5f, "Minimum size for smoke. Default value for vanilla valheim is '3'");
            sizeMax = CreateConfigEntry("General", "sizeMax", 1.2f, "Maximum size for smoke. Default value for vanilla valheim is '3'");
            moveSpeedMin = CreateConfigEntry("General", "moveSpeedMin", 3.5f, "Minimum move speed for smoke.");
            moveSpeedMax = CreateConfigEntry("General", "moveSpeedMax", 6f, "Maximum move speed for smoke.");
            spawnIntervalMin = CreateConfigEntry("General", "spawnIntervalMin", 0.2f, "Minimum time between each smoke is spawned.");
            spawnIntervalMax = CreateConfigEntry("General", "spawnIntervalMax", 0.5f, "Maximum time between each smoke is spawned");

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
