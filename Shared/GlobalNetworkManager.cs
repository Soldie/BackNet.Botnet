using System;
using System.IO;
using System.Net.Sockets;

namespace Shared
{
    public abstract class GlobalNetworkManager
    {
        public NetworkStream networkStream { get; set; }

        public StreamWriter streamWriter { get; set; }

        public StreamReader streamReader { get; set; }

        public BinaryWriter binaryWriter { get; set; }

        public BinaryReader binaryReader { get; set; }


        const int DEFAULT_BUFFER_SIZE = 4046;


        /// <summary>
        /// Read one line from the network stream
        /// </summary>
        /// <returns>String read from stream</returns>
        public string ReadLine()
        {
            try
            {
                var data = streamReader.ReadLine();
                if (data == null)
                {
                    throw new NetworkException();
                }
                return data;
            }
            catch (Exception)
            {
                throw new NetworkException();
            }
        }


        /// <summary>
        /// Read all the given ReadStream content and write it to the network stream as byte arrays, then flush
        /// </summary>
        /// <param name="stream">Stream to process, must be a readable stream</param>
        public void StreamToNetworkStream(Stream stream)
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
                try
                {
                    binaryWriter.Write(buffer, 0, bytesRead);
                    binaryWriter.Flush();
                }
                catch (Exception)
                {
                    throw new NetworkException();
                }
            }
        }


        /// <summary>
        /// Read all the network ReadStream content and write it to the given stream stream
        /// </summary>
        /// <param name="stream">Stream to process, must be a writable stream</param>
        /// <param name="dataSize">Size of the data to write</param>
        public void NetworkStreamToStream(Stream stream, int dataSize)
        {
            if (!stream.CanWrite)
            {
                // The stream can't write : invalid argument
                throw new ArgumentException();
            }

            // Send ready flag
            WriteLine("OK");

            var buffer = new byte[DEFAULT_BUFFER_SIZE];
            var bytesWritten = 0;
            try
            {
                int bytesRead;
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
            catch (Exception)
            {
                throw new NetworkException();
            }
        }


        /// <summary>
        /// Write the given string to the network stream, then flush
        /// </summary>
        /// <param name="data">String to write</param>
        public void WriteLine(string data)
        {
            try
            {
                streamWriter.WriteLine(data);
                streamWriter.Flush();
            }
            catch (Exception)
            {
                throw new NetworkException();
            }
        }


        /// <summary>
        /// Close the network stream
        /// </summary>
        public void Cleanup() => networkStream.Close();
    }
}
