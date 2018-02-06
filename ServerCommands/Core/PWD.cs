using System;
using System.Collections.Generic;
using System.IO;
using Shared;

namespace ServerCommands
{
    internal class PWD : ICommand
    {
        public string name { get; set; } = "pwd";


        public void Process(List<string> args)
            => ServerCommandsManager.networkManager.WriteLine(Directory.GetCurrentDirectory());
    }
}
