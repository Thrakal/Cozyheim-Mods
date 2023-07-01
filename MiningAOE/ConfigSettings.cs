using BepInEx.Configuration;
using ServerSync;
using UnityEngine;

namespace Cozyheim.MiningAOE {
    internal class ConfigSettings {
        // Config entries
        internal static ConfigEntry<bool> modEnabled;
        internal static ConfigEntry<bool> debugEnabled;

        internal static ConfigEntry<AOEType> aoeType;
        internal static ConfigEntry<float> minSkillRadius;
        internal static ConfigEntry<float> maxSkillRadius;
        internal static ConfigEntry<KeyCode> aoeKey;

        public static void Init() {
            // Assigning config entries
            modEnabled = CreateConfigEntry("General", "ModEnabled", true, "Enable this mod", true);
            debugEnabled = CreateConfigEntry("General", "DebugEnabled", false, "Display debug messages in the console", false);


            aoeType = CreateConfigEntry("Settings", "aoeType", AOEType.PickaxeSkill, "Determines how the aoe effect should function. (Disabled = No AOE effect, PickaxeSkill = Uses Pickaxe skill to determine value between min and max, AlwaysMinimum = Forces the minimum range to be used, AlwaysMaximum = Forces the maximum range to be used", false);
            minSkillRadius = CreateConfigEntry("Settings", "minSkillRadius", 0f, "The AOE range at pickaxe skill level 0", true);
            maxSkillRadius = CreateConfigEntry("Settings", "maxSkillRadius", 4f, "The AOE range at pickaxe skill level 100", true);
            aoeKey = CreateConfigEntry("Settings", "aoeKey", KeyCode.LeftShift, "Key that must be held down to enable AOE hits while mining", false);
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

public enum AOEType {
    Disabled,
    PickaxeSkill,
    AlwaysMinimum,
    AlwaysMaximum
}