using System;
using System.Collections.Generic;
using System.IO;

namespace Commands
{
    internal class LPWD : ICommand
    {
        public string name { get; set; } = "lpwd";

        public string description { get; set; } = "Display the local current working directory";

        public string syntaxHelper { get; set; } = "lpwd";

        public bool isLocal { get; set; } = true;

        public List<string> validArguments { get; set; } = null;


        public bool PreProcessCommand(List<string> args)
        {
            throw new NotImplementedException();
        }

        public void ClientMethod(List<string> args) => Console.WriteLine(Directory.GetCurrentDirectory());

        public void ServerMethod(List<string> args)
        {
            throw new NotImplementedException();
        }
    }
}
