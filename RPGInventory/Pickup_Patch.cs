using HarmonyLib;
using UnityEngine;

namespace Cozyheim.RPGInventory
{
    [HarmonyPatch]
    internal class Pickup_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Humanoid), "Pickup")]
        static bool Humanoid_Pickup_Prefix(ref Humanoid __instance, GameObject go, ref Inventory ___m_inventory)
        {
            ItemDrop item = go.GetComponent<ItemDrop>();

            if (__instance.IsTeleporting())
            {
                return true;
            }

            Player player = __instance.GetComponent<Player>();
            if (player == null)
            {
                return true;
            }

            if (player != Player.m_localPlayer)
            {
                return true;
            }

            if(item == null)
            {
                return true;
            }

            if(!RPGInventory.Instance.IsItemInInventory(item))
            {
                return true;
            }

            RPGInventory.Instance.AddItem(item);

            ConsoleLog.Print("You picked up " + item.m_itemData.m_stack + " " + item.m_itemData.m_dropPrefab.name, LogType.Message);
            ConsoleLog.Print("-> Total in inventory: " + RPGInventory.Instance.GetItemAmount(item));
            ZNetScene.instance.Destroy(go);
            return false;
        }
    }
}
