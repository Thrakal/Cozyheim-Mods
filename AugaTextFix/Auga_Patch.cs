using AugaUnity;
using HarmonyLib;
using System;
using UnityEngine.UI;

namespace Cozyheim.AugaTextFix {
    internal class Auga_Patch {

        [HarmonyPatch]
        private class Patch {

            [HarmonyPrefix]
            [HarmonyPatch(typeof(TooltipTextBox), "AddLine", new Type[] { typeof(Text), typeof(object), typeof(bool) })]
            static void TooltipTextBox_AddLine_Prefix(ref Text t, ref object s) {
                if(t == null) {
                    return;
                }

                s = FormatNumber(s);
            }


            [HarmonyPrefix]
            [HarmonyPatch(typeof(TooltipTextBox), "AddUpgradeLine")]
            static void TooltipTextBox_AddUpgradeLine_Prefix(ref object value2) {
                value2 = FormatNumber(value2);
            }


            [HarmonyPrefix]
            [HarmonyPatch(typeof(TooltipTextBox), "GenerateParenthetical")]
            static void TooltipTextBox_GenerateParenthetical_Prefix(ref object b) {
                b = FormatNumber(b);
            }


            private static object FormatNumber(object stringToCheck) {
                if(float.TryParse(stringToCheck.ToString(), out float value)) {
                    if(value >= 1000) {
                        return value.ToString("N0");
                    }

                    if(Math.Abs(value % 1) <= float.Epsilon) {
                        return value.ToString("N0");
                    } else {
                        return value.ToString("N1");
                    }
                }

                return stringToCheck;
            }
        }
    }
}
