using HarmonyLib;
using Jotunn.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Cozyheim.LevelingSystem
{
    internal class PatchXP_Mining : MonoBehaviour
    {
        [HarmonyPatch]
        private class PatchClass
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(MineRock5), "Damage")]
            private static void MineRock5_Damage_Prefix(MineRock5 __instance, HitData hit, ZNetView ___m_nview)
            {
                if(!___m_nview.IsValid())
                {
                    return;
                }

                if (hit.m_toolTier < __instance.m_minToolTier)
                {
                    return;
                }

                MiningXP(__instance.name, hit);
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(MineRock), "Damage")]
            private static void MineRock_Damage_Prefix(MineRock __instance, HitData hit, ZNetView ___m_nview)
            {
                if (!___m_nview.IsValid())
                {
                    return;
                }

                if (hit.m_toolTier < __instance.m_minToolTier)
                {
                    return;
                }

                MiningXP(__instance.name, hit);
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(Destructible), "Damage")]
            private static void Destructible_Damage_Prefix(Destructible __instance, HitData hit, ZNetView ___m_nview)
            {
                if (!___m_nview.IsValid())
                {
                    return;
                }

                if(hit.m_toolTier < __instance.m_minToolTier)
                {
                    return;
                }

                MiningXP(__instance.name, hit);
            }

            private static void MiningXP(string name, HitData hit)
            {
                // Check if the XP system is enabled
                if (!Main.miningXpEnabled.Value)
                {
                    return;
                }

                if(hit.m_damage.m_pickaxe <= 0)
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
                int xp = XPTable.GetMiningXP(name);
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
