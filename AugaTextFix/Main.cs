using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Managers;
using Jotunn.Utils;
using ServerSync;
using System.Collections.Generic;

namespace Cozyheim.AugaTextFix {
    [BepInPlugin(GUID, modName, version)]
    [BepInDependency("randyknapp.mods.auga", BepInDependency.DependencyFlags.SoftDependency)]
    internal class Main : BaseUnityPlugin {
        // Mod information
        internal const string modName = "AugaTextFix";
        internal const string version = "0.0.1";
        internal const string GUID = "dk.thrakal." + modName;

        // Core objects that is required to patch and configure the mod
        private readonly Harmony harmony = new Harmony(GUID);
        internal static ConfigSync configSync = new ConfigSync(GUID) { DisplayName = modName, CurrentVersion = version, MinimumRequiredVersion = version };
        internal static ConfigFile configFile;

        void Awake() {
            configFile = new ConfigFile(Config.ConfigFilePath, true);
            configFile.SaveOnConfigSet = true;

            ConfigSettings.Init();

            if(ConfigSettings.modEnabled.Value) {
                CommandManager.Instance.AddConsoleCommand(new ConsoleLog());
                if(CheckIfModIsLoaded("randyknapp.mods.auga")) {
                    harmony.PatchAll();
                }
            }
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

        void OnDestroy() {
            if(ConfigSettings.modEnabled.Value) {
                harmony.UnpatchSelf();
            }
        }
    }
}
