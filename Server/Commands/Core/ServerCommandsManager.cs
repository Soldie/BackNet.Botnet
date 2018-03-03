using System;
using System.Collections.Generic;
using System.Linq;
using Server.Keylogger;
using Shared;

namespace Server.Commands.Core
{
    public class ServerCommandsManager : CommandsManager
    {
        public ServerCommandsManager(NetworkManager networkManager) : base(networkManager) { }


        /// <summary>
        /// Find the keylogger ICommand in the commandList and give it the KeyLoggerManager instance
        /// This method should only be called by the MainWindow
        /// </summary>
        /// <param name="manager">KeyLoggerManager instance</param>
        public void PassKeyloggerManagerInstance(KeyLoggerManager manager)
        {
            var keyloggerCommandInstance = (KeyLogger)commandList.Find(x => x.name == "keylogger");
            keyloggerCommandInstance.GetKeyLoggerManagerInstance(manager);
        }


        /// <summary>
        /// Produce a string displaying a table from a list of Tuple(string, string)
        /// </summary>
        /// <param name="data">List of tuple to process</param>
        /// <returns>Table as a string</returns>
        internal static string TableDisplay(IReadOnlyCollection<Tuple<string, string>> data)
        {
            var result = "";
            var longestPrefix = data.Select(t => t.Item1.Length).Max();
            var longestValue = data.Select(t => t.Item2.Length).Max();
            var horizontalDelimiter = $"+{new string('-', longestPrefix + 2)}+{new string('-', longestValue + 2)}+";

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
