using System.Collections.Generic;
using Client.Commands.Core;
using Shared;

namespace Client.Commands
{
    internal class Terminate : IClientCommand
    {
        public string name { get; } = "terminate";

        public string description { get; } = "Close the connection and exit the server application";

        public bool isLocal { get; } = false;

        public List<string> validArguments { get; } = null;


        public void Process(List<string> args) => throw new ExitException();
    }
}
