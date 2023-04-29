using BepInEx.Configuration;
using System.Collections.Generic;
using static ItemDrop.ItemData;

namespace Cozyheim.AdjustUpgradeStats {
    internal class Stats {
        public string category;
        public string name;
        public ConfigEntry<float> damageMultiplier;
        public ConfigEntry<UpgradeType> upgradeDamageType;
        public ConfigEntry<float> upgradeDamageValue;
        public List<ItemType> types;

        public void Init() {
            damageMultiplier = ConfigSettings.CreateConfigEntry(category, "DamageMultiplier", 1f, "Base damage multiplier for all " + name + ". (1 = normal damage, 2 = 200% damage, 0.5 = 50% damage)", true);
            upgradeDamageType = ConfigSettings.CreateConfigEntry(category, "UpgradeModifierType", UpgradeType.None, "The mathematical way to adjust the upgrade damage values for all " + name + ". (Percentage = percentage adjusted from vanilla values, PercentageOfBase = percentage of base damage", true);
            upgradeDamageValue = ConfigSettings.CreateConfigEntry(category, "UpgradeDamageValue", 0.1f, "Upgrade damage multiplier for all " + name, true);
        }
    }
}
