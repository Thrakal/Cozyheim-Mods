using BepInEx.Configuration;
using ServerSync;
using UnityEngine;

namespace Cozyheim.JumpWithMount {
    internal class ConfigSettings : MonoBehaviour {
        // Config entries
        internal static ConfigEntry<bool> modEnabled;
        internal static ConfigEntry<bool> debugEnabled;

//        internal static ConfigEntry<UnmountType> unmountType;
        internal static ConfigEntry<KeyCode> unmountKey;


        public static void Init() {
            // Assigning config entries
            modEnabled = CreateConfigEntry("00 - General", "ModEnabled", true, "Enable this mod", true);
            debugEnabled = CreateConfigEntry("00 - General", "DebugEnabled", false, "Display debug messages in the console", false);
//            unmountType = CreateConfigEntry("01 - Settings", "unmountType", UnmountType.HoldKeyWhileJumping, "The way you would like to interact for unmounting.", false);
            unmountKey = CreateConfigEntry("01 - Settings", "unmountKey", KeyCode.E, "Display debug messages in the console", false);
        }

        public enum UnmountType {
            KeyPress,
            HoldKeyWhileJumping
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
