using System.Collections;
using UnityEngine;

namespace Cozyheim.AutoCloseDoors
{
    internal class AutoCloseCheck : MonoBehaviour
    {
        private bool checkAutoClose = false;

        private Coroutine coroutine;
        private ZNetView nview;

        void Start()
        {
            nview = GetComponent<ZNetView>();
            Interact();
        }

        public void Interact()
        {
            checkAutoClose = true;
            if(coroutine == null)
            {
                coroutine = StartCoroutine(CheckDoor());
            }
        }

        IEnumerator CheckDoor()
        {
            while(checkAutoClose)
            {
                yield return new WaitForSeconds(Main.checkTime.Value);

                int state = nview.GetZDO().GetInt("state");
                bool playerFound = false;
                bool craftingStationFound = false;

                if (state == 0) {
                    checkAutoClose = false;
                } else {
                    Collider[] colliders = Physics.OverlapSphere(transform.position, Main.checkDistance.Value);

                    foreach(Collider collider in colliders)
                    {
                        if (collider.GetComponent<Player>() != null)
                        {
                            playerFound = true;
                            break;
                        }

                        if (craftingStationFound == false)
                        {
                            if (collider.GetComponentInParent<CraftingStation>() != null)
                            {
                                craftingStationFound = true;
                                continue;
                            }
                        }
                    }

                    if(playerFound == false && craftingStationFound == true)
                    {
                        nview.InvokeRPC("UseDoor", true);
                        checkAutoClose = false;
                    }
                }
            }

            coroutine = null;
        }
    }
}
