using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkManager;

namespace Commands
{
    internal class Terminate : ICommand
    {
        public string name { get; } = "terminate";

        public string description { get; } = "Close the connection and exit the server application";

        public string syntaxHelper { get; } = "terminate";

        public bool isLocal { get; } = false;

        public List<List<Type>> validArguments { get; } = null;

        public List<string> clientFlags { get; } = new List<string>()
        {
            "{Terminate}"
        };

        public List<string> savedData { get; set; }


        public CommandsManager.PreProcessResult PreProcessCommand(List<string> args)
        {
            return CommandsManager.PreProcessResult.NoClientProcess;
        }

        public void ClientMethod(List<string> args)
        {
            throw new ExitException();
        }

        public void ServerMethod(List<string> args)
        {
            GlobalNetworkManager.WriteLine(clientFlags[0]);
            throw new StopServerException();
        }
    }
}
