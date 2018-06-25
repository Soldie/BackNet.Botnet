using System;
using System.Threading;

namespace Master.AdvancedConsole
{
    public static class ProgressDisplayer
    {
        static int completion = 0;

        static long lastValue = 0;

        static long total = 0;

        static int lastLine;

        static UpdateState updateState;

        enum UpdateState
        {
            Started,
            Stopped,
            SignaledToStop
        }
        
        /// <summary>
        /// Hide cursor to avoid annoying blink. Set necessary values.
        /// Then call the InternalUpdate method to start the updates
        /// </summary>
        /// <param name="newTotal">Value to reach</param>
        public static void Init(long newTotal)
        {
            Console.CursorVisible = false;
            total = newTotal;

            // Display base gauge
            Console.Write($"[{ new string(' ', 50) }]\n{ new string(' ', 52 - total.ToString().Length - 7) }/{total} bytes");
            Console.SetCursorPosition(1, Console.CursorTop);
            lastLine = Console.CursorTop;

            updateState = UpdateState.Started;
            InternalUpdate();
        }

        /// <summary>
        /// Set the new value the completion meter should display
        /// </summary>
        /// <param name="current">Current value</param>
        public static void Update(long current)
        {
            lastValue = current;
        }

        /// <summary>
        /// Updates the completion meter with a gauge and a percentage
        /// </summary>
        static void InternalUpdate()
        {
            while (updateState == UpdateState.Started)
            {
                // Copy the value in case it changes
                var value = lastValue;
                var newCompletion = (int)((decimal)value / total * 100);

                // Transfert progressed
                if (completion != newCompletion)
                {
                    completion = newCompletion;

                    if (completion % 2 == 0)
                    {
                        Console.SetCursorPosition(1, lastLine - 1);
                        Console.Write(new string('=', completion / 2 - 1));
                        Console.Write('>');
                    }

                    var leftStart = 52 - total.ToString().Length - 7 - value.ToString().Length;
                    Console.SetCursorPosition(leftStart, lastLine);
                    Console.Write(value);

                    // Transfert finished
                    if (completion == 100)
                    {
                        Console.SetCursorPosition(50, lastLine - 1);
                        Console.Write('=');
                    }
                }
            }

            updateState = UpdateState.Stopped;
        }

        /// <summary>
        /// Reset completion and cursor (position + visibility)
        /// </summary>
        public static void End()
        {
            // Wait for the update loop to stop
            updateState = UpdateState.SignaledToStop;
            while (updateState != UpdateState.Stopped)
            {
                Thread.Sleep(5);
            }

            // Write gauge and percentage
            Console.SetCursorPosition(1, lastLine - 1);
            Console.Write(new string('=', 50));
            var leftStart = 52 - total.ToString().Length - 7 - total.ToString().Length;
            Console.SetCursorPosition(leftStart, lastLine);
            Console.Write(total);

            completion = 0;
            // Return cursor
            Console.SetCursorPosition(0, lastLine + 1);
            Console.CursorVisible = true;
        }
    }
}
