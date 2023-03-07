using HarmonyLib;
using UnityEngine;

namespace CozyheimTweaks
{
    [HarmonyPatch]
    internal class PlayerGamePatches
    {
        // ref bool __result = returned value of the original method
        // ref <T> __instance = reference to original class (this)
        // object[] __args = original parameters
        // -> Example: Player player = (Player) __args[0];

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Player), "AutoPickup")]
        public static void Player_AutoPickup_Prefix(ref float ___m_autoPickupRange, ref Player __instance)
        {
            if (!__instance.IsPlayer())
            {
                return;
            }
            ___m_autoPickupRange = Main.pickupRadius.Value;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CraftingStation), "GetHoverText")]
        public static void CS_GetHoverText_Prefix(ref float ___m_useDistance)
        {
            ___m_useDistance = Main.useDistance.Value;
        }

        // Faster teleport
        // ---------------

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Player), "TeleportTo")]
        public static void Player_TeleportTo_Postfix(ref float ___m_teleportTimer, ref bool ___m_distantTeleport)
        {
            if(Main.enableFastTelport.Value)
            {
                ___m_teleportTimer = 1f;
                ___m_distantTeleport = false;
            }
        }

        // Beehive and Honey patch
        // -----------------------

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Beehive), "Awake")]
        public static void Beehive_Awake_Prefix(ref int ___m_maxHoney, ref float ___m_secPerUnit, ref Beehive __instance)
        {
            ___m_maxHoney = (int) Main.maxHoney.Value;
            ___m_secPerUnit = Main.honeyGenerateTime.Value;
            float beehiveScale = Main.resizeBeehive.Value;
            __instance.transform.localScale = new Vector3(beehiveScale, beehiveScale, beehiveScale);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Player), "UpdatePlacementGhost")]
        public static void Player_UpdateGhsotPlacement_Prefix(ref GameObject ___m_placementGhost)
        {
            if(___m_placementGhost != null)
            {
                if (___m_placementGhost.name.StartsWith("piece_beehive"))
                {
                    float beehiveScale = Main.resizeBeehive.Value;
                    ___m_placementGhost.transform.localScale = new Vector3(beehiveScale, beehiveScale, beehiveScale);
                }
            }
        }
    }
}
