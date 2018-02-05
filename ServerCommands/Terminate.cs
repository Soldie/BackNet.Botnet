using Shared;
using System;
using System.Collections.Generic;

namespace ServerCommands
{
    internal class Terminate : ICommand
    {
        public string name { get; } = "terminate";


        public void Process(List<string> args) => throw new StopServerException();
    }
}
