using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var path = savedData[0];
            ColorTools.WriteCommandMessage($"Starting download of file '{savedData[1]}' from the server");

            var dataLength = GlobalNetworkManager.ReadBytesAsInt32();

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

            savedData.Clear();
        }

        public void ServerMethod(List<string> args)
        {
            GlobalNetworkManager.WriteLine(clientFlags[0]);

            if (!File.Exists(args[0]))
            {
                GlobalNetworkManager.WriteLine("File not found");
                return;
            }

            // Send the data length first
            GlobalNetworkManager.WriteIntAsBytes((int)new FileInfo(args[0]).Length);

            using (var readStream = new FileStream(args[0], FileMode.Open))
            {
                GlobalNetworkManager.ReadStreamAndWriteToNetworkStream(readStream, 4096);
            }
        }
    }
}
