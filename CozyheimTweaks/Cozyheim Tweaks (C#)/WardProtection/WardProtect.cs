using UnityEngine;
using System.Collections;

namespace CozyheimTweaks {
    namespace Scripts {
        public class WardProtect : MonoBehaviour, ICustomUnityScript 
        {
            private CircleProjector areaMarker;
            private bool markerEnabled = false;
            private float markerActivateRadius = 10f;

            private Piece myPiece = null;

            // Start is called before the first frame update
            void Start()
            {
                areaMarker = GetComponentInChildren<CircleProjector>();
                areaMarker.m_radius = WardProtection.protectionRadius.Value;
                areaMarker.m_nrOfSegments = (int) WardProtection.protectionRadius.Value * 3;
                ToggleAreaMarker(false);

                StartCoroutine(GetPiece());
            }

            IEnumerator GetPiece()
            {
                while(ZNetScene.instance == null || Player.m_localPlayer == null) {
                    yield return null;
                }

                while (myPiece == null)
                {
//                    ToppLog.Print("Checking for ward piece", LogType.Message, WardProtection.debugMode.Value);
                    myPiece = GetComponent<Piece>();

                    if (myPiece.GetCreator() == 0L)
                    {
                        myPiece = null;
                    }

                    yield return new WaitForSeconds(0.5f);
                }
                
                markerActivateRadius = myPiece.IsCreator() ? 6f : WardProtection.protectionRadius.Value + 5f;

                ToppLog.Print("-> Ward piece found, is owner: " + myPiece.IsCreator().ToString(), LogType.Message, WardProtection.debugMode.Value);
                ToppLog.Print("-> Radius: " + markerActivateRadius.ToString("N0"), LogType.Message, WardProtection.debugMode.Value);
            }

            void Update()
            {
                if (ZNetScene.instance == null || Player.m_localPlayer == null)
                    return;

                if(myPiece != null)
                {
                    markerActivateRadius = myPiece.IsCreator() ? 6f : WardProtection.protectionRadius.Value + 5f;
                }

                if (Vector3.Distance(Player.m_localPlayer.transform.position, transform.position) < markerActivateRadius)
                {
                    if (!markerEnabled)
                    {
                        ToggleAreaMarker(true);
                    }
                }
                else
                {
                    if (markerEnabled)
                    {
                        ToggleAreaMarker(false);
                    }
                }
                
            }

            public void ToggleAreaMarker(bool value)
            {
                areaMarker.gameObject.SetActive(value);
                markerEnabled = value;
            }
        }
    }
}
