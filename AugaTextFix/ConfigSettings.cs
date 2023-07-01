using BepInEx.Configuration;
using ServerSync;

namespace Cozyheim.AugaTextFix {
    internal class ConfigSettings {
        // Config entries
        internal static ConfigEntry<bool> modEnabled;
        internal static ConfigEntry<bool> debugEnabled;

        public static void Init() {
            // Assigning config entries
            modEnabled = CreateConfigEntry("General", "ModEnabled", true, "Enable this mod", true);
            debugEnabled = CreateConfigEntry("General", "DebugEnabled", true, "Display debug messages in the console", false);
        }

        #region CreateConfigEntry Wrapper
        static ConfigEntry<T> CreateConfigEntry<T>(string group, string name, T value, ConfigDescription description, bool synchronizedSetting = true) {
            ConfigEntry<T> configEntry = Main.configFile.Bind(group, name, value, description);

            SyncedConfigEntry<T> syncedConfigEntry = Main.configSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }

        static ConfigEntry<T> CreateConfigEntry<T>(string group, string name, T value, string description, bool synchronizedSetting = true) => CreateConfigEntry(group, name, value, new ConfigDescription(description), synchronizedSetting);
        #endregion
    }
}
