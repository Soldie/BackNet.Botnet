using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using KeyLogger;
using Shared;

namespace Commands
{
    public static class CommandsManager
    {
        /// <summary>
        /// List of all Command classes
        /// </summary>
        public static List<ICommand> commandList;


        /// <summary>
        /// Constructor, fill commandList with the classes implementing the ICommand interface
        /// </summary>
        static CommandsManager()
        {
            var type = typeof(ICommand);

            commandList = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => type.IsAssignableFrom(t) && !t.IsInterface)
                .Select(t => (ICommand)Activator.CreateInstance(t))
                .ToList();
        }


        /// <summary>   
        /// Return the Command whose name was given as a parameter, or null if it doesn't exist
        /// </summary>
        /// <param name="commandName">Command name to find</param>
        /// <returns>Found ICommand or null</returns>
        public static ICommand GetCommandByName(string commandName) => SearchCommand(s => s.name == commandName);


        /// <summary>
        /// Search a command in the commandList matching the given predicate
        /// </summary>
        /// <param name="predicate">Predicate to use to find the command</param>
        /// <returns>Found ICommand or null</returns>
        static ICommand SearchCommand(Func<ICommand, bool> predicate)
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
        /// Check if the given arguments match match at least one validArguments combinaison of the given Command class
        /// </summary>
        /// <param name="command">Invoked Command</param>
        /// <param name="arguments">Passed arguments</param>
        /// <returns>Correct syntax boolean</returns>
        public static bool CheckCommandSyntax(ICommand command, List<string> arguments)
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

                if(!error)
                    return true;
            }

            return false;
        }


        /// <summary>
        /// Display an help message for the given Command on the client console,
        /// the help message is composed of the Command's description and its syntaxHelper
        /// </summary>
        /// <param name="command">Command to show the help for</param>
        public static void ShowCommandHelp(ICommand command)
        => ColorTools.WriteMessage($"{command.description}\nSyntax : {command.syntaxHelper}");
        

        /// <summary>
        /// Display an help message for all the Commands on the client console, calls ShowCommandHelp
        /// </summary>
        public static void ShowGlobalHelp()
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
        /// Find the ICommand keylogger in the commandList and give it the KeyLoggerManager instance
        /// This method should only be called by the MainWindow
        /// </summary>
        /// <param name="manager">KeyLoggerManager instance</param>
        public static void PassKeyloggerManagerInstance(KeyLoggerManager manager)
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
        public static List<string> GetSplittedCommand(string commandString)
        {
            return Regex.Matches(commandString, @"[\""].+?[\""]|[^ ]+")
                .Cast<Match>()
                .Where(m => m.Value != "\"")
                .Select(m => m.Value[0] == '\"' && m.Value[m.Length - 1] == '\"' ? m.Value.Substring(1, m.Value.Length - 2) : m.Value)
                .ToList();
        }
    }
}
