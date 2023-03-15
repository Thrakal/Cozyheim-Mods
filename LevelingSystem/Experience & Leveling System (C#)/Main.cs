using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Managers;
using Jotunn.Utils;
using ServerSync;
using System.Reflection;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Jotunn.Entities;
using System.IO;
using BepInEx.Bootstrap;

namespace Cozyheim.LevelingSystem
{
    [BepInPlugin(GUID, modName, version)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    [BepInDependency("randyknapp.mods.auga", BepInDependency.DependencyFlags.SoftDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class Main : BaseUnityPlugin
    {
        public enum Position
        {
            Above,
            Below
        };

        // Mod information
        internal const string modName = "LevelingSystem";
        internal const string version = "0.2.1";
        internal const string GUID = "dk.thrakal." + modName;

        // Core objects that is required to patch and configure the mod
        private readonly Harmony harmony = new Harmony(GUID);
        internal static ConfigSync configSync = new ConfigSync(GUID) { DisplayName = modName, CurrentVersion = version, MinimumRequiredVersion = version };
        internal static ConfigFile configFile;

        // Asset bundles
        internal static string assetsPath = "Assets/Leveling System/";
        internal static AssetBundle assetBundle;
        internal static string assetsItemPath = "Assets/_CustomItems/05_LevelingSystem/";
        internal static AssetBundle assetItemBundle;

        // Check for other mods loaded
        internal static bool modAugaLoaded = false;

        // Config entries
        // -----------

        // General
        internal static ConfigEntry<bool> modEnabled;
        internal static ConfigEntry<bool> debugEnabled;

        // XP Bar
        internal static ConfigEntry<bool> showLevel;
        internal static ConfigEntry<bool> showXp;
        internal static ConfigEntry<bool> showRequiredXp;
        internal static ConfigEntry<bool> showPercentageXP;
        internal static ConfigEntry<float> xpBarSize;
        internal static ConfigEntry<Vector2> xpBarPosition;
        internal static ConfigEntry<Position> xpBarLevelTextPosition;

        // Skills Menu
        internal static ConfigEntry<bool> showScrollbar;

        // VFX
        internal static ConfigEntry<bool> levelUpVFX;

        // XP Text
        internal static ConfigEntry<bool> displayXPInCorner;
        internal static ConfigEntry<bool> displayXPFloatingText;
        internal static ConfigEntry<float> xpFontSize;

        // XP Table
        internal static ConfigEntry<string> monsterXpTable;
        internal static ConfigEntry<string> playerXpTable;

        internal static ConfigEntry<bool> pickableXpEnabled;
        internal static ConfigEntry<string> pickableXpTable;
        internal static ConfigEntry<bool> miningXpEnabled;
        internal static ConfigEntry<string> miningXpTable;
        internal static ConfigEntry<bool> woodcuttingXpEnabled;
        internal static ConfigEntry<string> woodcuttingXpTable;

        // XP Multipliers
        internal static ConfigEntry<float> allXPMultiplier;
        internal static ConfigEntry<float> monsterLvlXPMultiplier;
        internal static ConfigEntry<float> restedXPMultiplier;
        internal static ConfigEntry<float> baseXpSpreadMin;
        internal static ConfigEntry<float> baseXpSpreadMax;

        void Awake()
        {
            modAugaLoaded = CheckIfModIsLoaded("randyknapp.mods.auga");

            harmony.PatchAll();
            configFile = new ConfigFile(Config.ConfigFilePath, true);
            configFile.SaveOnConfigSet = true;

            // Asset Bundle loaded
            assetBundle = GetAssetBundleFromResources("leveling_system");
            assetItemBundle = GetAssetBundleFromResources("leveling_assets");
            PrefabManager.OnVanillaPrefabsAvailable += LoadAssets;

            // Assigning config entries
            modEnabled = CreateConfigEntry("General", "modEnabled", true, "Enable this mod", true);
            debugEnabled = CreateConfigEntry("General", "debugEnabled", false, "Display debug messages in the console", false);

            // XP Bar
            showLevel = CreateConfigEntry("XP Bar", "showLevel", true, "Display Level text", false);
            showXp = CreateConfigEntry("XP Bar", "showXp", true, "Display XP text", false);
            showRequiredXp = CreateConfigEntry("XP Bar", "showRequiredXp", true, "Display XP required for next level. (ShowXP must be true) ", false);
            showPercentageXP = CreateConfigEntry("XP Bar", "showPercentageXP", true, "Display XP required for next level.", false);
            xpBarSize = CreateConfigEntry("XP Bar", "xpBarSize", 100f, "The width in percentage (%) of the default xp bar width. (100 = default size, 50 = half the size)", false);
            xpBarPosition = CreateConfigEntry("XP Bar", "xpBarPosition", new Vector2(0f,0f), "The offset position in (x,y) coordinates, from its default position. (x: 0.0 = center of screen, y: 0.0 = bottom of screen, y: 950.0 = top of screen)", false);
            xpBarLevelTextPosition = CreateConfigEntry("XP Bar", "xpBarLevelTextPosition", Position.Above, "The position of the level text, relative to the xp bar.", false);

            // Skills Menu
            showScrollbar = CreateConfigEntry("Skills Menu", "showScrollbar", true, "Display the scroll bar. (Setting to false only disables the graphics, you can still keep scrolling)", false);

            // VFX
            levelUpVFX = CreateConfigEntry("VFX", "levelUpVFX", true, "Display visual effects when leveling up", false);

            // XP Text
            displayXPInCorner = CreateConfigEntry("XP Text", "displayXPInCorner", true, "Display XP gained in top left corner", false);
            displayXPFloatingText = CreateConfigEntry("XP Text", "displayXPFloatingText", true, "Display XP gained as floating text", false);
            xpFontSize = CreateConfigEntry("XP Text", "xpFontSize", 100f, "The size  (in percentage) of the floating xp text. (100 = 100%, 50 = 50% etc.)", false);

            // XP Multipliers
            allXPMultiplier = CreateConfigEntry("XP Multipliers", "XPMultipliers", 100f, "XP gained (in percentage) compared to the Monster XP Table. (100 = Same as XP table, 150 = +50%, 70 = -30%)", true);
            monsterLvlXPMultiplier = CreateConfigEntry("XP Multipliers", "monsterLvlXPMultiplier", 50f, "Bonus XP gained per monster level. (0 = No Bonus, 50 = +50% per level)", true);
            restedXPMultiplier = CreateConfigEntry("XP Multipliers", "restedXPMultiplier", 30f, "Bonus XP gained while rested. (0 = No Bonus, 30 = +30%)", true);
            baseXpSpreadMin = CreateConfigEntry("XP Multipliers", "baseXpSpreadMin", 5f, "Base XP spread, Minimum. (0 = Same as XP table, 5 = -5% from XP table) Used to ensure that the same monster don't reward the exact same amount of XP every time.", true);
            baseXpSpreadMax = CreateConfigEntry("XP Multipliers", "baseXpSpreadMax", 5f, "Base XP spread, Maximum. (0 = Same as XP table, 5 = +5% from XP table) Used to ensure that the same monster don't reqard the exact same amount of XP every time.", true);

            SkillConfig.Init();



            // Generate monsterXPTable default
            int counter = 0;
            string monsterTableDefault = "";
            foreach (KeyValuePair<string, int> kvp in XPTable.monsterXPTable)
            {
                monsterTableDefault += counter != 0 ? ", " : "";
                monsterTableDefault += kvp.Key + ":" + kvp.Value.ToString();
                counter++;
            }
            monsterXpTable = CreateConfigEntry("XP Table", "monsterXpTable", monsterTableDefault, "The base xp of monsters. (Changes requires to realod the config file)", true);


            // Generate playerXPTable default
            XPTable.GenerateDefaultPlayerXPTable();

            string playerTableDefault = "";
            for (int i = 0; i < XPTable.playerXPTable.Length; i++)
            {
                playerTableDefault += i != 0 ? ", " : "";
                playerTableDefault += "Lv" + (i + 1).ToString() + ":" + XPTable.playerXPTable[i];
            }
            playerXpTable = CreateConfigEntry("XP Table", "playerXpTable", playerTableDefault, "The xp needed for each level. To reach a higher max level, simply add more values to the table. (Changes requires to reload the config file, which can be done in two ways. 1. Restart the server.  -  2. Admins can open the console in-game and type LevelingSystem ReloadConfig)", true);


            // Generate pickableXPTable default
            string pickableTableDefault = GenerateDefaultXPTableString(XPTable.pickableXPTable);
            pickableXpEnabled = CreateConfigEntry("XP Table", "pickableXpEnabled", true, "Gain XP when interacting with Pickables", true);
            pickableXpTable = CreateConfigEntry("XP Table", "pickableXpTable", pickableTableDefault, "The base xp of pickables. (Changes requires to reload the config file)", true);

            // Mining
            string miningTableDefault = GenerateDefaultXPTableString(XPTable.miningXPTable);
            miningXpEnabled = CreateConfigEntry("XP Table", "miningXpEnabled", true, "Gain XP when mining", true);
            miningXpTable = CreateConfigEntry("XP Table", "miningXpTable", miningTableDefault, "The base xp for mining. (Changes requires to reload the config file)", true);

            // Woodcutting
            string woodcuttingTableDefault = GenerateDefaultXPTableString(XPTable.woodcuttingXPTable);
            woodcuttingXpEnabled = CreateConfigEntry("XP Table", "woodcuttingXpEnabled", true, "Gain XP when chopping trees", true);
            woodcuttingXpTable = CreateConfigEntry("XP Table", "woodcuttingXpTable", woodcuttingTableDefault, "The base xp for woodcutting. (Changes requires to reload the config file)", true);


            CommandManager.Instance.AddConsoleCommand(new ConsoleLog());
            ConsoleLog.Init();

            UIManager.Init();
            XPManager.Init();
        }

        private string GenerateDefaultXPTableString(Dictionary<string, int> xpTable)
        {
            int counter = 0;
            string returnValue = "";
            foreach (KeyValuePair<string, int> kvp in xpTable)
            {
                returnValue += counter != 0 ? ", " : "";
                returnValue += kvp.Key + ":" + kvp.Value.ToString();
                counter++;
            }

            return returnValue;
        }


        private bool CheckIfModIsLoaded(string modGUID)
        {
            foreach(KeyValuePair<string, PluginInfo> plugin in Chainloader.PluginInfos)
            {
                BepInPlugin pluginData = plugin.Value.Metadata;
                if(pluginData.GUID.Equals(modGUID))
                {
                    return true;
                }
            }

            return false;
        }

        private void LoadAssets()
        {
            // Canvas UI with the XP Bar
            GameObject levelSystem = assetBundle.LoadAsset<GameObject>(assetsPath + "Prefabs/LevelingSystemUI.prefab");
            levelSystem.AddComponent<UIManager>();
            levelSystem.AddComponent<SkillManager>();
            PrefabManager.Instance.AddPrefab(levelSystem);

            GameObject xpText = assetBundle.LoadAsset<GameObject>(assetsPath + "Prefabs/XPText.prefab");
            xpText.AddComponent<XPText>();
            PrefabManager.Instance.AddPrefab(xpText);

            GameObject levelUpEffect = assetBundle.LoadAsset<GameObject>(assetsPath + "Prefabs/LevelUpEffectNew.prefab");
            PrefabManager.Instance.AddPrefab(levelUpEffect);

            GameObject skillUI = assetBundle.LoadAsset<GameObject>(assetsPath + "Prefabs/SkillUI.prefab");
            PrefabManager.Instance.AddPrefab(skillUI);

            GameObject trainingDummy = assetItemBundle.LoadAsset<GameObject>(assetsItemPath + "Prefabs/LevelingDummy.prefab");
            PieceManager.Instance.AddPiece(new CustomPiece(trainingDummy, "Hammer", false));

            GameObject trainingDummyStrawman = assetItemBundle.LoadAsset<GameObject>(assetsItemPath + "Prefabs/LevelingDummyStrawman.prefab");
            PieceManager.Instance.AddPiece(new CustomPiece(trainingDummyStrawman, "Hammer", false));

            PrefabManager.OnVanillaPrefabsAvailable -= LoadAssets;
        }

        public static AssetBundle GetAssetBundleFromResources(string fileName)
        {
            var execAssembly = Assembly.GetExecutingAssembly();

            var resourceName = execAssembly.GetManifestResourceNames().Single(str => str.EndsWith(fileName));

            using (var stream = execAssembly.GetManifestResourceStream(resourceName))
            {
                return AssetBundle.LoadFromStream(stream);
            }
        }

        void OnDestroy()
        {
            harmony.UnpatchSelf();
        }

        public static Sprite GetSpriteFromResources(string filePath)
        {
            Texture2D texture = null;
            byte[] data;

            data = File.ReadAllBytes(filePath);
            texture = new Texture2D(2, 2);
            texture.SetPixelData(data, 0);

            texture.LoadImage(data);

            Sprite newSprite = Sprite.Create(texture, new Rect(0.5f, 0.5f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);

            return newSprite;
        }

        #region CreateConfigEntry Wrapper
        public static ConfigEntry<T> CreateConfigEntry<T>(string group, string name, T value, ConfigDescription description, bool synchronizedSetting = true)
        {
            ConfigEntry<T> configEntry = configFile.Bind(group, name, value, description);

            SyncedConfigEntry<T> syncedConfigEntry = configSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }

        public static ConfigEntry<T> CreateConfigEntry<T>(string group, string name, T value, string description, bool synchronizedSetting = true) => CreateConfigEntry(group, name, value, new ConfigDescription(description), synchronizedSetting);
        #endregion
    }
}
