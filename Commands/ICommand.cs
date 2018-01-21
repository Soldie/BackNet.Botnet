using System;
using System.Collections.Generic;

namespace Commands
{
    /// <summary>
    /// Interface 
    /// </summary>
    public interface ICommand
    {
        #region Variables
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
        /// List of all possible arguments type combinaisons (sorted), null if none
        /// </summary>
        List<List<Type>> validArguments { get; }
        #endregion Variables


        #region Methods
        /// <summary>
        /// Possibly check for files or do some preparations
        /// </summary>
        /// <param name="args">Args passed with the command</param>
        /// <returns>Result of the operation</returns>
        bool PreProcessCommand(List<string> args);


        /// <summary>
        /// Method executed by the client
        /// </summary>
        /// <param name="args">Args passed with the command</param>
        void ClientMethod(List<string> args);


        /// <summary>
        /// Method executed by the server
        /// </summary>
        /// <param name="args">Args passed with the command</param>
        void ServerMethod(List<string> args);
        #endregion Methods
    }
}
