using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Cozyheim.CustomItems
{
    internal class StackableChests_Patch
    {
        [HarmonyPatch]
        private class Patch
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(Container), "Interact")]
            public static bool Container_Interact_Prefix(Container __instance, ref bool __result, Humanoid character, bool hold, bool alt)
            {
                if (__instance.name.StartsWith("piece_chest_1x1_stackable"))
                {
                    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    {
                        Sign sign = __instance.GetComponent<Sign>();
                        sign.Interact(character, hold, alt);
                        __result = false;
                        return false;
                    }
                }

                return true;
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(Sign), "Awake")]
            public static void Sign_Awake_Prefix(Sign __instance, ref TextMeshProUGUI ___m_textWidget)
            {
                if (__instance.name.StartsWith("piece_chest_1x1_stackable"))
                {
                    ___m_textWidget = __instance.transform.Find("New/Chest Container (Closed)/Sign/Text").GetComponent<TextMeshProUGUI>();
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(Sign), "UpdateText")]
            public static void Sign_UpdateText_Postfix(Sign __instance)
            {
                if (__instance.name.StartsWith("piece_chest_1x1_stackable"))
                {
                    TextMeshProUGUI[] texts = __instance.GetComponentsInChildren<TextMeshProUGUI>(true);
                    foreach (TextMeshProUGUI t in texts)
                    {
                        t.text = __instance.GetText();
                    }
                }
            }
            
        }
    }
}
