using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace BountyHunter
{
    internal class LoadAssets
    {
        // Asset bundles
        private static AssetBundle assetBundle;

        private const string assetsPath = "Assets/_CustomItems/02_Bounties/";
        private static List<PrefabList> prefabsList;
        private static string[] recipesList;
        private static string[,] bountiesList;

        internal static void CreatePrefabAndRecipeList()
        {
            // Load custom assets from the asset bundle
            assetBundle = GetAssetBundleFromResources("bounties");

            // Prefabs
            prefabsList = new List<PrefabList>
            {
                new PrefabList("piece_bountyboard", Category.Piece),
                new PrefabList("monster_greydwarfboss", Category.Monster),
                new PrefabList("bounty_greydwarfboss_1", Category.BountyItem),
                new PrefabList("bounty_greydwarfboss_2", Category.BountyItem),
                new PrefabList("bounty_greydwarfboss_3", Category.BountyItem),
                new PrefabList("bounty_greydwarfboss_4", Category.BountyItem)
            };

            recipesList = new string[] {
                // Add strings with the names of the recipes that must be added
            };

            // Recipe name, Prefab monster name
            bountiesList = new string[,] {
                {"Bounty_GreydwarfBoss_1", "monster_greydwarfboss"},
                {"Bounty_GreydwarfBoss_2", "monster_greydwarfboss"},
                {"Bounty_GreydwarfBoss_3", "monster_greydwarfboss"},
                {"Bounty_GreydwarfBoss_4", "monster_greydwarfboss"}
            };

            RegisterPrefabs();
            RegisterRecipes();
        }

        private static void RegisterRecipes()
        {
            // Add recipes
            for (int i = 0; i < recipesList.Length; i++)
            {
                Recipe recipeToLoad = assetBundle.LoadAsset<Recipe>(assetsPath + "Recipes/Recipe_" + recipesList[i] + ".asset");
                ItemManager.Instance.AddRecipe(new CustomRecipe(recipeToLoad, false, false));
            }

            // Add bounties
            for (int i = 0; i < bountiesList.GetLength(0); i++)
            {
                Recipe recipeToLoad = assetBundle.LoadAsset<Recipe>(assetsPath + "Recipes/Recipe_" + bountiesList[i, 0] + ".asset");
                ItemManager.Instance.AddRecipe(new CustomRecipe(recipeToLoad, true, true));
                BountyManager.AddBountyToPool(recipeToLoad, bountiesList[i, 1]);
            }
        }

        private static void RegisterPrefabs()
        {
            // Load and register every individual asset as a GameObject
            for (int i = 0; i < prefabsList.Count; i++)
            {
                GameObject prefab = assetBundle.LoadAsset<GameObject>(assetsPath + "Prefabs/" + prefabsList[i].prefabName + ".prefab");
                Category prefabCategory = prefabsList[i].category;

                // Add custom components to gameobjects
                if (prefab.name == "piece_bountyboard")
                {
                    prefab.AddComponent<BountyBoard>();
                }

                // Register items
                if (prefabCategory == Category.Item || prefabCategory == Category.BountyItem)
                {
                    if (prefabCategory == Category.BountyItem)
                    {
                        prefab.AddComponent<BountyScroll>();
                    }
                    ItemManager.Instance.AddItem(new CustomItem(prefab, true));
                }
                else
                // Register pieces
                if (prefabCategory == Category.Piece)
                {
                    PieceConfig pieceConfig = new PieceConfig();
                    pieceConfig.PieceTable = "Hammer";
                    CustomPiece piece = new CustomPiece(prefab, true, pieceConfig);
                    PieceManager.Instance.AddPiece(piece);
                }
                else
                // Register monster prefabs
                if (prefabCategory == Category.Monster)
                {
                    CreatureManager.Instance.AddCreature(new CustomCreature(prefab, true));
                }
                // Register remaining prefabs
                else
                {
                    PrefabManager.Instance.AddPrefab(prefab);
                }
            }
        }

        internal static AssetBundle GetAssetBundleFromResources(string fileName)
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
