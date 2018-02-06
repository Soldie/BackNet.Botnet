using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Media;

namespace ClientCommands
{
    internal class PlaySound : IClientCommand
    {
        public string name { get; } = "playsound";

        public string description { get; } = "Play a .wav audio file on the remote server";

        public string syntaxHelper { get; } = "playsound [FileName.wav]";

        public bool isLocal { get; } = false;

        public List<string> validArguments { get; } = new List<string>()
        {
            "?"
        };


        public void Process(List<string> args)
        {
            var result = ClientCommandsManager.networkManager.ReadLine();

            switch (result)
            {
                case "InvalidFormat":
                    ColorTools.WriteCommandError("Wrong format, expected .wav file");
                    break;
                case "FileError":
                    ColorTools.WriteCommandError("File not found on the remote server");
                    break;
                default:
                    ColorTools.WriteCommandSuccess("Playing sound");
                    break;
            }
        }
    }
}
