using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Client.AdvancedConsole;
using Shared;

namespace Client.Core
{
    public class ClientNetworkManager : NetworkManager
    {
        Socket socketForServer { get; set; }

        TcpListener tcpListener { get; set; }

        bool cleanedUp { get; set; }


        /// <summary>
        /// Listen for a connection request on the given port,
        /// when the connection succeed, instanciate the network stream,
        /// and based on it StreamReaders and StreamWriters
        /// </summary>
        /// <param name="port">Port to listen to</param>
        public void ListenAndConnect(int port)
        {
            tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();

            Console.Clear();
            ColorTools.WriteCommandMessage($"Listening on port {port} ...");
            // Wait for an incoming connection
            socketForServer = tcpListener.AcceptSocket();

            cleanedUp = false;

            // Initiate streams
            networkStream = new NetworkStream(socketForServer);
            streamReader = new StreamReader(networkStream);
            streamWriter = new StreamWriter(networkStream);
            binaryWriter = new BinaryWriter(networkStream);
            binaryReader = new BinaryReader(networkStream);

            ColorTools.WriteCommandMessage("Connected to " + (IPEndPoint)socketForServer.RemoteEndPoint + "\n");
        }


        /// <summary>
        /// Close the Socket and TcpListener, call GlobalNetworkManager.Cleanup() for the stream cleanup
        /// </summary>
        /// <param name="processingCommand">Was a command beeing processed</param>
        public void Cleanup(bool processingCommand)
        {
            cleanedUp = true;

            ColorTools.WriteWarning(processingCommand ? "\nDisconnected, operation stopped" : "\n\nDisconnected");

            try
            {
                base.Cleanup();
                socketForServer.Close();
                tcpListener.Stop();
            }
            catch (Exception)
            {
                // ignored
            }
        }


        /// <summary>
        /// Send a simple line to know if the other end of the connection is still connected
        /// This will throw an exception if the other end isn't connected
        /// </summary>
        /// <returns>Boolean stating the status of the connection</returns>
        public bool IsConnected()
        {
            try
            {
                WriteLine(".");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// Inform if the cleanup method as been called since the beggining of the connection
        /// </summary>
        /// <returns>Boolean</returns>
        public bool CleanupMade() => cleanedUp;
    }
}
