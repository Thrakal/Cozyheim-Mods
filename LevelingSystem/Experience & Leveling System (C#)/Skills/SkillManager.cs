﻿using Jotunn.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace Cozyheim.LevelingSystem
{
    internal class SkillManager : MonoBehaviour
    {
        public int unspendPoints = 0;

        public static SkillManager Instance;

        private GameObject criticalHitVFX;
        private GameObject criticalHitText;
        private float critTextOffsetY = 1f;
        private float critTextOffsetTowardsCam = 0.75f;

        internal static Dictionary<SkillType, SkillBase> skills;

        public static void InitSkills() {
            skills = new Dictionary<SkillType, SkillBase>();

            foreach(SkillSettings skill in SkillConfig.skillSettings)
            {
                if(skill.GetEnabled())
                {
                    switch(skill.skillType)
                    {
                        case SkillType.HP:
                            skills.Add(skill.skillType, new SkillHP(skill.GetMaxLevel(), skill.GetBonusValue(), "HP", "Health"));
                            break;
                        case SkillType.Stamina:
                            skills.Add(skill.skillType, new SkillStamina(skill.GetMaxLevel(), skill.GetBonusValue(), "Stamina", "Stamina"));
                            break;
                        case SkillType.Eitr:
                            skills.Add(skill.skillType, new SkillEitr(skill.GetMaxLevel(), skill.GetBonusValue(), "Eitr", "Eitr"));
                            break;
                        case SkillType.HPRegen:
                            skills.Add(skill.skillType, new SkillHPRegen(skill.GetMaxLevel(), skill.GetBonusValue(), "HPRegen", "Health Regen", "%"));
                            break;
                        case SkillType.StaminaRegen:
                            skills.Add(skill.skillType, new SkillStaminaRegen(skill.GetMaxLevel(), skill.GetBonusValue(), "StaminaRegen", "Stamina Regen", "%"));
                            break;
                        case SkillType.EitrRegen:
                            skills.Add(skill.skillType, new SkillEitrRegen(skill.GetMaxLevel(), skill.GetBonusValue(), "EitrRegen", "Eitr Regen", "%"));
                            break;
                        case SkillType.CarryWeight:
                            skills.Add(skill.skillType, new SkillCarryWeight(skill.GetMaxLevel(), skill.GetBonusValue(), "CarryWeight", "Carry Weight"));
                            break;
                        case SkillType.Woodcutting:
                            skills.Add(skill.skillType, new SkillWoodcutting(skill.GetMaxLevel(), skill.GetBonusValue(), "Woodcutting", "Woodcutting", "% damage"));
                            break;
                        case SkillType.Mining:
                            skills.Add(skill.skillType, new SkillMining(skill.GetMaxLevel(), skill.GetBonusValue(), "Mining", "Mining", "% damage"));
                            break;
                        case SkillType.PhysicalDamage:
                            skills.Add(skill.skillType, new SkillPhysicalDamage(skill.GetMaxLevel(), skill.GetBonusValue(), "PhysicalDamage", "Physical Damage", "%"));
                            break;
                        case SkillType.ElementalDamage:
                            skills.Add(skill.skillType, new SkillElementalDamage(skill.GetMaxLevel(), skill.GetBonusValue(), "ElementalDamage", "Elemental Damage", "%"));
                            break;

                        // Physical defense skills
                        case SkillType.PhysicalResistance:
                            skills.Add(skill.skillType, new SkillPhysicalResistance(skill.GetMaxLevel(), skill.GetBonusValue(), "PhysicalResistance", "Physical Resistance", "%"));
                            break;
                        case SkillType.ResistanceSlash:
                            skills.Add(skill.skillType, new SkillResistanceSlash(skill.GetMaxLevel(), skill.GetBonusValue(), "PhysicalResistance", "Slash Resistance", "%"));
                            break;
                        case SkillType.ResistancePierce:
                            skills.Add(skill.skillType, new SkillResistancePierce(skill.GetMaxLevel(), skill.GetBonusValue(), "PhysicalResistance", "Pierce Resistance", "%"));
                            break;
                        case SkillType.ResistanceBlunt:
                            skills.Add(skill.skillType, new SkillResistanceBlunt(skill.GetMaxLevel(), skill.GetBonusValue(), "PhysicalResistance", "Blunt Resistance", "%"));
                            break;

                        // Elemental defense skills
                        case SkillType.ElementalResistance:
                            skills.Add(skill.skillType, new SkillElementalResistance(skill.GetMaxLevel(), skill.GetBonusValue(), "ElementalResistance", "Elemental Resistance", "%"));
                            break;
                        case SkillType.ResistanceFire:
                            skills.Add(skill.skillType, new SkillResistanceFire(skill.GetMaxLevel(), skill.GetBonusValue(), "ElementalResistance", "Fire Resistance", "%"));
                            break;
                        case SkillType.ResistanceFrost:
                            skills.Add(skill.skillType, new SkillResistanceFrost(skill.GetMaxLevel(), skill.GetBonusValue(), "ElementalResistance", "Frost Resistance", "%"));
                            break;
                        case SkillType.ResistanceLightning:
                            skills.Add(skill.skillType, new SkillResistanceLightning(skill.GetMaxLevel(), skill.GetBonusValue(), "ElementalResistance", "Lightning Resistance", "%"));
                            break;
                        case SkillType.ResistancePoison:
                            skills.Add(skill.skillType, new SkillResistancePoison(skill.GetMaxLevel(), skill.GetBonusValue(), "ElementalResistance", "Poison Resistance", "%"));
                            break;
                        case SkillType.ResistanceSpirit:
                            skills.Add(skill.skillType, new SkillResistanceSpirit(skill.GetMaxLevel(), skill.GetBonusValue(), "ElementalResistance", "Spirit Resistance", "%"));
                            break;

                        case SkillType.MovementSpeed:
                            skills.Add(skill.skillType, new SkillMovementSpeed(skill.GetMaxLevel(), skill.GetBonusValue(), "MovementSpeed", "Movement Speed", "%"));
                            break;
                        case SkillType.CriticalChance:
                            skills.Add(skill.skillType, new SkillCriticalHitChance(skill.GetMaxLevel(), skill.GetBonusValue(), "CriticalHitChance", "Critical Hit Chance", "%", 1f));
                            break;
                        case SkillType.CriticalDamage:
                            skills.Add(skill.skillType, new SkillCriticalHitDamage(skill.GetMaxLevel(), skill.GetBonusValue(), "CriticalHitDamage", "Critical Hit Damage", "%", 10f));
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        
        public void SpawnCriticalHitVFX(Vector3 position, float damage)
        {
            if (!Main.criticalHitVFX.Value)
            {
                return;
            }

            if(Main.criticalHitShake.Value)
            {
                GameCamera.instance.AddShake(Player.m_localPlayer.transform.position, 10f, Main.criticalHitShakeIntensity.Value, false);
            }

            Vector3 dirToCamera = (GameCamera.instance.transform.position - position).normalized;
            Vector3 critPos = position + Vector3.up * critTextOffsetY + dirToCamera * critTextOffsetTowardsCam;

            GameObject critText = Instantiate(criticalHitText, critPos, Quaternion.identity);
            critText.GetComponent<CritTextAnim>().SetText(damage, 1);

            GameObject newVFX = Instantiate(criticalHitVFX, position, Quaternion.identity);
            Destroy(newVFX, 4f);
        }

        public void UpdateAllSkillInformation()
        {
            foreach (KeyValuePair<SkillType, SkillBase> kvp in skills)
            {
                kvp.Value.UpdateSkillInformation();
            }
        }

        public void SkillSetLevel(SkillType skill, int level)
        {
            ConsoleLog.Print("Set skill " + skill.ToString() + " to level " + level.ToString());
            skills[skill].SetLevel(level);
            UpdateUnspendPoints();
        }

        public int GetTotalSkillsCount()
        {
            return skills.Count;
        }

        public SkillBase GetSkillByIndex(int index)
        {
            if(skills.ContainsKey((SkillType)index))
            {
                return skills[(SkillType)index];
            }
            
            return null;
        }

        public SkillBase GetSkillByType(SkillType type)
        {
            return skills[type];
        }

        public bool SkillLevelUp(SkillType skill)
        {
            if(!HasUnspendPoints())
            {
                return false;
            }

            bool success = skills[skill].AddLevel();
            UpdateUnspendPoints();
            return success;
        }

        public bool SkillLevelDown(SkillType skill)
        {
            bool success = skills[skill].RemoveLevel();
            UpdateUnspendPoints();
            return success;
        }

        public int SkillReset(SkillType skill)
        {
            int value = skills[skill].ResetLevel();
            UpdateUnspendPoints();
            return value;
        }

        public void SkillResetAll()
        {
            foreach (KeyValuePair<SkillType, SkillBase> kvp in skills)
            {
                kvp.Value.ResetLevel();
            }
            UpdateUnspendPoints();
        }

        public bool IsSkillMaxLevel(SkillType skill)
        {
            return skills[skill].IsLevelMax();
        }

        public bool HasUnspendPoints()
        {
            return unspendPoints > 0;
        }

        public int GetSkillPointSpendOnCategory(SkillCategory category)
        {
            int count = 0;
            foreach(SkillSettings skill in SkillConfig.skillSettings)
            {
                if(skill.category == category)
                {
                    count += GetSkillByType(skill.skillType).GetLevel();
                }
            }

            return count;
        }

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            InitSkills();
            LoadSkills();

            criticalHitVFX = PrefabManager.Instance.GetPrefab("CriticalHitEffect");
            criticalHitText = PrefabManager.Instance.GetPrefab("CritDamageText");
        }

        public void UpdateUnspendPoints()
        {
            if(XPManager.Instance != null && skills != null)
            {
                int points = Mathf.FloorToInt((float)XPManager.Instance.GetPlayerLevel() * Main.pointsPerLevel.Value);
                foreach (KeyValuePair<SkillType, SkillBase> kvp in skills)
                {
                    points -= kvp.Value.GetLevel();
                }

                unspendPoints = points;
                UIManager.Instance.remainingPoints.text = "Remaining points: " + points;
                UIManager.Instance.UpdateCategoryPoints();

                SaveSkills();
                UpdateAllSkillInformation();
            }
        }

        void LoadSkills()
        {
            foreach(KeyValuePair<SkillType, SkillBase> kvp in skills)
            {
                string skillName = Main.modName + "_" + kvp.Key.ToString();
                if (Player.m_localPlayer.m_customData.ContainsKey(skillName))
                {
                    int value;
                    string savedString = Player.m_localPlayer.m_customData[skillName];
                    if (int.TryParse(savedString, out value))
                    {
                        kvp.Value.SetLevel(value);
                    }
                }
            }

            UpdateUnspendPoints();
        }

        void SaveSkills()
        {
            foreach (KeyValuePair<SkillType, SkillBase> kvp in skills)
            {
                string skillName = Main.modName + "_" + kvp.Key.ToString();
                Player.m_localPlayer.m_customData[skillName] = kvp.Value.GetLevel().ToString();                
            }
        }

        public void DestroySelf()
        {
            SaveSkills();
            Destroy(gameObject);
        }

        public void ReloadAllSkills()
        {
            InitSkills();
            LoadSkills();
            UIManager.Instance.UpdateUI(true);
        }
    }
}
