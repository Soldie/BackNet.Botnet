using System;
using System.Collections.Generic;
using System.IO;
using Server.Commands.Core;
using Shared;

namespace Server.Commands
{
    internal class Upload : ICommand
    {
        public string name { get; } = "upload";
        
        
        public void Process(List<string> args)
        {
            try
            {
                using (var fs = new FileStream(args[1], FileMode.Create))
                {
                    ServerCommandsManager.networkManager.NetworkStreamToStream(fs);
                }

                ServerCommandsManager.networkManager.WriteLine("Success");
            }
            catch (Exception)
            {
                // Delete the partially created file
                File.Delete(args[1]);
                ServerCommandsManager.networkManager.WriteLine("Error");
            }
        }
    }
}
