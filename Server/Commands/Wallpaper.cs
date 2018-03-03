using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Server.Commands.Core;
using Shared;

namespace Server.Commands
{
    internal class Wallpaper : ICommand
    {
        public string name { get; } = "wallpaper";


        public void Process(List<string> args)
        {
            var filename = args[0];

            if (!File.Exists(filename))
            {
                ServerCommandsManager.networkManager.WriteLine("FileError");
                return;
            }
            var fileInfo = new FileInfo(filename);

            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, fileInfo.FullName, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);

            ServerCommandsManager.networkManager.WriteLine("OK");
        }


        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(uint uiAction, uint uiParam, string pvParam, uint fWinIni);

        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 1;
        const int SPIF_SENDCHANGE = 2;
    }
}
