using BepInEx.Configuration;
using ServerSync;

namespace Cozyheim.UpgradeUnlimited {
    internal class ConfigSettings {
        // Config entries
        internal static ConfigEntry<bool> modEnabled;
        internal static ConfigEntry<bool> debugEnabled;

        public static ConfigEntry<int> maxItemLevel;
        public static ConfigEntry<int> forgeUpgradeLevelLimit;
        public static ConfigEntry<int> workbenchUpgradeLevelLimit;
        public static ConfigEntry<string> customCraftingStationUpgradeLevelLimit;
        public static ConfigEntry<int> unknownCraftingStationLevelLimit;
        public static ConfigEntry<float> statsIncrease;

        public static void Init() {
            // Assigning config entries
            modEnabled = CreateConfigEntry("General", "ModEnabled", true, "Enable this mod", true);
            debugEnabled = CreateConfigEntry("General", "DebugEnabled", true, "Display debug messages in the console", false);

            maxItemLevel = CreateConfigEntry("Settings", "maxItemLevel", 10, "Max item upgrade level", true);
            forgeUpgradeLevelLimit = CreateConfigEntry("Settings", "forgeUpgradeLevelLimit", 7, "When upgrading items, this is the level limit which is required. (1 = forge only has to be level 1 to upgrade unlimited)", true);
            workbenchUpgradeLevelLimit = CreateConfigEntry("Settings", "workbenchUpgradeLevelLimit", 5, "When upgrading items, this is the level limit which is required. (1 = workbench only has to be level 1 to upgrade unlimited)", true);
            unknownCraftingStationLevelLimit = CreateConfigEntry("Settings", "unknownCraftingStationLevelLimit", 1, "When upgrading items, this is the level limit which is required. (1 = other crafting stations only has to be level 1 to upgrade unlimited)", true);
            customCraftingStationUpgradeLevelLimit = CreateConfigEntry("Settings", "customCraftingStationUpgradeLevelLimit", "", "When upgrading items, this is the level limit which is required. Must follow this format -> craftingStationName:levelLimit. (1 = custom crafting station only has to be level 1 to upgrade unlimited)", true);
            statsIncrease = CreateConfigEntry("Settings", "statsIncrease", 1f, "A stats multiplier added for each upgrade. (1 = normal upgrade stats, 2 = 200% upgrade stats, 0.5 = 50% upgrade stats)", true);
        }

        public class Test {
            public string name;
            public int value;
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
