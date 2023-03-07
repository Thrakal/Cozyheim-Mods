using Jotunn.Entities;
using Jotunn.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BountyHunter
{
    public class BountyManager : MonoBehaviour
    {
        private static List<Recipe> bountiesPool = new List<Recipe>();
        private static List<string> bountiesMonsterPool = new List<string>();
        private static List<Bounty> activeBounties = new List<Bounty>();

        private static BountyManager instance;

        // Network communication RPC
        public static CustomRPC rpc_UpdateAllBountyPools;
        public static CustomRPC rpc_RecieveBountyPool;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            Logger.Print("BountyManager AWAKE");
            // Register RPC Methods
            rpc_UpdateAllBountyPools = NetworkManager.Instance.AddRPC("UpdateBountyPools", RPC_UpdateAllBountyPools, RPC_UpdateAllBountyPools);
            rpc_RecieveBountyPool = NetworkManager.Instance.AddRPC("RecieveBountyPool", RPC_RecieveBountyPool, RPC_RecieveBountyPool);
        }

        void Start()
        {
            StartCoroutine(CheckForInactiveBountyScrolls());
        }

        private IEnumerator CheckForInactiveBountyScrolls()
        {
            Logger.Print("Bounty Scroll Garbage Collector: Started");

            bool runGarbageCollector = true;

            while (runGarbageCollector)
            {
                if (ZNet.instance == null)
                {
                    yield return new WaitForSeconds(1f);
                    break;
                }
                if (Player.m_localPlayer != null)
                {
                    if (!ZNet.instance.IsServer())
                    {
                        runGarbageCollector = false;
                        break;
                    }
                    DestroyInactiveBountyScrolls(true);
                }
                yield return new WaitForSeconds(5f);
            }
        }

        public static void DestroyInactiveBountyScrolls(bool autoCheck = false)
        {
            if (ZNet.instance.IsServer())
            {
                BountyScroll[] bountyScrolls = BountyScroll.allBountyScrolls.ToArray();
                int destroyCount = 0;

                for (int i = 0; i < bountyScrolls.Length; i++)
                {
                    if (bountyScrolls[i] != null)
                    {
                        if (bountyScrolls[i].DestroyScrollGameobjectCheck())
                        {
                            destroyCount++;
                        }
                    }
                }
                if (destroyCount > 0)
                {
                    if (autoCheck)
                    {
                        Logger.Print("---- Auto Check ----");
                    }
                    Logger.Print("Checked for inactive bounty scrolls:");
                    Logger.Print("Destroyed " + destroyCount + " gameobjects");
                }
            }
        }

        public static bool BeginBounty(GameObject bountyGO)
        {
            foreach (Bounty b in activeBounties)
            {
                if (b.bountyName == bountyGO.name)
                {
                    return false;
                }
            }

            string monsterToSpawnName = "";
            int bountyID = -1;
            for (int i = 0; i < bountiesPool.Count; i++)
            {
                if (bountyGO.name == bountiesPool[i].m_item.name)
                {
                    monsterToSpawnName = bountiesMonsterPool[i];
                    bountyID = i;
                    break;
                }
            }

            Bounty bounty = new GameObject().AddComponent<Bounty>();
            bounty.InitBounty(bountyGO, monsterToSpawnName, bountyID);
            activeBounties.Add(bounty);
            return true;
        }

        public static bool RemoveBounty(string bountyName, int bountyID)
        {
            if (!ZNet.instance.IsServer())
            {
                return false;
            }

            for (int i = 0; i < activeBounties.Count; i++)
            {
                if (activeBounties[i].bountyName == bountyName)
                {
                    activeBounties.RemoveAt(i);
                    SetBountyInactiveOnBoard(bountyID);
                    DestroyInactiveBountyScrolls();
                    return true;
                }
            }
            return false;
        }

        public static Bounty GetActiveBounty(string bountyName)
        {
            // MUST BE CHANGED TO -> Get active bounty from server
            for (int i = 0; i < activeBounties.Count; i++)
            {
                if (activeBounties[i].bountyName.StartsWith(bountyName))
                {
                    return activeBounties[i];
                }
            }
            return null;
        }

        public static void AddBountyToPool(Recipe recipe, string monsterName)
        {
            bountiesMonsterPool.Add(monsterName);
            bountiesPool.Add(recipe);
            recipe.m_enabled = false;
        }

        public static void SetBountyActiveOnBoard(int index)
        {
            if (index < 0 || index > bountiesPool.Count - 1)
            {
                Logger.Print("Bounty index out of bounds");
                return;
            }

            bountiesPool[index].m_enabled = true;
            SaveBountyList();
        }

        public static void SetBountyInactiveOnBoard(int index)
        {
            if (index < 0 || index > bountiesPool.Count - 1)
            {
                Logger.Print("Bounty index out of bounds");
                return;
            }

            bountiesPool[index].m_enabled = false;
            SaveBountyList();
        }

        public static void SetBountyToggleOnBoard(int index)
        {
            if (index < 0 || index > bountiesPool.Count - 1)
            {
                Logger.Print("Bounty index out of bounds");
                return;
            }

            bountiesPool[index].m_enabled = !bountiesPool[index].m_enabled;
            SaveBountyList();
        }

        public static void SaveBountyList()
        {
            if (!ZNet.instance.IsServer())
            {
                return;
            }

            string activelist = "";
            for (int i = 0; i < bountiesPool.Count; i++)
            {
                if (bountiesPool[i].m_enabled)
                {
                    if (activelist == "")
                    {
                        activelist += i.ToString();
                    }
                    else
                    {
                        activelist += "," + i.ToString();
                    }
                }
            }
            string worldName = PlayerPrefs.GetString("world", "");
            PlayerPrefs.SetString(worldName + "_toppbountyactive", activelist);
            Logger.Print("Bounties saved at (" + worldName + "):");
            Logger.Print("Saved string: '" + activelist + "'");
            UpdateAllBountyBoardsScrolls();

            // Send update to all players
            rpc_UpdateAllBountyPools.SendPackage(ZRoutedRpc.Everybody, new ZPackage());
        }

        public static void LoadBountyList()
        {
            if (Game.instance != null)
            {
                if (!ZNet.instance.IsServer())
                {
                    rpc_UpdateAllBountyPools.SendPackage(ZRoutedRpc.Everybody, new ZPackage());
                    Logger.Print("Bounties loaded from (server)");
                    return;
                }

                string worldName = PlayerPrefs.GetString("world", "");
                string[] activeBounties = PlayerPrefs.GetString(worldName + "_toppbountyactive", "").Split(',');

                Logger.Print("Bounties loaded from (" + worldName + ")");

                for (int i = 0; i < activeBounties.Length; i++)
                {
                    if (activeBounties[i] != "")
                    {
                        int index = int.Parse(activeBounties[i]);
                        bountiesPool[index].m_enabled = true;
                        Logger.Print("Bounty id: " + index);
                    }
                }
            }
            UpdateAllBountyBoardsScrolls();
        }

        public static int GetAvailableBountiesCount()
        {
            int count = 0;
            foreach (Recipe bounty in bountiesPool)
            {
                if (bounty.m_enabled)
                {
                    count++;
                }
            }

            return count;
        }

        public static List<Bounty> GetActiveBountiesList()
        {
            return activeBounties;
        }

        private static void UpdateAllBountyBoardsScrolls()
        {
            Logger.Print("Updating bounty board scrolls");
            Logger.Print("-> Total scrolls visible: " + GetAvailableBountiesCount());

            foreach (BountyBoard bb in BountyBoard.allBountyBoards)
            {
                bb.UpdateVisibleBountyScrolls();
            }
        }

        private static IEnumerator RPC_UpdateAllBountyPools(long sender, ZPackage package)
        {
            yield return null;

            if (ZNet.instance.IsServer())
            {
                ZPackage newPackage = new ZPackage();

                for (int i = 0; i < bountiesPool.Count; i++)
                {
                    newPackage.Write(bountiesPool[i].m_enabled);
                }

                // Send bountylist to all clients
                rpc_RecieveBountyPool.SendPackage(ZRoutedRpc.Everybody, newPackage);
                Logger.Print("BountyManager RPC (Send)");
                Logger.Print("Updating all bounty pools");
            }

            yield return null;
        }

        private static IEnumerator RPC_RecieveBountyPool(long sender, ZPackage package)
        {
            yield return null;

            if (!ZNet.instance.IsServer())
            {
                Logger.Print("BountyManager RPC (Recieve)");
                Logger.Print("Updating local bounty pool");

                // Handle
                int bountyPoolSize = package.Size();

                for (int i = 0; i < bountyPoolSize; i++)
                {
                    bool enable = package.ReadBool();
                    bountiesPool[i].m_enabled = enable;
                    Logger.Print("Bounty " + i + ": " + enable);
                }

                yield return null;

                UpdateAllBountyBoardsScrolls();
            }
        }

    }

}