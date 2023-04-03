using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cozyheim.CustomItems
{
    internal class PrefabItem
    {
        public string prefabName;
        public string ingameName;
        public Category category;
        public Piece piece;
        public List<ICustomUnityScript> components;

        public PrefabItem(string prefabName, string ingameName, Category category, List<ICustomUnityScript> components = null)
        {
            this.prefabName = prefabName;
            this.ingameName = ingameName;
            this.category = category;
            if (components == null)
            {
                this.components = new List<ICustomUnityScript>();
            }
            else
            {
                this.components = components;
            }
        }
    }
}
