using Slave.Core;
using System.Windows;

namespace Slave
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// On startup, give the manager an ip and port to connect to if given in arguments
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void App_OnStartup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length == 2 && int.TryParse(e.Args[1], out int port))
            {
                SlaveManager.ip = e.Args[0];
                SlaveManager.port = port;
            }
        }
    }
}
