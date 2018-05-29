using System;
using System.Collections.Generic;
using System.IO;
using Master.AdvancedConsole;

namespace Master.Commands.Core
{
    internal class LLS : IMasterCommand
    {
        public string name { get; set; } = "lls";

        public string description { get; set; } = "Display the files and folders name of the local current working directory";

        public bool isLocal { get; set; } = true;

        public List<string> validArguments { get; set; } = null;

        public void Process(List<string> args)
        {
            var cwd = Directory.GetCurrentDirectory();
            try
            {
                foreach (var dir in Directory.GetDirectories(cwd))
                {
                    var dirInfo = new DirectoryInfo(dir);
                    Console.WriteLine($"{dirInfo.LastWriteTime:MM/dd/yy  HH:mm:ss}   <DIR>   {dirInfo.Name}");
                }

                foreach (var file in Directory.GetFiles(cwd))
                {
                    var fileInfo = new DirectoryInfo(file);
                    Console.WriteLine($"{fileInfo.LastWriteTime:MM/dd/yy  HH:mm:ss}           {fileInfo.Name}");
                }
            }
            catch (Exception)
            {
                ColorTools.WriteCommandError("Not enough permissions");
            }
        }
    }
}
