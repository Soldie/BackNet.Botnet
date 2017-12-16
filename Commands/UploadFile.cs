using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;

namespace Commands
{
    internal class UploadFile : ICommand
    {
        #region Variables
        public string name { get; } = "uploadfile";


        public string description { get; } = "Upload a file to the server";


        public string syntaxHelper { get; } = "uploadfile [filename]";


        public bool isLocal { get; } = false;


        public List<List<Type>> validArguments { get; } = new List<List<Type>>()
        {
            new List<Type>()
            {
                typeof(string)
            }
        };


        public List<string> clientFlags { get; } = new List<string>()
        {
            "{UploadFile:init}"
        };
        #endregion Variables
        

        #region Methods
        public bool PreProcessCommand(List<string> args)
        {
            var result = File.Exists(args[1]);
            if (!result)
            {
                ColorTools.WriteCommandError("The specified file doesn't exist");
            }

            return result;
        }


        public void ClientMethod(List<string> args)
        {
            throw new NotImplementedException();
        }


        public void ServerMethod()
        {
            throw new NotImplementedException();
        }
        #endregion Methods
    }
}
