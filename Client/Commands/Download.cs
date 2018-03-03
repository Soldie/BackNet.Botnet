using System;
using System.Collections.Generic;
using System.IO;
using Client.AdvancedConsole;
using Client.Commands.Core;

namespace Client.Commands
{
    internal class Download : IClientCommand
    {
        public string name { get; } = "download";

        public string description { get; } = "Download a file from the server";

        public string syntaxHelper { get; } = "download [remoteFileName] [localFileName]";

        public bool isLocal { get; } = false;

        public List<string> validArguments { get; } = new List<string>()
        {
            "? ?"
        };


        public void Process(List<string> args)
        {
            var initResult = ClientCommandsManager.networkManager.ReadLine();
            if (initResult != "OK")
            {
                ColorTools.WriteCommandError(initResult == "NotFound" ? "The remote file doesn't exist" : "An IO exception occured");
                return;
            }

            var path = args[1];
            ColorTools.WriteCommandMessage($"Starting download of file '{args[0]}' from the server");
            
            var dataLength = int.Parse(ClientCommandsManager.networkManager.ReadLine());

            try
            {
                using (var fs = new FileStream(path, FileMode.Create))
                {
                    ClientCommandsManager.networkManager.NetworkStreamToStream(fs, dataLength);
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
    }
}
