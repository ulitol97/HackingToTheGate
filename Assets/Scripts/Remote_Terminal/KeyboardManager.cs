using System;
using System.Collections.Generic;
using MEC;
using UnityEngine;
using UnityObserver;

namespace Remote_Terminal
{
    /// <summary>
    /// The KeyboardManager class is in charge of handling the keyboard input received by the game and translating and
    /// sending the detected keystrokes to the VNC server.
    /// As an observer object, it has a status <see cref="_isOnline"/> which is notified by
    /// the observed VncManager if changed.
    /// </summary>
    /// <remarks>Seems like Unity has an issue detecting the AltGr key, which is interpreted as Ctrl Left.
    /// This issue was solved programatically.</remarks>
    public class KeyboardManager : MonoBehaviour, IObserver
    {
     
        /// <summary>
        /// List of valid key codes that the game will listen to (that is, no mouse events or gamepad inputs will
        /// be checked by the KeyboardManager).
        /// </summary>
        private List<KeyCode> _keyCodes;
        
        /// <summary>
        /// Minimum Keycode number that the KeyboardManager will pay attention to.
        /// Any Keycode equal or lower than it will be dismissed.
        /// </summary>
        private const KeyCode MinKeycode = KeyCode.None;

        /// <summary>
        /// Maximum Keycode number that the KeyboardManager will pay attention to.
        /// Any Keycode equal or higher than it will be dismissed.
        /// </summary>
        private const KeyCode MaxKeycode = KeyCode.Help;
 
        /// <summary>
        /// Boolean flag indicating whether the Shift modifier is being pressed or not.
        /// </summary>
        private bool _shiftModifier;
        
        /// <summary>
        /// Boolean flag indicating whether the AltGr modifier is being pressed or not.
        /// </summary>
        private bool _altGrModifier;
        
        /// <summary>
        /// Boolean flag indicating whether the CapsLock is active or not.
        /// </summary>
        private bool _capsModifier;
        
        /// <summary>
        /// Represents the state of the KeyboardManager and whether it should send
        /// keyboard updates to the VNC server or not.
        /// </summary>
        private bool _isOnline;
        
        /// <summary>
        /// Represents the state of the KeyboardManager in-game. If toggled, it should send
        /// keyboard updates to the server.
        /// </summary>
        private bool _isOnScreen;
        
        /// <summary>
        /// Function called when the KeyboardManager is inserted into the game.
        /// Filters the valid keys that will be checked on each game frame and stores then in the _keyCodes property.
        /// </summary>
        private void Awake()
        {
            // Store the valid keys that will be checked in memory for future iteration.
           _keyCodes = new List<KeyCode>();
            foreach (KeyCode vKey in Enum.GetValues(typeof(KeyCode)))
            {
                if (vKey > MinKeycode && vKey < MaxKeycode)
                    _keyCodes.Add(vKey);
            }
        }

        /// <summary>
        /// Function called on each frame the KeyboardManager is present into the game.
        /// Checks for valid keystrokes and if any orders them to be sent to the server.
        /// </summary>
        /// <remarks>In order to balance efficiency and usability, the KeyboardManager handles a maximum of
        /// one key press and one key release per frame</remarks>
        private void Update()
        {
            if (!_isOnline || !_isOnScreen)
                return;
            
            KeyCode keyCodeDown = KeyCode.None;
            KeyCode keyCodeUp = KeyCode.None;
            
            // Iterate through valid keycodes and check for (un)pressed key
            foreach (KeyCode vKey in _keyCodes)
            {
                if (Input.GetKeyDown(vKey))
                    keyCodeDown = vKey;
                
                else if (Input.GetKeyUp(vKey))
                    keyCodeUp = vKey;
                
            }
            
            // Process 1 key up and 1 key down max per frame and avoid Unity AltGr bugging behavior.
            Timing.RunCoroutine(TranslateKeyStroke(keyCodeDown, true));
            Timing.RunCoroutine(TranslateKeyStroke(keyCodeUp, false));
            
        }
        
        /// <summary>
        /// Updates the KeyboardManager status with the current connection status of the VncManager observed,
        /// indicating if key strokes should be sent to the server or not.
        /// </summary>
        public void UpdateObserver()
        {
            if (VncManager.GetInstance(true) != null)
                _isOnline = VncManager.GetInstance(true).ConnectionStatus;
            _isOnScreen = !_isOnScreen;
        }

        
        /// <summary>
        /// Takes the input key detected by Unity, translates it regarding the present key modifiers
        /// (Shift, AltGr and Caps Lock), then translates it into its hexadecimal virtual keycode and sends it over
        /// to game VncClient.
        /// </summary>
        /// <param name="keyCode">Key stroke detected by the game.</param>
        /// <param name="pressed">True if the key stroke is a key press, false if it is a key release.</param>
        private IEnumerator<float> TranslateKeyStroke(KeyCode keyCode, bool pressed)
        {
            // If a modifier key was pressed down or up, update the corresponding modifier.
            CheckModifiers(keyCode, pressed);

            // Translate the received keycode to its byte representation sent to server
            uint virtualKey = VirtualKeyFromKey(keyCode);

            // Send the key stroke to server, marking it as a press or release
            if (VncManager.GetInstance(true) != null)
                VncManager.GetInstance(true).SendKeyToServer(virtualKey, pressed);

            yield break;
        }

        /// <summary>
        /// Checks the keystroke detected by the game in search for modifier keys: Shift, AltGr and Caps Lock
        /// and notifies the KeyboardManager when a match is found.
        /// </summary>
        /// <param name="keyCode">Key stroke detected by the game.</param>
        /// <param name="pressed">True if the key stroke is a key press, false if it is a key release.
        /// This is used to determine the state of the Caps Lock.</param>
        /// <returns>True if the key press/release corresponds to a modifier key, false otherwise.</returns>
        private void CheckModifiers(KeyCode keyCode, bool pressed)
        {
            if (keyCode == KeyCode.CapsLock && pressed)
            {
                _capsModifier = !_capsModifier;
                return;
            }

            switch (keyCode)
            {
                case KeyCode.LeftShift:
                case KeyCode.RightShift:
                    _shiftModifier = !_shiftModifier;
                    break;
               
                case (KeyCode.AltGr):
                    _altGrModifier = !_altGrModifier;
                    break;
            }
        }
        
        /// <summary>
        /// Performs the translation from the key event detected by the game to the key's hexadecimal virtual keycode.
        /// </summary>
        /// <param name="keyCode">Key stroke detected by the game.</param>
        /// <returns>The corresponding hexadecimal code of the key event detected, taking into account the active
        /// modifier keys at the moment of the translation.</returns>
        private uint VirtualKeyFromKey(KeyCode keyCode)
        {
            if (_shiftModifier && _shiftTranslationTable.ContainsKey(keyCode))
                return _shiftTranslationTable[keyCode];
            
            if (_altGrModifier && _altGrTranslationTable.ContainsKey(keyCode))
                return _altGrTranslationTable[keyCode];

            if (_capsModifier && _capsLockTranslationTable.ContainsKey(keyCode))
                return _capsLockTranslationTable[keyCode];
            
            if (_keyTranslationTable.ContainsKey(keyCode))
                return _keyTranslationTable[keyCode];
            
            return _keyTranslationTable[KeyCode.None]; 
        }

        /// <summary>
        /// Dictionary used to store in memory the translations from each key event code to it's key's hexadecimal
        /// virtual key value.
        /// </summary>
        private readonly Dictionary<KeyCode, uint> _keyTranslationTable = new Dictionary<KeyCode, uint>
        {
            {KeyCode.None, 0x00},
            {KeyCode.Backspace, 0xff08},
            {KeyCode.Tab, 0xff09},
            {KeyCode.Clear, 0xff08},
            {KeyCode.Return, 0xff0d},
            {KeyCode.Pause, 0xff13},
            {KeyCode.Escape, 0xff1b},
            {KeyCode.Space, 0x20},
            {KeyCode.Exclaim, 0x21},
            {KeyCode.DoubleQuote, 0x22},
            {KeyCode.Hash, 0x23},
            {KeyCode.Dollar, 0x24},
            {KeyCode.Ampersand, 0x26},
            {KeyCode.LeftParen, 0x28},
            {KeyCode.RightParen, 0x29},
            {KeyCode.Asterisk, 0x2a},
            {KeyCode.Plus, 0x2b},
            {KeyCode.Comma, 0x2c},
            {KeyCode.Minus, 0x2d},
            {KeyCode.Period, 0x2e},
            {KeyCode.Slash, 0x63}, // Cedilla, sending standard c.
            {KeyCode.Alpha0, 0x30},
            {KeyCode.Alpha1, 0x31},
            {KeyCode.Alpha2, 0x32},
            {KeyCode.Alpha3, 0x33},
            {KeyCode.Alpha4, 0x34},
            {KeyCode.Alpha5, 0x35},
            {KeyCode.Alpha6, 0x36},
            {KeyCode.Alpha7, 0x37},
            {KeyCode.Alpha8, 0x38},
            {KeyCode.Alpha9, 0x39},
            {KeyCode.Colon, 0x3a},
            {KeyCode.Less, 0x3c},
            {KeyCode.Equals, 0x2b}, // Plus
            {KeyCode.Greater, 0x3e},
            {KeyCode.Question, 0x3f},
            {KeyCode.At, 0x40},
            {KeyCode.LeftBracket, 0x27}, // Apostrophe
            {KeyCode.Backslash, 0x3c}, // Less, <
            {KeyCode.RightBracket, 0xa1}, // Open exclamation
            {KeyCode.Caret, 0x5e},
            {KeyCode.Underscore, 0x5f},
            {KeyCode.BackQuote, 0x6e}, // It is an Ñ (0xf1), we send an N
            {KeyCode.A, 0x61},
            {KeyCode.B, 0x62},
            {KeyCode.C, 0x63},
            {KeyCode.D, 0x64},
            {KeyCode.E, 0x65},
            {KeyCode.F, 0x66},
            {KeyCode.G, 0x67},
            {KeyCode.H, 0x68},
            {KeyCode.I, 0x69},
            {KeyCode.J, 0x6a},
            {KeyCode.K, 0x6b},
            {KeyCode.L, 0x6c},
            {KeyCode.M, 0x6d},
            {KeyCode.N, 0x6e},
            {KeyCode.O, 0x6f},
            {KeyCode.P, 0x70},
            {KeyCode.Q, 0x71},
            {KeyCode.R, 0x72},
            {KeyCode.S, 0x73},
            {KeyCode.T, 0x74},
            {KeyCode.U, 0x75},
            {KeyCode.V, 0x76},
            {KeyCode.W, 0x77},
            {KeyCode.X, 0x78},
            {KeyCode.Y, 0x79},
            {KeyCode.Z, 0x7a},
            {KeyCode.Delete, 0xffff},
            {KeyCode.Keypad0, 0x30},
            {KeyCode.Keypad1, 0x31},
            {KeyCode.Keypad2, 0x32},
            {KeyCode.Keypad3, 0x33},
            {KeyCode.Keypad4, 0x34},
            {KeyCode.Keypad5, 0x35},
            {KeyCode.Keypad6, 0x36},
            {KeyCode.Keypad7, 0x37},
            {KeyCode.Keypad8, 0x38},
            {KeyCode.Keypad9, 0x39},
            {KeyCode.KeypadPeriod, 0x2e},
            {KeyCode.KeypadDivide, 0x2f},
            {KeyCode.KeypadMultiply, 0x2a},
            {KeyCode.KeypadMinus, 0x2d},
            {KeyCode.KeypadPlus, 0x2b},
            {KeyCode.KeypadEnter, 0xff0d},
            {KeyCode.KeypadEquals, 0x3d},
            {KeyCode.UpArrow, 0xff52},
            {KeyCode.DownArrow, 0xff54},
            {KeyCode.RightArrow, 0xff53},
            {KeyCode.LeftArrow, 0xff51},
            {KeyCode.Insert, 0xff63},
            {KeyCode.Home, 0xff50},
            {KeyCode.End, 0xff57},
            {KeyCode.PageUp, 0xff55},
            {KeyCode.PageDown, 0x7e}, // Tilde, not page down 0xff56
            {KeyCode.F1, 0xffbe},
            {KeyCode.F2, 0xffbf},
            {KeyCode.F3, 0xffc0},
            {KeyCode.F4, 0xffc1},
            {KeyCode.F5, 0xffc2},
            {KeyCode.F6, 0xffc3},
            {KeyCode.F7, 0xffc4},
            {KeyCode.F8, 0xffc5},
            {KeyCode.F9, 0xffc6},
            {KeyCode.F10, 0xffc7},
            {KeyCode.F11, 0xffc8},
            {KeyCode.F12, 0xffc9},
            {KeyCode.F13, 0xffca},
            {KeyCode.F14, 0xffcb},
            {KeyCode.F15, 0xffcc},
            {KeyCode.Numlock, 0xff7f},
            {KeyCode.CapsLock, 0xffe5},  // Caps modifier is managed manually by the Keyboard Manager
            {KeyCode.ScrollLock, 0xff14},
            {KeyCode.RightControl, 0xffe4},
            {KeyCode.LeftControl, 0xffe3},
            {KeyCode.RightAlt, 0xffea},
            {KeyCode.LeftAlt, 0xffe9},

        };
        
        /// <summary>
        /// Dictionary used to store in memory the translations from each key event code to it's key's hexadecimal
        /// virtual key value, given that the Shift key modifier is active.
        /// </summary>
        private readonly Dictionary<KeyCode, uint> _shiftTranslationTable = new Dictionary<KeyCode, uint>
        {
            {KeyCode.None, 0x00},
            {KeyCode.Alpha0, 0x3d}, // Equals
            {KeyCode.Alpha1, 0x21}, // Close exclaim
            {KeyCode.Alpha2, 0x22}, // Double quote
            {KeyCode.Alpha3, 0x2e}, // Period (not) centered
            {KeyCode.Alpha4, 0x24}, // Dollar
            {KeyCode.Alpha5, 0x25}, // Percent
            {KeyCode.Alpha6, 0x26}, //Ampersand
            {KeyCode.Alpha7, 0x2f}, // Slash
            {KeyCode.Alpha8, 0x28}, // Left parenthesis
            {KeyCode.Alpha9, 0x29}, // Right parenthesis
            {KeyCode.LeftBracket, 0x3f}, // Close question
            {KeyCode.RightBracket, 0xbf}, // Open question
            {KeyCode.Semicolon, 0x5e}, // Circumflex (0xfe52), sending caret (0x5e)
            {KeyCode.Equals, 0x2a}, // Asterisk
            {KeyCode.Quote, 0x22}, // Diaeresis (0xfe57), sending double quote (0x22)
            {KeyCode.Backslash, 0x3e}, // Greater, >
            {KeyCode.Minus, 0x5f}, // Underscore
            {KeyCode.Comma, 0x3b}, // Semicolon
            {KeyCode.Period, 0x3a}, // Colon
            
            // Capitalized letters
            {KeyCode.A, 0x41},
            {KeyCode.B, 0x42},
            {KeyCode.C, 0x43},
            {KeyCode.D, 0x44},
            {KeyCode.E, 0x45},
            {KeyCode.F, 0x46},
            {KeyCode.G, 0x47},
            {KeyCode.H, 0x48},
            {KeyCode.I, 0x49},
            {KeyCode.J, 0x4a},
            {KeyCode.K, 0x4b},
            {KeyCode.L, 0x4c},
            {KeyCode.M, 0x4d},
            {KeyCode.N, 0x4e},
            {KeyCode.O, 0x4f},
            {KeyCode.P, 0x50},
            {KeyCode.Q, 0x51},
            {KeyCode.R, 0x52},
            {KeyCode.S, 0x53},
            {KeyCode.T, 0x54},
            {KeyCode.U, 0x55},
            {KeyCode.V, 0x56},
            {KeyCode.W, 0x57},
            {KeyCode.X, 0x58},
            {KeyCode.Y, 0x59},
            {KeyCode.Z, 0x5a},
        };

        /// <summary>
        /// Dictionary used to store in memory the translations from each key event code to it's key's hexadecimal
        /// virtual key value, given that the AltGr key modifier is active.
        /// </summary>
        private readonly Dictionary<KeyCode, uint> _altGrTranslationTable = new Dictionary<KeyCode, uint>
        {
            {KeyCode.None, 0x00},
            {KeyCode.Alpha1, 0x7c}, // Pipe |
            {KeyCode.Alpha2, 0x40}, // At @
            {KeyCode.Alpha3, 0x23}, // Hash #
            {KeyCode.Alpha4, 0x7e}, // Tilde ~
            {KeyCode.Semicolon, 0x5b}, // Left square bracket [
            {KeyCode.Equals, 0x5d}, // Right square bracket ]
            {KeyCode.Quote, 0x7b}, // Left square bracket {
            {KeyCode.Slash, 0x7d}, // Right curly bracket }
            {KeyCode.Backslash, 0x5c}, // Backslash
        };
        
        /// <summary>
        /// Dictionary used to store in memory the translations from each key event code to it's key's hexadecimal
        /// virtual key value, given that the Caps Lock is active.
        /// </summary>
        private readonly Dictionary<KeyCode, uint> _capsLockTranslationTable = new Dictionary<KeyCode, uint>
        {
            // Capitalized letters
            {KeyCode.A, 0x41},
            {KeyCode.B, 0x42},
            {KeyCode.C, 0x43},
            {KeyCode.D, 0x44},
            {KeyCode.E, 0x45},
            {KeyCode.F, 0x46},
            {KeyCode.G, 0x47},
            {KeyCode.H, 0x48},
            {KeyCode.I, 0x49},
            {KeyCode.J, 0x4a},
            {KeyCode.K, 0x4b},
            {KeyCode.L, 0x4c},
            {KeyCode.M, 0x4d},
            {KeyCode.N, 0x4e},
            {KeyCode.O, 0x4f},
            {KeyCode.P, 0x50},
            {KeyCode.Q, 0x51},
            {KeyCode.R, 0x52},
            {KeyCode.S, 0x53},
            {KeyCode.T, 0x54},
            {KeyCode.U, 0x55},
            {KeyCode.V, 0x56},
            {KeyCode.W, 0x57},
            {KeyCode.X, 0x58},
            {KeyCode.Y, 0x59},
            {KeyCode.Z, 0x5a},
        };
    }
}