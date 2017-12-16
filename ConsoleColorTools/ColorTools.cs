using System;

namespace Shared
{
    public static class ColorTools
    {
        public static void WriteCommandMessage(string message) =>
            WriteColorMessage(message, " [+] ", ConsoleColor.Gray);

        public static void WriteCommandError(string message) =>
            WriteColorMessage(message, " [-] ", ConsoleColor.Red);

        public static void WriteCommandSuccess(string message) =>
            WriteColorMessage(message, " [+] ", ConsoleColor.Cyan);

        public static void WriteMessage(string message) =>
            WriteColorMessage(message, "", ConsoleColor.Gray);

        public static void WriteWarning(string message) =>
            WriteColorMessage(message, "", ConsoleColor.Yellow);

        public static void WriteError(string message) =>
            WriteColorMessage(message, "", ConsoleColor.Red);


        static void WriteColorMessage(string message, string prefix, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(prefix + message);
            Console.ForegroundColor = ConsoleColor.Green;
        }
    }
}
