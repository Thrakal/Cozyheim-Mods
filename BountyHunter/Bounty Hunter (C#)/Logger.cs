using Jotunn;
using Jotunn.Managers;
using Jotunn.Configs;
using Jotunn.Entities;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using Log = Jotunn.Logger;

namespace BountyHunter
{
    public class Logger : ConsoleCommand
    {
        public override string Name => Main.modName;
        public override string Help => "Test commands, to test various mod behaviours";
        public override bool IsCheat => false;
        public override bool IsSecret => false;
        public override bool IsNetwork => true;
        public override bool OnlyServer => false;

        private Dictionary<string, Action> _commands = new Dictionary<string, Action>();
        private List<string> commandArgs;

        public class CommandList
        {
            public string name;
            public Action action;

            public CommandList(string name, Action action)
            {
                this.name = name;
                this.action = action;
            }
        }

        // Setup the list of available commands
        public override List<string> CommandOptionList()
        {
            List<string> commands = new List<string>();
            CommandList[] commandList = new CommandList[]
            {
            new CommandList("BountyToggle", BountyToggle),
            new CommandList("ResetPlayerPrefs", ResetPlayerPrefs)
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

        //    -------------------------
        //   ----- COMMAND METHODS -----
        //    -------------------------

        private void ResetPlayerPrefs()
        {
            string worldName = PlayerPrefs.GetString("world", "");
            PlayerPrefs.DeleteKey(worldName + "_toppbountyactive");
            Print("PlayerPrefs delete for ToppBounty");
        }

        private void BountyToggle()
        {
            int value;
            if (commandArgs.Count >= 1)
            {
                if (int.TryParse(commandArgs[0], out value))
                {
                    BountyManager.SetBountyToggleOnBoard(value);
                    Print(BountyManager.GetAvailableBountiesCount());
                }
            }
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

        internal static void Print(object printMsg, LogType type = LogType.Info)
        {
            if (Main.debugMode.Value)
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