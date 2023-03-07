using HarmonyLib;
using Jotunn.Managers;
using UnityEngine;
using System;

namespace Cozyheim.BurnBabyBurn
{
    [HarmonyPatch]
    internal class AlwaysBurn
    {
        // ref bool __result = returned value of the original method
        // ref <T> __instance = reference to original class (this)
        // object[] __args = original parameters
        // -> Example: Player player = (Player) __args[0];
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Fireplace), "Awake")]
        private static void Fireplace_Awake_Postfix(ref float ___m_startFuel, ref float ___m_maxFuel, Fireplace __instance)
        {
            ConsoleLog.Print("Awake fireplace!");
            if (IsExcluded(__instance)) return;

            ConsoleLog.Print("Not excluded: " + __instance.name);
            ___m_maxFuel = ___m_startFuel;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Fireplace), "UpdateFireplace")]
        private static void Fireplace_UpdateFireplace_Postfix(Fireplace __instance)
        {
            if (IsExcluded(__instance)) return;

            __instance.CancelInvoke("UpdateFireplace");
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Fireplace), "CheckEnv")]
        private static void Fireplace_CheckEnv_Postfix(Fireplace __instance)
        {
            if (IsExcluded(__instance)) return;

            __instance.CancelInvoke("CheckEnv");
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Fireplace), "IsBurning")]
        private static bool Fireplace_IsBurning_Prefix(ref bool __result, Fireplace __instance)
        {
            if (IsExcluded(__instance))
                return true;

            __result = true;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Fireplace), "Interact")]
        private static bool Fireplace_Interact_Prefix(ref bool __result, Fireplace __instance)
        {
            if (IsExcluded(__instance))
                return true;

            __result = false;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Fireplace), "GetHoverText")]
        private static bool Fireplace_GetHoverText_Prefix(ref string __result, string ___m_name, Fireplace __instance)
        {
            if (IsExcluded(__instance))
                return true;

            string text = Localization.instance.Localize(___m_name) + "\n";
            text += "<color=yellow>No fuel needed</color>";
            __result = text;
            return false;
        }

        private static bool IsExcluded(Fireplace fireplace)
        {
            if (Main.excludedFireplacesList.Count > 0)
            {
                foreach (string s in Main.excludedFireplacesList)
                {
                    if(s != "")
                    {
                        if (fireplace.name.StartsWith(s))
                            return true;
                    }
                }
            }

            return false;
        }
        
    }
    
}
