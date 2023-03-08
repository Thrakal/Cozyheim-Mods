using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using HarmonyLib;


namespace Cozyheim.NPCVendors
{
    internal class NpcZDO : MonoBehaviour
    {
        private ZNetView nview;
        private CraftingStation craftingStation;
        private NPCEquipment equipment;

        IEnumerator Start()
        {
            nview = GetComponent<ZNetView>();
            craftingStation = GetComponent<CraftingStation>();
            equipment = GetComponent<NPCEquipment>();

            yield return null;
            UpdateNPC();
        }

        private void UpdateNPC()
        {
            if (nview == null)
                return;

            GenderNPC gender = (GenderNPC) Enum.Parse(typeof(GenderNPC), nview.GetZDO().GetString("NPC_Gender", equipment.Gender.ToString()));
            equipment.UpdateGender(gender);

            int height = nview.GetZDO().GetInt("NPC_Height", (int)equipment.Height);
            equipment.UpdateHeight(height);
        }

        public void SetName(string name)
        {
            nview.GetZDO().Set("NPC_Name", name);
            UpdateNPC();
        }

        public void SetGender(GenderNPC gender)
        {
            nview.GetZDO().Set("NPC_Gender", gender.ToString());
            UpdateNPC();
        }

        public void SetHeight(NPCHeight height)
        {
            nview.GetZDO().Set("NPC_Height", (int)height);
            UpdateNPC();
        }

        public string GetNPCName()
        {
            string npcName = nview.GetZDO().GetString("NPC_Name", "");
            if(npcName == "")
            {
                npcName = equipment.Gender == GenderNPC.Male ? GetRandomMaleName() : GetRandomFemaleName();
            }

            return npcName + " the " + equipment.NPCType.ToString();
        }

        private string GetRandomFemaleName()
        {
            string[] names = new string[]
            {
                "Astrid",
                "Vida",
                "Hilda",
                "Gro",
                "Helga",
                "Randi",
                "Signe",
                "Tora",
                "Åse",
                "Sibba",
                "Tove"
            };

            string selectedName = names[UnityEngine.Random.Range(0, names.Length)];
            nview.GetZDO().Set("NPC_Name", selectedName);
            return selectedName;
        }

        private string GetRandomMaleName()
        {
            string[] names = new string[]
            {
                "Vidar",
                "Asbjørn",
                "Frode",
                "Gorm",
                "Halfdan",
                "Harald",
                "Knud",
                "Troels",
                "Toke",
                "Trygve",
                "Ulf",
                "Åge"
            };

            string selectedName = names[UnityEngine.Random.Range(0, names.Length)];
            nview.GetZDO().Set("NPC_Name", selectedName);
            return selectedName;
        }



        [HarmonyPatch]
        private class PatchClass
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(InventoryGui), "UpdateRecipe")]
            static void InventoryGui_UpdateRecipe_Postfix(InventoryGui __instance, Player player, ref Text ___m_craftingStationName)
            {
                CraftingStation craftingStation = player.GetCurrentCraftingStation();
                if(craftingStation != null)
                {
                    if (craftingStation.m_name.StartsWith("NPC_"))
                    {
                        ___m_craftingStationName.text = craftingStation.GetComponent<NpcZDO>().GetNPCName();
                    }
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(CraftingStation), "GetHoverText")]
            static bool CraftingStation_GetHoverText_Prefix(CraftingStation __instance, ref string __result)
            {
                if (__instance.m_name.StartsWith("NPC_"))
                {
                    string hoverText = __instance.GetComponent<NpcZDO>().GetNPCName();
                    hoverText += "\n[<color=yellow><b>$KEY_Use</b></color>] Talk";
                    __result = Localization.instance.Localize(hoverText);
                    return false;
                }

                return true;
            }
        }
    }
}
