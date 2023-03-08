using HarmonyLib;

namespace Cozyheim.TestMod
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
