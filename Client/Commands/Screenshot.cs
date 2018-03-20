using System;
using System.Collections.Generic;
using System.IO;
using Client.AdvancedConsole;
using Client.Commands.Core;

namespace Client.Commands
{
    internal class Screenshot : IClientCommand
    {
        public string name { get; } = "screenshot";

        public string description { get; } = "Take a screenshot of the server's screen and downnload it";

        public bool isLocal { get; } = false;

        public List<string> validArguments { get; } = new List<string>()
        {
            "",
            "?*:[filename.png]"
        };


        public void Process(List<string> args)
        {
            ColorTools.WriteCommandMessage("Waiting for screenshot data...");
            var fileName = args.Count == 1 ? args[0] :
                                             DateTime.Now.ToShortDateString().Replace('/', '-')
                                             + "_" + DateTime.Now.Hour
                                             + '-' + DateTime.Now.Minute
                                             + '-' + DateTime.Now.Second
                                             + ".png";
            
            try
            {
                using (var fs = new FileStream(fileName, FileMode.Create))
                {
                    ClientCommandsManager.networkManager.NetworkStreamToStream(fs);
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
