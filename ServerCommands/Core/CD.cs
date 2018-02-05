using Shared;
using System;
using System.Collections.Generic;
using System.IO;

namespace ServerCommands
{
    internal class CD : ICommand
    {
        public string name { get; set; } = "cd";

        
        public void Process(List<string> args)
        {
            if (Directory.Exists(args[0]))
            {
                Directory.SetCurrentDirectory(args[0]);
                ServerCommandsManager.networkManager.WriteLine(Directory.GetCurrentDirectory());
            }
            else
            {
                ServerCommandsManager.networkManager.WriteLine("KO");
            }
        }
    }
}
