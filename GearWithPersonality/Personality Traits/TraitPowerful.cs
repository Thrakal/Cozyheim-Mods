
namespace Cozyheim.GearWithPersonality.Traits {
    internal class TraitPowerful : TraitBase {

        public override void Check(ItemDrop.ItemData item, string key) {
            // Get monster name from key
            float damage = ItemPersonality.GetPersonalityValueAsFloat(item, key);

            // Go through all trait values
            for(int i = traitValues.Length - 1; i >= 0; i--) {
                int unlockValue = (int)traitValues[i].unlockValue;

                // Check if kill count is high enough to unlock this trait value
                if(damage >= unlockValue) {
                    string keyTrait = ItemPersonality.TRAIT_KEY + name;
                    int currentBonusLevel = ItemPersonality.GetPersonalityValueAsInt(item, keyTrait);

                    // Check if this bonus is already unlocked
                    if(currentBonusLevel >= i) {
                        break;
                    }

                    // Unlock trait value
                    ItemPersonality.SetPersonalityValue(item, keyTrait, i);
                    ConsoleLog.Print("Trait unlocked: " + name + " - Level: " + i + " (Bonus: " + traitValues[i].bonusValue + ")");
                    break;
                }
            }
        }

        public override float GetBonusValue(ItemDrop.ItemData item, string monsterName) {
            string keyTrait = ItemPersonality.TRAIT_KEY + name;
            int currentBonusLevel = ItemPersonality.GetPersonalityValueAsInt(item, keyTrait);

            if(currentBonusLevel >= 0) {
                return traitValues[currentBonusLevel].bonusValue;
            }

            return -1f;
        }
    }
}
