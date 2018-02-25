using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Shared
{
    public abstract class CommandsManager<T>
    {
        public static NetworkManager networkManager { get; set; }

        /// <summary>
        /// List of all instanciated command classes
        /// </summary>
        public List<ICommand> commandList;


        /// <summary>
        /// Constructor, fill commandList with the classes implementing the ICommand interface.
        /// Instantiate the network manager.
        /// </summary>
        protected CommandsManager(NetworkManager networkManager)
        {
            var type = typeof(ICommand);

            commandList = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => type.IsAssignableFrom(t) && !t.IsInterface)
                .Select(t => (ICommand)Activator.CreateInstance(t))
                .ToList();

            CommandsManager<T>.networkManager = networkManager;
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
