using Shared;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace NetworkManager
{
    public class ClientNetworkManager
    {
        Socket socketForServer { get; set; }

        TcpListener tcpListener { get; set; }

        bool connected { get; set; }



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
            connected = true;

            // Initiate streams
            GlobalNetworkManager.networkStream = new NetworkStream(socketForServer);
            GlobalNetworkManager.streamReader = new StreamReader(GlobalNetworkManager.networkStream);
            GlobalNetworkManager.streamWriter = new StreamWriter(GlobalNetworkManager.networkStream);
            GlobalNetworkManager.binaryWriter = new BinaryWriter(GlobalNetworkManager.networkStream);
            GlobalNetworkManager.binaryReader = new BinaryReader(GlobalNetworkManager.networkStream);

            ColorTools.WriteCommandMessage("Connected to " + (IPEndPoint)socketForServer.RemoteEndPoint + "\n");

            // Say hello to display the cmd path the first time
            GlobalNetworkManager.SayHello();
        }
        

        /// <summary>
        /// Return the current state of the connection
        /// </summary>
        /// <returns>Boolean</returns>
        public bool IsConnected() => connected;


        /// <summary>
        /// Close the Socket and TcpListener, call GlobalNetworkManager.Cleanup() for the stream cleanup
        /// </summary>
        /// <param name="processingCommand"></param>
        public void Cleanup(bool processingCommand)
        {
            ColorTools.WriteWarning(processingCommand ? "\nDisconnected, operation stopped [ENTER]" : "\nDisconnected [ENTER]");

            connected = false;

            try
            {
                GlobalNetworkManager.Cleanup();
                socketForServer.Close();
                tcpListener.Stop();
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
