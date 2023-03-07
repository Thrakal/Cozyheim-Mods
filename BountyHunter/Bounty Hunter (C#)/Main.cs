using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using ServerSync;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using Jotunn.Configs;

namespace BountyHunter
{
    [BepInPlugin(GUID, modName, version)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class Main : BaseUnityPlugin
    {
        // Mod information
        internal const string modName = "BountyHunter";
        internal const string version = "0.0.1";
        internal const string GUID = "dk.thrakal." + modName;

        // Core objects that is required to patch and configure the mod
        private readonly Harmony harmony = new Harmony(GUID);
        internal static ConfigSync configSync = new ConfigSync(GUID) { DisplayName = modName, CurrentVersion = version, MinimumRequiredVersion = version };
        internal static ConfigFile configFile;

        // Config entries
        internal static ConfigEntry<bool> debugMode;
        internal static ConfigEntry<string> descriptionColor;

        // Creating Custom RPCs
        internal static CustomRPC rpc_requestNumber;
        internal static CustomRPC rpc_sendNumber;

        void Awake()
        {
            harmony.PatchAll();
            configFile = new ConfigFile(Config.ConfigFilePath, true);
            configFile.SaveOnConfigSet = true;

            // Assigning config entries
            debugMode = CreateConfigEntry("General", "Enable debug mode", false, "Displays debug messages in the console", false); // false = non-synced (client)
            descriptionColor = CreateConfigEntry("General", "Description Color", "#3090e0", "Available colors: Custom hexcode (e.g. #3090e0), Red, Orange, Yellow, Green, Teal, Blue, Purple", true);

            // Configure RPC endpoints
            rpc_requestNumber = NetworkManager.Instance.AddRPC("RequestNumber", RPC_Request_RandomNumber, RPC_Request_RandomNumber);
            rpc_sendNumber = NetworkManager.Instance.AddRPC("SendNumber", RPC_Recieve_RandomNumber, RPC_Recieve_RandomNumber);

            // Load assets and set prefab and recipe values
            LoadAssets.CreatePrefabAndRecipeList();

            // Initialize all console commands
            CommandManager.Instance.AddConsoleCommand(new Logger());

            // Check player inventories
            CheckPlayerInventory inventory = new GameObject().AddComponent<CheckPlayerInventory>();
            DontDestroyOnLoad(inventory.gameObject);

            // Bounty Manager
            BountyManager bountyManager = new GameObject().AddComponent<BountyManager>();
            DontDestroyOnLoad(bountyManager.gameObject);
        }

        private void Request_RandomNumber(bool param = false)
        {
            // Get random number
            ZPackage package = new ZPackage();
            package.Write(param);
            rpc_requestNumber.SendPackage(ZRoutedRpc.Everybody, package);
        }

        private IEnumerator RPC_Request_RandomNumber(long sender, ZPackage package)
        {
            // Only handle the package if this is the server
            if (ZNet.instance.IsServer())
            {
                // Code here
            }

            yield return null;
        }

        private IEnumerator RPC_Recieve_RandomNumber(long sender, ZPackage package)
        {
            yield return null;
        }

        void OnDestroy()
        {
            harmony.UnpatchSelf();
        }

        #region Wrappers and helpful methods
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
