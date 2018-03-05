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

            // Transfert started
            if (newCompletion == 0 && Console.CursorLeft != 1)
            {
                Console.Write($"[{ new string(' ', 50) }]   0%");
                Console.SetCursorPosition(1, Console.CursorTop);
                completion = newCompletion;
            }
            // Transfert progressed
            else if (completion != newCompletion)
            {
                if (newCompletion % 2 == 0)
                {
                    Console.Write(new string('>', (newCompletion - completion + 1) / 2));
                }
                
                completion = newCompletion;

                var leftStart = 53 + (newCompletion == 100 ? 0 : newCompletion >= 10 ? 1 : 2);
                Console.SetCursorPosition(leftStart, Console.CursorTop);
                Console.Write(newCompletion);


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
