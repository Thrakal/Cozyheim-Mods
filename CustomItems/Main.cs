using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using ServerSync;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Cozyheim.CustomItems
{
    [BepInPlugin(GUID, modName, version)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    internal class Main : BaseUnityPlugin
    {
        // Mod information
        internal const string modName = "CustomItems";
        internal const string version = "0.0.1";
        internal const string GUID = "dk.thrakal." + modName;

        // Core objects that is required to patch and configure the mod
        private readonly Harmony harmony = new Harmony(GUID);
        internal static ConfigSync configSync = new ConfigSync(GUID) { DisplayName = modName, CurrentVersion = version, MinimumRequiredVersion = version };
        internal static ConfigFile configFile;

        private static string assetsPath = "Assets/_CustomItems/01_ToppItems/";
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

            AddRemoveItemsRecipesConfig.GenerateLists();
            CreatePrefabAndRecipeList();
            PrefabManager.OnVanillaPrefabsAvailable += RegisterPieces;
        }

        internal static void CreatePrefabAndRecipeList()
        {
            // Load custom assets from the asset bundle
            assetBundle = GetAssetBundleFromResources("toppitems");

            // Prefabs
            prefabsList = new List<PrefabItem>
            {
                new PrefabItem("piece_chest_1x1_stackable_small", "piece_chest_1x1_stackable_small", Category.Building),
                new PrefabItem("piece_chest_1x1_stackable_medium", "piece_chest_1x1_stackable_medium", Category.Building),
                new PrefabItem("piece_chest_1x1_stackable_large", "piece_chest_1x1_stackable_large", Category.Building),
                new PrefabItem("StoneheadArrow", "StoneheadArrow", Category.Crafting),
                new PrefabItem("build_paved_road", "PavedRoadCustom", Category.Hoe),
                new PrefabItem("build_raise_ground", "RaiseGroundCustom", Category.Hoe),
                new PrefabItem("piece_craftingpole", "CraftingPole", Category.Building),
                new PrefabItem("piece_blackforge_ext2", "BlackForgeExt2", Category.Building),
                new PrefabItem("piece_blackforge_ext3", "BlackForgeExt3", Category.Building),
                new PrefabItem("piece_dvergr_spiralstair_flametal", "DvergrSpiralstairFlametal", Category.Building),
                new PrefabItem("piece_dvergr_spiralstair_flametal_right", "DvergrSpiralstairFlametalRight", Category.Building),
                new PrefabItem("piece_dvergr_spiralstair_flametal_ruby", "DvergrSpiralstairFlametalRuby", Category.Building),
                new PrefabItem("piece_dvergr_spiralstair_flametal_ruby_right", "DvergrSpiralstairFlametalRubyRight", Category.Building),
                new PrefabItem("piece_groundtorch_wood_alt", "StandingWoodTorchAlt", Category.Building)
/*
                    new PrefabItem("piece_ward_protect_areamarker", "piece_ward_protect_areamarker", Category.None),
                    new PrefabItem("piece_ward_protect", "ProtectionWard", Category.Building, new List<ICustomUnityScript> {
                    new WardProtect()
                })
*/
            };

            recipesList = new string[] {
                "Fish2"
            };
        }

        internal static void RegisterPieces()
        {
            // Building Assets
            if (ConfigSettings.enableCraftingItems.Value)
            {
                for (int i = 0; i < prefabsList.Count; i++)
                {
                    GameObject prefab = assetBundle.LoadAsset<GameObject>(assetsPath + "Prefabs/" + prefabsList[i].prefabName + ".prefab");
                    prefab.name = prefabsList[i].ingameName;

                    foreach (ICustomUnityScript component in prefabsList[i].components)
                    {
                        prefab.AddComponent(component.GetType());
                    }

                    if (prefabsList[i].category == Category.Building)
                    {
                        PieceManager.Instance.AddPiece(new CustomPiece(prefab, "Hammer", false));
                    }

                    if (prefabsList[i].category == Category.Crafting)
                    {
                        ItemManager.Instance.AddItem(new CustomItem(prefab, false));
                    }

                    if (prefabsList[i].category == Category.None)
                    {
                        PrefabManager.Instance.AddPrefab(prefab);
                    }

                    ConsoleLog.Print("Added prefab: " + prefab.name);
                }
            }

            // Hoe Assets
            if (ConfigSettings.enableCraftingItems.Value)
            {
                for (int i = 0; i < prefabsList.Count; i++)
                {
                    if (prefabsList[i].category == Category.Hoe)
                    {
                        GameObject prefab = assetBundle.LoadAsset<GameObject>(assetsPath + "Prefabs/" + prefabsList[i].prefabName + ".prefab");
                        prefab.name = prefabsList[i].ingameName;
                        PieceManager.Instance.AddPiece(new CustomPiece(prefab, "Hoe", false));
                    }
                }
            }

            RegisterRecipes();
            PrefabManager.OnVanillaPrefabsAvailable -= RegisterPieces;
        }

        internal static void RegisterRecipes()
        {
            // Load Recipes
            if (ConfigSettings.enableRecipes.Value)
            {
                for (int i = 0; i < recipesList.Length; i++)
                {
                    Recipe recipeToLoad = assetBundle.LoadAsset<Recipe>(assetsPath + "Recipes/Recipe_" + recipesList[i] + ".asset");
                    ItemManager.Instance.AddRecipe(new CustomRecipe(recipeToLoad, false, false));
                }

                foreach (RecipeConfig recipe in AddRemoveItemsRecipesConfig.newRecipes)
                {
                    ItemManager.Instance.AddRecipe(new CustomRecipe(recipe));
                }
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
