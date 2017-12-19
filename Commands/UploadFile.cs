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
    internal class UploadFile : ICommand
    {
        #region Variables
        public string name { get; } = "uploadfile";


        public string description { get; } = "Upload a file to the server";


        public string syntaxHelper { get; } = "uploadfile [localFileName] [remoteFileName]";


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
            "{UploadFile:init}"
        };

        public List<string> savedData { get; set; } = new List<string>();
        #endregion Variables


        #region Methods
        public bool PreProcessCommand(List<string> args)
        {
            var result = File.Exists(args[1]);
            if (result)
            {
                savedData.Add(args[1]);
            }
            else
            {
                ColorTools.WriteCommandError("The specified file doesn't exist");
            }

            return result;
        }


        public void ClientMethod(List<string> args)
        {
            var path = savedData[1];
            ColorTools.WriteCommandMessage($"Starting upload of file '{path}' to the server");

            // Send the data length first
            GlobalNetworkManager.WriteIntAsBytes((int)new FileInfo(path).Length);

            using (var readStream = new FileStream(path, FileMode.Open))
            {
                GlobalNetworkManager.ReadFileStreamAndWriteToNetworkStream(readStream, 4096);
            }

            var result = GlobalNetworkManager.ReadLine();

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
            GlobalNetworkManager.WriteLine("{ReceiveFileFromClient:init}");

            var dataLength = GlobalNetworkManager.ReadBytesAsInt32();

            try
            {
                using (var fs = new FileStream(args[2], FileMode.Create))
                {
                    GlobalNetworkManager.ReadNetworkStreamAndWriteToFileStream(fs, 4096, dataLength);
                }

                GlobalNetworkManager.WriteLine("Success");
            }
            catch (Exception)
            {
                // Delete the partially created file
                File.Delete(args[2]);
                GlobalNetworkManager.WriteLine("Error");
            }
        }
        #endregion Methods
    }
}
