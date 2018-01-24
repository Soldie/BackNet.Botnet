using Shared;
using System;
using System.Collections.Generic;

namespace Commands
{
    internal class Terminate : ICommand
    {
        public string name { get; } = "terminate";

        public string description { get; } = "Close the connection and exit the server application";

        public string syntaxHelper { get; } = "terminate";

        public bool isLocal { get; } = false;

        public List<string> validArguments { get; } = null;


        public bool PreProcessCommand(List<string> args)
        {
            throw new NotImplementedException();
        }

        public void ClientMethod(List<string> args) => throw new ExitException();

        public void ServerMethod(List<string> args) => throw new StopServerException();
    }
}
