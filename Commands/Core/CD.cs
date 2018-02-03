using Shared;
using System;
using System.Collections.Generic;
using System.IO;

namespace Commands
{
    internal class CD : ICommand
    {
        public string name { get; set; } = "cd";

        public string description { get; set; } = "Change the remote current working directory";

        public string syntaxHelper { get; set; } = "cd";

        public bool isLocal { get; set; } = false;

        public List<string> validArguments { get; set; } = new List<string>()
        {
            "?"
        };


        public bool PreProcessCommand(List<string> args)
        {
            throw new NotImplementedException();
        }

        public void ClientMethod(List<string> args)
        {
            var result = CommandsManager.networkManager.ReadLine();
            if (result == "KO")
            {
                ColorTools.WriteCommandError("No such remote directory");
            }
            else
            {
                Console.WriteLine($"cwd => {result}");
            }
        }

        public void ServerMethod(List<string> args)
        {
            if (Directory.Exists(args[0]))
            {
                Directory.SetCurrentDirectory(args[0]);
                CommandsManager.networkManager.WriteLine(Directory.GetCurrentDirectory());
            }
            else
            {
                CommandsManager.networkManager.WriteLine("KO");
            }
        }
    }
}
