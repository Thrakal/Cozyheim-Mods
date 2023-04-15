using Jotunn.Configs;
using System.Collections.Generic;

namespace Cozyheim.CustomItems
{
    internal class AddRemoveItemsRecipesConfig
    {
        public static void GenerateLists()
        {
            CustomRecipe[] customRecipes = new CustomRecipe[]
            {
                new CustomRecipe()
                {
                    originalRecipeName = "Recipe_QueensJam",
                    item = "QueensJam",
                    requirements = new[] {
                        new RequirementConfig("Raspberry", 2),
                        new RequirementConfig("Blueberries", 2)
                    }
                },
                new CustomRecipe()
                {
                    originalRecipeName = "Recipe_Sausages",
                    item = "Sausages",
                    requirements = new[] {
                        new RequirementConfig("Entrails", 1),
                        new RequirementConfig("RawMeat", 2),
                        new RequirementConfig("Thistle", 2)
                    }
                },
                new CustomRecipe()
                {
                    originalRecipeName = "$custom_item_honeymushroomstew",
                    recipeName = "Recipe_HoneyMushroomStew",
                    item = "HoneyMushroomStew",
                    requirements = new[] {
                        new RequirementConfig("MushroomMagecap", 2),
                        new RequirementConfig("Honey", 2),
                        new RequirementConfig("MushroomJotunPuffs", 2)
                    }
                },
                new CustomRecipe()
                {
                    originalRecipeName = "$custom_item_honeymushroomsoup",
                    recipeName = "Recipe_HoneyMushroomSoup",
                    item = "HoneyMushroomSoup",
                    requirements = new[] {
                        new RequirementConfig("Mushroom", 2),
                        new RequirementConfig("MushroomYellow", 2),
                        new RequirementConfig("Honey", 2)
                    }
                },
                new CustomRecipe()
                {
                    originalRecipeName = "Recipe_ShocklateSmoothie",
                    item = "ShocklateSmoothie",
                    requirements = new[] {
                        new RequirementConfig("Ooze", 1),
                        new RequirementConfig("Raspberry", 2),
                        new RequirementConfig("Blueberries", 2)
                    }
                },
                new CustomRecipe()
                {
                    originalRecipeName = "Recipe_CarrotSoup",
                    item = "CarrotSoup",
                    requirements = new[] {
                        new RequirementConfig("Mushroom", 2),
                        new RequirementConfig("Carrot", 2),
                        new RequirementConfig("Raspberry", 2)
                    }
                },
                new CustomRecipe()
                {
                    originalRecipeName = "Recipe_DeerStew",
                    item = "DeerStew",
                    requirements = new[] {
                        new RequirementConfig("DeerMeat", 1),
                        new RequirementConfig("Blueberries", 2),
                        new RequirementConfig("Carrot", 2)
                    }
                },
                new CustomRecipe()
                {
                    originalRecipeName = "Recipe_TurnipStew",
                    item = "TurnipStew",
                    requirements = new[] {
                        new RequirementConfig("RawMeat", 2),
                        new RequirementConfig("Turnip", 2)
                    }
                },
                new CustomRecipe()
                {
                    originalRecipeName = "Recipe_Blacksoup",
                    item = "BlackSoup",
                    requirements = new[] {
                        new RequirementConfig("Bloodbag", 1),
                        new RequirementConfig("Honey", 2),
                        new RequirementConfig("Turnip", 2)
                    }
                },
                new CustomRecipe()
                {
                    originalRecipeName = "Recipe_MinceMeatSauce",
                    item = "MinceMeatSauce",
                    requirements = new[] {
                        new RequirementConfig("NeckTail", 1),
                        new RequirementConfig("RawMeat", 2),
                        new RequirementConfig("Carrot", 2)
                    }
                }
            };

            foreach(CustomRecipe customRecipe in customRecipes)
            {
                customRecipe.ReplaceOriginal();
            }
        }

        public static List<string> removedItems = new List<string>()
        {
            "paved_road",
            "paved_road_v2",
            "raise",
            "raise_v2",
            "guard_stone"
        };

        public static List<string> removedRecipes = new List<string>()
        {
            "Recipe_Fish1"
        };

        public static List<RecipeConfig> newRecipes = new List<RecipeConfig>()
        {
            new RecipeConfig()
            {
                Name = "Recipe_Chain_Single",
                Item = "Chain",
                Amount = 5,
                CraftingStation = "forge",
                Requirements = new[]
                {
                    new RequirementConfig("Iron", 5),
                    new RequirementConfig("SurtlingCore", 1)
                }
            },
            new RecipeConfig()
            {
                Name = "Recipe_StoneheadArrow",
                Item = "StoneheadArrow",
                Amount = 20,
                Requirements = new[]
                {
                    new RequirementConfig("Wood", 8),
                    new RequirementConfig("Stone", 4)
                }
            },

            // Raw Fish Recipes
            new RecipeConfig()
            {
                Name = "Recipe_FishRaw_Fish1_New", // Perch
                Item = "FishRaw",
                Amount = 2,
                CraftingStation = "ButchersBench_FYA",                
                Requirements = new[]
                {
                    new RequirementConfig("Fish1", 1)
                }
            },
            new RecipeConfig()
            {
                Name = "Recipe_FishRaw_Fish2_New", // Pike
                Item = "FishRaw",
                Amount = 2,
                CraftingStation = "ButchersBench_FYA",
                Requirements = new[]
                {
                    new RequirementConfig("Fish2", 1)
                }
            },
            new RecipeConfig()
            {
                Name = "Recipe_FishRaw_Fish3_New", // Tuna
                Item = "FishRaw",
                Amount = 6,
                CraftingStation = "ButchersBench_FYA",
                Requirements = new[]
                {
                    new RequirementConfig("Fish3", 1)
                }
            },
            new RecipeConfig()
            {
                Name = "Recipe_FishRaw_Fish4Cave_New", // Tetra
                Item = "FishRaw",
                Amount = 4,
                CraftingStation = "ButchersBench_FYA",
                Requirements = new[]
                {
                    new RequirementConfig("Fish4_cave", 1)
                }
            },
            new RecipeConfig()
            {
                Name = "Recipe_FishRaw_Fish5_New", // Trollfish
                Item = "FishRaw",
                Amount = 3,
                CraftingStation = "ButchersBench_FYA",
                Requirements = new[]
                {
                    new RequirementConfig("Fish5", 1)
                }
            },
            new RecipeConfig()
            {
                Name = "Recipe_FishRaw_Fish6_New", // Giant Herring
                Item = "FishRaw",
                Amount = 4,
                CraftingStation = "ButchersBench_FYA",
                Requirements = new[]
                {
                    new RequirementConfig("Fish6", 1)
                }
            },
            new RecipeConfig()
            {
                Name = "Recipe_FishRaw_Fish7_New", // Grouper
                Item = "FishRaw",
                Amount = 5,
                CraftingStation = "ButchersBench_FYA",
                Requirements = new[]
                {
                    new RequirementConfig("Fish7", 1)
                }
            },
            new RecipeConfig()
            {
                Name = "Recipe_FishRaw_Fish8_New", // Coral Cod
                Item = "FishRaw",
                Amount = 8,
                CraftingStation = "ButchersBench_FYA",
                Requirements = new[]
                {
                    new RequirementConfig("Fish8", 1)
                }
            },
            new RecipeConfig()
            {
                Name = "Recipe_FishRaw_Fish9_New", // Anglerfish
                Item = "FishRaw",
                Amount = 6,
                CraftingStation = "ButchersBench_FYA",
                Requirements = new[]
                {
                    new RequirementConfig("Fish9", 1)
                }
            },
            new RecipeConfig()
            {
                Name = "Recipe_FishRaw_Fish10_New", // Northern Salmon
                Item = "FishRaw",
                Amount = 10,
                CraftingStation = "ButchersBench_FYA",
                Requirements = new[]
                {
                    new RequirementConfig("Fish10", 1)
                }
            },
            new RecipeConfig()
            {
                Name = "Recipe_FishRaw_Fish11_New", // Magmafish
                Item = "FishRaw",
                Amount = 10,
                CraftingStation = "ButchersBench_FYA",
                Requirements = new[]
                {
                    new RequirementConfig("Fish11", 1)
                }
            },
            new RecipeConfig()
            {
                Name = "Recipe_FishRaw_Fish12_New", // Pufferfish
                Item = "FishRaw",
                Amount = 6,
                CraftingStation = "ButchersBench_FYA",
                Requirements = new[]
                {
                    new RequirementConfig("Fish12", 1)
                }
            }
        };

        public class CustomRecipe
        {
            public string originalRecipeName;
            public string recipeName = "";
            public string item;
            public RequirementConfig[] requirements;

            public CustomRecipe ReplaceOriginal(bool replace = true)
            {
                if (!replace)
                    return this;

                removedRecipes.Add(originalRecipeName);

                recipeName = recipeName == "" ? originalRecipeName : recipeName;

                for (int i = 0; i < 2; i++)
                {
                    string name = i == 0 ? recipeName + "_Single" : recipeName + "_Stack";
                    int itemAmount = i == 0 ? 1 : 10;

                    List<RequirementConfig> requirementConfigs = new List<RequirementConfig>();

                    foreach (RequirementConfig config in requirements)
                    {
                        int reqAmount = i == 0 ? config.Amount : config.Amount * 8;
                        requirementConfigs.Add(new RequirementConfig(config.Item, reqAmount));
                    }

                    newRecipes.Add(
                        new RecipeConfig()
                        {
                            Name = name,
                            Item = item,
                            Amount = itemAmount,
                            CraftingStation = "piece_cauldron",
                            Requirements = requirementConfigs.ToArray()
                        }
                    );
                }

                return this;
            }
        }
    }
}
