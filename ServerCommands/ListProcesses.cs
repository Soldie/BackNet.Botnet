using System;
using System.Collections.Generic;
using System.Linq;
using Shared;

namespace ServerCommands
{
    internal class ListProcesses : ICommand
    {
        public string name { get; } = "ps";


        public void Process(List<string> args)
        {
            var processlist = System.Diagnostics.Process.GetProcesses();
            var processesInfos = new List<Tuple<string, string>>();
            foreach (var process in processlist.ToList().Where(x => x.ProcessName != "svchost").OrderBy(x => x.ProcessName).ThenBy(x => x.Id))
            {
                processesInfos.Add(new Tuple<string, string>(process.Id.ToString(), process.ProcessName));
            }

            ServerCommandsManager.networkManager.WriteLine(ServerCommandsManager.TableDisplay(processesInfos));
            ServerCommandsManager.networkManager.WriteLine("{end}");
        }
    }
}
