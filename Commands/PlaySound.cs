using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Media;

namespace Commands
{
    internal class PlaySound : ICommand
    {
        public string name { get; } = "playsound";

        public string description { get; } = "Play a .wav audio file on the remote server";

        public string syntaxHelper { get; } = "playsound [FileName.wav]";

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
            var result = CommandsManager.networkManager.ReadLine();

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

        public void ServerMethod(List<string> args)
        {
            var filename = args[0];

            if (!File.Exists(filename))
            {
                CommandsManager.networkManager.WriteLine("FileError");
                return;
            }
            
            try
            {
                var player = new SoundPlayer(filename);
                player.Play();
                CommandsManager.networkManager.WriteLine("OK");
            }
            catch (InvalidOperationException)
            {
                CommandsManager.networkManager.WriteLine("InvalidFormat");
            }
        }
    }
}
