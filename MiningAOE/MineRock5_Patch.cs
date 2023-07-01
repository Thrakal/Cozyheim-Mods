using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cozyheim.MiningAOE {
    internal class MineRock5_Patch : MonoBehaviour {

        private static bool isMultiHitting = false;
        private List<Collider> rocksHitList = new List<Collider>();

        public static MineRock5_Patch Instance;

        private void Awake() {
            if(Instance == null) {
                Instance = this;
            } else {
                Destroy(this);
            }
        }

        private IEnumerator DoDamageAOE(HitData hit) {
            if(isMultiHitting) {
                yield break;
            }

            isMultiHitting = true;

            float pickaxeSkill = Player.m_localPlayer.GetSkillLevel(Skills.SkillType.Pickaxes);
            float radius = Mathf.Lerp(ConfigSettings.minSkillRadius.Value, ConfigSettings.maxSkillRadius.Value, pickaxeSkill / 100f);

            if(ConfigSettings.aoeType.Value == AOEType.AlwaysMinimum) {
                radius = ConfigSettings.minSkillRadius.Value;
            }
            if(ConfigSettings.aoeType.Value == AOEType.AlwaysMaximum) {
                radius = ConfigSettings.maxSkillRadius.Value;
            }

            ConsoleLog.Print("Pickaxe: " + pickaxeSkill + " - Range: " + radius);

            Collider[] colliders = Physics.OverlapSphere(hit.m_point, radius);
            foreach(Collider coll in colliders) {
                if(coll == null) {
                    continue;
                }

                MineRock5 mineRock5 = coll.GetComponentInParent<MineRock5>();
                if(mineRock5 == null) {
                    continue;
                }

                ZNetView zNetView = mineRock5.GetComponent<ZNetView>();
                if(zNetView == null || !zNetView.IsValid()) {
                    continue;
                }

                HitData newHit = hit;
                newHit.m_hitCollider = coll;
                newHit.m_point = coll.bounds.ClosestPoint(hit.m_point);
                mineRock5.Damage(newHit);

                yield return null;
            }

            isMultiHitting = false;
        }


        [HarmonyPatch]
        private class Patch {

            [HarmonyPrefix]
            [HarmonyPatch(typeof(MineRock5), "Damage")]
            private static bool MineRock5_RPCDamage_Prefix(HitData hit) {
                if(ConfigSettings.aoeType.Value == AOEType.Disabled) {
                    ConsoleLog.Print("AOE is disabled");
                    return true;
                }

                if(!Input.GetKey(ConfigSettings.aoeKey.Value)) {
                    ConsoleLog.Print("Not holding AOE key");
                    return true;
                }

                if(isMultiHitting) {
                    ConsoleLog.Print("Already multi-hitting");
                    return true;
                }

                Instance.StartCoroutine(Instance.DoDamageAOE(hit));
                return false;

                /*
                hit.m_point


                float skillFactor = Player.m_localPlayer.GetSkillFactor(Skills.SkillType.Pickaxes);
                float num = Mathf.Lerp(ConfigSettings.minSkillRadius.Value, ConfigSettings.maxSkillRadius.Value, skillFactor);

                Vector3 point = hit.m_point;
                int num2 = 0;
                for(int i = 0; i < __instance.m_hitAreas.Count; i++) {
                    if(i != hitAreaIndex) {
                        MineRock5.HitArea hitArea = __instance.m_hitAreas[i];
                        if(hitArea.m_health > 0f && Vector3.Distance(point, hitArea.m_collider.bounds.center) <= num) {
                            __instance.DamageArea(i, hit);
                            num2++;
                        }
                    }
                }
                Plugin.Log.LogDebug((object)$"inRange: {num2}");
                */

            }
        }

    }
}





