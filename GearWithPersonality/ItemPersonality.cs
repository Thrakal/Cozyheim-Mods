using System.Collections.Generic;
using System.Linq;

namespace Cozyheim.GearWithPersonality {
    internal class ItemPersonality {

        private const string PERSONALITY_KEY = "cozyheim_personality_";

        public const string KILL_KEY = PERSONALITY_KEY + "kills_";
        public const string MAX_DMG_KEY = PERSONALITY_KEY + "maxdamage";
        public const string TRAIT_KEY = PERSONALITY_KEY + "trait_";

        public static void SetPersonalityValue(ItemDrop.ItemData item, string key, string value) {
            if(item.m_customData.ContainsKey(key)) {
                item.m_customData[key] = value;
            } else {
                item.m_customData.Add(key, value);
            }
        }

        public static void SetPersonalityValue(ItemDrop.ItemData item, string key, int value) {
            SetPersonalityValue(item, key, value.ToString());
        }

        public static void SetPersonalityValue(ItemDrop.ItemData item, string key, float value) {
            SetPersonalityValue(item, key, value.ToString());
        }

        public static string GetPersonalityValueAsString(ItemDrop.ItemData item, string key) {
            if(!item.m_customData.ContainsKey(key)) {
                ConsoleLog.Print("Item doesn't contain requested key: " + key);
                return null;
            }

            return item.m_customData[key];
        }

        public static int GetPersonalityValueAsInt(ItemDrop.ItemData item, string key) {
            int value = int.TryParse(GetPersonalityValueAsString(item, key), out value) ? value : -1;
            return value;
        }

        public static float GetPersonalityValueAsFloat(ItemDrop.ItemData item, string key) {
            float value = float.TryParse(GetPersonalityValueAsString(item, key), out value) ? value : -1f;
            return value;
        }

        public static bool RemovePersonalityKey(ItemDrop.ItemData item, string key) {
            if(!item.m_customData.ContainsKey(key)) {
                ConsoleLog.Print("Can't remove key, as it doesn't exists: " + key);
                return false;
            }

            item.m_customData.Remove(key);
            return true;
        }

        public static void RemoveAllPersonalities(ItemDrop.ItemData item) {
            string[] values = item.m_customData.Keys.ToArray();

            foreach(string value in values) {
                if(value.StartsWith(PERSONALITY_KEY)) {
                    item.m_customData.Remove(value);
                }
            }

            ConsoleLog.Print(item.m_shared.m_name + ": Personalities has ben removed!", LogType.Message);
        }

        public static bool HasPersonalityKey(ItemDrop.ItemData item, string key) {
            return item.m_customData.ContainsKey(key);
        }

        public static void PrintAllPersonalityValues(ItemDrop.ItemData item) {
            int counter = 0;

            ConsoleLog.Print(item.m_shared.m_name + ": Personality Keys", LogType.Message);

            foreach(KeyValuePair<string, string> kvp in item.m_customData) {
                if(kvp.Key.StartsWith(PERSONALITY_KEY)) {
                    ConsoleLog.Print("->" + kvp.Key + ": " + kvp.Value);
                    counter++;
                }
            }

            if(counter == 0) {
                ConsoleLog.Print("-> No personality keys found");
            }
        }
    }
}
