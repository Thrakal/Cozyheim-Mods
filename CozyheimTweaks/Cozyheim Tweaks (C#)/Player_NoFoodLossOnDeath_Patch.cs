using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace CozyheimTweaks {

    [HarmonyPatch(typeof(Player), "OnDeath")]
    internal class Player_NoFoodLossOnDeath_Patch {

        static List<Player.Food> savedFood = new List<Player.Food>();

        static void Prefix(Player __instance, List<Player.Food> ___m_foods) {
            if(__instance != Player.m_localPlayer) {
                return;
            } 

            savedFood.Clear();
            foreach(Player.Food food in ___m_foods) {
                savedFood.Add(food);
            }
        }

        static void Postfix(Player __instance, ref List<Player.Food> ___m_foods) {
            if(__instance != Player.m_localPlayer) {
                return;
            }

            foreach(Player.Food food in savedFood) {
                food.m_time -= food.m_item.m_shared.m_foodBurnTime * 0.25f;
                if(food.m_time > 0f) {
                    ___m_foods.Add(food);
                }
            }
        }
    }
}
