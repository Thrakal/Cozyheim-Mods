using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cozyheim.ModTemplate
{
    internal class ChatCommands : MonoBehaviour
    {
        private static string commandChatSymbol = "-";
        private static List<string> chatArgs = new List<string>();

        private static Dictionary<string, Action> commands = new Dictionary<string, Action>()
        {
            {"Command", ChatCommand }
        };

        private static void ChatCommand()
        {
            // Only admins may use this command
            if (!ConsoleLog.IsUserAdmin()) return;
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
                    if(identifier.Equals(commandChatSymbol + "tame"))
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
