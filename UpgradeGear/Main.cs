using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Managers;
using Jotunn.Utils;
using ServerSync;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static ItemDrop;

namespace Cozyheim.UpgradeUnlimited {
    [BepInPlugin(GUID, modName, version)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    [BepInDependency("randyknapp.mods.auga", BepInDependency.DependencyFlags.SoftDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class Main : BaseUnityPlugin {
        // Mod information
        internal const string modName = "UpgradeUnlimited";
        internal const string version = "0.0.1";
        internal const string GUID = "dk.thrakal." + modName;

        // Core objects that is required to patch and configure the mod
        internal readonly Harmony harmony = new Harmony(GUID);
        internal static ConfigSync configSync = new ConfigSync(GUID) { DisplayName = modName, CurrentVersion = version, MinimumRequiredVersion = version };
        internal static ConfigFile configFile;

        void Awake() {
            configFile = new ConfigFile(Config.ConfigFilePath, true);
            configFile.SaveOnConfigSet = true;

            ConfigSettings.Init();

            CommandManager.Instance.AddConsoleCommand(new ConsoleLog());
            PrefabManager.OnVanillaPrefabsAvailable += UpdateAllRecipes;

            // Check player inventories
            UpdatePlayerInventory inventory = new GameObject().AddComponent<UpdatePlayerInventory>();
            DontDestroyOnLoad(inventory.gameObject);

            harmony.PatchAll();
        }

        private void UpdateAllRecipes() {
            ConsoleLog.Print("Updating all recipes", LogType.Message);

            for(int i = 0; i < ObjectDB.instance.m_recipes.Count; i++) {
                Recipe recipe = ObjectDB.instance.m_recipes[i];
                ItemDrop.ItemData.ItemType itemType = recipe.m_item.m_itemData.m_shared.m_itemType;

                if(itemType == ItemDrop.ItemData.ItemType.None) {
                    continue;
                }

                ObjectDB.instance.m_recipes[i].m_item.m_itemData.m_shared.m_maxQuality = ConfigSettings.maxItemLevel.Value;
            }
        }

        internal static bool IsItemAllowed(ItemDrop itemDrop) {
            return IsItemAllowed(itemDrop.m_itemData);
        }

        internal static bool IsItemAllowed(ItemData itemData) {
            List<int> allowedTypes = new List<int>() {
                    3, 4, 5, 6, 7, 11, 12, 14, 15, 17, 18, 19, 22
                };

            if(allowedTypes.Contains((int)itemData.m_shared.m_itemType)) {
                return true;
            }

            return false;
        }

            void OnDestroy() {
            harmony.UnpatchSelf();
        }
    }
}
