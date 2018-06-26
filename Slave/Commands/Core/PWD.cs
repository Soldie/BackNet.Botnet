using Shared;
using System.Collections.Generic;
using System.IO;
using Slave.Core;

namespace Slave.Commands.Core
{
    internal class PWD : ICommand
    {
        public string name { get; set; } = "pwd";

        public void Process(List<string> args)
            => SlaveNetworkManager.GetInstance().WriteLine(Directory.GetCurrentDirectory());
    }
}
