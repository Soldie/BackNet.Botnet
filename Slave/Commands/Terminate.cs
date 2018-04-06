using System.Collections.Generic;
using Shared;

namespace Slave.Commands
{
    internal class Terminate : ICommand
    {
        public string name { get; } = "terminate";


        public void Process(List<string> args) => throw new StopSlaveException();
    }
}
