using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Managers;
using Jotunn.Utils;
using ServerSync;

namespace Cozyheim.JumpWithMount {
    [BepInPlugin(GUID, modName, version)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class Main : BaseUnityPlugin {
        // Mod information
        internal const string modName = "JumpWithMount";
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
            CommandManager.Instance.AddConsoleCommand(new ConsoleLog());

            if(ConfigSettings.modEnabled.Value) {
                harmony.PatchAll();
            }
        }

        void OnDestroy() {
            harmony.UnpatchSelf();
        }
    }
}
