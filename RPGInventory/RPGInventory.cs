using System.Collections.Generic;

namespace Cozyheim.RPGInventory
{
    internal class RPGInventory
    {
        private string saveNamePrefix = "RPGInventory_Cozy_";
        private static RPGInventory _instance;

        private List<string> _items;

        public static RPGInventory Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RPGInventory();
                }
                return _instance;
            }
        }

        private RPGInventory()
        {
            GenerateItemsList();
        }

        public void GenerateItemsList()
        {
            _items = new List<string>();
            string[] items = Main.itemList.Value.Split(',');
            foreach (string item in items)
            {
                _items.Add(item.Trim());
            }
        }

        public bool IsItemInInventory(ItemDrop item)
        {
            if(item == null)
            {
                return false;
            }

            if(item.m_itemData.m_dropPrefab == null)
            {
                return false;
            }

            ItemDrop.ItemData itemData = item.m_itemData;
            return _items.Contains(itemData.m_dropPrefab.name);
        }

        public void AddItem(ItemDrop item)
        {
            ItemDrop.ItemData itemData = item.m_itemData;

            int amount = LoadItem(itemData);
            amount += itemData.m_stack;
            SaveItem(itemData, amount);
        }

        public bool RemoveItem(ItemDrop item, int value)
        {
            ItemDrop.ItemData itemData = item.m_itemData;

            int amount = LoadItem(itemData);

            if (amount < value || value == 0)
            {
                return false;
            }

            amount -= value;
            SaveItem(itemData, amount);
            return true;
        }

        public int GetItemAmount(ItemDrop item)
        {
            ItemDrop.ItemData itemData = item.m_itemData;
            return LoadItem(itemData);
        }

        public Dictionary<string, int> GetInventory()
        {
            Dictionary<string, int> allItems = new Dictionary<string, int>();

            foreach (KeyValuePair<string, string> kvp in Player.m_localPlayer.m_customData)
            {
                if (kvp.Key.StartsWith(saveNamePrefix))
                {
                    string itemName = kvp.Key.Split('_')[2];
                    int value = 0;
                    int.TryParse(kvp.Value, out value);
                    allItems[itemName] = value;
                }
            }

            return allItems;
        }

        private int LoadItem(ItemDrop.ItemData item)
        {
            string itemSaveName = saveNamePrefix + item.m_dropPrefab.name;

            int value = 0;
            if (Player.m_localPlayer.m_customData.ContainsKey(itemSaveName))
            {
                string savedString = Player.m_localPlayer.m_customData[itemSaveName];
                int.TryParse(savedString, out value);
            }

            return value;
        }

        private void SaveItem(ItemDrop.ItemData item, int value)
        {
            string itemSaveName = saveNamePrefix + item.m_dropPrefab.name;
            Player.m_localPlayer.m_customData[itemSaveName] = value.ToString();
        }
    }
}
