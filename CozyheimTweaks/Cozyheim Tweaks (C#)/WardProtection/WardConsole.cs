using Jotunn.Entities;
using Jotunn.Managers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CozyheimTweaks
{
    internal class WardConsole : ConsoleCommand
    {
        public override string Name => WardProtection.patchName;
        public override string Help => "Control commands for " + Name + " (admins only)";
        public override bool IsCheat => false;
        public override bool IsSecret => false;
        public override bool IsNetwork => true;
        public override bool OnlyServer => false;

        private Dictionary<string, Action> _commands = new Dictionary<string, Action>();
        private static List<string> commandArgs;


        // -- Custom Commands --
        // ------ START --------

        // Setup commands
        CommandList[] commandList = new CommandList[]
        {
            new CommandList("WhitelistAdd", WhitelistAdd),
            new CommandList("WhitelistUpdate", WhitelistUpdate),
            new CommandList("WhitelistRemove", WhitelistRemove),
            new CommandList("ReloadJSON", ReloadJSON)
        };

        private static void ReloadJSON()
        {
            if (!SynchronizationManager.Instance.PlayerIsAdmin)
            {
                Player.m_localPlayer.Message(MessageHud.MessageType.Center, "Only admins are allowed to use this command");
                return;
            }

            WardProtection.rpc_ReloadJSON.SendPackage(ZRoutedRpc.Everybody, new ZPackage());
        }


        private static void WhitelistUpdate()
        {
            if (!SynchronizationManager.Instance.PlayerIsAdmin)
            {
                Player.m_localPlayer.Message(MessageHud.MessageType.Center, "Only admins are allowed to use this command");
                return;
            }

            long userID;
            long whitelistID;
            int status = -1;

            if (commandArgs.Count == 3)
            {
                long.TryParse(commandArgs[0], out userID);
                long.TryParse(commandArgs[1], out whitelistID);
                if (commandArgs.Count >= 3)
                {
                    int.TryParse(commandArgs[2], out status);
                }
                if(status >= 0 && status <= 3)
                {
                    WardProtection.Instance.UpdateWhitelist(userID, whitelistID, status);
                }
            }
        }


        private static void WhitelistAdd()
        {
            if (!SynchronizationManager.Instance.PlayerIsAdmin)
            {
                Player.m_localPlayer.Message(MessageHud.MessageType.Center, "Only admins are allowed to use this command");
                return;
            }

            long userID;
            long whitelistID;
            int status = 3;

            if (commandArgs.Count >= 2)
            {
                long.TryParse(commandArgs[0], out userID);
                long.TryParse(commandArgs[1], out whitelistID);
                if(commandArgs.Count >= 3)
                {
                    int.TryParse(commandArgs[2], out status);
                }
                if (status >= 0 && status <= 3)
                {
                    WardProtection.Instance.AddToWhitelist(userID, whitelistID, status);
                } else
                {
                    WardProtection.Instance.AddToWhitelist(userID, whitelistID);
                }
            }
        }

        private static void WhitelistRemove()
        {
            if (!SynchronizationManager.Instance.PlayerIsAdmin)
            {
                Player.m_localPlayer.Message(MessageHud.MessageType.Center, "Only admins are allowed to use this command");
                return;
            }

            long userID;
            long whitelistID;

            if (commandArgs.Count >= 2)
            {
                long.TryParse(commandArgs[0], out userID);
                long.TryParse(commandArgs[1], out whitelistID);
                WardProtection.Instance.RemoveFromWhitelist(userID, whitelistID);
            }
        }


        #region Console Setup
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

            ToppLog.Print("The command doesn't exist: '" + command + "'");
        }
        #endregion
    }
}
