using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace CozyheimTweaks
{
    [HarmonyPatch]
    internal class SmokeFix
    {
        internal static ConfigFile patchConfig;

        internal static ConfigEntry<bool> enable;
        internal static ConfigEntry<float> timeUntilFade;
        internal static ConfigEntry<float> fadeTime;
        internal static ConfigEntry<float> sizeMin;
        internal static ConfigEntry<float> sizeMax;
        internal static ConfigEntry<float> moveSpeedMin;
        internal static ConfigEntry<float> moveSpeedMax;
        internal static ConfigEntry<float> spawnIntervalMin;
        internal static ConfigEntry<float> spawnIntervalMax;

        internal const string patchName = "SmokeFix";

        internal static void Init()
        {
            patchConfig = new ConfigFile(BepInEx.Paths.ConfigPath + "/Cozyheim/" + patchName + ".cfg", true);
            patchConfig.SaveOnConfigSet = true;

            enable = Main.CreateConfigEntry("General", "Enable this mod", true, "Allows this mod to be used", patchConfig);
            timeUntilFade = Main.CreateConfigEntry("General", "timeUntilFade", 3f, "Default value for vanilla valheim is '10'", patchConfig);
            fadeTime = Main.CreateConfigEntry("General", "fadeTime", 3f, "Default value for vanilla valheim is '3'", patchConfig);
            sizeMin = Main.CreateConfigEntry("General", "sizeMin", 0.5f, "Minimum size for smoke. Default value for vanilla valheim is '3'", patchConfig);
            sizeMax = Main.CreateConfigEntry("General", "sizeMax", 1.2f, "Maximum size for smoke. Default value for vanilla valheim is '3'", patchConfig);
            moveSpeedMin = Main.CreateConfigEntry("General", "moveSpeedMin", 3.5f, "Minimum move speed for smoke.", patchConfig);
            moveSpeedMax = Main.CreateConfigEntry("General", "moveSpeedMax", 6f, "Maximum move speed for smoke.", patchConfig);
            spawnIntervalMin = Main.CreateConfigEntry("General", "spawnIntervalMin", 0.2f, "Minimum time between each smoke is spawned.", patchConfig);
            spawnIntervalMax = Main.CreateConfigEntry("General", "spawnIntervalMax", 0.5f, "Maximum time between each smoke is spawned", patchConfig);
        }

        // ref bool __result = returned value of the original method
        // ref <T> __instance = reference to original class (this)
        // object[] __args = original parameters
        // -> Example: Player player = (Player) __args[0];

        [HarmonyPatch]
        private class PatchClass
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(Smoke), "Awake")]
            static void Smoke_Awake_Prefix(ref Smoke __instance, ref float ___m_ttl, ref float ___m_fadetime, ref float ___m_force)
            {
                if (enable.Value)
                {
                    ___m_ttl = timeUntilFade.Value;
                    ___m_fadetime = fadeTime.Value;

                    float speedValue = Random.Range(moveSpeedMin.Value, moveSpeedMax.Value);
                    ___m_force = speedValue;

                    float sizeValue = Random.Range(sizeMin.Value, sizeMax.Value);
                    __instance.transform.localScale = new UnityEngine.Vector3(sizeValue, sizeValue, sizeValue);
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(SmokeSpawner), "Spawn")]
            static void SmokeSpawner_Spawn_Prefix(ref float ___m_interval)
            {
                if (enable.Value)
                {
                    float spawnValue = Random.Range(spawnIntervalMin.Value, spawnIntervalMax.Value);
                    ___m_interval = spawnValue;
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(SmokeSpawner), "TestBlocked")]
            static bool SmokeSpawner_TestBlocked_Prefix(ref bool __result)
            {
                if (enable.Value)
                {
                    __result = false;
                    return false;
                }

                return true;
            }
        }
    }
}
