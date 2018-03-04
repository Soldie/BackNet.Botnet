using System;

namespace Client.AdvancedConsole
{
    public static class ProgressDisplayer
    {
        static int completion = 0;

        /// <summary>
        /// Displays a completion meter with a gauge and a percentage
        /// </summary>
        /// <param name="current">Current value</param>
        /// <param name="total">Total value to reach</param>
        public static void DisplayCompletionMeter(long current, long total)
        {
            var newCompletion = (int)((decimal)current / total * 100);

            if (newCompletion == 0 && Console.CursorLeft != 1)
            {
                // Transfert started
                Console.Write($"[{ new string(' ', 50) }]   0%");
                Console.SetCursorPosition(1, Console.CursorTop);
                completion = newCompletion;
            }
            else if (completion != newCompletion && newCompletion % 2 == 0)
            {
                // Transfert progressed
                Console.Write(new string('>', (newCompletion - completion) / 2));
                completion = newCompletion;

                var leftStart = 53 + (completion == 100 ? 0 : completion >= 10 ? 1 : 2);
                Console.SetCursorPosition(leftStart, Console.CursorTop);
                Console.Write(completion);


                // Transfert finished
                if (completion == 100)
                {
                    completion = 0;
                    // Return cursor
                    Console.SetCursorPosition(0, Console.CursorTop + 1);
                }
                else
                {
                    // Set cursor position at the end of the inner gauge completion
                    Console.SetCursorPosition(completion / 2 + 1, Console.CursorTop);
                }
            }
        }
    }
}
