using System.Collections.Generic;
using Client.Commands.Core;
using Shared;

namespace Client.Commands
{
    internal class StopConnection : IClientCommand
    {
        public string name { get; } = "stopconnection";

        public string description { get; } = "Stop the infected host from trying to connect to you.\nIt will still connect back to the master botnet server to get new commands.";

        public string syntaxHelper { get; } = "stopconnection";

        public bool isLocal { get; } = false;

        public List<string> validArguments { get; } = null;


        public void Process(List<string> args) => throw new ExitException();
    }
}
