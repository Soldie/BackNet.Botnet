using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using Server.Commands.Core;
using Shared;

namespace Server.Commands
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
