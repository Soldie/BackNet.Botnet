using System;
using System.Collections.Generic;
using System.IO;

namespace Commands
{
    internal class LCD : ICommand
    {
        public string name { get; set; } = "lcd";

        public string description { get; set; } = "Change the local current working directory";

        public string syntaxHelper { get; set; } = "lcd [newPath]";

        public bool isLocal { get; set; } = true;

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
            if (Directory.Exists(args[0]))
            {
                Directory.SetCurrentDirectory(args[0]);
                Console.WriteLine($"lcwd => {Directory.GetCurrentDirectory()}");
            }
            else
            {
                CommandsManager.networkManager.WriteLine("No such directory");
            }
        }

        public void ServerMethod(List<string> args)
        {
            throw new NotImplementedException();
        }
    }
}
