using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedConsole
{
    public static class DisplayTools
    {
        const string BANNER = "\n    ██████╗  █████╗  ██████╗██╗  ██╗███╗   ██╗███████╗████████╗\n    ██╔══██╗██╔══██╗██╔════╝██║ ██╔╝████╗  ██║██╔════╝╚══██╔══╝\n    ██████╔╝███████║██║     █████╔╝ ██╔██╗ ██║█████╗     ██║   \n    ██╔══██╗██╔══██║██║     ██╔═██╗ ██║╚██╗██║██╔══╝     ██║   \n    ██████╔╝██║  ██║╚██████╗██║  ██╗██║ ╚████║███████╗   ██║   \n    ╚═════╝ ╚═╝  ╚═╝ ╚═════╝╚═╝  ╚═╝╚═╝  ╚═══╝╚══════╝   ╚═╝\n\n";

        internal const string PROMPT = "BackNet>";


        /// <summary>
        /// Show the program's banner
        /// </summary>
        public static void DisplayBanner()
        {
            Console.WriteLine(BANNER);
        }


        /// <summary>
        /// Print the cmd prompt of the program to indicate it's waiting for user input
        /// </summary>
        public static void DisplayCommandPrompt() => Console.Write(PROMPT);
    }
}
