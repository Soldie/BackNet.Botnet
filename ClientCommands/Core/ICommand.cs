using System.Collections.Generic;

namespace ClientCommands
{
    public interface ICommand
    {
        /// <summary>
        /// Name of the command
        /// </summary>
        string name { get; }


        /// <summary>
        /// Complete description of the command,
        /// doesn't include the syntax, it will be added with the syntaxHelper variable
        /// </summary>
        string description { get; }


        /// <summary>
        /// Correct syntax to use (example: upload [filePath])
        /// Separate each possible syntax with a '\n'
        /// </summary>
        string syntaxHelper { get; }


        /// <summary>
        /// Is the command only executed locally or also from the server side ?
        /// </summary>
        bool isLocal { get; }


        /// <summary>
        /// List of all possible arguments combinaisons, null if none.
        /// Explicitly name the argument, or '?' for a string, or '0' for an integer
        /// </summary>
        List<string> validArguments { get; }


        /// <summary>
        /// Method executed by the client
        /// </summary>
        /// <param name="args">Args passed with the command</param>
        void Process(List<string> args);
    }
}
