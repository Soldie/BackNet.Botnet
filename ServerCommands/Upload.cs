using Shared;
using System;
using System.Collections.Generic;
using System.IO;

namespace ServerCommands
{
    internal class Upload : ICommand
    {
        public string name { get; } = "upload";
        
        
        public void Process(List<string> args)
        {
            var dataLength = int.Parse(ServerCommandsManager.networkManager.ReadLine());

            try
            {
                using (var fs = new FileStream(args[1], FileMode.Create))
                {
                    ServerCommandsManager.networkManager.NetworkStreamToStream(fs, dataLength);
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
