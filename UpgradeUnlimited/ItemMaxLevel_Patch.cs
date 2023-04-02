using HarmonyLib;

namespace Cozyheim.UpgradeUnlimited
{
    internal class ItemMaxLevel_Patch
    {
        [HarmonyPatch]
        private class Patch
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(ItemDrop), "Awake")]
            private static void ItemDrop_Awake_Prefix(ref ItemDrop __instance)
            {
                __instance.m_itemData.m_shared.m_maxQuality = Main.itemMaxLevel.Value;
            }
        }
    }
}

// private void UpdateRecipe(Player player, float dt)