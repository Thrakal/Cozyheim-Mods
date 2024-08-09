using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;


// Mangler at få Custom NPC Recipes til at enable/disable ved global keys

namespace Cozyheim.NPCVendors
{
    [HarmonyPatch]
    internal class NPC : MonoBehaviour
    {
        internal static void Init()
        {
            NPCList.CreatePrefabAndRecipeList();
            PrefabManager.OnVanillaPrefabsAvailable += RegisterPieces;
        }

        internal static void RegisterPieces()
        {
            // Crafting Assets
            if (Main.modEnabled.Value)
            {
                ConsoleLog.Print("Register NPC Prefabs:", LogType.Message);
                // Load custom NPC assets
                foreach (NPCItem npc in NPCList.customNPCs)
                {
                    GameObject newNPC = CreateNewNPC(npc.npcType, npc.description, npc.iconName);

                    NPCEquipment equipment = newNPC.AddComponent<NPCEquipment>();
                    equipment.CopyEquipment(npc.equipment);
                    equipment.NPCType = npc.npcType;

                    npc.piece = newNPC.GetComponent<Piece>();
                    npc.piece.m_enabled = false;
                    npc.piece.m_category = Piece.PieceCategory.Misc;

                    newNPC.AddComponent<NpcZDO>();

                    if(PieceManager.Instance.AddPiece(new CustomPiece(newNPC, "Hammer", true)))
                    {
                        ConsoleLog.Print("- Added NPC: " + newNPC.name);
                    }
                }

                RegisterRecipes();
            }

            PrefabManager.OnVanillaPrefabsAvailable -= RegisterPieces;
        }

        internal static void RegisterRecipes()
        {
            ConsoleLog.Print("Register NPC Recipes:", LogType.Message);
            // Load Recipes
            if (Main.modEnabled.Value)
            {
                foreach (NPCRecipe recipe in NPCList.customNPCRecipes)
                {
                    CraftingStation craftingStationName = recipe.recipe.m_craftingStation;
                    recipe.recipe.m_enabled = false;
                    if (ItemManager.Instance.AddRecipe(new CustomRecipe(recipe.recipe, true, true)))
                    {
                        ConsoleLog.Print("- Added recipe: " + recipe.recipe.name);
                        recipe.recipe.m_craftingStation = craftingStationName;
                    }
                }
            }
        }

        internal static GameObject CreateNewNPC(NPCType npcType, string description, string iconName)
        {
            // Clone the default Player GO
            string npcTypeName = npcType.ToString();
            GameObject npcTemplate = PrefabManager.Instance.CreateClonedPrefab("NPC_" + npcTypeName, "Player");
            npcTemplate.layer = 10;

            // Remove default components that is not needed
            Destroy(npcTemplate.GetComponent<Rigidbody>());
            Destroy(npcTemplate.GetComponent<PlayerController>());
            Destroy(npcTemplate.GetComponent<Player>());
            Destroy(npcTemplate.GetComponent<ZSyncTransform>());
            Destroy(npcTemplate.GetComponent<ZSyncAnimation>());
            Destroy(npcTemplate.GetComponent<Talker>());
            Destroy(npcTemplate.GetComponent<Skills>());
            Destroy(npcTemplate.GetComponent<FootStep>());
            Destroy(npcTemplate.transform.GetChild(1).gameObject); // EyePos
            Destroy(npcTemplate.GetComponentInChildren<CharacterAnimEvent>());

            // Load assets from AssetBundle
            Sprite icon = Main.CreateSpriteFromFile(iconName);

            ZNetView zNetView = npcTemplate.GetComponent<ZNetView>();
            zNetView.m_persistent = true;
            zNetView.m_type = ZDO.ObjectType.Solid;

            // Add other components and set default values
            Piece piece = npcTemplate.AddComponent<Piece>();
            piece.m_primaryTarget = true;
            piece.m_noClipping = true;
            piece.m_noInWater = true;
            piece.m_icon = icon;
            piece.m_name = npcTypeName;
            piece.m_description = description;

            CraftingStation cs = npcTemplate.AddComponent<CraftingStation>();
            cs.m_craftRequireRoof = false;
            cs.m_craftRequireFire = false;
            cs.m_useDistance = 4f;
            cs.m_rangeBuild = 20;
            cs.m_icon = icon;
            cs.m_name = "NPC_" + npcTypeName;

            HitData.DamageModifier immune = HitData.DamageModifier.Immune;
            WearNTear wear = npcTemplate.AddComponent<WearNTear>();
            wear.m_new = npcTemplate.transform.GetChild(0).gameObject;
            wear.m_worn = npcTemplate.transform.GetChild(0).gameObject;
            wear.m_broken = npcTemplate.transform.GetChild(0).gameObject;
            wear.m_noRoofWear = false;
            wear.m_noSupportWear = false;
            wear.m_supports = false;
            wear.m_damages = new HitData.DamageModifiers()
            {
                m_blunt = immune,
                m_slash = immune,
                m_pierce = immune,
                m_chop = immune,
                m_pickaxe = immune,
                m_fire = immune,
                m_frost = immune,
                m_lightning = immune,
                m_poison = immune,
                m_spirit = immune
            };

            return npcTemplate;
        }

        [HarmonyPatch]
        private static class Patch_Class
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(WearNTear), "Remove")]
            static bool WearNTear_Remove_Prefix(WearNTear __instance)
            {
                if (Main.modEnabled.Value)
                {
                    if (!SynchronizationManager.Instance.PlayerIsAdmin)
                    {
                        if (__instance.name.StartsWith("NPC_"))
                        {
                            Player.m_localPlayer.Message(MessageHud.MessageType.Center, "Don't kill the NPC");
                            return false;
                        }
                    }
                }
                return true;
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(CraftingStation), "Interact")]
            static void CraftingStation_Interact_Prefix()
            {
                if (Main.modEnabled.Value)
                {
                    foreach (NPCRecipe recipe in NPCList.customNPCRecipes)
                    {
                        if(recipe.recipe != null)
                        {
                            string globalKey = recipe.globalKeyToUnlock;
                            bool enableCheck = ZoneSystem.instance.GetGlobalKey(globalKey) || globalKey == "" ? true : false;
                            recipe.recipe.m_enabled = enableCheck;
                        }
                    }
                }
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(Humanoid), "EquipItem")]
            static void Humanoid_EquipItem_Prefix()
            {
                if (Main.modEnabled.Value)
                {
                    foreach (NPCItem npc in NPCList.customNPCs)
                    {
                        if (npc.piece != null)
                        {
                            npc.piece.m_enabled = SynchronizationManager.Instance.PlayerIsAdmin;
                        }
                    }
                }
            }
        }
    }
}
