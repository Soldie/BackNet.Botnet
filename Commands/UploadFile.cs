using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commands
{
    public class UploadFile : ICommand
    {
        public string name { get; } = "uploadfile";

        public bool isLocal { get; } = false;

        public int argCount { get; } = 1;

        public List<string> clientFlags { get; } = new List<string>()
        {
            "{UploadFile:init}"
        };


        public bool PreProcessCommand(string command)
        {
            throw new NotImplementedException();
        }

        public void ClientMethod()
        {
            throw new NotImplementedException();
        }

        public void ServerMethod()
        {
            throw new NotImplementedException();
        }
    }
}
