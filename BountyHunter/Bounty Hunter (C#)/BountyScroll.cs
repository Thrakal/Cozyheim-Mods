using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BountyHunter
{
    public class BountyScroll : MonoBehaviour
    {
        public static List<BountyScroll> allBountyScrolls = new List<BountyScroll>();

        private ItemDrop item;

        public bool DestroyScrollGameobjectCheck()
        {
            item = GetComponent<ItemDrop>();

            if (item == null)
            {
                ZNetScene.instance.Destroy(gameObject);
                return true;
            }
            else
            {
                if (BountyManager.GetActiveBounty(item.m_itemData.m_dropPrefab.name) == null)
                {
                    ZNetScene.instance.Destroy(gameObject);
                    return true;
                }
            }

            return false;
        }

        private void OnDestroy()
        {
            allBountyScrolls.Remove(this);
            Logger.Print(gameObject.name + " was removed from list");
            Logger.Print("List length: " + allBountyScrolls.Count);

        }

        void Start()
        {
            allBountyScrolls.Add(this);
            Logger.Print(gameObject.name + " got added to list");
            Logger.Print("List length: " + allBountyScrolls.Count);
        }
    }
}