using HarmonyLib;
using UnityEngine;

namespace Cozyheim.LessSmoke
{
    internal class SmokeFix
    {
        [HarmonyPatch]
        private class PatchClass
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(Smoke), "Awake")]
            static void Smoke_Awake_Prefix(ref Smoke __instance, ref float ___m_ttl, ref float ___m_fadetime, ref float ___m_force)
            {
                if (Main.modEnabled.Value)
                {
                    ___m_ttl = Main.timeUntilFade.Value;
                    ___m_fadetime = Main.fadeTime.Value;

                    float speedValue = Random.Range(Main.moveSpeedMin.Value, Main.moveSpeedMax.Value);
                    ___m_force = speedValue;

                    float sizeValue = Random.Range(Main.sizeMin.Value, Main.sizeMax.Value);
                    __instance.transform.localScale = new UnityEngine.Vector3(sizeValue, sizeValue, sizeValue);
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(SmokeSpawner), "Spawn")]
            static void SmokeSpawner_Spawn_Prefix(ref float ___m_interval)
            {
                if (Main.modEnabled.Value)
                {
                    float spawnValue = Random.Range(Main.spawnIntervalMin.Value, Main.spawnIntervalMax.Value);
                    ___m_interval = spawnValue;
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(SmokeSpawner), "TestBlocked")]
            static bool SmokeSpawner_TestBlocked_Prefix(ref bool __result)
            {
                if (Main.modEnabled.Value)
                {
                    __result = false;
                    return false;
                }

                return true;
            }
        }
    }
}
