using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Shared
{
    public abstract class GlobalCommandsManager
    {
        public static GlobalNetworkManager networkManager { get; set; }

        /// <summary>
        /// List of all instanciated command classes
        /// </summary>
        public List<ICommand> commandList;

        /// <summary>
        /// Constructor, fill commandList with the classes implementing the ICommand interface.
        /// Instantiate the network manager.
        /// </summary>
        protected GlobalCommandsManager(GlobalNetworkManager networkManager)
        {
            var type = typeof(ICommand);

            commandList = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => type.IsAssignableFrom(t) && !t.IsInterface)
                .Select(t => (ICommand)Activator.CreateInstance(t))
                .OrderBy(x => x.name)
                .ToList();

            GlobalCommandsManager.networkManager = networkManager;
        }

        /// <summary>
        /// Return the ICommand whose name was given as a parameter, or null if it doesn't exist
        /// </summary>
        /// <param name="commandName">ICommand name to find</param>
        /// <returns>Found ICommand or null</returns>
        public ICommand GetCommandByName(string commandName)
        {
            ICommand foundCommand;
            try
            {
                foundCommand = commandList.First(s => s.name == commandName);
            }
            catch (Exception)
            {
                foundCommand = null;
            }

            return foundCommand;
        }

        /// <summary>
        /// Split the given string by using the space delimiter,
        /// this takes into account double quotes (ex : for file paths) and includes those into the result
        /// </summary>
        /// <param name="commandString">String to process</param>
        /// <returns>List of string</returns>
        public List<string> GetSplittedCommandWithQuotes(string commandString)
        {
            return Regex.Matches(commandString, @"[\""].+?[\""]|[^ ]+")
                .Cast<Match>().Select(x => x.Value).ToList();
        }

        /// <summary>
        /// Split the given string by using the space delimiter,
        /// this takes into account double quotes (ex : for file paths) but doesn't include those into the result
        /// </summary>
        /// <param name="commandString">String to process</param>
        /// <returns>List of string</returns>
        public List<string> GetSplittedCommandWithoutQuotes(string commandString)
        {
            return Regex.Matches(commandString, @"[\""].+?[\""]|[^ ]+")
                .Cast<Match>()
                .Where(m => m.Value != "\"")
                .Select(m =>
                    m.Value[0] == '\"' && m.Value[m.Length - 1] == '\"'
                        ? m.Value.Substring(1, m.Value.Length - 2)
                        : m.Value)
                .ToList();
        }
    }
}
