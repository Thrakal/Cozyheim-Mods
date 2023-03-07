using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace CozyheimTweaks
{
    [HarmonyPatch]
    internal class SleepBetter
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Bed), "CheckFire")]
        static bool CheckFirePatch(ref bool __result, ref Bed __instance, object[] __args)
        {
            Player player = (Player)__args[0];

            if (!EffectArea.IsPointInsideArea(__instance.transform.position, EffectArea.Type.Heat, Main.fireRange.Value))
            {
                string pluralExtension = "";
                if (Main.fireRange.Value != 1f)
                {
                    pluralExtension = "s";
                }
                player.Message(MessageHud.MessageType.Center, "No fire within " + Main.fireRange.Value.ToString("N0") + " meter" + pluralExtension + "!");
                __result = false;
            }
            else
            {
                string sleepMsg = "Resting";

                if (Main.useCustomSleepMessages.Value)
                {
                    string[] sleepMessages = Main.customSleepMessages.Value.Split(',');
                    if (sleepMessages.Length > 0)
                    {
                        if (sleepMessages[0] != "")
                        {
                            sleepMsg = sleepMessages[UnityEngine.Random.Range(0, sleepMessages.Length)];
                        }
                    }
                }

                player.Message(MessageHud.MessageType.Center, sleepMsg);
                __result = true;
            }

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Bed), "CheckEnemies")]
        static bool CheckEnemiesPatch(ref bool __result)
        {
            if (Main.sleepWithEnemiesNearby.Value)
            {
                __result = true;
                return false; // won't execute the original method
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Bed), "CheckWet")]
        static bool CheckWetPatch(ref bool __result)
        {
            if (Main.sleepWhileWet.Value)
            {
                __result = true;
                return false; // won't execute the original method
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(EnvMan), "CanSleep")]
        static bool CanAlwaysSleep(ref bool __result)
        {
            if (Main.sleepDuringDaytime.Value)
            {
                __result = true;
                return false; // won't execute the original method
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Hud), "UpdateBlackScreen")]
        static void Hud_UpdateBlackScreen_Prefix(ref CanvasGroup ___m_loadingScreen, ref Player player, ref GameObject ___m_sleepingProgress)
        {
            if (Main.disableSleepingUI.Value)
            {
                if ((Object)(object)player != null && player.IsSleeping())
                {
                    ___m_sleepingProgress.SetActive(false);
                    ___m_loadingScreen.alpha = 0f;
                }
            }
        }
    }
}
