using BepInEx.Configuration;
using HarmonyLib;

namespace Cozyheim.RPGInventory
{
    [HarmonyPatch]
    internal class ConsumeResources_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Player), "ConsumeResources")]
        public static void Player_ConsumeResources_Prefix(Piece.Requirement[] requirements, int qualityLevel)
        {
            foreach (Piece.Requirement requirement in requirements)
            {
                if ((bool)requirement.m_resItem)
                {
                    int amount = requirement.GetAmount(qualityLevel);
                    if (amount <= 0)
                    {
                        continue;
                    }

                    ItemDrop item = requirement.m_resItem;

                    if (RPGInventory.Instance.IsItemInInventory(item))
                    {
                        RPGInventory.Instance.RemoveItem(item, amount);
                    }
                }
            }
        }
    }
}
