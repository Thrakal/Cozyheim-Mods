using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cozyheim.CustomItems
{
    internal class RecipeItem
    {
        public string recipeName;
        public string globalKeyToUnlock;
        public Recipe recipe;

        public RecipeItem(string recipeName, BossToUnlock bossToUnlock)
        {
            string[] globalBossKeys = new string[]
            {
                "",
                "defeated_eikthyr",
                "defeated_gdking",
                "defeated_bonemass",
                "defeated_dragon",
                "defeated_goblinking",
                "defeated_queen"
            };

            this.recipeName = recipeName;
            globalKeyToUnlock = globalBossKeys[(int)bossToUnlock];
        }
    }
}
