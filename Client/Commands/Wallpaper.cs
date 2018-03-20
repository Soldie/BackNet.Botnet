using System.Collections.Generic;
using Client.AdvancedConsole;
using Client.Commands.Core;

namespace Client.Commands
{
    internal class Wallpaper : IClientCommand
    {
        public string name { get; } = "wallpaper";

        public string description { get; } = "Set the remote server wallpaper";

        public bool isLocal { get; } = false;

        public List<string> validArguments { get; } = new List<string>()
        {
            "?:[remoteFileName]"
        };


        public void Process(List<string> args)
        {
            if (ClientCommandsManager.networkManager.ReadLine() == "OK")
            {
                ColorTools.WriteCommandSuccess("Wallpaper changed");
            }
            else
            {
                ColorTools.WriteCommandError("File not found on the remote server");
            }
        }
    }
}
