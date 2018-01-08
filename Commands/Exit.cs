using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Commands
{
    internal class Exit : ICommand
    {
        public string name { get; } = "exit";

        public string description { get; } = "Stop the connection\nThe server remains active, allowing further connections if wanted";

        public string syntaxHelper { get; } = "exit";

        public bool isLocal { get; } = true;

        public List<List<Type>> validArguments { get; } = null;

        public List<string> clientFlags { get; } = new List<string>();

        public List<string> savedData { get; set; }


        public CommandsManager.PreProcessResult PreProcessCommand(List<string> args)
        {
            throw new NotImplementedException();
        }

        public void ClientMethod(List<string> args)
        {
            throw new ExitException();
        }

        public void ServerMethod(List<string> args)
        {
            throw new NotImplementedException();
        }
    }
}
