using HarmonyLib;

namespace Cozyheim.ModTemplate
{
    internal class PatchClass
    {
        [HarmonyPatch]
        private class Patch
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(Player), "Awake")]
            private static void Player_Awake_Prefix(ref Player __instance)
            {
                // Code here
            }
        }
    }
}
