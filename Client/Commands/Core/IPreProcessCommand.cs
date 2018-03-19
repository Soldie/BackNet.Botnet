using System.Collections.Generic;

namespace Client.Commands.Core
{
    /// <summary>
    /// ClientCommands needing a check or process before sending the command to the server should implement this interface
    /// </summary>
    public interface IPreProcessCommand
    {
        /// <summary>
        /// Method to be executed before the command is sent to the server
        /// </summary>
        /// <param name="args">Arguments passed along the command</param>
        /// <returns>Boolean stating the result of the operation</returns>
        bool PreProcess(List<string> args);
    }
}
