using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Managers;
using UnityEngine;
using CozyheimTweaks.Scripts;
using Jotunn.Entities;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using System.Net.Sockets;

namespace CozyheimTweaks
{
    internal class WardProtection : MonoBehaviour
    {
        private static WardProtection _instance;
        public static WardProtection Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new WardProtection();
                }
                return _instance;
            }
        }

        internal static ConfigFile patchConfig;

        internal static ConfigEntry<bool> enable;
        internal static ConfigEntry<bool> debugMode;
        internal static ConfigEntry<bool> adminOverrules;
        internal static ConfigEntry<float> protectionRadius;

        internal const string patchName = "WardProtection";

        // JSON file save info
        private static string dirPath = BepInEx.Paths.ConfigPath + "/Cozyheim/" + patchName + "_WorldSaves";
        private static string jsonFilePath = "";
        private static List<WardPlayerData> playerData;

        // RPC Network communication
        public static CustomRPC rpc_CheckJSON;
        public static CustomRPC rpc_UpdatePlayerData;
        public static CustomRPC rpc_SendPlayerDataToServer;
        public static CustomRPC rpc_CreateNewUser;
        public static CustomRPC rpc_ReloadJSON;

        internal void Init()
        {
            CommandManager.Instance.AddConsoleCommand(new WardConsole());

            patchConfig = new ConfigFile(BepInEx.Paths.ConfigPath + "/Cozyheim/" + patchName + "Config.cfg", true);
            patchConfig.SaveOnConfigSet = true;

            enable = Main.CreateConfigEntry("General", "Enable this mod", true, "Allows this mod to be used", patchConfig);
            debugMode = Main.CreateConfigEntry("General", "debugMode", false, "Write debug messages in the console", patchConfig);
            adminOverrules = Main.CreateConfigEntry("General", "adminOverrules", true, "Allow admins to ignore the protection of the wards", patchConfig);
            protectionRadius = Main.CreateConfigEntry("General", "protectionRadius", 30f, "The radius at which each Ward protects", patchConfig);

            rpc_CheckJSON = NetworkManager.Instance.AddRPC("CheckJSON", RPC_CheckJSON, RPC_CheckJSON);
            rpc_UpdatePlayerData = NetworkManager.Instance.AddRPC("UpdatePlayerData", RPC_UpdatePlayerData, RPC_UpdatePlayerData);
            rpc_SendPlayerDataToServer = NetworkManager.Instance.AddRPC("SendPlayerDataToServer", RPC_SendPlayerDataToServer, RPC_SendPlayerDataToServer);
            rpc_CreateNewUser = NetworkManager.Instance.AddRPC("CreateNewUser", RPC_CreateNewUser, RPC_CreateNewUser);
            rpc_ReloadJSON = NetworkManager.Instance.AddRPC("ReloadJSON", RPC_ReloadPlayerDataJSON, RPC_ReloadPlayerDataJSON);

            playerData = new List<WardPlayerData>();

            ToppLog.Print("Ward Init!", LogType.Message, debugMode.Value);
        }

        internal IEnumerator RPC_CreateNewUser(long sender, ZPackage package)
        {
            if(!ZNet.instance.IsServer())
            {
                yield break;
            } 

            string username = package.ReadString();
            long userID = package.ReadLong();

            bool foundUser = false;
            foreach (WardPlayerData player in playerData)
            {
                if (player.userID == userID)
                {
                    foundUser = true;
                    break;
                }
            }

            if (!foundUser)
            {
                playerData.Add(new WardPlayerData(username, userID, 1, 0));
            }

            SavePlayerDataJSON();
            yield return null;
        }

        internal void AddToWhitelist(long userID, long whitelistID, int status = 3)
        {
            foreach (WardPlayerData player in playerData)
            {
                if (player.userID == userID)
                {
                    player.WhitelistAddUser(whitelistID, (WhitelistStatus) status);
                    SendPlayerDataToServer(player);
                    break;
                }
            }
        }

        internal void UpdateWhitelist(long userID, long whitelistID, int status)
        {
            foreach (WardPlayerData player in playerData)
            {
                if (player.userID == userID)
                {
                    player.WhitelistUpdateStatus(whitelistID, (WhitelistStatus)status);
                    SendPlayerDataToServer(player);
                    break;
                }
            }
        }

        internal void RemoveFromWhitelist(long userID, long whitelistID)
        {
            foreach (WardPlayerData player in playerData)
            {
                if (player.userID == userID)
                {
                    player.WhitelistRemoveUser(whitelistID);
                    SendPlayerDataToServer(player);
                    break;
                }
            }
        }

        internal bool BuildWard()
        {
            WardPlayerData player = GetPlayerData(Player.m_localPlayer.GetPlayerID());
            if(player != null)
            {
                if(!player.WardBuild())
                {
                    Player.m_localPlayer.Message(MessageHud.MessageType.Center, "You are not allowed to build more Wards");
                    return false;
                } else
                {
                    SendPlayerDataToServer(player);
                    return true;
                }
            }
            Player.m_localPlayer.Message(MessageHud.MessageType.Center, "* Error: Ward Player Data Missing *");
            return false;
        }

        internal bool RemoveWard(long creatorID)
        {
            WardPlayerData player = GetPlayerData(creatorID);
            if (player != null)
            {
                if (!player.WardDestroy())
                {
                    Player.m_localPlayer.Message(MessageHud.MessageType.Center, "* Error: Ward Count *");
                    return false;
                }
                else
                {
                    SendPlayerDataToServer(player);
                    return true;
                }
            }
            Player.m_localPlayer.Message(MessageHud.MessageType.Center, "* Error: Ward Player Data Missing *");
            return false;
        }

        internal void SendPlayerDataToServer(WardPlayerData player)
        {
            ZPackage package = new ZPackage();
            package.Write(player.GetPlayerJSON().ToString());
            rpc_SendPlayerDataToServer.SendPackage(ZRoutedRpc.Everybody, package);
        }

        internal WardPlayerData GetPlayerData(long userID)
        {
            foreach(WardPlayerData player in playerData)
            {
                if(player.userID == userID)
                {
                    return player;
                }
            }

            return null;
        }

        internal IEnumerator RPC_SendPlayerDataToServer(long sender, ZPackage package)
        {
            ToppLog.Print("SendPlayerDataToServer - received");
            if(!ZNet.instance.IsServer())
            {
                ToppLog.Print("SendPlayerDataToServer - not server");
                yield break;
            }

            ToppLog.Print("SendPlayerDataToServer - is server");
            JSONObject playerJSON = (JSONObject) JSON.Parse(package.ReadString());
            long userID = playerJSON["userID"].AsLong;

            foreach(WardPlayerData player in playerData)
            {
                ToppLog.Print("SendPlayerDataToServer - checking player: " + player.userID);
                if (player.userID == userID)
                {
                    ToppLog.Print("SendPlayerDataToServer - player match found");
                    player.UpdatePlayerData(playerJSON);
                }
            }

            ToppLog.Print("SendPlayerDataToServer - saving");
            SavePlayerDataJSON();

            yield return null;
        }

        internal IEnumerator RPC_CheckJSON(long sender, ZPackage package)
        {
            // Wait until the connection is established
            while (ZNet.instance == null)
            {
                ToppLog.Print("Loading JSON World file - Check", debugMode.Value);
                yield return new WaitForSeconds(2f);
            }

            // Stop if it's not the server
            if (!ZNet.instance.IsServer())
            {
                yield break;
            }

            string worldName = ZNet.instance.GetWorldName() + "_" + ZNet.instance.GetWorldUID().ToString();
            jsonFilePath = dirPath + "/" + worldName + ".json";

            // Check if the folder already exists
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            // Check if the JSON file for the world already exists
            if (!File.Exists(jsonFilePath))
            {
                JSONObject jsonObject = new JSONObject();
                jsonObject.Add("worldName", ZNet.instance.GetWorldName());
                jsonObject.Add("worldUID", ZNet.instance.GetWorldUID().ToString());

                JSONArray playerData = new JSONArray();

                jsonObject.Add("playerData", playerData);

                File.WriteAllText(jsonFilePath, jsonObject.ToString());
            }

            LoadPlayerDataJSON();
        }

        internal IEnumerator RPC_UpdatePlayerData(long sender, ZPackage package)
        {
            playerData.Clear();
            string jsonArray = package.ReadString();
            JSONArray wardPlayerData = JSON.Parse(jsonArray).AsArray;
            foreach (JSONObject player in wardPlayerData)
            {
                playerData.Add(new WardPlayerData(player));
            }

            yield return null;
        }

        internal IEnumerator RPC_ReloadPlayerDataJSON(long sender, ZPackage package)
        {
            if(ZNet.instance.IsServer())
            {
                LoadPlayerDataJSON();
            }
            yield return null;
        }

        internal void LoadPlayerDataJSON()
        {
            playerData.Clear();
            JSONObject jsonObject = (JSONObject)JSON.Parse(File.ReadAllText(jsonFilePath));
            JSONArray wardPlayerData = jsonObject["playerData"].AsArray;

            foreach (JSONObject player in wardPlayerData)
            {
                playerData.Add(new WardPlayerData(player));
            }

            SavePlayerDataJSON();
        }

        internal void SavePlayerDataJSON()
        {
            // Save the playerdata in the JSON file
            JSONObject jsonObject = (JSONObject)JSON.Parse(File.ReadAllText(jsonFilePath));

            JSONArray playerArray = new JSONArray();
            foreach (WardPlayerData player in playerData)
            {
                playerArray.Add(player.GetPlayerJSON());
            }

            jsonObject["playerData"] = playerArray;
            File.WriteAllText(jsonFilePath, jsonObject.ToString());

            // Send the playerData to all clients

            ZPackage package = new ZPackage();
            package.Write(playerArray.ToString());
            rpc_UpdatePlayerData.SendPackage(ZRoutedRpc.Everybody, package);
        }


        [HarmonyPatch]
        private class PatchClass
        {
            private static float lastInteractTime;
            private static string creatorName;

            [HarmonyPrefix]
            [HarmonyPatch(typeof(Player), "Interact")]
            static bool Player_Interact_Prefix(GameObject go)
            {
                if(Time.time - lastInteractTime < 0.3f)
                {
                    return false;
                }
                lastInteractTime = Time.time;

//                ToppLog.Print("Interacted with " + go.transform.root.name, LogType.Message);
                
                if (InteractInsideProtectedArea(go.transform.root.position))
                {
                    Player.m_localPlayer.Message(MessageHud.MessageType.Center, creatorName + "'s area: Interaction not allowed");
                    return false;
                }

                return true;
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(WearNTear), "Damage")]
            static bool WearNTear_Damage_Prefix(WearNTear __instance, HitData hit)
            {
                Player player = hit.GetAttacker().GetComponent<Player>();
                if(player != Player.m_localPlayer)
                {
                    return true;
                }

                if (BuildInsideProtectedArea(__instance.transform.position))
                {
                    Player.m_localPlayer.Message(MessageHud.MessageType.Center, creatorName + "'s area: Damaging not allowed");
                    return false;
                }

                return true;
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(ZNet), "LoadWorld")]
            static void ZNet_LoadWorld_Postfix()
            {
                ToppLog.Print("Loading world", debugMode.Value);
                WardProtection.rpc_CheckJSON.SendPackage(ZRoutedRpc.Everybody, new ZPackage());
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(Player), "OnSpawned")]
            static void Player_OnSpawned_Prefix() {
                if (ZNet.instance != null && Player.m_localPlayer != null)
                {
                    ZPackage package = new ZPackage();
                    package.Write(Game.instance.GetPlayerProfile().GetName());
                    package.Write(Player.m_localPlayer.GetPlayerID());
                    WardProtection.rpc_CreateNewUser.SendPackage(ZRoutedRpc.Everybody, package);
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(Player), "PlacePiece")]
            static bool Player_PlacePiece_Prefix(Piece piece, GameObject ___m_placementGhost)
            {
                WardProtect ward = piece.GetComponent<WardProtect>();

                // Check if piece is inside a protected area
                if (BuildInsideProtectedArea(___m_placementGhost.transform.position, ward != null))
                {
                    Player.m_localPlayer.Message(MessageHud.MessageType.Center, creatorName + "'s area: Building not allowed");
                    return false;
                }

                if(ward != null)
                {
                    return WardProtection.Instance.BuildWard();
                }

                return true;
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(Player), "RemovePiece")]
            static bool Player_RemovePiece_Prefix(ref bool __result, Transform ___m_eye, float ___m_maxPlaceDistance, int ___m_removeRayMask)
            {
                if (Physics.Raycast(GameCamera.instance.transform.position, GameCamera.instance.transform.forward, out var hitInfo, 50f, ___m_removeRayMask) && Vector3.Distance(hitInfo.point, ___m_eye.position) < ___m_maxPlaceDistance)
                {
                    Piece piece = hitInfo.collider.GetComponentInParent<Piece>();
                    if (piece == null && (bool)hitInfo.collider.GetComponent<Heightmap>())
                    {
                        piece = TerrainModifier.FindClosestModifierPieceInRange(hitInfo.point, 2.5f);
                    }

                    if ((bool) piece)
                    {
                        if (BuildInsideProtectedArea(piece.transform.position))
                        {
                            Player.m_localPlayer.Message(MessageHud.MessageType.Center, creatorName + "'s area: Destroying not allowed");
                            __result = false;
                            return false;
                        }

                        WardProtect ward = piece.GetComponent<WardProtect>();
                        if(ward != null)
                        {
                            return WardProtection.Instance.RemoveWard(piece.GetCreator());
                        }

                        return true;
                    }
                }

                return true;
            }

            private static bool InteractInsideProtectedArea(Vector3 position)
            {
                float checkRadius = protectionRadius.Value;
                Collider[] colls = Physics.OverlapSphere(position, checkRadius);
                foreach (Collider coll in colls)
                {
                    // Check if a ward is found
                    WardProtect ward = coll.GetComponentInParent<WardProtect>();
                    if (ward == null)
                    {
                        continue;
                    }

                    // Check if the user is the creator of the ward
                    long creatorID = ward.GetComponent<Piece>().GetCreator();
                    if (ward.GetComponent<Piece>().IsCreator() || creatorID == 0L)
                    {
                        continue;
                    }

                    // If a ward is found, check if the user has Admin status
                    WardPlayerData creator = WardProtection.Instance.GetPlayerData(creatorID);
                    if (SynchronizationManager.Instance.PlayerIsAdmin && adminOverrules.Value)
                    {
                        Player.m_localPlayer.Message(MessageHud.MessageType.Center, creator.username + "'s area. (Admin override)");
                        return false;
                    }

                    // Check if the user is whitelisted by the creator
                    WhitelistStatus status = creator.GetWhitelistStatus(Player.m_localPlayer.GetPlayerID());
                    if (status == WhitelistStatus.InteractOnly || status == WhitelistStatus.Everything)
                    {
                        return false;
                    }

                    creatorName = creator.username;
                    return true;
                }

                return false;
            }

            private static bool BuildInsideProtectedArea(Vector3 position, bool placeWard = false)
            {
                float checkRadius = !placeWard ? protectionRadius.Value : protectionRadius.Value * 2;
                Collider[] colls = Physics.OverlapSphere(position, checkRadius);
                foreach (Collider coll in colls)
                {
                    // Check if a ward is found
                    WardProtect ward = coll.GetComponentInParent<WardProtect>();
                    if (ward == null)
                    {
                        continue;
                    }

                    // Check if the user is the creator of the ward
                    long creatorID = ward.GetComponent<Piece>().GetCreator();
                    if (ward.GetComponent<Piece>().IsCreator() || creatorID == 0L)
                    {
                        continue;
                    }

                    // If a ward is found, check if the user has Admin status
                    WardPlayerData creator = WardProtection.Instance.GetPlayerData(creatorID);
                    if (SynchronizationManager.Instance.PlayerIsAdmin && adminOverrules.Value)
                    {
                        Player.m_localPlayer.Message(MessageHud.MessageType.Center, creator.username.ToString() + "'s area. (Admin override)");
                        return false;
                    }

                    // Check if the user is whitelisted by the creator
                    WhitelistStatus status = creator.GetWhitelistStatus(Player.m_localPlayer.GetPlayerID());
                    if(status == WhitelistStatus.BuildOnly || status == WhitelistStatus.Everything)
                    {
                        return false;
                    }

                    creatorName = creator.username;
                    return true;
                }

                return false;
            }
        }

    }
}
