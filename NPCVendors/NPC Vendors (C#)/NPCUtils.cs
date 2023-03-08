using Jotunn.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cozyheim.NPCVendors
{
    internal class Utils
    {
        internal string GetBossKey(BossToUnlock boss)
        {
            string[] globalBossKeys = new string[]
            {
                "",
                "defeated_eikthyr",
                "defeated_gdking",
                "defeated_bonemass",
                "defeated_dragon",
                "defeated_goblinking",
                "defeated_queen"
            };

            return globalBossKeys[(int)boss];
        }
    }

    internal class NPCRecipe
    {
        public string globalKeyToUnlock;
        public Recipe recipe;

        public NPCRecipe(NPCType npcType, string item, int amount, int coinPrice, BossToUnlock boss)
        {
            RecipeConfig config = new RecipeConfig()
            {
                Name = "Recipe_" + npcType.ToString() + "_" + item + "_" + amount.ToString() + "for" + coinPrice.ToString(),
                Item = item,
                Amount = amount,
                CraftingStation = "NPC_" + npcType.ToString(),
                Requirements = new[]
                {
                    new RequirementConfig("Coins", coinPrice)
                }
            };

            recipe = config.GetRecipe();

            string[] globalBossKeys = new string[]
            {
                    "",
                    "defeated_eikthyr",
                    "defeated_gdking",
                    "defeated_bonemass",
                    "defeated_dragon",
                    "defeated_goblinking",
                    "defeated_queen"
            };

            globalKeyToUnlock = globalBossKeys[(int)boss];
        }
    }

    internal class NPCItem
    {
        public NPCType npcType;
        public string description;
        public string iconName;
        public NPCEquipment equipment;
        public Piece piece;

        public NPCItem(NPCType npcType, string description, string iconName, NPCEquipment equipment)
        {
            this.npcType = npcType;
            this.description = description;
            this.iconName = iconName;
            this.equipment = equipment;
        }
    }

    internal enum BossToUnlock
    {
        None = 0,
        Eikthyr = 1,
        Elder = 2,
        Bonemass = 3,
        Moder = 4,
        Yagluth = 5,
        Queen = 6
    }

    internal enum NPCType
    {
        Vendor,
        Blacksmith,
        Farmer,
        Mason,
        Forester,
        Hunter,
        Butcher,
        Völva,
        Fisher,
        Woodcutter
    }

    internal enum NPCHeight
    {
        h150 = 150,
        h155 = 155,
        h160 = 160,
        h165 = 165,
        h170 = 170,
        h175 = 175,
        h180 = 180,
        h185 = 185,
        h190 = 190,
        h195 = 195,
        h200 = 200,
        h205 = 205,
        h210 = 210,
        h215 = 215,
        h220 = 220
    }
}
