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
        /// if the command accepts multiple names, write them separated by '|' (ex: 'ls|dir')
        /// </summary>
        string name { get; }


        /// <summary>
        /// Complete description of the command,
        /// doesn't include the syntax, it will be added with the syntaxHelper variable
        /// </summary>
        string description { get; }


        /// <summary>
        /// Correct syntax to use (example: upload [filePath])
        /// </summary>
        string syntaxHelper { get; }


        /// <summary>
        /// Is the command only executed locally or also from the server side ?
        /// </summary>
        bool isLocal { get; }


        /// <summary>
        /// List of all possible arguments type combinaisons (sorted), null if none
        /// </summary>
        List<List<Type>> validArguments { get; }    // maybe generate dynamically when compiling, with all ClientMethod()(s) parameters


        /// <summary>
        /// Flags sent by the server to init a client-side function
        /// </summary>
        List<string> clientFlags { get; }
        #endregion Variables


        #region Methods
        /// <summary>
        /// Possibly check for files presence or do some preparations
        /// </summary>
        /// <param name="args">Args passed with the command</param>
        /// <returns>Result of the operation</returns>
        bool PreProcessCommand(List<string> args);


        /// <summary>
        /// Method executed by the client when a flag is received
        /// </summary>
        /// <param name="args">Args passed with the command</param>
        void ClientMethod(List<string> args);


        /// <summary>
        /// Method executed by the server when a flag is received
        /// </summary>
        /// <param name="args">Args passed with the command</param>
        void ServerMethod(List<string> args);
        #endregion Methods
    }
}
