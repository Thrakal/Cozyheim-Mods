using Jotunn.Entities;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using Log = Jotunn.Logger;
using Jotunn.Managers;

namespace CozyheimTweaks
{
    internal class ToppLog : ConsoleCommand
    {
        public override string Name => Main.modName;
        public override string Help => "Control commands for Cozyheim (admins only)";
        public override bool IsCheat => false;
        public override bool IsSecret => false;
        public override bool IsNetwork => true;
        public override bool OnlyServer => false;

        private Dictionary<string, Action> _commands = new Dictionary<string, Action>();
        private List<string> commandArgs;


        // Setup the list of available commands
        public override List<string> CommandOptionList()
        {
            List<string> commands = new List<string>();
            CommandList[] commandList = new CommandList[]
            {

            };

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
            if (!SynchronizationManager.Instance.PlayerIsAdmin)
            {
                Player.m_localPlayer.Message(MessageHud.MessageType.Center, "Only admins are allowed to use these commands");
                return;
            }

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
            if (Main.debugMode.Value && debugMode)
            {
                string textToPrint = printMsg.ToString();
                switch(type)
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