using HarmonyLib;
using UnityEngine;

namespace Cozyheim.BuildMore
{
    internal class GetMaterialProperties_Patch
    {
        [HarmonyPatch]
        private class Patch
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(WearNTear), "GetMaterialProperties")]
            static bool WearNTear_GetMaterialProperties_Prefix(ref WearNTear.MaterialType ___m_materialType, ref float maxSupport, ref float minSupport, ref float horizontalLoss, ref float verticalLoss)
            {
                if (ConfigSettings.enableCustomBuildSettings.Value)
                {
                    if (ConfigSettings.makeAllSame.Value)
                    {
                        maxSupport = ConfigSettings.allMaxSupport.Value;
                        minSupport = ConfigSettings.allWeight.Value;
                        horizontalLoss = 1f / Mathf.Max(1f, ConfigSettings.allBuildHorizontal.Value);
                        verticalLoss = 1f / Mathf.Max(1f, ConfigSettings.allBuildVertical.Value);
                        return false;
                    }

                    switch (___m_materialType)
                    {
                        case WearNTear.MaterialType.Wood:
                            maxSupport = ConfigSettings.woodMaxSupport.Value;
                            minSupport = ConfigSettings.woodWeight.Value;
                            horizontalLoss = 1f / Mathf.Max(1f, ConfigSettings.woodBuildHorizontal.Value);
                            verticalLoss = 1f / Mathf.Max(1f, ConfigSettings.woodBuildVertical.Value);
                            break;
                        case WearNTear.MaterialType.HardWood:
                            maxSupport = ConfigSettings.hardwoodMaxSupport.Value;
                            minSupport = ConfigSettings.hardwoodWeight.Value;
                            horizontalLoss = 1f / Mathf.Max(1f, ConfigSettings.hardwoodBuildHorizontal.Value);
                            verticalLoss = 1f / Mathf.Max(1f, ConfigSettings.hardwoodBuildVertical.Value);
                            break;
                        case WearNTear.MaterialType.Stone:
                            maxSupport = ConfigSettings.stoneMaxSupport.Value;
                            minSupport = ConfigSettings.stoneWeight.Value;
                            horizontalLoss = 1f / Mathf.Max(1f, ConfigSettings.stoneBuildHorizontal.Value);
                            verticalLoss = 1f / Mathf.Max(1f, ConfigSettings.stoneBuildVertical.Value);
                            break;
                        case WearNTear.MaterialType.Marble:
                            maxSupport = ConfigSettings.marbleMaxSupport.Value;
                            minSupport = ConfigSettings.marbleWeight.Value;
                            horizontalLoss = 1f / Mathf.Max(1f, ConfigSettings.marbleBuildHorizontal.Value);
                            verticalLoss = 1f / Mathf.Max(1f, ConfigSettings.marbleBuildVertical.Value);
                            break;
                        case WearNTear.MaterialType.Iron:
                            maxSupport = ConfigSettings.ironMaxSupport.Value;
                            minSupport = ConfigSettings.ironWeight.Value;
                            horizontalLoss = 1f / Mathf.Max(1f, ConfigSettings.ironBuildHorizontal.Value);
                            verticalLoss = 1f / Mathf.Max(1f, ConfigSettings.ironBuildVertical.Value);
                            break;
                        default:
                            maxSupport = ConfigSettings.allMaxSupport.Value;
                            minSupport = ConfigSettings.allWeight.Value;
                            horizontalLoss = 1f / Mathf.Max(1f, ConfigSettings.allBuildHorizontal.Value);
                            verticalLoss = 1f / Mathf.Max(1f, ConfigSettings.allBuildVertical.Value);
                            break;
                    }

                    return false;
                }

                return true;
            }
        }
    }
}
