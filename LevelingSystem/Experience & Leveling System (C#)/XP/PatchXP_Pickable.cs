using HarmonyLib;
using Jotunn.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Cozyheim.LevelingSystem
{
    internal class PatchXP_Pickable : MonoBehaviour
    {
        [HarmonyPatch]
        private class PatchClass
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(Pickable), "Interact")]
            private static void Pickable_Interact_Prefix(Pickable __instance, Humanoid character, ZNetView ___m_nview, bool ___m_tarPreventsPicking)
            {
                if (__instance == null || character == null || ___m_nview == null)
                {
                    return;
                }

                if (!Main.pickableXpEnabled.Value)
                {
                    return;
                }

                if(!___m_nview.IsValid())
                {
                    return;
                }

                if(___m_tarPreventsPicking)
                {
                    return;
                }

                Player player = character.gameObject.GetComponent<Player>();
                if (player == null)
                {
                    return;
                }

                int xp = XPTable.GetPickableXP(__instance.name);

                ZPackage newPackage = new ZPackage();
                long playerID = player.GetPlayerID();
                newPackage.Write(playerID);
                newPackage.Write(xp);

                XPManager.rpc_RewardXP.SendPackage(ZRoutedRpc.Everybody, newPackage);
            }
        }
    }
}
