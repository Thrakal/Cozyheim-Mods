using HarmonyLib;
using Jotunn.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cozyheim.TameAll
{
    internal class ChatCommands : MonoBehaviour
    {
        private static List<string> chatArgs = new List<string>();

        private static Dictionary<string, Action> commands = new Dictionary<string, Action>()
        {
            {"Spawn", Spawn }
        };

        private static void Spawn()
        {
            // Only admins may use this command
            if (!ConsoleLog.IsUserAdmin()) return;

            if (chatArgs.Count == 0)
            {
                ConsoleLog.Print("Please type in what animal you want to spawn", LogType.Error);
                return;
            }

            string monsterToSpawn = chatArgs[0];
            int monsterLevel = 1;

            // Command code here
            GameObject animalGO = CreatureManager.Instance.GetCreaturePrefab(monsterToSpawn);
            if (animalGO == null)
            {
                ConsoleLog.Print("This animal does not exist.", LogType.Error);
                return;
            }

            if (chatArgs.Count >= 2)
            {
                int.TryParse(chatArgs[1], out monsterLevel);
            }

            monsterLevel = monsterLevel < 1 ? 1 : monsterLevel;
            monsterLevel = monsterLevel > 3 ? 3 : monsterLevel;

            Tameable tameableCompSettings = CreatureManager.Instance.GetCreaturePrefab("Boar").GetComponent<Tameable>();
            Tameable tameableComp = animalGO.GetComponent<Tameable>();

            if(tameableComp == null)
            {
                tameableComp = animalGO.AddComponent<Tameable>();
            }

            tameableComp.m_tamedEffect = tameableCompSettings.m_tamedEffect;
            tameableComp.m_sootheEffect = tameableCompSettings.m_sootheEffect;
            tameableComp.m_petEffect = tameableCompSettings.m_petEffect;
            tameableComp.m_commandable = true;

            GameObject newAnimal = Instantiate(animalGO, Player.m_localPlayer.transform.position + Camera.main.transform.forward * 2f, Quaternion.identity);

            newAnimal.GetComponent<MonsterAI>().MakeTame();
            newAnimal.GetComponent<MonsterAI>().SetDespawnInDay(false);
            newAnimal.GetComponent<Humanoid>().SetLevel(monsterLevel);
        }

        [HarmonyPatch]
        private class PatchClass
        {
            [HarmonyPrefix]
            [HarmonyPatch(typeof(Chat), "SendText")]
            private static bool Chat_SendText_Prefix(string text)
            {
                string[] data = text.Split(' ');
                if(data.Length >= 2 ) {
                    string identifier = data[0].ToLower();
                    if(identifier.Equals("-" + "tame"))
                    {
                        string action = data[1].ToLower();
                        foreach(KeyValuePair<string, Action> kvp in commands)
                        {
                            if(kvp.Key.ToLower().Equals(action)) {
                                chatArgs = data.ToList();
                                chatArgs.RemoveAt(0);
                                chatArgs.RemoveAt(0);
                                
                                kvp.Value();
                                return false;
                            }
                        }
                    }                    
                }
                return true;
            }
        }
    }
}
