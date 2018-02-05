using System.Collections.Generic;

namespace ServerCommands
{
    public interface ICommand
    {
        /// <summary>
        /// Name of the command
        /// </summary>
        string name { get; }


        /// <summary>
        /// Method executed by the server
        /// </summary>
        /// <param name="args">Args passed with the command</param>
        void Process(List<string> args);
    }
}