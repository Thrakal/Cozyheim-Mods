using HarmonyLib;

namespace Cozyheim.MagicFromTheStart
{
    internal class PatchClass
    {
        [HarmonyPatch]
        private class Patch
        {
            // ref bool __result = returned value of the original method
            // ref <T> __instance = reference to original class (this)
            // object[] __args = original parameters
            // -> Example: Player player = (Player) __args[0];

            [HarmonyPrefix]
            [HarmonyPostfix]
            [HarmonyPatch(typeof(Player), "AutoPickup")]
            private static bool AutoPickupPatch(ref float ___m_autoPickupRange, ref Player __instance)
            {
                if (!__instance.IsPlayer())
                {
                    return false; // false = do not execute the original method
                }

                return true; // true = execute the original method after this patch is applied
            }
        }
    }
}
