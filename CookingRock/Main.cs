using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using ServerSync;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Cozyheim.CookingRock
{
    [BepInPlugin(GUID, modName, version)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class Main : BaseUnityPlugin
    {
        // Mod information
        internal const string modName = "CookingRock";
        internal const string version = "0.0.1";
        internal const string GUID = "dk.thrakal." + modName;

        // Core objects that is required to patch and configure the mod
        private readonly Harmony harmony = new Harmony(GUID);
        internal static ConfigSync configSync = new ConfigSync(GUID) { DisplayName = modName, CurrentVersion = version, MinimumRequiredVersion = version };
        internal static ConfigFile configFile;

        private static string assetsPath = "Assets/_CustomItems/06_CookingRock/";
        private static AssetBundle assetBundle;

        private static List<PrefabItem> prefabsList;
        private static string[] recipesList;

        void Awake()
        {
            configFile = new ConfigFile(Config.ConfigFilePath, true);
            configFile.SaveOnConfigSet = true;
            ConfigSettings.Init();

            harmony.PatchAll();

            CommandManager.Instance.AddConsoleCommand(new ConsoleLog());

            CreatePrefabAndRecipeList();
            PrefabManager.OnVanillaPrefabsAvailable += RegisterPieces;
        }

        internal static void CreatePrefabAndRecipeList()
        {
            // Load custom assets from the asset bundle
            assetBundle = GetAssetBundleFromResources("cookingrock");

            // Prefabs
            prefabsList = new List<PrefabItem>
            {
                new PrefabItem("piece_CookingRock_Cozy", Category.Hammer),
                new PrefabItem("HoneyRaspberry_Cozy", Category.Item),
                new PrefabItem("MushriesForest_Cozy", Category.Item),
                new PrefabItem("MushriesMeadows_Cozy", Category.Item),
                new PrefabItem("VolvaAntiPoison_Cozy", Category.Item)
            };

            recipesList = new string[] {
                "HoneyRaspberry",
                "MushriesForest",
                "MushriesMeadows",
                "VolvaAntiPoison"
            };
        }

        internal static void RegisterPieces()
        {
            for (int i = 0; i < prefabsList.Count; i++)
            {
                GameObject prefab = assetBundle.LoadAsset<GameObject>(assetsPath + "Prefabs/" + prefabsList[i].prefabName + ".prefab");
                prefab.name = prefabsList[i].prefabName;

                if (prefabsList[i].category == Category.Hammer)
                {
                    PieceManager.Instance.AddPiece(new CustomPiece(prefab, "Hammer", false));
                }

                if (prefabsList[i].category == Category.Item)
                {
                    ItemManager.Instance.AddItem(new CustomItem(prefab, false));
                }

                ConsoleLog.Print("Added prefab: " + prefab.name);
            }

            RegisterRecipes();
            PrefabManager.OnVanillaPrefabsAvailable -= RegisterPieces;
        }

        internal static void RegisterRecipes()
        {
            for (int i = 0; i < recipesList.Length; i++)
            {
                Recipe recipeToLoad = assetBundle.LoadAsset<Recipe>(assetsPath + "Recipes/Recipe_" + recipesList[i] + "_Cozy.asset");
                ItemManager.Instance.AddRecipe(new CustomRecipe(recipeToLoad, false, false));
            }
        }

        void OnDestroy()
        {
            harmony.UnpatchSelf();
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
    }
}
