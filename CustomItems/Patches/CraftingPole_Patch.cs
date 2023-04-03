using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace Cozyheim.CustomItems
{
    internal class CraftingPole_Patch
    {
        [HarmonyPatch]
        private class Patch
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
        }
    }
}
