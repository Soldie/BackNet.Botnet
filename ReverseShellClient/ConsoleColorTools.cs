using System;

namespace ReverseShellClient
{
    public static class ConsoleColorTools
    {
        public static void WriteCommandMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(" [+] " + message);
            Console.ForegroundColor = ConsoleColor.Green;
        }

        public static void WriteWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.Green;
        }

        public static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.Green;
        }

        public static void WriteCommandError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" [-] " + message);
            Console.ForegroundColor = ConsoleColor.Green;
        }

        public static void WriteCommandSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(" [+] " + message);
            Console.ForegroundColor = ConsoleColor.Green;
        }
    }
}
