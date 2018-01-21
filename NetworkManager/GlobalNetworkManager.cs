using System;
using System.IO;
using System.Net.Sockets;

namespace NetworkManager
{
    public static class GlobalNetworkManager
    {
        public static ClientNetworkManager clientNetworkManager { get; }

        public static ServerNetworkManager ServerNetworkManager { get; }
        

        internal static NetworkStream networkStream { get; set; }

        internal static StreamWriter streamWriter { get; set; }

        internal static StreamReader streamReader { get; set; }

        internal static BinaryWriter binaryWriter { get; set; }

        internal static BinaryReader binaryReader { get; set; }


        const int DEFAULT_BUFFER_SIZE = 4046;


        /// <summary>
        /// Constructor, initiate clientNetworkManager and ServerNetworkManager
        /// </summary>
        static GlobalNetworkManager()
        {
            clientNetworkManager = new ClientNetworkManager();
            ServerNetworkManager = new ServerNetworkManager();
        }
        

        /// <summary>
        /// Read one line from the network stream
        /// </summary>
        /// <returns>String read from stream</returns>
        public static string ReadLine() => streamReader.ReadLine();

        
        /// <summary>
        /// Read all the given ReadStream content and write it to the network stream as byte arrays, then flush
        /// </summary>
        /// <param name="stream">Stream to process, must be a readable stream</param>
        public static void StreamToNetworkStream(Stream stream)
        {
            if (!stream.CanRead)
            {
                // The stream can't be read : invalid argument
                throw new ArgumentException();
            }

            // Wait for ready flag
            ReadLine();

            var buffer = new byte[DEFAULT_BUFFER_SIZE];
            int bytesRead;
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                binaryWriter.Write(buffer, 0, bytesRead);
                binaryWriter.Flush();
            }
        }


        /// <summary>
        /// Read all the network ReadStream content and write it to the given stream stream
        /// </summary>
        /// <param name="stream">Stream to process, must be a writable stream</param>
        /// <param name="dataSize">Size of the data to write</param>
        public static void NetworkStreamToStream(Stream stream, int dataSize)
        {
            if (!stream.CanWrite)
            {
                // The stream can't write : invalid argument
                throw new ArgumentException();
            }

            // Send ready flag
            WriteLine("OK");

            var buffer = new byte[DEFAULT_BUFFER_SIZE];
            int bytesRead;
            var bytesWritten = 0;
            while ((bytesRead = binaryReader.Read(buffer, 0, buffer.Length)) > 0)
            {
                stream.Write(buffer, 0, bytesRead);
                bytesWritten += bytesRead;

                // The file has been totally written
                if (bytesWritten == dataSize)
                {
                    break;
                }
            }
        }


        /// <summary>
        /// Write the given string to the network stream, then flush
        /// </summary>
        /// <param name="data">String to write</param>
        public static void WriteLine(string data)
        {
            streamWriter.WriteLine(data);
            streamWriter.Flush();
        }


        /// <summary>
        /// Close the network stream
        /// </summary>
        internal static void Cleanup() => networkStream.Close();
    }
}
