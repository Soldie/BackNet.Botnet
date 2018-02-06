using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ClientCommands
{
    internal class GetOpenPrograms : IClientCommand
    {
        public string name { get; } = "getopenprograms";

        public string description { get; } = "Displays a list of all open programs on the remote computer";

        public string syntaxHelper { get; } = "getopenprograms";

        public bool isLocal { get; } = false;

        public List<string> validArguments { get; } = null;


        public void Process(List<string> args)
        {
            var data = "";
            while (data != "{end}")
            {
                if(data != "")
                    Console.WriteLine(data);
                data = ClientCommandsManager.networkManager.ReadLine();
            }
        }
    }
}
