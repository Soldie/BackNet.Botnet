using System;
using System.Collections.Generic;
using Client.AdvancedConsole;

namespace Client.Commands.Core
{
    internal class CD : IClientCommand
    {
        public string name { get; set; } = "cd";

        public string description { get; set; } = "Change the remote current working directory";

        public bool isLocal { get; set; } = false;

        public List<string> validArguments { get; set; } = new List<string>()
        {
            "?:[directory]"
        };


        public void Process(List<string> args)
        {
            var result = ClientCommandsManager.networkManager.ReadLine();
            if (result == "KO")
            {
                ColorTools.WriteCommandError("No such remote directory");
            }
            else
            {
                Console.WriteLine($"cwd => {result}");
            }
        }
    }
}
