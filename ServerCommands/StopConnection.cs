using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;

namespace ServerCommands
{
    internal class StopConnection : ICommand
    {
        public string name { get; } = "stopconnection";


        public void Process(List<string> args) => throw new ExitException();
    }
}
