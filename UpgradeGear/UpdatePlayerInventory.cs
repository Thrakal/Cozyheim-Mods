using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static ItemDrop;

namespace Cozyheim.UpgradeUnlimited {
    internal class UpdatePlayerInventory : MonoBehaviour {

        private static bool inventoryUpdated = false;

        IEnumerator Start() {
            ConsoleLog.Print("Waiting for game to start", LogType.Message);
            while(!inventoryUpdated) {
                if(Player.m_localPlayer == null) {
                    yield return new WaitForSeconds(5f);
                    continue;
                }

                List<ItemData> allItems = Player.m_localPlayer.GetInventory().GetAllItems();
                for(int i = 0; i < allItems.Count; i++) {
                    ItemData itemData = allItems[i];
                    if(Main.IsItemAllowed(itemData)) {
                        itemData.m_shared.m_maxQuality = ConfigSettings.maxItemLevel.Value;
                    }
                }
                inventoryUpdated = true;
                StartCoroutine(CheckForMainScene());
            }
        }

        IEnumerator CheckForMainScene() {
            ConsoleLog.Print("Updating items in inventory", LogType.Message);
            while(inventoryUpdated) {
                yield return new WaitForSeconds(5f);
                if(ZNet.instance == null && Player.m_localPlayer == null) {
                    inventoryUpdated = false;
                    StartCoroutine(Start());
                }
            }
        }
    }
}
