using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Entities;
using Jotunn.Managers;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

namespace Cozyheim.LevelingSystem
{
    internal class XPManager : MonoBehaviour
    {
        private static string saveLevelString = "CozyLevel";
        private static string saveXpString = "CozyXP";

        private List<MonsterXP> xpObjects = new List<MonsterXP>();

        // Network communication RPC
        public static CustomRPC rpc_AddMonsterDamage = null;
        public static CustomRPC rpc_RewardXPMonster = null;
        public static CustomRPC rpc_RewardXP = null;

        private static XPManager _instance;
        public static XPManager Instance {
            get {
                if (_instance == null)
                {
                    _instance = new GameObject("XPManager").AddComponent<XPManager>();
                }
                return _instance;
            }
        }

        public static void Init()
        {
            // Register RPC Methods
            rpc_AddMonsterDamage = NetworkManager.Instance.AddRPC("AddMonsterDamage", RPC_AddMonsterDamage, RPC_AddMonsterDamage);
            rpc_RewardXPMonster = NetworkManager.Instance.AddRPC("RewardXPMonster", RPC_RewardXPMonsters, RPC_RewardXPMonsters);
            rpc_RewardXP = NetworkManager.Instance.AddRPC("RewardXP", RPC_RewardXP, RPC_RewardXP);
        }

        private static IEnumerator RPC_AddMonsterDamage(long sender, ZPackage package)
        {
            if (!ZNet.instance.IsServer())
            {
                yield break;
            }

            uint monsterID = package.ReadUInt();
            long playerID = package.ReadLong();
            float damage = package.ReadSingle();
            string playerName = package.ReadString();

            MonsterXP obj = Instance.GetMonsterXP(monsterID);
            if (obj != null)
            {
                ConsoleLog.Print("Updated monster damage (Server)");
                obj.AddDamage(playerID, damage, playerName);
            }
            else
            {
                ConsoleLog.Print("Created new monster damage (Server)");
                MonsterXP newObj = Instance.CreateNewMonsterXP(monsterID);
                newObj.AddDamage(playerID, damage, playerName);
            }

            yield return null;
        }

        public void AddMonsterDamage(Character monster, Character player, float damage)
        {
            uint monsterID = monster.GetZDOID().id;
            long playerID = player.GetComponent<Player>().GetPlayerID();
            string playerName = player.GetComponent<Player>().GetPlayerName();

            ZPackage newPackage = new ZPackage();
            newPackage.Write(monsterID);
            newPackage.Write(playerID);
            newPackage.Write(damage);
            newPackage.Write(playerName);

            ConsoleLog.Print("Sending damage to server RPC");
            rpc_AddMonsterDamage.SendPackage(ZRoutedRpc.Everybody, newPackage);
        }

        private MonsterXP CreateNewMonsterXP(uint monsterID)
        {
            MonsterXP newObj = new MonsterXP(monsterID);
            xpObjects.Add(newObj);

            return newObj;
        }

        private static IEnumerator RPC_RewardXP(long sender, ZPackage package)
        {
            if (!ZNet.instance.IsServer())
            {
                yield break;
            }

            ZPackage newPackage = new ZPackage();
            long playerID = package.ReadLong();
            int xpAmount = package.ReadInt();

            float baseXpSpreadMin = Mathf.Min(1 - (Main.baseXpSpreadMin.Value / 100f), 1f);
            float baseXpSpreadMax = Mathf.Max(1 + (Main.baseXpSpreadMax.Value / 100f), 1f);
            float xpMultiplier = Mathf.Max(0f, Main.allXPMultiplier.Value / 100f);

            newPackage.Write(playerID);
            newPackage.Write((int)(xpAmount * xpMultiplier * Random.Range(baseXpSpreadMin, baseXpSpreadMax)));

            UIManager.rpc_AddExperience.SendPackage(ZRoutedRpc.Everybody, newPackage);
        } 

        private static IEnumerator RPC_RewardXPMonsters(long sender, ZPackage package)
        {
            if (!ZNet.instance.IsServer())
            {
                yield break;
            }

            ConsoleLog.Print("Monster died (Server)");

            uint monsterID = package.ReadUInt();
            uint monsterLevel = package.ReadUInt();
            string monsterName = package.ReadString();

            MonsterXP obj = Instance.GetMonsterXP(monsterID);
            if (obj != null)
            {
                float totalDamage = obj.GetTotalDamageDealt();


                // Find the correct monster in the list
                foreach (PlayerDamage damage in obj.playerDamages)
                {
                    ZPackage newPackage = new ZPackage();

                    // Get the percentage of damage the player has dealt
                    float xpPercentage = damage.playerTotalDamage / totalDamage;

                    // Reward with xp based on monster type killed
                    float baseXpSpreadMin = Mathf.Min(1 - (Main.baseXpSpreadMin.Value / 100f), 1f);
                    float baseXpSpreadMax = Mathf.Max(1 + (Main.baseXpSpreadMax.Value / 100f), 1f);
                    float monsterLvlMultiplier = Mathf.Max(0f, Main.monsterLvlXPMultiplier.Value / 100f);
                    float xpMultiplier = Mathf.Max(0f, Main.allXPMultiplier.Value / 100f);
                    float restedMultiplier = Mathf.Max(0f, Main.restedXPMultiplier.Value / 100f);


                    float awardedXP = XPTable.GetMonsterXP(monsterName) * xpPercentage * Random.Range(baseXpSpreadMin, baseXpSpreadMax) * xpMultiplier;
                    float monsterLevelBonusXp = (monsterLevel - 1) * monsterLvlMultiplier * awardedXP;
                    float restedBonusXp = awardedXP * restedMultiplier;

                    newPackage.Write((int)awardedXP);
                    newPackage.Write((int)monsterLevelBonusXp);
                    newPackage.Write((int)restedBonusXp);
                    newPackage.Write(damage.playerID);


                    ConsoleLog.Print("Sending " + (xpPercentage * 100f).ToString("N1") + "% xp to " + damage.playerName + ". (Awarded: " + (int)awardedXP + ", Level bonus: " + (int)monsterLevelBonusXp + ", Rested bonus: " + (int)restedBonusXp + ")");

                    UIManager.rpc_AddExperienceMonster.SendPackage(ZRoutedRpc.Everybody, newPackage);
                }

                Instance.xpObjects.Remove(obj);
            }
        }

        public void RewardXP(Character monster)
        {
            ConsoleLog.Print("Monster died (Client)");

            ZPackage newPackage = new ZPackage();

            newPackage.Write(monster.GetInstanceID());
            newPackage.Write(monster.GetLevel());
            newPackage.Write(monster.name);

            rpc_RewardXPMonster.SendPackage(ZRoutedRpc.Everybody, newPackage);
        }

        private MonsterXP GetMonsterXP(uint monsterID)
        {
            foreach (MonsterXP obj in xpObjects)
            {
                if (obj.monsterID == monsterID)
                {
                    return obj;
                }
            }

            return null;
        }

        public string GetAllMonsterXpString()
        {
            string response = "Total monsters: " + xpObjects.Count;
            foreach (MonsterXP obj in xpObjects)
            {
                response += "\n-> MonsterID: " + obj.monsterID;
            }

            return response;
        }

        public void SetPlayerLevel(int level)
        {
            Player.m_localPlayer.m_customData[saveLevelString] = level.ToString();
        }

        public void SavePlayerLevel()
        {
            Player.m_localPlayer.m_customData[saveLevelString] = UIManager.Instance.playerLevel.ToString();
        }

        public void SetPlayerXP(int xp)
        {
            Player.m_localPlayer.m_customData[saveXpString] = xp.ToString();
        }

        public void SavePlayerXP()
        {
            Player.m_localPlayer.m_customData[saveXpString] = UIManager.Instance.playerXP.ToString();
        }

        public int GetPlayerLevel()
        {
            int value = 1;
            if(Player.m_localPlayer.m_customData.ContainsKey(saveLevelString))
            {
                string savedString = Player.m_localPlayer.m_customData[saveLevelString];
                int.TryParse(savedString, out value);
            }

            return value;
        }

        public int GetPlayerXP()
        {
            int value = 0;
            if (Player.m_localPlayer.m_customData.ContainsKey(saveXpString))
            {
                string savedString = Player.m_localPlayer.m_customData[saveXpString];
                int.TryParse(savedString, out value);
            }

            return value;
        }

    }
}
