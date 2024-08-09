using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Managers;
using Jotunn.Utils;
using ServerSync;
using System.Reflection;
using System.Linq;
using UnityEngine;
using BepInEx.Bootstrap;
using System.Collections.Generic;

// To-Do List
// - Claim bed only 1 within range of another bed

namespace CozyheimTweaks
{
    [BepInPlugin(GUID, modName, version)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    [BepInDependency("com.undeadbits.valheimmods.portalstation", BepInDependency.DependencyFlags.SoftDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class Main : BaseUnityPlugin
    {
        // Mod information
        internal const string modName = "CozyheimTweaks";
        internal const string version = "0.1.1";
        internal const string GUID = "dk.thrakal." + modName;

        // Core objects that is required to patch and configure the mod
        private readonly Harmony harmony = new Harmony(GUID);
        internal static ConfigSync configSync = new ConfigSync(GUID) { DisplayName = modName, CurrentVersion = version, MinimumRequiredVersion = version };

        // Config files
        internal static ConfigFile mainConfig;
        internal static ConfigFile playerTweaksConfig;
        internal static ConfigFile sleepBetterConfig;
        internal static ConfigFile mistlandsConfig;
        internal static ConfigFile honeyConfig;

        // Config entries
        internal static ConfigEntry<bool> debugMode;

        internal static ConfigEntry<float> pickupRadius;
        internal static ConfigEntry<float> useDistance;
        internal static ConfigEntry<bool> enableFastTelport;

        internal static ConfigEntry<bool> enableCraftingItems;
        internal static ConfigEntry<bool> enableRecipes;

        internal static ConfigEntry<float> maxHoney;
        internal static ConfigEntry<float> honeyGenerateTime;
        internal static ConfigEntry<float> resizeBeehive;

        internal static ConfigEntry<bool> useCustomSleepMessages;
        internal static ConfigEntry<string> customSleepMessages;
        internal static ConfigEntry<float> fireRange;
        internal static ConfigEntry<bool> sleepDuringDaytime;
        internal static ConfigEntry<bool> sleepWithEnemiesNearby;
        internal static ConfigEntry<bool> sleepWhileWet;
        internal static ConfigEntry<bool> disableSleepingUI;

        internal static ConfigEntry<float> misterRadius;
        internal static ConfigEntry<bool> enableLocalMist;

        internal static bool isPortalStationsLoaded = false;

        void Awake()
        {
            harmony.PatchAll();

            mainConfig = new ConfigFile(BepInEx.Paths.ConfigPath + "/Cozyheim/mainConfig.cfg", true);
            mainConfig.SaveOnConfigSet = true;

            playerTweaksConfig = new ConfigFile(BepInEx.Paths.ConfigPath + "/Cozyheim/playerTweaksConfig.cfg", true);
            playerTweaksConfig.SaveOnConfigSet = true;

            sleepBetterConfig = new ConfigFile(BepInEx.Paths.ConfigPath + "/Cozyheim/sleepBetterConfig.cfg", true);
            sleepBetterConfig.SaveOnConfigSet = true;

            mistlandsConfig = new ConfigFile(BepInEx.Paths.ConfigPath + "/Cozyheim/mistlandsConfig.cfg", true);
            mistlandsConfig.SaveOnConfigSet = true;

            honeyConfig = new ConfigFile(BepInEx.Paths.ConfigPath + "/Cozyheim/honeyConfig.cfg", true);
            honeyConfig.SaveOnConfigSet = true;

            // Assigning config entries -> true = synced (server), false = non-synced (client)
            // 00: General Settings
            debugMode = CreateConfigEntry("General", "debugMode", true, "Display debug messages in the console", mainConfig);

            // 01: Player Tweaks config
            pickupRadius = CreateConfigEntry("Player Tweaks", "pickupRadius", 3f, "Default for vanilla valheim is '2'", playerTweaksConfig);
            useDistance = CreateConfigEntry("Player Tweaks", "craftingStationInteractRadius", 4f, "Default for vanilla valheim is '2'", playerTweaksConfig);
            enableFastTelport = CreateConfigEntry("Player Tweaks", "fastTeleport", true, "Remove the teleport animation and makes it instant", playerTweaksConfig);
            
            // 03: Sleep Better config
            useCustomSleepMessages = CreateConfigEntry("Sleep Better", "enableCustomSleepMessages", true, "Use custom sleep messages", sleepBetterConfig);
            customSleepMessages = CreateConfigEntry("Sleep Better", "customSleepMessages", "Sweet dreams, Have a great night, See you tomorrow", "Write your own sleep messages. A random will be chosen every time you go to sleep. Add more messages by seperating every message with a comma", sleepBetterConfig);
            fireRange = CreateConfigEntry("Sleep Better", "fireRange", 15f, "Default for vanilla valheim is '0'. Range to check if a fire is nearby", sleepBetterConfig);
            sleepDuringDaytime = CreateConfigEntry("Sleep Better", "sleepDuringDaytime", false, "Allows you to sleep during daytime", sleepBetterConfig);
            sleepWithEnemiesNearby = CreateConfigEntry("Sleep Better", "sleepWithEnemiesNearby", false, "Allows you to sleep while enemies are nearby", sleepBetterConfig);
            sleepWhileWet = CreateConfigEntry("Sleep Better", "sleepWhileWet", false, "Allows you to sleep while being wet", sleepBetterConfig);
            disableSleepingUI = CreateConfigEntry("Sleep Better", "disableSleepingTextUI", true, "Allows you to see a timelapse of the world, instead of a black screen with text", sleepBetterConfig);

            // 04: Mistlands mist config
            misterRadius = CreateConfigEntry("Mistlands Settings", "misterRadius", 50f, "Default for vanilla valheim is '35'", mistlandsConfig);
            enableLocalMist = CreateConfigEntry("Mistlands Settings", "enableLocalMist", false, "Default for vanilla valheim is 'true'. Enable locally generated mist when in Mistlands", mistlandsConfig);

            // 06: Honey/beehive config
            maxHoney = CreateConfigEntry("Honey/Beehive Settings", "maxHoney", 8f, "Default for vanilla valheim is '4'", honeyConfig);
            honeyGenerateTime = CreateConfigEntry("Honey/Beehive Settings", "honeyGenerateTime", 900f, "Default for vanilla valheim is '1200'", honeyConfig);
            resizeBeehive = CreateConfigEntry("Honey/Beehive Settings", "resizeBeehive", 0.6f, "Default for vanilla valheim is '1'", honeyConfig);

            // Init all patches
            OnlyOneBed.Init();

            isPortalStationsLoaded = CheckIfModIsLoaded("com.undeadbits.valheimmods.portalstation");

            GameObject go = new GameObject();
            go.AddComponent<PortalStationFix>();

            CommandManager.Instance.AddConsoleCommand(new ToppLog());
        }

        private bool CheckIfModIsLoaded(string modGUID) {
            foreach(KeyValuePair<string, PluginInfo> plugin in Chainloader.PluginInfos) {
                BepInPlugin pluginData = plugin.Value.Metadata;
                if(pluginData.GUID.Equals(modGUID)) {
                    return true;
                }
            }

            return false;
        }

        void OnDestroy()
        {
            harmony.UnpatchSelf();
        }

        #region CreateConfigEntry Wrapper
        public static AssetBundle GetAssetBundleFromResources(string fileName)
        {
            var execAssembly = Assembly.GetExecutingAssembly();

            var resourceName = execAssembly.GetManifestResourceNames().Single(str => str.EndsWith(fileName));

            using (var stream = execAssembly.GetManifestResourceStream(resourceName))
            {
                return AssetBundle.LoadFromStream(stream);
            }
        }

        internal static ConfigEntry<T> CreateConfigEntry<T>(string group, string name, T value, string description, bool synchronizedSetting = true, ConfigFile file = null)
        {
            ConfigDescription configDescription = new ConfigDescription(description);

            if(file == null)
            {
                file = mainConfig;
            }
            ConfigEntry<T> configEntry = file.Bind(group, name, value, configDescription);
            SyncedConfigEntry<T> syncedConfigEntry = configSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;
            return configEntry;
        }

        internal static ConfigEntry<T> CreateConfigEntry<T>(string group, string name, T value, string description, ConfigFile file) => CreateConfigEntry(group, name, value, description, true, file);
        #endregion
    }
}
