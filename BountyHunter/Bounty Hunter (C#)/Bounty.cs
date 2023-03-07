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
    public class Bounty : MonoBehaviour
    {
        public string bountyName;
        public string monsterToSpawn;
        public Vector3 monsterSpawnPos;
        public float timeRemaining;

        private bool isMonsterSpawned = false;
        private string defaultDescription;
        private float detectDistance = 20f;
        private float spawnDistance = 10f;
        private int id;
        private Transform monster;

        public void InitBounty(GameObject bounty, string monsterToSpawnName, int bountyID)
        {
            ItemDrop item = bounty.GetComponent<ItemDrop>();
            id = bountyID;
            bountyName = item.name;
            monsterToSpawn = monsterToSpawnName;
            monsterSpawnPos = Player.m_localPlayer.transform.position + Player.m_localPlayer.transform.forward * 30f;

            Vector3 playerPos = Player.m_localPlayer.transform.position;
            Vector3 targetPos = monsterSpawnPos;
            float distanceToMonster = Vector2.Distance(new Vector2(playerPos.x, playerPos.z), new Vector2(targetPos.x, targetPos.z));

            timeRemaining = 30f;
            defaultDescription = item.m_itemData.m_shared.m_description;

            Logger.Print("Bounty created info:");
            Logger.Print("BountyName: " + bountyName);
            Logger.Print("MonsterName: " + monsterToSpawn);
            Logger.Print("PlayerPos: " + playerPos);
            Logger.Print("MonsterPos: " + targetPos);
            Logger.Print("Distance: " + distanceToMonster);
        }

        void Update()
        {
            if (!isMonsterSpawned)
            {
                timeRemaining -= Time.deltaTime;

                if (timeRemaining <= 0f)
                {
                    DestroyBounty();
                }

                Vector3 playerPos = Player.m_localPlayer.transform.position;
                Vector3 targetPos = monsterSpawnPos;
                float distanceToBounty = Vector2.Distance(new Vector2(playerPos.x, playerPos.z), new Vector2(targetPos.x, targetPos.z));
                bool playerGotBounty = CheckPlayerInventory.GotBountyInInventory(this);

                if (distanceToBounty <= spawnDistance && playerGotBounty)
                {
                    Logger.Print("Spawning bounty monster:");
                    Logger.Print("Bounty: " + bountyName);
                    Logger.Print("Monster: " + monsterToSpawn);
                    GameObject monsterGO = CreatureManager.Instance.GetCreaturePrefab(monsterToSpawn);
                    monster = Instantiate(monsterGO, monsterSpawnPos, Quaternion.identity).transform;
                    isMonsterSpawned = true;
                }
            }
            else
            {
                // Monster is spawned and has been killed
                if (monster == null)
                {
                    DestroyBounty();
                }
            }
        }

        public string GetDescription()
        {
            string returnString = defaultDescription + "<color=" + Main.descriptionColor.Value + ">";
            Vector3 playerPos = Player.m_localPlayer.transform.position;
            Vector3 targetPos = monsterSpawnPos;

            if (isMonsterSpawned)
            {
                if (monster != null)
                {
                    returnString += "\n\nMonster is spawned";

                    targetPos = monster.position;
                    float distanceToMonster = Vector2.Distance(new Vector2(playerPos.x, playerPos.z), new Vector2(targetPos.x, targetPos.z));
                    if (distanceToMonster > detectDistance)
                    {
                        returnString += "\nDistance from you: " + distanceToMonster.ToString("N0") + "m";
                    }
                    else
                    {
                        returnString += "\n\nBounty monster is nearby!";
                    }
                    return returnString + "</color>";
                }
            }

            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = (int)timeRemaining % 60;
            returnString += "\n\nTime remaining: " + minutes.ToString("D2") + ": " + seconds.ToString("D2");

            float distanceToBounty = Vector2.Distance(new Vector2(playerPos.x, playerPos.z), new Vector2(targetPos.x, targetPos.z));
            returnString += "\nDistance from you: " + distanceToBounty.ToString("N0") + "m";

            return returnString + "</color>";
        }

        private void DestroyBounty()
        {
            BountyManager.RemoveBounty(bountyName, id);
            Destroy(gameObject);
        }
    }
}
