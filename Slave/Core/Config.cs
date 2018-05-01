namespace Slave.Core
{
    internal static class Config
    {
        /// <summary>
        /// IP to connect to
        /// </summary>
        public static string ip { get; } = null;

        /// <summary>
        /// Port to connect to
        /// </summary>
        public static int port { get; } = 0;

        /// <summary>
        /// Time in ms to wait between each master connection attempt
        /// </summary>
        public static int masterConnectionRetryDelay { get; } = 2000;

        /// <summary>
        /// Number of times the slave will try to connect to the master
        /// This is taken into account only if a botnet server address was specified
        /// </summary>
        public static int maxConnectionRetryCount { get; } = 20;

        /// <summary>
        /// Master botnet server adress
        /// </summary>
        public static string botnetAdress { get; } = "http://127.0.0.1/backnet/index.php";

        /// <summary>
        /// Time in ms to wait for between each botnet request
        /// </summary>
        public static int botnetServerRequestRetryDelay { get; } = 5000;
    }
}
