using Shared;
using System.Collections.Generic;
using System.IO;
using Slave.Core;

namespace Slave.Commands.Core
{
    internal class CD : ICommand
    {
        public string name { get; set; } = "cd";

        public void Process(List<string> args)
        {
            if (Directory.Exists(args[0]))
            {
                Directory.SetCurrentDirectory(args[0]);
                SlaveNetworkManager.GetInstance().WriteLine(Directory.GetCurrentDirectory());
            }
            else
            {
                SlaveNetworkManager.GetInstance().WriteLine("KO");
            }
        }
    }
}
