using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Commands
{
    internal class DownloadUrl : ICommand
    {
        public string name { get; } = "downloadurl";

        public string description { get; } = "Make the server download a file from the specified url";

        public string syntaxHelper { get; } = "downloadurl [url]";

        public bool isLocal { get; } = false;

        public List<string> validArguments { get; } = new List<string>()
        {
            "?"
        };


        public bool PreProcessCommand(List<string> args)
        {
            throw new NotImplementedException();
        }

        public void ClientMethod(List<string> args)
        {
            ColorTools.WriteCommandMessage("Starting download of file from url");

            var result = CommandsManager.networkManager.ReadLine();
            if (result == "Success")
            {
                ColorTools.WriteCommandSuccess("File downloaded successfully from URL");
            }
            else
            {
                ColorTools.WriteCommandError($"Download failed : {(result == "IO" ? "IO exception" : "Network error")}");
            }
        }

        public void ServerMethod(List<string> args)
        {
            var url = args[0];
            var newFile = url.Split('/').Last();

            var Client = new WebClient();
            try
            {
                Client.DownloadFile(url, newFile);
                CommandsManager.networkManager.WriteLine("Success");
            }
            catch (IOException)
            {
                CommandsManager.networkManager.WriteLine("IO");
            }
            catch (Exception)
            {
                // Delete the partially created file
                File.Delete(newFile);
                CommandsManager.networkManager.WriteLine("Web");
            }
        }
    }
}
