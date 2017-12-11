using System;
using System.Collections.Generic;

namespace ReverseShellClient
{
    public static class ConsoleColorTools
    {
        public static void WriteCommandMessage(string message) =>
            WriteColorMessage(message, " [+] ", ConsoleColor.Gray);

        public static void WriteCommandError(string message) =>
            WriteColorMessage(message, " [-] ", ConsoleColor.Red);

        public static void WriteCommandSuccess(string message) =>
            WriteColorMessage(message, " [+] ", ConsoleColor.Cyan);

        public static void WriteWarning(string message) =>
            WriteColorMessage(message, "", ConsoleColor.Yellow);

        public static void WriteError(string message) =>
            WriteColorMessage(message, "", ConsoleColor.Red);


        static void WriteColorMessage(string message, string messagePrefix, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(messagePrefix + message);
            Console.ForegroundColor = ConsoleColor.Green;
        }
    }
}
