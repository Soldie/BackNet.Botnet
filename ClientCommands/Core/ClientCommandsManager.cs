using System;
using System.Collections.Generic;
using System.Linq;
using Shared;

namespace ClientCommands
{
    public class ClientCommandsManager : CommandsManager<ClientCommandsManager>
    {
        public ClientCommandsManager(NetworkManager networkManager) : base(networkManager) { }


        /// <summary>
        /// Check if the given arguments match at least one validArguments combinaison of the given IClientCommand class
        /// </summary>
        /// <param name="command">Invoked Command</param>
        /// <param name="arguments">Passed arguments</param>
        /// <returns>Correct syntax boolean</returns>
        public bool CheckCommandSyntax(IClientCommand command, List<string> arguments)
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
