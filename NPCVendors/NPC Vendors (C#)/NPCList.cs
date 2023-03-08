using Jotunn.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Cozyheim.NPCVendors.NPC;

namespace Cozyheim.NPCVendors
{
    internal class NPCList
    {
        internal static List<NPCItem> customNPCs;
        internal static List<NPCRecipe> customNPCRecipes;

        internal static void CreatePrefabAndRecipeList()
        {
            customNPCs = new List<NPCItem>
            {
                // Skarde, Egil, Lars, Svend, Frida, Leif, Asbjørn, Siri
                new NPCItem(NPCType.Vendor, "Buy building materials for valuables", "Icon_Vendor.png", new NPCEquipment()
                {
                    Gender = GenderNPC.Female,
                    Hair = HairNPC.Braided_4,
                    HairColor = ColorNPC.BlondLight,
                    SkinColor = ColorNPC.SkinLight,
                    Chest = ChestNPC.ArmorTrollLeatherChest,
                    Legs = LegsNPC.ArmorTrollLeatherLegs,
                    Height = NPCHeight.h160
                }),
                new NPCItem(NPCType.Blacksmith, "Buy mining materials for valuables", "Icon_Blacksmith.png", new NPCEquipment()
                {
                    Gender = GenderNPC.Male,
                    Hair = HairNPC.Braids_Gathered,
                    Beard = BeardNPC.Royal_1,
                    HairColor = ColorNPC.Black,
                    SkinColor = ColorNPC.Brown,
                    Chest = ChestNPC.ArmorIronChest,
                    Legs = LegsNPC.ArmorLeatherLegs,
                    MainHandBack = NPCEquipment.GetItem(TwoHandedWeaponNPC.SledgeIron),
                    Height = NPCHeight.h210
                    
                }),
                new NPCItem(NPCType.Farmer, "Buy farming materials for valuables", "Icon_Farmer.png", new NPCEquipment()
                {
                    Gender = GenderNPC.Male,
                    Hair = HairNPC.NoHair,
                    Beard = BeardNPC.Long_2,
                    HairColor = ColorNPC.Ginger,
                    SkinColor = ColorNPC.BrownLight,
                    Helmet = HelmetNPC.HelmetLeather,
                    Chest = ChestNPC.ArmorLeatherChest,
                    Legs = LegsNPC.ArmorLeatherLegs,
                    Shoulder = ShoulderNPC.CapeLox,
                    MainHand = NPCEquipment.GetItem(ToolNPC.Cultivator),
                    Height = NPCHeight.h190
                }),
                new NPCItem(NPCType.Mason, "Buy stone materials for valuables", "Icon_Vendor.png", new NPCEquipment()
                {
                    Gender = GenderNPC.Male,
                    Hair = HairNPC.Long_1,
                    Beard = BeardNPC.Stonedweller,
                    HairColor = ColorNPC.Grey,
                    SkinColor = ColorNPC.Skin,
                    Chest = ChestNPC.ArmorPaddedCuirass,
                    Legs = LegsNPC.ArmorMageLegs,
                    MainHandBack = NPCEquipment.GetItem(ToolNPC.PickaxeBlackMetal),
                    MainHand = NPCEquipment.GetItem(ToolNPC.Hammer),
                    Height = NPCHeight.h205
                }),
                new NPCItem(NPCType.Forester, "Buy forest seeds for valuables", "Icon_Vendor.png", new NPCEquipment()
                {
                    Gender = GenderNPC.Female,
                    Hair = HairNPC.Curls_Pulled_Back,
                    HairColor = ColorNPC.BrownDark,
                    SkinColor = ColorNPC.Brown,
                    Chest = ChestNPC.ArmorLeatherChest,
                    Legs = LegsNPC.ArmorLeatherLegs,
                    Shoulder = ShoulderNPC.CapeLinen,
                    ShoulderVariant = 0,
                    MainHandBack = NPCEquipment.GetItem(ToolNPC.AxeIron),
                    MainHand = NPCEquipment.GetItem(OneHandedWeaponNPC.KnifeChitin),
                    Height = NPCHeight.h170
                }),
                new NPCItem(NPCType.Hunter, "Buy animal materials for valuables", "Icon_Vendor.png", new NPCEquipment()
                {
                    Gender = GenderNPC.Female,
                    Hair = HairNPC.Ponytail_2,
                    HairColor = ColorNPC.GingerDark,
                    SkinColor = ColorNPC.SkinDark,
                    Chest = ChestNPC.ArmorFenringChest,
                    Legs = LegsNPC.ArmorFenringLegs,
                    Shoulder = ShoulderNPC.CapeWolf,
                    MainHand =  NPCEquipment.GetItem(TwoHandedWeaponNPC.BowHuntsman),
                    MainHandBack = NPCEquipment.GetItem(TrophyNPC.TrophySeekerBrute),
                    Height = NPCHeight.h170
                }),
                new NPCItem(NPCType.Butcher, "Buy meat materials for valuables", "Icon_Vendor.png", new NPCEquipment()
                {
                    Gender = GenderNPC.Male,
                    Hair = HairNPC.NoHair,
                    Beard = BeardNPC.Thick_2,
                    HairColor = ColorNPC.GingerLight,
                    SkinColor = ColorNPC.SkinLight,
                    Chest = ChestNPC.ArmorLeatherChest,
                    Legs = LegsNPC.ArmorLeatherLegs,
                    MainHand = NPCEquipment.GetItem(ToolNPC.KnifeButcher),
                    MainHandBack = NPCEquipment.GetItem(ToolNPC.AxeIron),
                    Height = NPCHeight.h195
                }),
                new NPCItem(NPCType.Völva, "Buy magic materials for valuables", "Icon_Vendor.png", new NPCEquipment()
                {
                    Gender = GenderNPC.Female,
                    Hair = HairNPC.SideSwept_2,
                    HairColor = ColorNPC.White,
                    SkinColor = ColorNPC.Skin,
                    Chest = ChestNPC.ArmorRootChest,
                    Legs = LegsNPC.ArmorRootLegs,
                    Shoulder = ShoulderNPC.CapeFeather,
                    MainHand = NPCEquipment.GetItem(TwoHandedWeaponNPC.StaffIceShards),
                    MainHandBack = NPCEquipment.GetItem(TrophyNPC.TrophyAbomination),
                    Height = NPCHeight.h180
                }),
                new NPCItem(NPCType.Fisher, "Buy fishing materials for valuables", "Icon_Vendor.png", new NPCEquipment()
                {
                    Gender = GenderNPC.Male,
                    Hair = HairNPC.NoHair,
                    Beard = BeardNPC.Braided_5,
                    HairColor = ColorNPC.Brown,
                    SkinColor = ColorNPC.Skin,
                    Helmet = HelmetNPC.HelmetTrollLeather,
                    Chest = ChestNPC.ArmorRagsChest,
                    Legs = LegsNPC.ArmorRagsLegs,
                    Shoulder = ShoulderNPC.CapeTrollHide,
                    MainHand = NPCEquipment.GetItem(ToolNPC.FishingRod),
                    Height = NPCHeight.h185
                }),
                new NPCItem(NPCType.Woodcutter, "Buy wood materials for valuables", "Icon_Vendor.png", new NPCEquipment()
                {
                    Gender = GenderNPC.Male,
                    Hair = HairNPC.SideSwept_3,
                    Beard = BeardNPC.Short_4,
                    HairColor = ColorNPC.BlondDark,
                    SkinColor = ColorNPC.SkinDark,
                    Chest = ChestNPC.None,
                    Legs = LegsNPC.ArmorWolfLegs,
                    MainHand = NPCEquipment.GetItem(ToolNPC.AxeIron),
                    MainHandBack = NPCEquipment.GetItem(ToolNPC.AxeBronze),
                    Height = NPCHeight.h195
                })
            };

            customNPCRecipes = new List<NPCRecipe>()
            {
                new NPCRecipe(NPCType.Vendor, "Resin", 50, 50, BossToUnlock.Eikthyr),
                new NPCRecipe(NPCType.Vendor, "Coal", 50, 80, BossToUnlock.Elder),
                new NPCRecipe(NPCType.Vendor, "Tar", 50, 100, BossToUnlock.Yagluth),
                new NPCRecipe(NPCType.Vendor, "QueenBee", 1, 50, BossToUnlock.Elder),
                new NPCRecipe(NPCType.Vendor, "Crystal", 50, 150, BossToUnlock.Moder),
                new NPCRecipe(NPCType.Vendor, "JuteRed", 4, 50, BossToUnlock.Moder),
                new NPCRecipe(NPCType.Vendor, "JuteBlue", 4, 50, BossToUnlock.Queen),
                new NPCRecipe(NPCType.Vendor, "ChickenEgg", 50, 2000, BossToUnlock.Yagluth),


                new NPCRecipe(NPCType.Blacksmith, "TinOre", 30, 150, BossToUnlock.Elder),
                new NPCRecipe(NPCType.Blacksmith, "CopperOre", 30, 150, BossToUnlock.Elder),
                new NPCRecipe(NPCType.Blacksmith, "IronScrap", 30, 200, BossToUnlock.Bonemass),
                new NPCRecipe(NPCType.Blacksmith, "SilverOre", 30, 250, BossToUnlock.Moder),
                new NPCRecipe(NPCType.Blacksmith, "BlackMetalScrap", 30, 350, BossToUnlock.Yagluth),
                new NPCRecipe(NPCType.Blacksmith, "FlametalOre", 30, 300, BossToUnlock.Queen),
                new NPCRecipe(NPCType.Blacksmith, "Chain", 2, 50, BossToUnlock.Bonemass),


                new NPCRecipe(NPCType.Farmer, "CarrotSeeds", 50, 50, BossToUnlock.Elder),
                new NPCRecipe(NPCType.Farmer, "TurnipSeeds", 50, 50, BossToUnlock.Bonemass),
                new NPCRecipe(NPCType.Farmer, "OnionSeeds", 50, 50, BossToUnlock.Moder),
                new NPCRecipe(NPCType.Farmer, "Barley", 50, 100, BossToUnlock.Yagluth),
                new NPCRecipe(NPCType.Farmer, "Flax", 50, 100, BossToUnlock.Yagluth),


                new NPCRecipe(NPCType.Mason, "Stone", 50, 50, BossToUnlock.Eikthyr),
                new NPCRecipe(NPCType.Mason, "Flint", 30, 50, BossToUnlock.Eikthyr),
                new NPCRecipe(NPCType.Mason, "Obsidian", 50, 100, BossToUnlock.Moder),
                new NPCRecipe(NPCType.Mason, "Chitin", 50, 150, BossToUnlock.Yagluth),
                new NPCRecipe(NPCType.Mason, "BlackMarble", 50, 150, BossToUnlock.Queen),


                new NPCRecipe(NPCType.Forester, "BeechSeeds", 20, 100, BossToUnlock.Eikthyr),
                new NPCRecipe(NPCType.Forester, "FirCone", 50, 150, BossToUnlock.Elder),
                new NPCRecipe(NPCType.Forester, "PineCone", 50, 150, BossToUnlock.Elder),
                new NPCRecipe(NPCType.Forester, "BirchSeeds", 50, 150, BossToUnlock.Elder),
                new NPCRecipe(NPCType.Forester, "Acorn", 50, 150, BossToUnlock.Yagluth),


                new NPCRecipe(NPCType.Woodcutter, "Wood", 50, 50, BossToUnlock.Eikthyr),
                new NPCRecipe(NPCType.Woodcutter, "RoundLog", 50, 50, BossToUnlock.Elder),
                new NPCRecipe(NPCType.Woodcutter, "FineWood", 50, 50, BossToUnlock.Elder),
                new NPCRecipe(NPCType.Woodcutter, "ElderBark", 50, 100, BossToUnlock.Bonemass),
                new NPCRecipe(NPCType.Woodcutter, "YggdrasilWood", 50, 100, BossToUnlock.Queen),


                new NPCRecipe(NPCType.Hunter, "Feathers", 20, 50, BossToUnlock.Eikthyr),
                new NPCRecipe(NPCType.Hunter, "LeatherScraps", 20, 50, BossToUnlock.Eikthyr),
                new NPCRecipe(NPCType.Hunter, "DeerHide", 20, 100, BossToUnlock.Eikthyr),
                new NPCRecipe(NPCType.Hunter, "TrollHide", 20, 150, BossToUnlock.Elder),
                new NPCRecipe(NPCType.Hunter, "WolfPelt", 20, 200, BossToUnlock.Moder),
                new NPCRecipe(NPCType.Hunter, "LoxPelt", 20, 200, BossToUnlock.Yagluth),
                new NPCRecipe(NPCType.Hunter, "Needle", 20, 200, BossToUnlock.Yagluth), 
                new NPCRecipe(NPCType.Hunter, "ScaleHide", 20, 200, BossToUnlock.Queen), 
                new NPCRecipe(NPCType.Hunter, "WolfClaw", 20, 200, BossToUnlock.Queen), 
                new NPCRecipe(NPCType.Hunter, "WolfFang", 20, 200, BossToUnlock.Queen), 
                new NPCRecipe(NPCType.Hunter, "Carapace", 20, 200, BossToUnlock.Queen),
                new NPCRecipe(NPCType.Hunter, "Mandible", 2, 200, BossToUnlock.Queen), 


                new NPCRecipe(NPCType.Butcher, "NeckTail", 20, 75, BossToUnlock.Eikthyr),
                new NPCRecipe(NPCType.Butcher, "RawMeat", 20, 75, BossToUnlock.Eikthyr),
                new NPCRecipe(NPCType.Butcher, "DeerMeat", 20, 75, BossToUnlock.Eikthyr),
                new NPCRecipe(NPCType.Butcher, "Entrails", 20, 100, BossToUnlock.Bonemass),
                new NPCRecipe(NPCType.Butcher, "WolfMeat", 20, 100, BossToUnlock.Moder),
                new NPCRecipe(NPCType.Butcher, "LoxMeat", 20, 150, BossToUnlock.Yagluth),
                new NPCRecipe(NPCType.Butcher, "BugMeat", 20, 150, BossToUnlock.Queen),
                new NPCRecipe(NPCType.Butcher, "HareMeat", 20, 150, BossToUnlock.Queen),
                new NPCRecipe(NPCType.Butcher, "ChickenMeat", 20, 150, BossToUnlock.Queen),


                new NPCRecipe(NPCType.Völva, "Bilebag", 20, 100, BossToUnlock.Queen),
                new NPCRecipe(NPCType.Völva, "Bloodbag", 20, 100, BossToUnlock.Bonemass),
                new NPCRecipe(NPCType.Völva, "BoneFragments", 20, 100, BossToUnlock.Elder),
                new NPCRecipe(NPCType.Völva, "Guck", 50, 80, BossToUnlock.Bonemass),
                new NPCRecipe(NPCType.Völva, "FreezeGland", 20, 150, BossToUnlock.Moder),
                new NPCRecipe(NPCType.Völva, "GiantBloodSack", 20, 150, BossToUnlock.Queen),
                new NPCRecipe(NPCType.Völva, "GreydwarfEye", 20, 100, BossToUnlock.Elder),
                new NPCRecipe(NPCType.Völva, "Ooze", 20, 100, BossToUnlock.Bonemass),
                new NPCRecipe(NPCType.Völva, "Root", 20, 150, BossToUnlock.Bonemass),
                new NPCRecipe(NPCType.Völva, "RottenMeat", 1, 10, BossToUnlock.None),
                new NPCRecipe(NPCType.Völva, "Softtissue", 20, 150, BossToUnlock.Queen),
                new NPCRecipe(NPCType.Völva, "WolfHairBundle", 20, 150, BossToUnlock.Moder),
                new NPCRecipe(NPCType.Völva, "YmirRemains", 1, 250, BossToUnlock.Bonemass),
                

                new NPCRecipe(NPCType.Fisher, "SerpentMeat", 20, 100, BossToUnlock.Yagluth),
                new NPCRecipe(NPCType.Fisher, "SerpentScale", 20, 100, BossToUnlock.Yagluth),
                new NPCRecipe(NPCType.Fisher, "FishingBait", 50, 20, BossToUnlock.Eikthyr),
                new NPCRecipe(NPCType.Fisher, "FishingBaitForest", 50, 50, BossToUnlock.Elder),
                new NPCRecipe(NPCType.Fisher, "FishingBaitSwamp", 50, 50, BossToUnlock.Bonemass),
                new NPCRecipe(NPCType.Fisher, "FishingBaitCave", 50, 50, BossToUnlock.Moder),
                new NPCRecipe(NPCType.Fisher, "FishingBaitOcean", 50, 50, BossToUnlock.Yagluth),
                new NPCRecipe(NPCType.Fisher, "FishingBaitPlains", 50, 50, BossToUnlock.Yagluth),
                new NPCRecipe(NPCType.Fisher, "FishingBaitMistlands", 50, 50, BossToUnlock.Queen),
                new NPCRecipe(NPCType.Fisher, "FishingBaitAshlands", 50, 50, BossToUnlock.Queen),
                new NPCRecipe(NPCType.Fisher, "FishingBaitDeepNorth", 50, 50, BossToUnlock.Queen),
                new NPCRecipe(NPCType.Fisher, "FishingRod", 1, 600, BossToUnlock.Eikthyr)
            };
        }
    }
}
