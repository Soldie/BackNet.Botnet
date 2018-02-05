using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ServerCommands
{
    internal class GetOpenPrograms : ICommand
    {
        public string name { get; } = "getopenprograms";


        public void Process(List<string> args)
        {
            var processlist = System.Diagnostics.Process.GetProcesses();
            var processesInfos = new List<Tuple<string, string>>();
            foreach (var process in processlist)
            {
                if (!string.IsNullOrEmpty(process.MainWindowTitle))
                {
                    processesInfos.Add(new Tuple<string, string>(process.Id.ToString(), process.ProcessName));
                }
            }

            ServerCommandsManager.networkManager.WriteLine(ServerCommandsManager.TableDisplay(processesInfos));
            ServerCommandsManager.networkManager.WriteLine("{end}");
        }
    }
}
