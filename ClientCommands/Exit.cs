using Shared;
using System;
using System.Collections.Generic;

namespace ClientCommands
{
    internal class Exit : ICommand
    {
        public string name { get; } = "exit";

        public string description { get; } = "Stop the connection\nThe server remains active, allowing further connections if wanted";

        public string syntaxHelper { get; } = "exit";

        public bool isLocal { get; } = true;

        public List<string> validArguments { get; } = null;


        public void Process(List<string> args)
        {
            throw new ExitException();
        }
    }
}
