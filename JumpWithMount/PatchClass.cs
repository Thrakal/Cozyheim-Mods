using HarmonyLib;
using System;
using UnityEngine;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

namespace Cozyheim.JumpWithMount {
    internal class PatchClass {
        [HarmonyPatch]
        private class Patch {

            private static bool allowDismount = false;

            [HarmonyPrefix]
            [HarmonyPatch(typeof(Player), "Update")]
            private static void Player_Update_Prefix(ref Player __instance) {
                if(__instance.IsRiding()) {
                    if(Input.GetKeyDown(ConfigSettings.unmountKey.Value)) {
                        allowDismount = true;
                        __instance.StopDoodadControl();
                    }
                }
            }

                

            [HarmonyPrefix]
            [HarmonyPatch(typeof(Player), "StopDoodadControl")]
            private static bool Player_StopDoodadControl_Prefix(ref Player __instance) {
                if(__instance.IsRiding()) {
                    if(allowDismount) {
                        allowDismount = false;
                        return true;
                    }

                    return false;
                }

                return true;
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(Character), "Jump")]
            private static bool Character_Jump_Prefix(ref Character __instance, ref Rigidbody ___m_body, Vector3 ___m_lastGroundNormal, float ___m_jumpForce, Vector3 ___m_moveDir, float ___m_jumpForceForward, ref SEMan ___m_seman) {
                Player player = __instance.GetComponent<Player>();
                if(player == null) {
                    ConsoleLog.Print("Character not a player");
                    return true;
                }

                if(player != Player.m_localPlayer) {
                    ConsoleLog.Print("Character is not local player");
                    return true;
                }

                if(!player.IsRiding()) {
                    ConsoleLog.Print("Player is not riding");
                    return true;
                }

                // Find allColliders within 0.3m from player position
                Collider[] colls = Physics.OverlapSphere(__instance.transform.position, 0.3f);
                if(colls.Length == 0) {
                    ConsoleLog.Print("No colliders found");
                    return true;
                }

                // Check all colliders if they have a Sadle component
                foreach(Collider coll in colls) {
                    ConsoleLog.Print("Checking " + coll.name + " for Sadle component", LogType.Message);
                    Sadle sadle = coll.GetComponent<Sadle>();
                    if(sadle == null) {
                        ConsoleLog.Print("-> False");
                        continue;
                    }
                    ConsoleLog.Print("-> True");

                    Character animal = sadle.GetTameable().GetComponent<Character>();
                    if(animal == null) {
                        ConsoleLog.Print("Animal does not have a Character component");
                        continue;
                    }

                    animal.Jump();
                    ConsoleLog.Print("Animal Jumped!");
                    return false;
                }

                return true;
            }

        }
    }
}
