using System.Collections.Generic;

namespace Cozyheim.CookingRock {
    internal class PrefabItem
    {
        public string prefabName;
        public Category category;

        public PrefabItem(string prefabName, Category category)
        {
            this.prefabName = prefabName;
            this.category = category;
        }
    }
}
