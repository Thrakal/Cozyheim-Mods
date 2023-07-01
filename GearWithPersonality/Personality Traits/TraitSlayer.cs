
namespace Cozyheim.GearWithPersonality.Traits {
    internal class TraitSlayer : TraitBase {

        public override void Check(ItemDrop.ItemData item, string key) {
            // Get monster name from key
            int kills = ItemPersonality.GetPersonalityValueAsInt(item, key);
            string monsterName = key.Replace(ItemPersonality.KILL_KEY, "");

            // Go through all trait values
            for(int i = traitValues.Length - 1; i >= 0; i--) {
                int unlockValue = (int)traitValues[i].unlockValue;

                // Check if kill count is high enough to unlock this trait value
                if(kills >= unlockValue) {
                    string keyTrait = ItemPersonality.TRAIT_KEY + monsterName + "_" + name;
                    int currentBonusLevel = ItemPersonality.GetPersonalityValueAsInt(item, keyTrait);

                    // Check if this bonus is already unlocked
                    if(currentBonusLevel >= i) {
                        break;
                    }

                    // Unlock trait value
                    ItemPersonality.SetPersonalityValue(item, keyTrait, i);
                    ConsoleLog.Print("Trait unlocked: " + name + " (" + monsterName + ") - Level: " + i + " (Bonus: " + traitValues[i].bonusValue + ")", LogType.Warning);
                    break;
                }
            }
        }

        public override float GetBonusValue(ItemDrop.ItemData item, string monsterName) {
            string keyTrait = ItemPersonality.TRAIT_KEY + monsterName + "_" + name;
            int currentBonusLevel = ItemPersonality.GetPersonalityValueAsInt(item, keyTrait);

            if(currentBonusLevel >= 0) {
                return traitValues[currentBonusLevel].bonusValue;
            }

            return -1f;
        }
    }
}
