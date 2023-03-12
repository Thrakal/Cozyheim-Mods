using HarmonyLib;
using UnityEngine;

namespace Cozyheim.LevelingSystem
{
    internal class PatchXP_Woodcutting : MonoBehaviour
    {
        [HarmonyPatch]
        private class PatchClass
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(TreeBase), "Damage")]
            private static void MineRock5_Damage_Prefix(TreeBase __instance, HitData hit, ZNetView ___m_nview)
            {
                if (__instance == null || hit == null || ___m_nview == null)
                {
                    return;
                }

                if (!___m_nview.IsValid())
                {
                    return;
                }

                if (hit.m_toolTier < __instance.m_minToolTier)
                {
                    return;
                }

                WoodcuttingXP(__instance.name, hit);
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(TreeLog), "Damage")]
            private static void MineRock_Damage_Prefix(TreeLog __instance, HitData hit, ZNetView ___m_nview)
            {
                if (__instance == null || hit == null || ___m_nview == null)
                {
                    return;
                }

                if (!___m_nview.IsValid())
                {
                    return;
                }

                if (hit.m_toolTier < __instance.m_minToolTier)
                {
                    return;
                }

                WoodcuttingXP(__instance.name, hit);
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(Destructible), "Damage")]
            private static void Destructible_Damage_Prefix(Destructible __instance, HitData hit, ZNetView ___m_nview, bool ___m_firstFrame)
            {
                if (__instance == null || hit == null || ___m_nview == null)
                {
                    return;
                }

                if (!___m_nview.IsValid() || ___m_firstFrame)
                {
                    return;
                }

                if(hit.m_toolTier < __instance.m_minToolTier)
                {
                    return;
                }

                WoodcuttingXP(__instance.name, hit);
            }

            private static void WoodcuttingXP(string name, HitData hit)
            {
                // Check if the XP system is enabled
                if (!Main.woodcuttingXpEnabled.Value)
                {
                    return;
                }

                if (hit.m_damage.m_chop <= 0)
                {
                    return;
                }

                // Check if the attacker is a player
                Player player = hit.GetAttacker().GetComponent<Player>();
                if (player == null)
                {
                    return;
                }

                // Check if the hit did any damage
                if (hit.GetTotalDamage() <= 0)
                {
                    return;
                }

                // Get xp from the table
                int xp = XPTable.GetWoodcuttingXP(name);
                if(xp <= 0)
                {
                    return;
                }

                // Send xp to the player
                ZPackage newPackage = new ZPackage();
                long playerID = player.GetPlayerID();
                newPackage.Write(playerID);
                newPackage.Write(xp);

                XPManager.rpc_RewardXP.SendPackage(ZRoutedRpc.Everybody, newPackage);
            }
        }
    }
}
