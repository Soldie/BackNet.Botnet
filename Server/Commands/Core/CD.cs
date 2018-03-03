using System.Collections.Generic;
using System.IO;
using Shared;

namespace Server.Commands.Core
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
