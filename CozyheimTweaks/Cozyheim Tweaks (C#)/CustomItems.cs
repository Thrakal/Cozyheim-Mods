using CozyheimTweaks.Scripts;
using CozyheimTweaks.Resources;
using HarmonyLib;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace CozyheimTweaks
{
    internal class CustomItems
    {
        // Asset bundles
        private static string assetsPath = "Assets/_CustomItems/01_ToppItems/";
        private static AssetBundle assetBundle;

        private static List<PrefabItem> prefabsList;
        private static string[] recipesList;

        internal static void Init()
        {
            AddRemoveItemsRecipesConfig.GenerateLists();
            CreatePrefabAndRecipeList();
            PrefabManager.OnVanillaPrefabsAvailable += RegisterPieces;
        }

        internal static void CreatePrefabAndRecipeList()
        {
            // Load custom assets from the asset bundle
            assetBundle = Main.GetAssetBundleFromResources("toppitems");

            // Prefabs
            prefabsList = new List<PrefabItem>
            {
                new PrefabItem("piece_ward_protect_areamarker", "piece_ward_protect_areamarker", Category.None),
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
                new PrefabItem("piece_groundtorch_wood_alt", "StandingWoodTorchAlt", Category.Building),
                new PrefabItem("piece_chest_1x1_stackable_small", "piece_chest_1x1_stackable_small", Category.Building),
                new PrefabItem("piece_chest_1x1_stackable_medium", "piece_chest_1x1_stackable_medium", Category.Building),
                new PrefabItem("piece_chest_1x1_stackable_large", "piece_chest_1x1_stackable_large", Category.Building),
                new PrefabItem("piece_ward_protect", "ProtectionWard", Category.Building, new List<ICustomUnityScript> {
                    new WardProtect()
                })
            };

            recipesList = new string[] {
                "Fish2"
            };
        }

        internal static void RegisterPieces()
        {
            // Building Assets
            if (Main.enableCraftingItems.Value)
            {
                ToppLog.Print("-- Adding Topp Items --", LogType.Message);
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

                    ToppLog.Print("Added prefab: " + prefab.name);
                }
            }

            // Hoe Assets
            if (Main.enableCraftingItems.Value)
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
            if (Main.enableRecipes.Value)
            {
                for (int i = 0; i < recipesList.Length; i++)
                {
                    Recipe recipeToLoad = assetBundle.LoadAsset<Recipe>(assetsPath + "Recipes/Recipe_" + recipesList[i] + ".asset");
                    ItemManager.Instance.AddRecipe(new CustomRecipe(recipeToLoad, false, false));
                }

                foreach(RecipeConfig recipe in AddRemoveItemsRecipesConfig.newRecipes)
                {
                    ItemManager.Instance.AddRecipe(new CustomRecipe(recipe));
                }
            }
        }

        [HarmonyPatch]
        private class CraftingPole_Patch
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(CraftingStation), "Start")]
            public static void CS_Start_Patch(CraftingStation __instance, ref List<CraftingStation> ___m_allStations)
            {
                if (__instance.name == "piece_craftingpole")
                {
                    if (!___m_allStations.Contains(__instance))
                    {
                        ___m_allStations.Add(__instance);
                    }
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(CraftingStation), "FixedUpdate")]
            public static void CS_FixedUpdate_Patch(CraftingStation __instance, ref float ___m_useTimer, ref float ___m_updateExtensionTimer, GameObject ___m_inUseObject)
            {
                if (__instance.name == "piece_craftingpole")
                {
                    ___m_useTimer += Time.deltaTime;
                    ___m_updateExtensionTimer += Time.deltaTime;
                    if (___m_inUseObject)
                    {
                        ___m_inUseObject.SetActive(___m_useTimer < 1f);
                    }
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(Container), "Interact")]
            public static bool Prefix(Container __instance, ref bool __result, Humanoid character, bool hold, bool alt)
            {
                if(__instance.name.StartsWith("piece_chest_1x1_stackable"))
                {
                    if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
                        Sign sign = __instance.GetComponent<Sign>();
                        sign.Interact(character, hold, alt);
                        __result = false;
                        return false;
                    }
                }

                return true;
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(Sign), "UpdateText")]
            public static void Postfix(Sign __instance)
            {
                if (__instance.name.StartsWith("piece_chest_1x1_stackable"))
                {
                    Text[] texts = __instance.GetComponentsInChildren<Text>(true);
                    foreach(Text t in texts)
                    {
                        t.text = __instance.GetText();
                    }
                }
            }
        }
    }
}

