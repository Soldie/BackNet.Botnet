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
                
                for (var i = 0; i < splittedValidSyntax.Length; i++)
                {
                    if (splittedValidSyntax[i].Contains("?") || splittedValidSyntax[i].Contains("?*")) continue;

                    if (splittedValidSyntax[i] == "0")
                    {
                        if (!int.TryParse(arguments[i], out int dummy))
                        {
                            break;
                        }
                    }
                    else if (splittedValidSyntax[i] != arguments[i])
                    {
                        break;
                    }
                }
                
                var splittedString = commandString.Split(' ');
                var list = new List<string> { splittedString[0] };

                // Remove private informations (marked with '*' mark)
                for (int i = 0; i < splittedValidSyntax.Length; i++)
                {
                    list.Add(splittedValidSyntax[i].Contains("?*") ? "*" : splittedString[i + 1]);
                }
                commandString = list.Aggregate((x, y) => $"{x} {y}");
                return true;
            }

            return false;
        }


        /// <summary>
        /// Display an help message for the given IClientCommand on the client console
        /// </summary>
        /// <param name="command">Command to show the help for</param>
        public void ShowCommandHelp(IClientCommand command)
        {
            var help = new StringBuilder();
            help.AppendLine(command.description);
            
            help.Append("Syntax: ");

            if (command.validArguments == null) help.Append(command.name);

            for (int i = 0; i < command.validArguments?.Count; i++)
            {
                if (i != 0)
                {
                    help.Append(new string(' ', 8));
                }
                var syntax = command.name + ' ' + command.validArguments[i].Replace("?", "").Replace("*", "").Replace(":", "");

                if (i == command.validArguments.Count - 1)
                {
                    // Don't add a line return for the last syntax
                    help.Append(syntax);
                }
                else
                {
                    help.AppendLine(syntax);
                }
            }

            ColorTools.WriteMessage(help.ToString());
        } 


        /// <summary>
        /// Display an help message for all the commands on the client console, calls ShowCommandHelp
        /// </summary>
        public void ShowGlobalHelp()
        {
            foreach (var command in commandList)
            {
                Console.WriteLine(" ");
                ColorTools.WriteMessage($"-- {command.name} --");
                ShowCommandHelp((IClientCommand)command);
            }
        }
    }
}
