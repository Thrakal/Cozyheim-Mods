using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace CozyheimTweaks
{
    internal class BuildMore
    {
        // Config variables
        internal static ConfigEntry<bool> enableCustomBuildSettings;

        internal static ConfigEntry<bool> buildInsideNoBuild;

        internal static ConfigEntry<bool> makeAllSame;
        internal static ConfigEntry<float> allMaxSupport;
        internal static ConfigEntry<float> allWeight;
        internal static ConfigEntry<float> allBuildVertical;
        internal static ConfigEntry<float> allBuildHorizontal;

        internal static ConfigEntry<float> woodMaxSupport;
        internal static ConfigEntry<float> woodWeight;
        internal static ConfigEntry<float> woodBuildVertical;
        internal static ConfigEntry<float> woodBuildHorizontal;

        internal static ConfigEntry<float> hardwoodMaxSupport;
        internal static ConfigEntry<float> hardwoodWeight;
        internal static ConfigEntry<float> hardwoodBuildVertical;
        internal static ConfigEntry<float> hardwoodBuildHorizontal;

        internal static ConfigEntry<float> stoneMaxSupport;
        internal static ConfigEntry<float> stoneWeight;
        internal static ConfigEntry<float> stoneBuildVertical;
        internal static ConfigEntry<float> stoneBuildHorizontal;

        internal static ConfigEntry<float> ironMaxSupport;
        internal static ConfigEntry<float> ironWeight;
        internal static ConfigEntry<float> ironBuildVertical;
        internal static ConfigEntry<float> ironBuildHorizontal;

        internal static ConfigEntry<float> marbleMaxSupport;
        internal static ConfigEntry<float> marbleWeight;
        internal static ConfigEntry<float> marbleBuildVertical;
        internal static ConfigEntry<float> marbleBuildHorizontal;

        public static void Init()
        {
            enableCustomBuildSettings = Main.CreateConfigEntry("Build/Material Settings", "enableCustomBuildSetting", true, "Allow to use custom settings for building", Main.buildMoreConfig);

            buildInsideNoBuild = Main.CreateConfigEntry("Build/Material Settings", "allowBuildInNoBuildZone", true, "Allow to build inside areas where you normally would see the text 'A mystical force prevents...'", Main.buildMoreConfig);

            // All materials are the same
            makeAllSame = Main.CreateConfigEntry("Build/Material Settings", "makeAllMaterialsTheSame", false, "Let all materials behave in the same way. If this is enabled, it will override all the individual material settings.", Main.buildMoreConfig);
            allMaxSupport = Main.CreateConfigEntry("Build/Material Settings", "allMaxSupport", 100f, "The maximum weight this material can support", Main.buildMoreConfig);
            allWeight = Main.CreateConfigEntry("Build/Material Settings", "allWeight", 10f, "The weight of the build material. If the total weight exceeds the 'max support' of the underlaying material, this material will collapse", Main.buildMoreConfig);
            allBuildVertical = Main.CreateConfigEntry("Build/Material Settings", "allVerticalBuildHeight", 8f, "The height at which you can build before collapse. Minimum value: 1", Main.buildMoreConfig);
            allBuildHorizontal = Main.CreateConfigEntry("Build/Material Settings", "allHorizontalBuildWidth", 5f, "The width at which you can build before collapse. Minimum value: 1", Main.buildMoreConfig);

            // Wood
            woodMaxSupport = Main.CreateConfigEntry("Build/Material Settings", "woodMaxSupport", 100f, "The maximum weight this material can support", Main.buildMoreConfig);
            woodWeight = Main.CreateConfigEntry("Build/Material Settings", "woodWeight", 10f, "The weight of the build material. If the total weight exceeds the 'max support' of the underlaying material, this material will collapse", Main.buildMoreConfig);
            woodBuildVertical = Main.CreateConfigEntry("Build/Material Settings", "woodVerticalBuildHeight", 8f, "The height at which you can build before collapse. Minimum value: 1", Main.buildMoreConfig);
            woodBuildHorizontal = Main.CreateConfigEntry("Build/Material Settings", "woodHorizontalBuildWidth", 5f, "The width at which you can build before collapse. Minimum value: 1", Main.buildMoreConfig);

            // Hard Wood
            hardwoodMaxSupport = Main.CreateConfigEntry("Build/Material Settings", "hardwoodMaxSupport", 140f, "The maximum weight this material can support", Main.buildMoreConfig);
            hardwoodWeight = Main.CreateConfigEntry("Build/Material Settings", "hardwoodWeight", 10f, "The weight of the build material. If the total weight exceeds the 'max support' of the underlaying material, this material will collapse", Main.buildMoreConfig);
            hardwoodBuildVertical = Main.CreateConfigEntry("Build/Material Settings", "hardwoodVerticalBuildHeight", 10f, "The height at which you can build before collapse. Minimum value: 1", Main.buildMoreConfig);
            hardwoodBuildHorizontal = Main.CreateConfigEntry("Build/Material Settings", "hardwoodHorizontalBuildWidth", 6f, "The width at which you can build before collapse. Minimum value: 1", Main.buildMoreConfig);

            // Stone
            stoneMaxSupport = Main.CreateConfigEntry("Build/Material Settings", "stoneMaxSupport", 1000f, "The maximum weight this material can support", Main.buildMoreConfig);
            stoneWeight = Main.CreateConfigEntry("Build/Material Settings", "stoneWeight", 100f, "The weight of the build material. If the total weight exceeds the 'max support' of the underlaying material, this material will collapse", Main.buildMoreConfig);
            stoneBuildVertical = Main.CreateConfigEntry("Build/Material Settings", "stoneVerticalBuildHeight", 8f, "The height at which you can build before collapse. Minimum value: 1", Main.buildMoreConfig);
            stoneBuildHorizontal = Main.CreateConfigEntry("Build/Material Settings", "stoneHorizontalBuildWidth", 1f, "The width at which you can build before collapse. Minimum value: 1", Main.buildMoreConfig);

            // Marble
            marbleMaxSupport = Main.CreateConfigEntry("Build/Material Settings", "marbleMaxSupport", 1500f, "The maximum weight this material can support", Main.buildMoreConfig);
            marbleWeight = Main.CreateConfigEntry("Build/Material Settings", "marbleWeight", 100f, "The weight of the build material. If the total weight exceeds the 'max support' of the underlaying material, this material will collapse", Main.buildMoreConfig);
            marbleBuildVertical = Main.CreateConfigEntry("Build/Material Settings", "marbleVerticalBuildHeight", 8f, "The height at which you can build before collapse. Minimum value: 1", Main.buildMoreConfig);
            marbleBuildHorizontal = Main.CreateConfigEntry("Build/Material Settings", "marbleHorizontalBuildWidth", 2f, "The width at which you can build before collapse. Minimum value: 1", Main.buildMoreConfig);

            // Iron
            ironMaxSupport = Main.CreateConfigEntry("Build/Material Settings", "ironMaxSupport", 1500f, "The maximum weight this material can support", Main.buildMoreConfig);
            ironWeight = Main.CreateConfigEntry("Build/Material Settings", "ironWeight", 20f, "The weight of the build material. If the total weight exceeds the 'max support' of the underlaying material, this material will collapse", Main.buildMoreConfig);
            ironBuildVertical = Main.CreateConfigEntry("Build/Material Settings", "ironVerticalBuildHeight", 13f, "The height at which you can build before collapse. Minimum value: 1", Main.buildMoreConfig);
            ironBuildHorizontal = Main.CreateConfigEntry("Build/Material Settings", "ironHorizontalBuildWidth", 13f, "The width at which you can build before collapse. Minimum value: 1", Main.buildMoreConfig);
        }

        [HarmonyPatch]
        private class BuildMore_Patch
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(Location), "IsInsideNoBuildLocation")]
            static bool Location_IsInsideNoBuildLocation_Prefix(ref bool __result)
            {
                if (buildInsideNoBuild.Value)
                {
                    __result = false;
                    return false;
                }
                return true;
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(WearNTear), "GetMaterialProperties")]
            static bool WearNTear_GetMaterialProperties_Prefix(ref WearNTear.MaterialType ___m_materialType, ref float maxSupport, ref float minSupport, ref float horizontalLoss, ref float verticalLoss)
            {
                if (enableCustomBuildSettings.Value)
                {
                    if (makeAllSame.Value)
                    {
                        maxSupport = allMaxSupport.Value;
                        minSupport = allWeight.Value;
                        horizontalLoss = 1f / Mathf.Max(1f, allBuildHorizontal.Value);
                        verticalLoss = 1f / Mathf.Max(1f, allBuildVertical.Value);
                        return false;
                    }

                    switch (___m_materialType)
                    {
                        case WearNTear.MaterialType.Wood:
                            maxSupport = woodMaxSupport.Value;
                            minSupport = woodWeight.Value;
                            horizontalLoss = 1f / Mathf.Max(1f, woodBuildHorizontal.Value);
                            verticalLoss = 1f / Mathf.Max(1f, woodBuildVertical.Value);
                            break;
                        case WearNTear.MaterialType.HardWood:
                            maxSupport = hardwoodMaxSupport.Value;
                            minSupport = hardwoodWeight.Value;
                            horizontalLoss = 1f / Mathf.Max(1f, hardwoodBuildHorizontal.Value);
                            verticalLoss = 1f / Mathf.Max(1f, hardwoodBuildVertical.Value);
                            break;
                        case WearNTear.MaterialType.Stone:
                            maxSupport = stoneMaxSupport.Value;
                            minSupport = stoneWeight.Value;
                            horizontalLoss = 1f / Mathf.Max(1f, stoneBuildHorizontal.Value);
                            verticalLoss = 1f / Mathf.Max(1f, stoneBuildVertical.Value);
                            break;
                        case WearNTear.MaterialType.Marble:
                            maxSupport = marbleMaxSupport.Value;
                            minSupport = marbleWeight.Value;
                            horizontalLoss = 1f / Mathf.Max(1f, marbleBuildHorizontal.Value);
                            verticalLoss = 1f / Mathf.Max(1f, marbleBuildVertical.Value);
                            break;
                        case WearNTear.MaterialType.Iron:
                            maxSupport = ironMaxSupport.Value;
                            minSupport = ironWeight.Value;
                            horizontalLoss = 1f / Mathf.Max(1f, ironBuildHorizontal.Value);
                            verticalLoss = 1f / Mathf.Max(1f, ironBuildVertical.Value);
                            break;
                        default:
                            maxSupport = allMaxSupport.Value;
                            minSupport = allWeight.Value;
                            horizontalLoss = 1f / Mathf.Max(1f, allBuildHorizontal.Value);
                            verticalLoss = 1f / Mathf.Max(1f, allBuildVertical.Value);
                            break;
                    }

                    return false;
                }

                return true;
            }
        }
    }
}
