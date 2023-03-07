using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CozyheimTweaks
{
    [HarmonyPatch]
    internal class RemoveMistlandMist
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ParticleMist), "Update")]
        public static bool PM_Update_Patch(ParticleMist __instance)
        {
            if (!Main.enableLocalMist.Value)
            {
                __instance.m_localRange = 0f;
                __instance.m_maxDistance = 0f;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Mister), "Awake")]
        public static void Mister_Awake_Patch(ref float ___m_radius)
        {
            ___m_radius = Main.misterRadius.Value;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Mister), "Inside")]
        public static void Mister_Inside_Patch(ref float ___m_radius)
        {
            ___m_radius = Main.misterRadius.Value;
        }
    }
}
