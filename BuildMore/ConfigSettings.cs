using BepInEx.Configuration;
using ServerSync;

namespace Cozyheim.BuildMore
{
    internal class ConfigSettings
    {
        // Config entries
        internal static ConfigEntry<bool> modEnabled;
        internal static ConfigEntry<bool> debugEnabled;

        internal static ConfigEntry<bool> enableCustomBuildSettings;

        internal static ConfigEntry<bool> buildInsideNoBuild;

        internal static ConfigEntry<bool> makeAllSame;
        internal static ConfigEntry<float> allMaxSupport;
        internal static ConfigEntry<float> allWeight;
        internal static ConfigEntry<float> allBuildVertical;
        internal static ConfigEntry<float> allBuildHorizontal;

        internal static ConfigEntry<float> woodMaxSupport;
        internal static ConfigEntry<float> woodWeight;
        internal static ConfigEntry<float> woodBuildVertical;
        internal static ConfigEntry<float> woodBuildHorizontal;

        internal static ConfigEntry<float> hardwoodMaxSupport;
        internal static ConfigEntry<float> hardwoodWeight;
        internal static ConfigEntry<float> hardwoodBuildVertical;
        internal static ConfigEntry<float> hardwoodBuildHorizontal;

        internal static ConfigEntry<float> stoneMaxSupport;
        internal static ConfigEntry<float> stoneWeight;
        internal static ConfigEntry<float> stoneBuildVertical;
        internal static ConfigEntry<float> stoneBuildHorizontal;

        internal static ConfigEntry<float> ironMaxSupport;
        internal static ConfigEntry<float> ironWeight;
        internal static ConfigEntry<float> ironBuildVertical;
        internal static ConfigEntry<float> ironBuildHorizontal;

        internal static ConfigEntry<float> marbleMaxSupport;
        internal static ConfigEntry<float> marbleWeight;
        internal static ConfigEntry<float> marbleBuildVertical;
        internal static ConfigEntry<float> marbleBuildHorizontal;

        public static void Init()
        {
            // Assigning config entries
            modEnabled = CreateConfigEntry("General", "ModEnabled", true, "Enable this mod", true);
            debugEnabled = CreateConfigEntry("General", "DebugEnabled", true, "Display debug messages in the console", false);

            enableCustomBuildSettings = CreateConfigEntry("Build/Material Settings", "enableCustomBuildSetting", true, "Allow to use custom settings for building");

            buildInsideNoBuild =  CreateConfigEntry("Build/Material Settings", "allowBuildInNoBuildZone", true, "Allow to build inside areas where you normally would see the text 'A mystical force prevents...'");

            // All materials are the same
            makeAllSame = CreateConfigEntry("Build/Material Settings", "makeAllMaterialsTheSame", false, "Let all materials behave in the same way. If this is enabled, it will override all the individual material settings.");
            allMaxSupport = CreateConfigEntry("Build/Material Settings", "allMaxSupport", 100f, "The maximum weight this material can support");
            allWeight = CreateConfigEntry("Build/Material Settings", "allWeight", 10f, "The weight of the build material. If the total weight exceeds the 'max support' of the underlaying material, this material will collapse");
            allBuildVertical = CreateConfigEntry("Build/Material Settings", "allVerticalBuildHeight", 8f, "The height at which you can build before collapse. Minimum value: 1");
            allBuildHorizontal = CreateConfigEntry("Build/Material Settings", "allHorizontalBuildWidth", 5f, "The width at which you can build before collapse. Minimum value: 1");

            // Wood
            woodMaxSupport = CreateConfigEntry("Build/Material Settings", "woodMaxSupport", 100f, "The maximum weight this material can support");
            woodWeight = CreateConfigEntry("Build/Material Settings", "woodWeight", 10f, "The weight of the build material. If the total weight exceeds the 'max support' of the underlaying material, this material will collapse");
            woodBuildVertical = CreateConfigEntry("Build/Material Settings", "woodVerticalBuildHeight", 8f, "The height at which you can build before collapse. Minimum value: 1");
            woodBuildHorizontal = CreateConfigEntry("Build/Material Settings", "woodHorizontalBuildWidth", 5f, "The width at which you can build before collapse. Minimum value: 1");

            // Hard Wood
            hardwoodMaxSupport = CreateConfigEntry("Build/Material Settings", "hardwoodMaxSupport", 140f, "The maximum weight this material can support");
            hardwoodWeight = CreateConfigEntry("Build/Material Settings", "hardwoodWeight", 10f, "The weight of the build material. If the total weight exceeds the 'max support' of the underlaying material, this material will collapse");
            hardwoodBuildVertical = CreateConfigEntry("Build/Material Settings", "hardwoodVerticalBuildHeight", 10f, "The height at which you can build before collapse. Minimum value: 1");
            hardwoodBuildHorizontal = CreateConfigEntry("Build/Material Settings", "hardwoodHorizontalBuildWidth", 6f, "The width at which you can build before collapse. Minimum value: 1");

            // Stone
            stoneMaxSupport = CreateConfigEntry("Build/Material Settings", "stoneMaxSupport", 1000f, "The maximum weight this material can support");
            stoneWeight = CreateConfigEntry("Build/Material Settings", "stoneWeight", 100f, "The weight of the build material. If the total weight exceeds the 'max support' of the underlaying material, this material will collapse");
            stoneBuildVertical = CreateConfigEntry("Build/Material Settings", "stoneVerticalBuildHeight", 8f, "The height at which you can build before collapse. Minimum value: 1");
            stoneBuildHorizontal = CreateConfigEntry("Build/Material Settings", "stoneHorizontalBuildWidth", 1f, "The width at which you can build before collapse. Minimum value: 1");

            // Marble
            marbleMaxSupport = CreateConfigEntry("Build/Material Settings", "marbleMaxSupport", 1500f, "The maximum weight this material can support");
            marbleWeight = CreateConfigEntry("Build/Material Settings", "marbleWeight", 100f, "The weight of the build material. If the total weight exceeds the 'max support' of the underlaying material, this material will collapse");
            marbleBuildVertical = CreateConfigEntry("Build/Material Settings", "marbleVerticalBuildHeight", 8f, "The height at which you can build before collapse. Minimum value: 1");
            marbleBuildHorizontal = CreateConfigEntry("Build/Material Settings", "marbleHorizontalBuildWidth", 2f, "The width at which you can build before collapse. Minimum value: 1");

            // Iron
            ironMaxSupport = CreateConfigEntry("Build/Material Settings", "ironMaxSupport", 1500f, "The maximum weight this material can support");
            ironWeight = CreateConfigEntry("Build/Material Settings", "ironWeight", 20f, "The weight of the build material. If the total weight exceeds the 'max support' of the underlaying material, this material will collapse");
            ironBuildVertical = CreateConfigEntry("Build/Material Settings", "ironVerticalBuildHeight", 13f, "The height at which you can build before collapse. Minimum value: 1");
            ironBuildHorizontal = CreateConfigEntry("Build/Material Settings", "ironHorizontalBuildWidth", 13f, "The width at which you can build before collapse. Minimum value: 1");
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
