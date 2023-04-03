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
                if (ConfigSettings.modEnabled.Value)
                {
                    ___m_ttl = ConfigSettings.timeUntilFade.Value;
                    ___m_fadetime = ConfigSettings.fadeTime.Value;

                    float speedValue = Random.Range(ConfigSettings.moveSpeedMin.Value, ConfigSettings.moveSpeedMax.Value);
                    ___m_force = speedValue;

                    float sizeValue = Random.Range(ConfigSettings.sizeMin.Value, ConfigSettings.sizeMax.Value);
                    __instance.transform.localScale = new UnityEngine.Vector3(sizeValue, sizeValue, sizeValue);
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(SmokeSpawner), "Spawn")]
            static void SmokeSpawner_Spawn_Prefix(ref float ___m_interval)
            {
                if (ConfigSettings.modEnabled.Value)
                {
                    float spawnValue = Random.Range(ConfigSettings.spawnIntervalMin.Value, ConfigSettings.spawnIntervalMax.Value);
                    ___m_interval = spawnValue;
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(SmokeSpawner), "TestBlocked")]
            static bool SmokeSpawner_TestBlocked_Prefix(ref bool __result)
            {
                if (ConfigSettings.modEnabled.Value)
                {
                    __result = false;
                    return false;
                }

                return true;
            }
        }
    }
}
