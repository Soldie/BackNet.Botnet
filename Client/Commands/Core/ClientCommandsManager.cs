using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Client.AdvancedConsole;
using Shared;

namespace Client.Commands.Core
{
    public class ClientCommandsManager : GlobalCommandsManager
    {
        // Call base constructor and fill AdvancedConsole AutoCOmpletionManager command list
        public ClientCommandsManager(GlobalNetworkManager networkManager) : base(networkManager)
        {
            AutoCompletionManager.commands = commandList.Select(x => x.name).ToList();
        }


        /// <summary>
        /// Check if the given arguments match at least one validArguments combinaison of the given IClientCommand class.
        /// The given command string can be modified to avoid disclosing user's infos.
        /// </summary>
        /// <param name="command">Invoked Command</param>
        /// <param name="arguments">Passed arguments</param>
        /// <param name="commandString">Command from user input</param>
        /// <returns>Correct syntax boolean</returns>
        public bool CheckCommandSyntax(IClientCommand command, List<string> arguments, ref string commandString)
        {
            if (command.validArguments == null ||
                (arguments.Count == 0 && command.validArguments.Any(string.IsNullOrEmpty)))
            {
                return arguments.Count == 0;
            }

            foreach (var validSyntax in command.validArguments)
            {
                var splittedValidSyntax = validSyntax.Split(' ');
                if (splittedValidSyntax.Length != arguments.Count)
                {
                    continue;
                }

                var error = false;
                for (var i = 0; i < splittedValidSyntax.Length; i++)
                {
                    if (splittedValidSyntax[i] == "?" || splittedValidSyntax[i] == "?*") continue;

                    if (splittedValidSyntax[i] == "0")
                    {
                        if (!int.TryParse(arguments[i], out int dummy))
                        {
                            error = true;
                            break;
                        }
                    }
                    else if (splittedValidSyntax[i] != arguments[i])
                    {
                        error = true;
                        break;
                    }
                }

                if (!error)
                {
                    var splittedString = commandString.Split(' ');
                    var list = new List<string> { splittedString[0] };

                    // Remove personnal informations (marked with '*' mark)
                    for (int i = 0; i < splittedValidSyntax.Length; i++)
                    {
                        list.Add(splittedValidSyntax[i] == "?*" ? "*" : splittedString[i + 1]);
                    }
                    commandString = list.Aggregate((x, y) => $"{x} {y}");
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Display an help message for the given IClientCommand on the client console,
        /// the help message is composed of the command's description and its syntaxHelper
        /// </summary>
        /// <param name="command">Command to show the help for</param>
        public void ShowCommandHelp(IClientCommand command)
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
                ShowCommandHelp((IClientCommand)command);
            }
        }
    }
}
