using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Constructor, fill commandList with the Command classes
        /// </summary>
        static CommandsManager()
        {
            var type = typeof(ICommand);

            commandList = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsInterface)
                .Cast<ICommand>()
                .ToList();
        }


        /// <summary>
        /// Return the Command whose name was given as a parameter, or null if it doesn't exist
        /// </summary>
        /// <param name="commandName">Command name to find</param>
        /// <returns>Found Command or null</returns>
        public static ICommand GetCommandByName(string commandName)
            => commandList.First(s => s.name == commandName);


        /// <summary>
        /// Return the Command whose clientFlags list contains the given flag
        /// </summary>
        /// <param name="flag">Flag to find</param>
        /// <returns>Found Command or null</returns>
        public static ICommand GetCommandByFlag(string flag)
            => commandList.First(s => s.clientFlags.Contains(flag));


        /// <summary>
        /// Check if the given arguments match match at least one validArguments combinaison of the given Command class
        /// </summary>
        /// <param name="command">Invoked Command</param>
        /// <param name="arguments">Passed arguments</param>
        /// <returns>Correct syntax boolean</returns>
        public static bool CheckCommandSyntax(ICommand command, List<string> arguments)
        {
            int dummy;  // Only for use of int.TryParse()
            var argsType = arguments.Select(s => int.TryParse(s, out dummy) ? typeof(int) : typeof(string)).ToList();

            return commandList.Count(w => w.validArguments.All(a => a.SequenceEqual(argsType))) != 0;
        }


        /// <summary>
        /// Display an help message for the given Command on the client console,
        /// the help message is composed of the Command's description and its syntaxHelper
        /// </summary>
        /// <param name="command">Command to show the help for</param>
        public static void ShowCommandHelp(ICommand command)
        {
            ColorTools.WriteMessage(command.description);
            ColorTools.WriteMessage($"\nSyntax : {command.syntaxHelper}");
        }


        /// <summary>
        /// Display an help message for all the Commands on the client console, calls ShowCommandHelp
        /// </summary>
        public static void ShowGlobalHelp()
        {
            ColorTools.WriteMessage("--Global help--");
            foreach (var command in commandList)
            {
                ShowCommandHelp(command);
            }
        }
    }
}
