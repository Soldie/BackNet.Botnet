using System.Collections.Generic;
using Shared;

namespace ServerCommands
{
    internal class Terminate : ICommand
    {
        public string name { get; } = "terminate";


        public void Process(List<string> args) => throw new StopServerException();
    }
}
