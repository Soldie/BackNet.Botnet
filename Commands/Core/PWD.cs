using System;
using System.Collections.Generic;
using System.IO;

namespace Commands
{
    internal class PWD : ICommand
    {
        public string name { get; set; } = "pwd";

        public string description { get; set; } = "Display the remote current working directory";

        public string syntaxHelper { get; set; } = "pwd";

        public bool isLocal { get; set; } = false;

        public List<string> validArguments { get; set; } = null;


        public bool PreProcessCommand(List<string> args)
        {
            throw new NotImplementedException();
        }

        public void ClientMethod(List<string> args)
            => Console.WriteLine(CommandsManager.networkManager.ReadLine());

        public void ServerMethod(List<string> args)
            => CommandsManager.networkManager.WriteLine(Directory.GetCurrentDirectory());
    }
}
