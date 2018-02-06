using System;
using System.Collections.Generic;

namespace ClientCommands
{
    internal class PWD : IClientCommand
    {
        public string name { get; set; } = "pwd";

        public string description { get; set; } = "Display the remote current working directory";

        public string syntaxHelper { get; set; } = "pwd";

        public bool isLocal { get; set; } = false;

        public List<string> validArguments { get; set; } = null;


        public void Process(List<string> args)
            => Console.WriteLine(ClientCommandsManager.networkManager.ReadLine());
    }
}
