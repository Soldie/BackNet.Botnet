using System.Collections.Generic;

namespace Commands
{
    public interface ICommand
    {
        string name { get; }

        bool isLocal { get; }

        int argCount { get; }

        /// <summary>
        /// Flags sent by the server to init a client-side function
        /// </summary>
        List<string> clientFlags { get; }


        /// <summary>
        /// Possibly check for files presence or do some preparations
        /// </summary>
        /// <param name="command">Whole command typed</param>
        /// <returns>Result of the operation</returns>
        bool PreProcessCommand(string command);

        /// <summary>
        /// Method executed by the client when a flag is received
        /// </summary>
        void ClientMethod();

        /// <summary>
        /// Method executed by the server when a flag is received
        /// </summary>
        void ServerMethod();
    }
}
