using HarmonyLib;
using Jotunn.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Cozyheim.LevelingSystem
{
    internal class PatchXP : MonoBehaviour
    {
        [HarmonyPatch]
        private class PatchClass
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(Character), "Damage")]
            private static void Character_Damage_Prefix(Character __instance, ref HitData hit, ZNetView ___m_nview)
            {
                if (!___m_nview.IsValid())
                {
                    ConsoleLog.Print("Damage: ZNetView not valid!", LogType.Error);
                    return;
                }

                if (hit == null)
                {
                    ConsoleLog.Print("Damage: No HitData found!", LogType.Error);
                    return;
                }

                if (__instance == null)
                {
                    ConsoleLog.Print("Damage: No Character found!", LogType.Error);
                    return;
                }

                Character target = __instance;
                Character attacker = hit.GetAttacker();
                float totalDamage = hit.GetTotalDamage();

                if (target == null)
                {
                    ConsoleLog.Print("Damage: No target found!", LogType.Error);
                    return;
                }

                if (!CanTargetAwardXP(target))
                {
                    ConsoleLog.Print("Damage: Target not a Monster!", LogType.Error);
                    return;
                }

                if (attacker == null)
                {
                    ConsoleLog.Print("Damage: No attacker found!", LogType.Error);
                    return;
                }

                if (!attacker.IsPlayer())
                {
                    ConsoleLog.Print("Damage: Attacker not a Player!", LogType.Error);
                    return;
                }

                if(Player.m_localPlayer == null)
                {
                    ConsoleLog.Print("Damage: No local player found!", LogType.Error);
                    return;
                }

                if (totalDamage <= 0f)
                {
                    ConsoleLog.Print("Damage: Total damage is less than 0!", LogType.Error);
                    return;
                }

                Player player = attacker.GetComponent<Player>();
                if(player != Player.m_localPlayer)
                {
                    ConsoleLog.Print("Damage: The attacker is not you!", LogType.Error);
                    return;
                }

                ConsoleLog.Print("Damage: Success! (Target = " + target.name + ", Attacker = " + player.GetPlayerName() + ", Damage: " + totalDamage.ToString("N1") + ")", LogType.Message);
                XPManager.Instance.AddMonsterDamage(target, attacker, totalDamage);
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(Character), "OnDeath")]
            private static void Character_OnDeath_Prefix(Character __instance)
            {
                if (CanTargetAwardXP(__instance) && Player.m_localPlayer != null)
                {
                    ZPackage newPackage = new ZPackage();

                    newPackage.Write(__instance.GetZDOID().id);
                    newPackage.Write(__instance.GetLevel());
                    newPackage.Write(__instance.name);

                    XPManager.rpc_RewardXPMonster.SendPackage(ZRoutedRpc.Everybody, newPackage);
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(Pickable), "Interact")]
            private static void Pickable_Interact_Prefix(Pickable __instance, Humanoid character, ZNetView ___m_nview, bool ___m_tarPreventsPicking)
            {
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

            [HarmonyPostfix]
            [HarmonyPatch(typeof(Player), "Start")]
            private static void Player_Start_Postfix(ref ZNetView ___m_nview)
            {
                if (ZNet.instance != null && Player.m_localPlayer != null)
                {
                    if(UIManager.Instance == null)
                    {
                        Instantiate(PrefabManager.Instance.GetPrefab("LevelingSystemUI"));
                    }
                }
            }


            [HarmonyPrefix]
            [HarmonyPatch(typeof(Game), "Logout")]
            private static void Game_Logout_Prefix()
            {
                if(UIManager.Instance != null)
                {
                    UIManager.Instance.DestroySelf();
                }
            }


            [HarmonyPrefix]
            [HarmonyPatch(typeof(Hud), "SetupPieceInfo")]
            private static void Hud_SetupPieceInfo_Prefix(ref RectTransform ___m_staminaBar2Root, ref GameObject ___m_buildHud, ref Text ___m_buildSelection, ref Text ___m_pieceDescription, ref GameObject[] ___m_requirementItems)
            {
                Vector2 hudOffset = new Vector2(0f, 50f);

                if(Main.modAugaLoaded)
                {
                    RectTransform buildSelection = ___m_buildSelection.GetComponent<RectTransform>();
                    buildSelection.anchoredPosition = new Vector2(214f, -23f) + hudOffset;

                    RectTransform pieceDescription = ___m_pieceDescription.GetComponent<RectTransform>();
                    pieceDescription.anchoredPosition = new Vector2(214f, -59f) + hudOffset;

                    RectTransform background = buildSelection.parent.Find("Darken").GetComponent<RectTransform>();
                    background.anchoredPosition = new Vector2(0f, -10f) + hudOffset;

                    for (int i = 0; i < ___m_requirementItems.Length; i++)
                    {
                        if (___m_requirementItems[i].activeSelf)
                        {
                            RectTransform rect = ___m_requirementItems[i].GetComponent<RectTransform>();
                            rect.anchoredPosition = new Vector2(32f + 70f * i, -32f) + hudOffset;
                        }
                    }
                } else {
                    RectTransform buildHUD = ___m_buildHud.GetComponent<RectTransform>();
                    buildHUD.anchoredPosition = hudOffset;

                    ___m_staminaBar2Root.anchorMin = new Vector2(0.5f, 0.05f);
                    ___m_staminaBar2Root.anchorMax = new Vector2(0.5f, 0.05f);
                }
            }

            private static bool CanTargetAwardXP(Character target)
            {
                if(target.IsMonsterFaction() || target.IsBoss() || target.GetFaction() == Character.Faction.AnimalsVeg || target.GetFaction() == Character.Faction.Dverger)
                {
                    return true;
                } else
                {
                    return false;
                }
            }
        }
    }
}
