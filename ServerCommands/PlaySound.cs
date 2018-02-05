using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Media;

namespace ServerCommands
{
    internal class PlaySound : ICommand
    {
        public string name { get; } = "playsound";


        public void Process(List<string> args)
        {
            var filename = args[0];

            if (!File.Exists(filename))
            {
                ServerCommandsManager.networkManager.WriteLine("FileError");
                return;
            }
            
            try
            {
                var player = new SoundPlayer(filename);
                player.Play();
                ServerCommandsManager.networkManager.WriteLine("OK");
            }
            catch (InvalidOperationException)
            {
                ServerCommandsManager.networkManager.WriteLine("InvalidFormat");
            }
        }
    }
}
