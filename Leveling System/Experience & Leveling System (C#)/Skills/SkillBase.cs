using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cozyheim.LevelingSystem
{
    internal enum SkillType
    {
        HP,
        HPRegen,
        Stamina,
        StaminaRegen,
        Eitr,
        EitrRegen,
        CarryWeight,
        MovementSpeed,
        Woodcutting,
        Mining,
        PhysicalDamage,
        ElementalDamage,
        PhysicalResistance,
        ElementalResistance,
        Hunting,
        Farming,
        EndOfEnum
    }


    internal class SkillBase
    {
        private int _level;

        protected SkillOption uiSettings;
        protected SkillType skillType;
        protected int maxLevel;
        public string displayName;
        public float bonusPerLevel;
        public string bonusUnit;
        public string iconName;

        protected int level {
            get {
                return _level;
            }
            set {
                ConsoleLog.Print(skillType.ToString() + ": Level (" + _level + " -> " + value + ")");
                _level = value;
            }
        }

        public SkillBase(int maxLevel, float bonusPerLevel, string iconName, string displayName, string bonusUnit = "")
        {
            this.level = 0;
            this.maxLevel = maxLevel;
            this.bonusPerLevel = bonusPerLevel;
            this.bonusUnit = bonusUnit;
            this.displayName = displayName;
            this.iconName = iconName;
        }

        public void SetSkillUI(SkillOption uiSettings)
        {
            uiSettings.addPointButton.onClick.RemoveAllListeners();
            uiSettings.removePointButton.onClick.RemoveAllListeners();
            uiSettings.resetPointButton.onClick.RemoveAllListeners();

            ConsoleLog.Print("Setting up skill");
            this.uiSettings = uiSettings;
            uiSettings.addPointButton.onClick.AddListener(delegate ()
            {
                SkillManager.Instance.SkillLevelUp(skillType);
            });

            uiSettings.removePointButton.onClick.AddListener(delegate ()
            {
                SkillManager.Instance.SkillLevelDown(skillType);
            });

            uiSettings.resetPointButton.onClick.AddListener(delegate ()
            {
                SkillManager.Instance.SkillReset(skillType);
            });
        }

        public int ResetLevel()
        {
            int returnValue = level;
            level = 0;
            return returnValue;
        }

        public void SetLevel(int level)
        {
            if(level > GetMaxLevel())
            {
                level = GetMaxLevel();
            }

            if(level < 0)
            {
                level = 0;
            }

            this.level = level;
        }

        public bool AddLevel()
        {
            if(IsLevelMax())
            {
                return false;
            }

            level++;
            ConsoleLog.Print("Added skill point to " + skillType.ToString() + " - now level " + level);
            return true;
        }

        public bool RemoveLevel()
        {
            if(level <= 0)
            {
                return false;
            }

            level--;
            return true;
        }

        public bool IsLevelMax()
        {
            return level == maxLevel;
        }

        public bool IsLevelZero()
        {
            return level == 0;
        }

        public float GetBonus()
        {
            return bonusPerLevel * level;
        }

        public string GetName() {
            return displayName;
        }

        public int GetLevel()
        {
            return level;
        }

        public int GetMaxLevel()
        {
            return maxLevel;
        }

        public void UpdateSkillInformation()
        {
            ConsoleLog.Print(skillType.ToString() + ": UpdateSkillInformation called", LogType.Message);

            if (uiSettings != null)
            {
                uiSettings.UpdateAllButtonVisibility(this);
                uiSettings.UpdateInformation(this);
            }
        }
    }
}
