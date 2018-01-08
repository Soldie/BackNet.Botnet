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

        public List<string> clientFlags { get; } = new List<string>()
        {
            "{sysinfo:init}"
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
            };

            GlobalNetworkManager.WriteLine(TableDisplay(infos));
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

        string TableDisplay(IReadOnlyCollection<Tuple<string, string>> data)
        {
            var result = "";
            var longestPrefix = data.Select(t => t.Item1.Length).Max();
            var longestValue = data.Select(t => t.Item2.Length).Max();
            var longestLine = longestPrefix + longestValue + 7;
            var horizontalDelimiter = $"+{new string('-', longestLine - 2)}+";

            result += horizontalDelimiter + "\n";
            foreach (var tuple in data)
            {
                var spaces = new string(' ', longestPrefix - tuple.Item1.Length);
                result += $"| {tuple.Item1}{spaces} |";

                spaces = new string(' ', longestValue - tuple.Item2.Length);
                result += $" {tuple.Item2}{spaces} |\n";
            }
            result += horizontalDelimiter;

            return result;
        }
    }
}
