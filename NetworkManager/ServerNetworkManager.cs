using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace NetworkManager
{
    public class ServerNetworkManager
    {
        TcpClient tcpClient { get; set; }


        /// <summary>
        /// Try to connect to the client with the given ip and port number
        /// </summary>
        /// <param name="remoteAdress">Client's adress (IP or hostname)</param>
        /// <param name="remotePort">Client's port number</param>
        /// <returns>Boolean for result</returns>
        public bool ConnectToClient(string remoteAdress, int remotePort)
        {
            tcpClient = new TcpClient();

            IPAddress ipAddress = null;
            try
            {
                ipAddress = IPAddress.Parse(remoteAdress);
            }
            catch (FormatException)
            {
                // Ignored
            }

            try
            {
                if (ipAddress == null)
                {
                    tcpClient.Connect(remoteAdress, remotePort);
                }
                else
                {
                    tcpClient.Connect(ipAddress, remotePort);
                }

                InitiateStreams();
            }
            catch (Exception)
            {
                // if no Client, don't continue
                return false;
            }

            return true;
        }


        /// <summary>
        /// Instanciate the network stream, and based on it StreamReaders and StreamWriters
        /// </summary>
        void InitiateStreams()
        {
            GlobalNetworkManager.networkStream = tcpClient.GetStream();
            GlobalNetworkManager.streamReader = new StreamReader(GlobalNetworkManager.networkStream);
            GlobalNetworkManager.streamWriter = new StreamWriter(GlobalNetworkManager.networkStream);
            GlobalNetworkManager.binaryWriter = new BinaryWriter(GlobalNetworkManager.networkStream);
            GlobalNetworkManager.binaryReader = new BinaryReader(GlobalNetworkManager.networkStream);
        }


        /// <summary>
        /// Call the Cleanup method of GlobalNetworkManager to close the streams
        /// </summary>
        public void Cleanup() => GlobalNetworkManager.Cleanup();
    }
}
