using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Shared;

namespace Slave.Core
{
    public class SlaveNetworkManager : GlobalNetworkManager
    {
        TcpClient tcpClient { get; set; }


        /// <summary>
        /// Try to connect to the master with the given ip and port number
        /// </summary>
        /// <param name="remoteAdress">Master's adress (IP or hostname)</param>
        /// <param name="remotePort">Master's port number</param>
        /// <returns>Boolean for result</returns>
        public bool ConnectToMaster(string remoteAdress, int remotePort)
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
                // if no client, don't continue
                return false;
            }

            return true;
        }


        /// <summary>
        /// Instanciate the network stream, and based on it StreamReaders and StreamWriters
        /// </summary>
        void InitiateStreams()
        {
            networkStream = tcpClient.GetStream();
            streamReader = new StreamReader(networkStream);
            streamWriter = new StreamWriter(networkStream);
            binaryWriter = new BinaryWriter(networkStream);
            binaryReader = new BinaryReader(networkStream);
        }
    }
}
