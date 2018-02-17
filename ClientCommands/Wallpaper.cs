using System.Collections.Generic;
using AdvancedConsole;
using Shared;

namespace ClientCommands
{
    internal class Wallpaper : IClientCommand
    {
        public string name { get; } = "wallpaper";

        public string description { get; } = "Set the remote server wallpaper";

        public string syntaxHelper { get; } = "wallpaper [remoteFileName]";

        public bool isLocal { get; } = false;

        public List<string> validArguments { get; } = new List<string>()
        {
            "?"
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
