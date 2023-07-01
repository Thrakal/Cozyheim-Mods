using HarmonyLib;
using System;
using UndeadBits.ValheimMods.PortalStation;
using UnityEngine;

namespace CozyheimTweaks
{
    [HarmonyPatch]
    internal class PortalStationFix : MonoBehaviour {

        private static bool openGUI = false;

        /*
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BaseTeleportationGUI), "OpenGUI")]
        static void BaseTeleportationGUI_OpenGUI_Prefix()
        {
            ToppLog.Print("Teleport OpenGUI called!");
            openGUI = true;
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(BaseTeleportationGUI), "Close")]
        public static void BaseTeleportationGUI_CloseGUI() {
            ToppLog.Print("Teleport Close called!");
        }

        void Update() {
            if(Main.isPortalStationsLoaded) {
                if(openGUI) {
                    if(Input.GetKeyDown(KeyCode.Escape)) {
                        ToppLog.Print("Escape pressed!");
                        BaseTeleportationGUI_CloseGUI();
                        openGUI = false;
                    }
                }
            }
        }
        */
    }
}
