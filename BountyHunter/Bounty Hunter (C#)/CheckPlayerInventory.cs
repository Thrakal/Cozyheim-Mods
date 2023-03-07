using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static ItemDrop;

namespace BountyHunter
{
    public class CheckPlayerInventory : MonoBehaviour
    {
        private static Inventory inventory = null;
        private bool gameRunning = false;

        private Coroutine updateScrollDescriptions = null;
        private bool isRemovingItems = false;

        void Start()
        {
            StartCoroutine(Init());
        }

        public static bool GotBountyInInventory(Bounty bounty)
        {
            ItemData[] bountyItems = GetSpecificBountyItemsInInventory(bounty);
            return bountyItems.Length > 0;
        }

        public IEnumerator Init()
        {
            Logger.Print("Started checking for inventory on player");
            while (true)
            {
                if (Player.m_localPlayer != null)
                {
                    if (!gameRunning)
                    {
                        inventory = Player.m_localPlayer.GetInventory();
                        inventory.m_onChanged += CheckForBountyScrollOnChange;
                        gameRunning = true;
                        updateScrollDescriptions = StartCoroutine(UpdateScrollDescriptionsAndCheckIfActive());

                        Logger.Print("Found Inventory");
                    }
                }
                else
                {
                    if (gameRunning)
                    {
                        gameRunning = false;
                        StopCoroutine(updateScrollDescriptions);
                        updateScrollDescriptions = null;
                        inventory = null;
                        Logger.Print("Game not running anymore");
                        Logger.Print("Reset inventory");
                    }
                }
                yield return new WaitForSeconds(2f);
            }
        }

        private void CheckForBountyScrollOnChange()
        {
            Logger.Print("Checking inventory");
            List<ItemDrop.ItemData> allItems = inventory.GetAllItems();

            for (int i = 0; i < allItems.Count; i++)
            {
                if (allItems[i].m_dropPrefab.name.StartsWith("bounty_"))
                {
                    if (!isRemovingItems)
                    {
                        // MUST BE CHANGED TO -> Call server to begin bounty
                        BountyManager.BeginBounty(allItems[i].m_dropPrefab);
                    }

                    // MUST BE CHANGED TO -> Get active bounty from server
                    Bounty bounty = BountyManager.GetActiveBounty(allItems[i].m_dropPrefab.name);
                    if (bounty != null)
                    {
                        allItems[i].m_shared.m_description = bounty.GetDescription();
                    }
                }
            }
        }

        private IEnumerator UpdateScrollDescriptionsAndCheckIfActive()
        {
            Logger.Print("UpdateScrollDescriptionsAndCheckIfActive - Started");
            WaitForSeconds waitTime = new WaitForSeconds(1f);
            while (true)
            {
                yield return waitTime;
                isRemovingItems = false;

                if (inventory != null)
                {
                    ItemData[] bountyItems = GetAllBountyItemsInInventory();

                    foreach (ItemData itemData in bountyItems)
                    {
                        Bounty bounty = BountyManager.GetActiveBounty(itemData.m_dropPrefab.name);
                        if (bounty != null)
                        {
                            // Update scroll descriptions
                            itemData.m_shared.m_description = bounty.GetDescription();
                            Logger.Print("Description: " + itemData.m_dropPrefab.name);
                        }
                        else
                        {
                            // Remove inactive bounty scrolls
                            isRemovingItems = true;
                            Logger.Print("Removed: " + itemData.m_dropPrefab.name);
                            inventory.RemoveItem(itemData);
                        }
                    }
                }
            }
        }

        private static ItemData[] GetAllBountyItemsInInventory()
        {
            List<ItemData> bountyItems = new List<ItemData>();

            List<ItemData> allItems = inventory.GetAllItems();
            for (int i = 0; i < allItems.Count; i++)
            {
                ItemData itemData = allItems[i];
                if (itemData.m_dropPrefab.name.StartsWith("bounty_"))
                {
                    bountyItems.Add(itemData);
                }
            }

            return bountyItems.ToArray();
        }

        private static ItemData[] GetSpecificBountyItemsInInventory(Bounty bounty)
        {
            List<ItemData> bountyItems = new List<ItemData>();

            List<ItemData> allItems = inventory.GetAllItems();
            for (int i = 0; i < allItems.Count; i++)
            {
                ItemData itemData = allItems[i];
                if (itemData.m_dropPrefab.name == bounty.bountyName)
                {
                    bountyItems.Add(itemData);
                }
            }

            return bountyItems.ToArray();
        }
    }

}
