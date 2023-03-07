using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BountyHunter
{
    public class BountyBoard : MonoBehaviour
    {
        public static List<BountyBoard> allBountyBoards = new List<BountyBoard>();

        private List<GameObject> bountyScrollGOs = new List<GameObject>();

        void Start()
        {
            Transform scrolls = transform.Find("Scrolls");
            for (int i = 0; i < scrolls.childCount; i++)
            {
                GameObject scroll = scrolls.GetChild(i).gameObject;
                bountyScrollGOs.Add(scroll);
                scroll.SetActive(false);
            }
            BountyManager.LoadBountyList();
        }

        public void UpdateVisibleBountyScrolls()
        {
            int activeBounties = BountyManager.GetAvailableBountiesCount();
            for (int i = 0; i < bountyScrollGOs.Count; i++)
            {
                if (i < activeBounties)
                {
                    bountyScrollGOs[i].SetActive(true);
                }
                else
                {
                    bountyScrollGOs[i].SetActive(false);
                }
            }
        }

        void OnEnable()
        {
            allBountyBoards.Add(this);
        }

        void OnDisable()
        {
            allBountyBoards.Remove(this);
        }
    }

}