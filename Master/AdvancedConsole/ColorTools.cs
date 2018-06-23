using System;

namespace Master.AdvancedConsole
{
    public static class ColorTools
    {
        /// <summary>
        /// Sets the default consolecolor to green
        /// </summary>
        public static void SetDefaultConsoleColor()
        {
            Console.ForegroundColor = ConsoleColor.Green;
        }

        public static void WriteCommandMessage(string message) =>
            WriteColorMessage(message, " [ ] ", ConsoleColor.Gray);

        public static void WriteCommandError(string message) =>
            WriteColorMessage(message, " [-] ", ConsoleColor.Red);

        public static void WriteCommandSuccess(string message) =>
            WriteColorMessage(message, " [+] ", ConsoleColor.Cyan);

        public static void WriteMessage(string message, bool inline = false) =>
            WriteColorMessage(message, ConsoleColor.Gray, inline);

        public static void WriteWarning(string message, bool inline = false) =>
            WriteColorMessage(message, ConsoleColor.Yellow, inline);

        public static void WriteError(string message, bool inline = false) =>
            WriteColorMessage(message, ConsoleColor.Red, inline);

        public static void WriteSuccess(string message, bool inline = false) =>
            WriteColorMessage(message, ConsoleColor.Cyan, inline);

        public static void WriteCustomColor(string message, ConsoleColor color, bool inline = false) =>
            WriteColorMessage(message, color, inline);

        static void WriteColorMessage(string message, string prefix, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(prefix + message);
            Console.ForegroundColor = ConsoleColor.Green;
        }

        static void WriteColorMessage(string message, ConsoleColor color, bool inline)
        {
            Console.ForegroundColor = color;
            if (inline)
            {
                Console.Write(message);
            }
            else
            {
                Console.WriteLine(message);
            }
            Console.ForegroundColor = ConsoleColor.Green;
        }
    }
}
