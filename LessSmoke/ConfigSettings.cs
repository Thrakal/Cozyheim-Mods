using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Managers;
using ServerSync;

namespace Cozyheim.LessSmoke
{
    internal class ConfigSettings
    {
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

        public static void Init()
        {
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
        }


        #region CreateConfigEntry Wrapper
        static ConfigEntry<T> CreateConfigEntry<T>(string group, string name, T value, ConfigDescription description, bool synchronizedSetting = true)
        {
            ConfigEntry<T> configEntry = Main.configFile.Bind(group, name, value, description);

            SyncedConfigEntry<T> syncedConfigEntry = Main.configSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }

        static ConfigEntry<T> CreateConfigEntry<T>(string group, string name, T value, string description, bool synchronizedSetting = true) => CreateConfigEntry(group, name, value, new ConfigDescription(description), synchronizedSetting);
        #endregion
    }
}
