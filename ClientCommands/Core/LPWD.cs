using System;
using System.Collections.Generic;
using System.IO;

namespace ClientCommands
{
    internal class LPWD : ICommand
    {
        public string name { get; set; } = "lpwd";

        public string description { get; set; } = "Display the local current working directory";

        public string syntaxHelper { get; set; } = "lpwd";

        public bool isLocal { get; set; } = true;

        public List<string> validArguments { get; set; } = null;


        public bool PreProcess(List<string> args)
        {
            throw new NotImplementedException();
        }

        public void Process(List<string> args) => Console.WriteLine(Directory.GetCurrentDirectory());
    }
}
