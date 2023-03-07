using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BountyHunter
{
    internal class PrefabList
    {
        public string prefabName;
        public Category category;

        public PrefabList(string prefabName, Category category)
        {
            this.prefabName = prefabName;
            this.category = category;
        }
    }
    internal enum Category
    {
        Piece = 0,
        Monster = 1,
        Item = 2,
        BountyItem = 3
    }
}
