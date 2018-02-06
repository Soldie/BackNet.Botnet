using System.Collections.Generic;
using Shared;

namespace ClientCommands
{
    internal class Terminate : IClientCommand
    {
        public string name { get; } = "terminate";

        public string description { get; } = "Close the connection and exit the server application";

        public string syntaxHelper { get; } = "terminate";

        public bool isLocal { get; } = false;

        public List<string> validArguments { get; } = null;


        public void Process(List<string> args) => throw new ExitException();
    }
}
