using System;
using System.Collections.Generic;
using System.Linq;
using NetworkManager;

namespace Commands
{
    internal class SysInfo : ICommand
    {
        public string name { get; } = "sysinfo";

        public string description { get; } = "Display the remote system's informations";

        public string syntaxHelper { get; } = "sysinfo";

        public bool isLocal { get; } = false;

        public List<List<Type>> validArguments { get; } = new List<List<Type>>()
        {
            new List<Type>()
        };


        public bool PreProcessCommand(List<string> args)
        {
            throw new NotImplementedException();
        }

        public void ClientMethod(List<string> args)
        {
            var data = "";
            while (data != "{end}")
            {
                if (data != "")
                    Console.WriteLine(data);
                data = GlobalNetworkManager.ReadLine();
            }
        }

        public void ServerMethod(List<string> args)
        {
            var infos = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("Machine name", Environment.MachineName),
                new Tuple<string, string>("Current user's name", Environment.UserName),
                new Tuple<string, string>("Current user's domain name", Environment.UserDomainName),
                new Tuple<string, string>("Os version", $"{Environment.OSVersion} , {(Environment.Is64BitOperatingSystem ? "64" : "32")}bit operating system"),
                new Tuple<string, string>(".NET version", Environment.Version.ToString()),
                new Tuple<string, string>("Number of processors", Environment.ProcessorCount.ToString()),
                new Tuple<string, string>("Machine uptime", TimespanAsString(TimeSpan.FromMilliseconds(Environment.TickCount))),
                new Tuple<string, string>("Drives", Environment.GetLogicalDrives().Aggregate((current, drive) => $"{current} , " + drive))
                // TODO :
                // Mac adress
                // Ip adress
                // IsCurrentUserAdmin
            };

            GlobalNetworkManager.WriteLine(CommandsManager.TableDisplay(infos));
            GlobalNetworkManager.WriteLine("{end}");
        }

        string TimespanAsString(TimeSpan t)
        {
            string result;

            if (t.TotalMinutes < 1.0)
            {
                result = $"{t.Seconds}s";
            }
            else if (t.TotalHours < 1.0)
            {
                result = $"{t.Minutes}m:{t.Seconds:D2}s";
            }
            else
            {
                result = $"{(int)t.TotalHours}h:{t.Minutes:D2}m:{t.Seconds:D2}s";
            }

            return result;
        }
    }
}
