using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Server.Commands.Core;
using Shared;

namespace Server.Commands
{
    internal class DownloadUrl : ICommand
    {
        public string name { get; } = "downloadurl";

        
        public void Process(List<string> args)
        {
            var url = args[0];
            var newFile = url.Split('/').Last();

            var Client = new WebClient();
            try
            {
                Client.DownloadFile(url, newFile);
                ServerCommandsManager.networkManager.WriteLine("Success");
            }
            catch (IOException)
            {
                ServerCommandsManager.networkManager.WriteLine("IO");
            }
            catch (Exception)
            {
                // Delete the partially created file
                File.Delete(newFile);
                ServerCommandsManager.networkManager.WriteLine("Web");
            }
        }
    }
}
