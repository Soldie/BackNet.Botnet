using System.Collections.Generic;
using System.IO;
using Client.AdvancedConsole;
using Client.Commands.Core;

namespace Client.Commands
{
    internal class Upload : IClientCommand, IPreProcessCommand
    {
        public string name { get; } = "upload";
        
        public string description { get; } = "Upload a file to the server";
        
        public string syntaxHelper { get; } = "upload [localFileName] [remoteFileName]";
        
        public bool isLocal { get; } = false;
        
        public List<string> validArguments { get; } = new List<string>()
        {
            "?* ?"
        };


        /// <summary>
        /// Check if the specified local file exists
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool PreProcess(List<string> args)
        {
            if (File.Exists(args[0]))
            {
                return true;
            }

            ColorTools.WriteCommandError("The specified file doesn't exist");
            return false;
        }

        
        public void Process(List<string> args)
        {
            var path = args[0];

            ColorTools.WriteCommandMessage($"Starting upload of file '{path}' to the server");

            // Send the data length first
            ClientCommandsManager.networkManager.WriteLine(new FileInfo(path).Length.ToString());

            using (var readStream = new FileStream(path, FileMode.Open))
            {
                ClientCommandsManager.networkManager.StreamToNetworkStream(readStream);
            }

            var result = ClientCommandsManager.networkManager.ReadLine();

            if (result == "Success")
            {
                ColorTools.WriteCommandSuccess("File successfully uploaded to the server");
            }
            else
            {
                ColorTools.WriteCommandError("An error occured");
            }
        }
    }
}
