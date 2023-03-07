using HarmonyLib;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Cozyheim.AutoCloseDoors
{
    internal class PatchClass
    {
        [HarmonyPatch]
        private class Patch
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(Door), "Awake")]
            private static void Door_Awake_Postfix(Door __instance)
            {
                AutoCloseCheck check = __instance.GetComponent<AutoCloseCheck>();
                if (check == null) {
                    __instance.gameObject.AddComponent<AutoCloseCheck>();
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(Door), "Interact")]
            private static void Door_Interact_Prefix(Door __instance)
            {
                AutoCloseCheck check = __instance.GetComponent<AutoCloseCheck>();
                if (check != null)
                {
                    check.Interact();
                }
            }
        }
    }
}
