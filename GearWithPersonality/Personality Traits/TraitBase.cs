namespace Cozyheim.GearWithPersonality.Traits {
    internal class TraitBase {
        public string name;
        public string description;
        public TraitType traitType;
        public TraitValue[] traitValues;

        public virtual void Check(ItemDrop.ItemData item, string key) {}

        public virtual float GetBonusValue(ItemDrop.ItemData item, string monsterName) { return -1f; }
    }

    public struct TraitValue {
        public float unlockValue;
        public float bonusValue;
    }

    public enum TraitType {
        Slayer,
        Powerful
    }
}
