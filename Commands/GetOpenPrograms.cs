using NetworkManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Commands
{
    internal class GetOpenPrograms : ICommand
    {
        public string name { get; } = "getopenprograms";

        public string description { get; } = "Displays a list of all open programs on the remote computer";

        public string syntaxHelper { get; } = "getopenprograms";

        public bool isLocal { get; } = false;

        public List<List<Type>> validArguments { get; } = null;

        public List<string> clientFlags { get; } = new List<string>()
        {
            "{GetOpenPrograms:init}"
        };

        public List<string> savedData { get; set; }


        public CommandsManager.PreProcessResult PreProcessCommand(List<string> args)
        {
            throw new NotImplementedException();
        }

        public void ClientMethod(List<string> args)
        {
            Console.WriteLine(GlobalNetworkManager.ReadLine());
        }

        public void ServerMethod(List<string> args)
        {
            GlobalNetworkManager.WriteLine(clientFlags[0]);
            
            var processlist = Process.GetProcesses();
            var processesInfos = new List<Tuple<string, string>>();
            foreach (var process in processlist)
            {
                if (!string.IsNullOrEmpty(process.MainWindowTitle))
                {
                    processesInfos.Add(new Tuple<string, string>(process.Id.ToString(), process.ProcessName));
                }
            }
            
            GlobalNetworkManager.WriteLine(CommandsManager.TableDisplay(processesInfos));
        }
    }
}
