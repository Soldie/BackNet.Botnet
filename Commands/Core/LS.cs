using System;
using System.Collections.Generic;
using System.IO;

namespace Commands
{
    internal class LS : ICommand
    {
        public string name { get; set; } = "ls";

        public string description { get; set; } = "Display the files and folders name of the remote current working directory";

        public string syntaxHelper { get; set; } = "ls";

        public bool isLocal { get; set; } = false;

        public List<string> validArguments { get; set; } = null;


        public bool PreProcessCommand(List<string> args)
        {
            throw new NotImplementedException();
        }

        public void ClientMethod(List<string> args)
        {
            var data = "";
            while (data != "{end}")
            {
                if (data != "")
                    Console.WriteLine(data);
                data = CommandsManager.networkManager.ReadLine();
            }
        }

        public void ServerMethod(List<string> args)
        {
            var result = "";
            var cwd = Directory.GetCurrentDirectory();
            foreach (var dir in Directory.GetDirectories(cwd))
            {
                var dirInfo = new DirectoryInfo(dir);
                result += $"{dirInfo.LastWriteTime:MM/dd/yy  H:mm:ss}   <DIR>   {dirInfo.Name}\n";
            }
            foreach (var file in Directory.GetFiles(cwd))
            {
                var fileInfo = new DirectoryInfo(file);
                result += $"{fileInfo.LastWriteTime:MM/dd/yy  H:mm:ss}           {fileInfo.Name}\n";
            }

            CommandsManager.networkManager.WriteLine(result);
            CommandsManager.networkManager.WriteLine("{end}");
        }
    }
}
