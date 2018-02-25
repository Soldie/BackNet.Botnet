using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;

namespace ClientCommands
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
