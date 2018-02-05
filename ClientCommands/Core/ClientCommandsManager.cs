using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ClientCommands
{
    public class ClientCommandsManager
    {
        /// <summary>
        /// List of all command classes
        /// </summary>
        public List<ICommand> commandList;

        internal static NetworkManager networkManager { get; set; }


        /// <summary>
        /// Constructor, fill commandList with the classes implementing the ICommand interface
        /// </summary>
        public ClientCommandsManager(NetworkManager networkManager)
        {
            var type = typeof(ICommand);

            commandList = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => type.IsAssignableFrom(t) && !t.IsInterface)
                .Select(t => (ICommand)Activator.CreateInstance(t))
                .ToList();

            ClientCommandsManager.networkManager = networkManager;
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
        /// Check if the given arguments match at least one validArguments combinaison of the given ICommand class
        /// </summary>
        /// <param name="command">Invoked Command</param>
        /// <param name="arguments">Passed arguments</param>
        /// <returns>Correct syntax boolean</returns>
        public bool CheckCommandSyntax(ICommand command, List<string> arguments)
        {
            if (command.validArguments == null || command.validArguments.Any(string.IsNullOrEmpty))
            {
                return arguments.Count == 0;
            }

            foreach (var validSyntax in command.validArguments)
            {
                var slittedValidSyntax = validSyntax.Split(' ');
                if (slittedValidSyntax.Length != arguments.Count)
                {
                    continue;
                }

                var error = false;
                for (var i = 0; i < slittedValidSyntax.Length; i++)
                {
                    if (slittedValidSyntax[i] == "?") continue;

                    if (slittedValidSyntax[i] == "0")
                    {
                        if (!int.TryParse(arguments[i], out int dummy))
                        {
                            error = true;
                            break;
                        }
                    }
                    else if (slittedValidSyntax[i] != arguments[i])
                    {
                        error = true;
                        break;
                    }
                }

                if (!error)
                    return true;
            }

            return false;
        }


        /// <summary>
        /// Display an help message for the given ICommand on the client console,
        /// the help message is composed of the command's description and its syntaxHelper
        /// </summary>
        /// <param name="command">Command to show the help for</param>
        public void ShowCommandHelp(ICommand command)
        => ColorTools.WriteMessage($"{command.description}\nSyntax : {command.syntaxHelper}");


        /// <summary>
        /// Display an help message for all the commands on the client console, calls ShowCommandHelp
        /// </summary>
        public void ShowGlobalHelp()
        {
            ColorTools.WriteMessage("+-----------------+\n|   Global help   |\n+-----------------+");
            foreach (var command in commandList)
            {
                Console.WriteLine(" ");
                ColorTools.WriteMessage($" - {command.name}");
                ShowCommandHelp(command);
            }
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
