using Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Commands
{
    internal class Screenshot : ICommand
    {
        public string name { get; } = "screenshot";

        public string description { get; } = "Take a screenshot of the server's screen and downnload it";

        public string syntaxHelper { get; } = "screenshot";

        public bool isLocal { get; } = false;

        public List<string> validArguments { get; } = null;


        public bool PreProcessCommand(List<string> args)
        {
            throw new NotImplementedException();
        }

        public void ClientMethod(List<string> args)
        {
            ColorTools.WriteCommandMessage("Waiting for screenshot data...");
            var fileName = DateTime.Now.ToShortDateString().Replace('/', '-')
                           + "_"
                           + DateTime.Now.Hour + '-' + DateTime.Now.Minute + '-' + DateTime.Now.Second
                           + ".png";

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
            using (var ms = new MemoryStream())
            {
                var bounds = GetScreenRectangle();
                using (var bitmap = new Bitmap(bounds.Width, bounds.Height))
                {
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                    }

                    bitmap.Save(ms, ImageFormat.Png);
                }


                // Send the data length first
                CommandsManager.networkManager.WriteLine(ms.Length.ToString());

                // Reset memory stream position
                ms.Position = 0;
                CommandsManager.networkManager.StreamToNetworkStream(ms);
            }
        }

        static Rectangle GetScreenRectangle()
        {
            try
            {
                // Resolve DPI issue (altered screen resolution)
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    SetProcessDpiAwareness(ProcessDPIAwareness.ProcessPerMonitorDPIAware);
                }
            }
            catch (EntryPointNotFoundException)
            {
                // Exception occures if OS does not implement this API, just ignore it
            }

            return new Rectangle(0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
        }


        enum ProcessDPIAwareness
        {
            ProcessDPIUnaware = 0,
            ProcessSystemDPIAware = 1,
            ProcessPerMonitorDPIAware = 2
        }


        [DllImport("shcore.dll")]
        static extern int SetProcessDpiAwareness(ProcessDPIAwareness value);
    }
}
