using System;
using System.Collections.Generic;
using System.IO;

namespace Client.Commands.Core
{
    internal class LPWD : IClientCommand
    {
        public string name { get; set; } = "lpwd";

        public string description { get; set; } = "Display the local current working directory";

        public string syntaxHelper { get; set; } = "lpwd";

        public bool isLocal { get; set; } = true;

        public List<string> validArguments { get; set; } = null;


        public void Process(List<string> args) => Console.WriteLine(Directory.GetCurrentDirectory());
    }
}
