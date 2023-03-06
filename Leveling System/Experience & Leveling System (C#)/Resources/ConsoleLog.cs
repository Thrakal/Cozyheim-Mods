using Jotunn.Entities;
using Jotunn.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using UnityEngine;
using Log = Jotunn.Logger;

namespace Cozyheim.LevelingSystem
{
    internal class ConsoleLog : ConsoleCommand
    {
        public override string Name => Main.modName;
        public override string Help => "Commands for '" + Name + "'";
        public override bool IsCheat => true;
        public override bool IsSecret => false;
        public override bool IsNetwork => true;
        public override bool OnlyServer => false;

        private Dictionary<string, Action> _commands = new Dictionary<string, Action>();
        private static List<string> commandArgs;

        public static CustomRPC rpc_ReloadConfigServer = null;
        public static CustomRPC rpc_ReloadConfigClient = null;
        public static CustomRPC rpc_SetLevel = null;

        CommandList[] commandList = new CommandList[]
        {
            new CommandList("ReloadConfig", ReloadConfig),
            new CommandList("ResetOwnLevel", ResetOwnLevel),
            new CommandList("SetLevel", SetLevel)
        };

        public static void Init()
        {
            if (rpc_ReloadConfigServer == null) {
                rpc_ReloadConfigServer = NetworkManager.Instance.AddRPC("ReloadConfigServer", RPC_ReloadConfigServer, RPC_ReloadConfigServer);
            }
            if (rpc_ReloadConfigClient == null) {
                rpc_ReloadConfigClient = NetworkManager.Instance.AddRPC("ReloadConfigClient", RPC_ReloadConfigClient, RPC_ReloadConfigClient);
            }
            if (rpc_SetLevel == null)
            {
                rpc_SetLevel = NetworkManager.Instance.AddRPC("SetLevel", RPC_SetLevel, RPC_SetLevel);
            }
        }

        public static void ReloadConfig()
        {
            // Only admins may use this command
            if (!IsUserAdmin())
            {
                ReloadAndUpdateAll();
            } else
            {
                rpc_ReloadConfigServer.SendPackage(ZRoutedRpc.Everybody, new ZPackage());
            }
        }

        private static IEnumerator RPC_ReloadConfigClient(long sender, ZPackage package)
        {
            if (Player.m_localPlayer != null)
            {
                ReloadAndUpdateAll();
            }
            yield return null;
        }

        private static IEnumerator RPC_ReloadConfigServer(long sender, ZPackage package)
        {
            if(ZNet.instance.IsServer())
            {
                Main.configFile.Reload();
                rpc_ReloadConfigClient.SendPackage(ZRoutedRpc.Everybody, new ZPackage());
            }
            yield return null;
        }

        private static void ReloadAndUpdateAll()
        {
            XPTable.UpdatePlayerXPTable();
            XPTable.UpdateMonsterXPTable();
            SkillManager.Instance.ReloadAllSkills();
        }

        private static void GetAll()
        {
            // Only admins may use this command
            if (!IsUserAdmin()) return;

            // Command code here
            string stringToPrint = XPManager.Instance.GetAllMonsterXpString();
            Print(stringToPrint);
        }

        private static void ResetOwnLevel()
        {
            SetPlayerLevel(1);
        }

        private static void SetPlayerLevel(int level)
        {
            XPManager.Instance.SetPlayerLevel(level);
            XPManager.Instance.SetPlayerXP(0);
            UIManager.Instance.playerLevel = level;
            UIManager.Instance.playerXP = 0;
            SkillManager.Instance.SkillResetAll();
            UIManager.Instance.UpdateUI(true);
        }

        private static void SetLevel()
        {
            // Only admins may use this command
            if(!IsUserAdmin()) return;

            if(commandArgs.Count >= 2)
            {
                if (int.TryParse(commandArgs[0], out int level))
                {
                    ZPackage newPackage = new ZPackage();
                    newPackage.Write(level);

                    string playerName = "";
                    for (int i = 1; i < commandArgs.Count; i++)
                    {
                        if (i > 1)
                        {
                            playerName += " ";
                        }
                        playerName += commandArgs[i].ToString();
                    }

                    newPackage.Write(playerName);

                    rpc_SetLevel.SendPackage(ZRoutedRpc.Everybody, newPackage);
                    return;
                }
            }
            
            Debug.Log("* Incorrect Arguments * (Example: 'LevelingSystem SetLevel 10 Name of the player')");    
        }

        private static IEnumerator RPC_SetLevel(long sender, ZPackage package)
        {
            int level = package.ReadInt();
            string playerName = package.ReadString();

            if (Player.m_localPlayer != null)
            {
                if (Player.m_localPlayer.GetPlayerName().ToLower() == playerName.ToLower())
                {
                    if (level < 1)
                    {
                        level = 1;
                    }

                    if (level > XPTable.playerXPTable.Length + 1)
                    {
                        level = XPTable.playerXPTable.Length + 1;
                    }

                    SetPlayerLevel(level);
                    Player.m_localPlayer.Message(MessageHud.MessageType.Center, "Admin set your level to " + level.ToString());
                }
            }

            yield return null;
        }

        #region Console Setup
        private static bool IsUserAdmin()
        {
            if (!SynchronizationManager.Instance.PlayerIsAdmin)
            {
                Player.m_localPlayer.Message(MessageHud.MessageType.Center, "Only admins are allowed to use this command");
                return false;
            }

            return true;
        }

        // Setup the list of available commands
        public override List<string> CommandOptionList()
        {
            List<string> commands = new List<string>();
            foreach (CommandList com in commandList)
            {
                if (!_commands.ContainsKey(com.name))
                {
                    _commands.Add(com.name, com.action);
                }
                commands.Add(com.name);
            }
            return commands;
        }

        // Check if the command exists and execute the associated method
        public override void Run(string[] args)
        {
            string command = args[0];

            commandArgs = args.ToList();
            commandArgs.RemoveAt(0);

            foreach (KeyValuePair<string, Action> com in _commands)
            {
                if (com.Key.ToLower() == command.ToLower())
                {
                    com.Value();
                    return;
                }
            }

            Debug.Log("The command doesn't exist: '" + command + "'");
        }

        //    -----------------------
        //   ----- PRINT METHODS -----
        //    -----------------------

        internal static void Print(object printMsg, LogType type = LogType.Info, bool debugMode = true)
        {
            if (Main.debugEnabled.Value && debugMode)
            {
                string textToPrint = "[Time: " + Time.time.ToString("N0") + "] " + printMsg.ToString();
                switch (type)
                {
                    case LogType.Info: Log.LogInfo(textToPrint); break;
                    case LogType.Message: Log.LogMessage(textToPrint); break;
                    case LogType.Warning: Log.LogWarning(textToPrint); break;
                    case LogType.Error: Log.LogError(textToPrint); break;
                    case LogType.Fatal: Log.LogFatal(textToPrint); break;
                    default: Log.LogInfo(textToPrint); break;
                }
            }
        }
        internal static void Print(object printMsg, bool debugMode) => Print(printMsg, LogType.Info, debugMode);
        #endregion
    }

    internal class CommandList
    {
        public string name;
        public Action action;

        public CommandList(string name, Action action)
        {
            this.name = name;
            this.action = action;
        }
    }

    internal enum LogType
    {
        Info,
        Message,
        Error,
        Warning,
        Fatal
    }
}