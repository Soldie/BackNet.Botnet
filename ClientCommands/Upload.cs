using Shared;
using System;
using System.Collections.Generic;
using System.IO;

namespace ClientCommands
{
    internal class Upload : IClientCommand
    {
        public string name { get; } = "upload";
        
        public string description { get; } = "Upload a file to the server";
        
        public string syntaxHelper { get; } = "upload [localFileName] [remoteFileName]";
        
        public bool isLocal { get; } = false;
        
        public List<string> validArguments { get; } = new List<string>()
        {
            "? ?"
        };

        
        public void Process(List<string> args)
        {
            var path = args[0];

            if (!File.Exists(path))
            {
                ClientCommandsManager.networkManager.WriteLine("KO");
                ColorTools.WriteCommandError("The specified file doesn't exist");
                return;
            }
            ClientCommandsManager.networkManager.WriteLine("OK");

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
