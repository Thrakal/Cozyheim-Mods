using BepInEx.Configuration;
using HarmonyLib;

namespace Cozyheim.RPGInventory
{
    [HarmonyPatch]
    internal class HaveRequirements_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Player), "HaveRequirementItems")]
        static void Player_HaveRequirementItems_Postfix(Player __instance, Recipe piece, int qualityLevel, bool __result)
        {
            if(__instance == null || Player.m_localPlayer == null || Player.m_localPlayer != __instance)
            {
                return;
            }

            if(piece == null)
            {
                return;
            }

            if(__result)
            {
                ConsoleLog.Print("Original result was: " + __result);
                return;
            }

            Piece.Requirement[] resources = piece.m_resources;
            foreach (Piece.Requirement requirement in resources)
            {
                ItemDrop item = requirement.m_resItem;
                if(!RPGInventory.Instance.IsItemInInventory(item))
                {
                    ConsoleLog.Print("Item not in inventory");
                    continue;
                }

                int amountNeeded = requirement.GetAmount(qualityLevel);
                int amountInInventory = RPGInventory.Instance.GetItemAmount(item);
                if (amountInInventory < amountNeeded)
                {
                    ConsoleLog.Print("Found " + item.m_itemData.m_stack + " of " + item.m_itemData.m_dropPrefab.name + " in inventory", LogType.Message);
                    ConsoleLog.Print("-> Total in inventory: " + RPGInventory.Instance.GetItemAmount(item));
                    ConsoleLog.Print("-> NOT ENOUGH!", LogType.Warning);
                    __result = false;
                    return;
                }

                ConsoleLog.Print("Found " + item.m_itemData.m_stack + " of " + item.m_itemData.m_dropPrefab.name + " in inventory", LogType.Message);
                ConsoleLog.Print("-> Total in inventory: " + RPGInventory.Instance.GetItemAmount(item));
            }

            __result = true;
        }
    }
}
