namespace Slave.Commands.KeyLogger
{
    /// <summary>
    /// Class managing the logging of the keys in the memory
    /// </summary>
    internal class Logger
    {
        string logs = "";

        /// <summary>
        /// Returns the current logs and clear them
        /// </summary>
        /// <returns>Current logs</returns>
        public string GetLogs()
        {
            var currentLogs = logs;
            ClearLogs();
            return currentLogs;
        }

        /// <summary>
        /// Add the given key to the 'logs' variable
        /// </summary>
        /// <param name="keyToLog">String representing a key</param>
        public void LogKey(string keyToLog)
        {
            logs += keyToLog;
        }

        /// <summary>
        /// Empty logs variable
        /// </summary>
        public void ClearLogs() => logs = "";
    }
}
