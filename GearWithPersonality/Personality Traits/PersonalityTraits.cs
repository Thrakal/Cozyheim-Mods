using System.Collections.Generic;
using System.Linq;
using Cozyheim.GearWithPersonality.Traits;

namespace Cozyheim.GearWithPersonality {
    internal class PersonalityTraits {

        private static List<TraitBase> traits = new List<TraitBase>() {
            new TraitSlayer() {
                traitType = TraitType.Slayer,
                name = "Slayer",
                description = "Increases damage against this monster type",
                traitValues = new TraitValue[] {
                    new TraitValue() {
                        unlockValue = 5, bonusValue = 0.1f
                    },
                    new TraitValue() {
                        unlockValue = 10, bonusValue = 0.5f
                    },
                    new TraitValue() {
                        unlockValue = 15, bonusValue = 1f
                    },
                    new TraitValue() {
                        unlockValue = 20, bonusValue = 2f
                    }
                }
            },
            new TraitPowerful() {
                traitType = TraitType.Powerful,
                name = "Powerful",
                description = "Adds critical hit chance to this weapon",
                traitValues = new TraitValue[] {
                    new TraitValue() {
                        unlockValue = 50, bonusValue = 0.1f
                    },
                    new TraitValue() {
                        unlockValue = 100, bonusValue = 0.5f
                    },
                    new TraitValue() {
                        unlockValue = 150, bonusValue = 1f
                    },
                    new TraitValue() {
                        unlockValue = 200, bonusValue = 2f
                    }
                }
            }
        };

        public static void UpdateWeaponTraits(ItemDrop.ItemData item) {
            string[] itemKeys = item.m_customData.Keys.ToArray();

            // Go through all custom data keys
            for(int i = 0; i < itemKeys.Length; i++) {
                string key = itemKeys[i];

                // Check if key is a kill key
                if(key.StartsWith(ItemPersonality.KILL_KEY)) {
                    CheckTrait(TraitType.Slayer, item, key);
                }

                // Check if key is a kill key
                if(key.StartsWith(ItemPersonality.MAX_DMG_KEY)) {
                    CheckTrait(TraitType.Powerful, item, key);
                }
            }
        }

        private static void CheckTrait(TraitType traitType, ItemDrop.ItemData item, string key) {
            foreach(TraitBase trait in traits) {
                if(traitType == trait.traitType) {
                    trait.Check(item, key);
                    break;
                }
            }
        }

        public static float GetWeaponTraitBonus(ItemDrop.ItemData item, TraitType traitType, string monsterName) {
            foreach(TraitBase trait in traits) {
                if(traitType == trait.traitType) {
                    return trait.GetBonusValue(item, monsterName);
                }
            }

            return -1f;
        }

        public static float GetWeaponTraitBonus(ItemDrop.ItemData item, TraitType traitType) {
            return GetWeaponTraitBonus(item, traitType, "");
        }

    }
}
