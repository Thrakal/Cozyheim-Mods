using HarmonyLib;
using Jotunn.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cozyheim.NPCVendors
{
    [HarmonyPatch]
    internal class NPCEquipment : MonoBehaviour
    {
        public VisEquipment equipment;

        public NPCType NPCType;

        public GenderNPC Gender = GenderNPC.Male;
        public ColorNPC SkinColor = ColorNPC.Brown;
        public ColorNPC HairColor = ColorNPC.Blond;
        public HairNPC Hair = HairNPC.NoHair;
        public BeardNPC Beard = BeardNPC.NoBeard;
        public ChestNPC Chest = ChestNPC.None;
        public LegsNPC Legs = LegsNPC.None;
        public HelmetNPC Helmet = HelmetNPC.None;
        public ShoulderNPC Shoulder = ShoulderNPC.None;
        public int ShoulderVariant = 0;
        public string MainHand = "";
        public string MainHandBack = "";
        public ShieldNPC OffHand = ShieldNPC.None;
        public ShieldNPC OffHandBack = ShieldNPC.None;
        public int ShieldVariant = 0;
        public ToolNPC Tool = ToolNPC.None;
        public FoodNPC Food = FoodNPC.None;
        public NPCHeight Height = NPCHeight.h180;

        public void CopyEquipment(NPCEquipment copyEquipment)
        {
            NPCType = copyEquipment.NPCType;
            Gender = copyEquipment.Gender;
            SkinColor = copyEquipment.SkinColor;
            HairColor = copyEquipment.HairColor;
            Hair = copyEquipment.Hair;
            Beard = copyEquipment.Beard;
            Chest = copyEquipment.Chest;
            Legs = copyEquipment.Legs;
            Helmet = copyEquipment.Helmet;
            Shoulder = copyEquipment.Shoulder;
            ShoulderVariant = copyEquipment.ShoulderVariant;
            MainHand = copyEquipment.MainHand;
            MainHandBack = copyEquipment.MainHandBack;
            OffHand = copyEquipment.OffHand;
            OffHandBack = copyEquipment.OffHandBack;
            ShieldVariant = copyEquipment.ShieldVariant;
            Tool = copyEquipment.Tool;
            Food = copyEquipment.Food;
            Height = copyEquipment.Height;
        }

        // Start is called before the first frame update
        IEnumerator Start()
        {
            equipment = GetComponent<VisEquipment>();

            yield return null;

            // Character Model Visuals
            equipment.SetModel(GetGender(Gender));
            equipment.SetSkinColor(GetColor(SkinColor));
            equipment.SetHairColor(GetColor(HairColor));
            equipment.SetHairItem(GetHair(Hair));
            equipment.SetBeardItem(GetBeard(Beard));
            
            // Character Equipment
            if (Chest != ChestNPC.None) equipment.SetChestItem(Chest.ToString());
            if (Legs != LegsNPC.None) equipment.SetLegItem(Legs.ToString());
            if (Helmet != HelmetNPC.None) equipment.SetHelmetItem(Helmet.ToString());
            if (Shoulder != ShoulderNPC.None) equipment.SetShoulderItem(Shoulder.ToString(), ShoulderVariant);
            if (OffHand != ShieldNPC.None) equipment.SetLeftItem(OffHand.ToString(), ShieldVariant);
            if (OffHandBack != ShieldNPC.None) equipment.SetLeftBackItem(OffHandBack.ToString(), ShieldVariant);
            if (MainHand != "") equipment.SetRightItem(MainHand);
            if (MainHandBack != "") equipment.SetRightBackItem(MainHandBack);

            yield return null;

            UpdateHeight((int)Height);
        }

        public void UpdateGender(GenderNPC gender)
        {
            equipment.SetModel(GetGender(gender));
        }

        public void UpdateHeight(int height)
        {
            float heightScale = height / 180f;
            transform.localScale = Vector3.one * heightScale;
        }

        public static string GetItem(ToolNPC item)
        {
            return item.ToString();
        }

        public static string GetItem(FoodNPC item)
        {
            return item.ToString();
        }

        public static string GetItem(OneHandedWeaponNPC item)
        {
            return item.ToString();
        }

        public static string GetItem(TwoHandedWeaponNPC item)
        {
            return item.ToString();
        }

        public static string GetItem(TrophyNPC item)
        {
            return item.ToString();
        }

        public Vector3 GetColor(ColorNPC color)
        {
            Vector3[] colors = new Vector3[] {
                new Vector3(250, 250, 250), // White = 0,
                new Vector3(128, 128, 128), // Grey = 1,
                new Vector3(25, 25, 25), // Black = 2,

                new Vector3(138, 86, 56), //BrownLight = 3,
                new Vector3(109, 68, 44), //Brown = 4,
                new Vector3(90, 56, 37), //BrownDark = 5,

                new Vector3(250, 250, 210), //BlondLight = 6,
                new Vector3(245, 232, 168), //Blond = 7,
                new Vector3(239, 219, 118), //BlondDark = 8,
        
                new Vector3(236, 157, 79), //GingerLight = 9,
                new Vector3(218, 104, 15), //Ginger = 10,
                new Vector3(170, 39, 6), //GingerDark = 11

                new Vector3(253, 231, 214), //SkinLight = 12,
                new Vector3(255, 226, 198), //Skin = 13,
                new Vector3(255, 215, 174), //SkinDark = 14
            };

            Vector3 selectedColor = colors[(int)color];
            selectedColor.x /= 255f;
            selectedColor.y /= 255f;
            selectedColor.z /= 255f;

            return selectedColor;
        }

        private int GetGender(GenderNPC gender)
        {
            return (int)gender;
        }

        private string GetBeard(BeardNPC beard)
        {
            string[] prefabNames = new string[] {
                "BeardNone",
                "Beard1",
                "Beard2",
                "Beard3",
                "Beard4",
                "Beard5",
                "Beard6",
                "Beard7",
                "Beard8",
                "Beard9",
                "Beard10",
                "Beard11",
                "Beard12",
                "Beard13",
                "Beard14",
                "Beard15",
                "Beard16"
            };

            return prefabNames[(int)beard];
        }

        private string GetHair(HairNPC hair)
        {
            string[] prefabNames = new string[] {
                "HairNone",
                "Hair1",
                "Hair2",
                "Hair3",
                "Hair4",
                "Hair5",
                "Hair6",
                "Hair7",
                "Hair8",
                "Hair9",
                "Hair10",
                "Hair11",
                "Hair12",
                "Hair13",
                "Hair14",
                "Hair15",
                "Hair16",
                "Hair17",
                "Hair18",
                "Hair19",
                "Hair20",
                "Hair21",
                "Hair22",
                "Hair23"
            };

            return prefabNames[(int)hair];
        }
    }

    internal enum FoodNPC
    {
        None,
        LoxMeat,
        RawMeat,
        RottenMeat,
        SerpentMeat,
        WolfMeat,
        BugMeat,
        ChickenMeat,
        DeerMeat,
        HareMeat
    }

    internal enum ToolNPC
    {
        None,
        Cultivator,
        Hoe,
        Hammer,
        PickaxeAntler,
        PickaxeBlackMetal,
        PickaxeBronze,
        PickaxeIron,
        PickaxeStone,
        AxeBlackMetal,
        AxeBronze,
        AxeFlint,
        AxeIron,
        AxeStone,
        KnifeButcher,
        FishingRod
    }

    internal enum OneHandedWeaponNPC
    {
        None,
        Club,
        KnifeBlackMetal,
        KnifeChitin,
        KnifeCopper,
        KnifeFlint,
        KnifeSilver,
        MaceBronze,
        MaceIron,
        MaceNeedle,
        MaceSilver,
        SpearBronze,
        SpearCarapace,
        SpearChitin,
        SpearElderbark,
        SpearFlint,
        SpearWolfFang,
        SwordBlackmetal,
        SwordBronze,
        SwordCheat,
        SwordIron,
        SwordIronFire,
        SwordMistwalker,
        SwordSilver
    }

    internal enum TwoHandedWeaponNPC
    {
        None,
        SledgeIron,
        SledgeStagbreaker,
        SledgeDemolisher,
        AtgeirBronze,
        AtgeirBlackmetal,
        AtgeirHimminAfl,
        AtgeirIron,
        Battleaxe,
        BattleaxeCrystal,
        StaffFireball,
        StaffIceShards,
        StaffShield,
        StaffSkeleton,
        THSwordKrom,
        CrossbowArbalest,
        Bow,
        BowDraugrFang,
        BowFineWood,
        BowHuntsman,
        BowSpineSnap
    }

    internal enum TrophyNPC
    {
        None,
        TrophyAbomination,
        TrophyBlob,
        TrophyBoar,
        TrophyBonemass,
        TrophyCultist,
        TrophyDeathsquito,
        TrophyDeer,
        TrophyDragonQueen,
        TrophyDraugr,
        TrophyDraugrElite,
        TrophyDraugrFem,
        TrophyDvergr,
        TrophyEikthyr,
        TrophyFenring,
        TrophyForestTroll,
        TrophyFrostTroll,
        TrophyGjall,
        TrophyGoblin,
        TrophyGoblinBrute,
        TrophyGoblinKing,
        TrophyGoblinShaman,
        TrophyGreydwarf,
        TrophyGreydwarfBrute,
        TrophyGreydwarfShaman,
        TrophyGrowth,
        TrophyHare,
        TrophyHatchling,
        TrophyLeech,
        TrophyLox,
        TrophyNeck,
        TrophySeeker,
        TrophySeekerBrute,
        TrophySeekerQueen,
        TrophySerpent,
        TrophySGolem,
        TrophySkeleton,
        TrophySkeletonPoison,
        TrophySurtling,
        TrophyTheElder,
        TrophyTick,
        TrophyUlv,
        TrophyWolf,
        TrophyWraith
    }

    internal enum ShieldNPC
    {
        None,
        ShieldBanded,
        ShieldBlackmetal,
        ShieldBlackmetalTower,
        ShieldBoneTower,
        ShieldBronzeBuckler,
        ShieldCarapace,
        ShieldCarapaceBuckler,
        ShieldIronBuckler,
        ShieldIronSquare,
        ShieldIronTower,
        ShieldKnight,
        ShieldSerpentscale,
        ShieldSilver,
        ShieldWood,
        ShieldWoodTower
    }

    internal enum ShoulderNPC
    {
        None,
        CapeDeerHide,
        CapeFeather,
        CapeLinen,
        CapeLox,
        CapeOdin,
        CapeTrollHide,
        CapeWolf
    }

    internal enum HelmetNPC
    {
        None,
        HelmetBronze,
        HelmetCarapace,
        HelmetDrake,
        HelmetDverger,
        HelmetFenring,
        HelmetIron,
        HelmetLeather,
        HelmetMage,
        HelmetMidsummerCrown,
        HelmetOdin,
        HelmetPadded,
        HelmetRoot,
        HelmetTrollLeather,
        HelmetYule
    }

    internal enum LegsNPC
    {
        None,
        ArmorBronzeLegs,
        ArmorCarapaceLegs,
        ArmorFenringLegs,
        ArmorIronLegs,
        ArmorLeatherLegs,
        ArmorMageLegs,
        ArmorRagsLegs,
        ArmorRootLegs,
        ArmorTrollLeatherLegs,
        ArmorWolfLegs
    }

    internal enum ChestNPC
    {
        None,
        ArmorBronzeChest,
        ArmorCarapaceChest,
        ArmorFenringChest,
        ArmorIronChest,
        ArmorLeatherChest,
        ArmorMageChest,
        ArmorPaddedCuirass,
        ArmorRagsChest,
        ArmorRootChest,
        ArmorTrollLeatherChest,
        ArmorWolfChest
    }

    internal enum GenderNPC
    {
        Male = 0,
        Female = 1
    }

    internal enum BeardNPC
    {
        NoBeard = 0,
        Long_1 = 1,
        Long_2 = 2,
        Short_1 = 3,
        Short_2 = 4,
        Braided_1 = 5,
        Braided_2 = 6,
        Short_3 = 7,
        Thick_1 = 8,
        Braided_3 = 9,
        Braided_4 = 10,
        Thick_2 = 11,
        Royal_1 = 12,
        Royal_2 = 13,
        Braided_5 = 14,
        Short_4 = 15,
        Stonedweller = 16
    }

    internal enum HairNPC
    {
        NoHair = 0,
        Ponytail_1 = 1,
        Ponytail_2 = 2,
        Braided_1 = 3,
        Ponytail_3 = 4,
        Short_1 = 5,
        Long_1 = 6,
        Ponytail_4 = 7,
        Short_2 = 8,
        SideSwept_1 = 9,
        SideSwept_2 = 10,
        Braided_2 = 11,
        Braided_3 = 12,
        Braided_4 = 13,
        SideSwept_3 = 14,
        Curls_Pulled_Back = 15,
        Braids_Gathered = 16,
        Braids_Neat = 17,
        Braids_Royal = 18,
        Curls_1 = 19,
        Curls_2 = 20,
        Buns_Twin = 21,
        Buns_Single = 22,
        Curls_Short = 23
    }

    internal enum ColorNPC
    {
        White = 0,
        Grey = 1,
        Black = 2,

        BrownLight = 3,
        Brown = 4,
        BrownDark = 5,

        BlondLight = 6,
        Blond = 7,
        BlondDark = 8,

        GingerLight = 9,
        Ginger = 10,
        GingerDark = 11,

        SkinLight = 12,
        Skin = 13,
        SkinDark = 14
    }
}
