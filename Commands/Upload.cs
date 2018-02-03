using Shared;
using System;
using System.Collections.Generic;
using System.IO;

namespace Commands
{
    internal class Upload : ICommand
    {
        #region Variables
        public string name { get; } = "upload";
        
        public string description { get; } = "Upload a file to the server";
        
        public string syntaxHelper { get; } = "upload [localFileName] [remoteFileName]";
        
        public bool isLocal { get; } = false;
        
        public List<string> validArguments { get; } = new List<string>()
        {
            "? ?"
        };
        #endregion Variables


        #region Methods
        public bool PreProcessCommand(List<string> args)
        {
            var result = File.Exists(args[0]);
            if (!result)
            {
                ColorTools.WriteCommandError("The specified file doesn't exist");
            }

            return result;
        }
        
        public void ClientMethod(List<string> args)
        {
            var path = args[0];
            ColorTools.WriteCommandMessage($"Starting upload of file '{path}' to the server");

            // Send the data length first
            CommandsManager.networkManager.WriteLine(new FileInfo(path).Length.ToString());

            using (var readStream = new FileStream(path, FileMode.Open))
            {
                CommandsManager.networkManager.StreamToNetworkStream(readStream);
            }

            var result = CommandsManager.networkManager.ReadLine();

            if (result == "Success")
            {
                ColorTools.WriteCommandSuccess("File successfully uploaded to the server");
            }
            else
            {
                ColorTools.WriteCommandError("An error occured");
            }
        }
        public void ServerMethod(List<string> args)
        {
            var dataLength = int.Parse(CommandsManager.networkManager.ReadLine());

            try
            {
                using (var fs = new FileStream(args[1], FileMode.Create))
                {
                    CommandsManager.networkManager.NetworkStreamToStream(fs, dataLength);
                }

                CommandsManager.networkManager.WriteLine("Success");
            }
            catch (Exception)
            {
                // Delete the partially created file
                File.Delete(args[1]);
                CommandsManager.networkManager.WriteLine("Error");
            }
        }
        #endregion Methods
    }
}
