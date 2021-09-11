namespace RetroSpy.Readers
{
    internal class Pippin
    {
        private const int PACKET_SIZE = 61;
        private const int POLISHED_PACKET_SIZE = 32;
        private const int JOYSTICK_PACKET_SIZE = 26;
        private const int TABLET_PACKET_SIZE = 42;

        private static int minX = int.MaxValue, minY = int.MaxValue;
        private static int maxX = int.MinValue, maxY = int.MinValue;

        private static readonly string[] BUTTONS = {
            null, "1", "2", "blue", "yellow", "d_up", "d_left", "d_right", "d_down", "red", "green", "square", "circle", "diamond"
        };

        private static readonly string[] JOYSTICK_BUTTONS = {
            "2", "3", "1", "4", "5", null, null, null
        };

        private static readonly string[] KEYBOARD_CODES =
        {
            "A", "S", "D", "F", "H", "G", "Z", "X",
            "C", "V", null, "B", "Q", "W", "E", "R",
            "Y", "T", "D1", "D2", "D3", "D4", "D6", "D5",
            "Equals", "D9", "D7", "Minus", "D8", "D0", "RightBracket", "O",

            "U", "LeftBracket", "I", "P", "Return", "L", "J", "Apostrophe",
            "K", "Semicolon", "Backslash", "Comma", "Slash", "N", "M", "Period",
            "Tab", "Space", "Grave", "Back", null, "Escape", "LeftControl", "LeftAlt",
            "LeftShift", "Capital", "LeftWindowsKey", "Left", "Right", "Down", "Up", null,

            null, "Decimal", null, "Subtract" /*Multiply*/, null, "MacAdd" /*Add*/, null, "NumberLock" /*Clear*/,
            null, null, null, "Multiply", "NumberPadEnter", null, "Add" /* Subtract */, null,
            null, "Divide" /* NumPad Equal */, "NumberPad0", "NumberPad1", "NumberPad2", "NumberPad3", "NumberPad4", "NumberPad5",
            "NumberPad6", "NumberPad7", null, "NumberPad8", "NumberPad9", null, null, null,

            "F5", "F6", "F7", "F3", "F8", "F9", null, "F11",
            null, "PrintScreen", null, "ScrollLock", null, "F10", null, "F12",
            null, "Pause", "Insert", "Home", "PageUp", "Delete", "F4", "End",
            "F2", "PageDown", "F1", null, null, null, null, "Power",
        };

        private static float ReadMouse(byte data)
        {
            if (data >= 64)
            {
                return (-1.0f * (128 - data)) / 64.0f;
            }
            else
            {
                return data / 64.0f;
            }
        }

        private static float ReadJoystick(byte data)
        {
            return (data - 128.0f) / 128.0f;
        }

        private static int ScaleInteger(float oldValue, float oldMin, float oldMax, float newMin, float newMax)
        {
            float newValue = ((oldValue - oldMin) * (newMax - newMin)) / (oldMax - oldMin) + newMin;
            if (newValue > newMax)
                return (int)newMax;
            if (newValue < newMin)
                return (int)newMin;

            return (int)newValue;
        }

        private static void SetTablet(int X, int Y, ControllerStateBuilder state)
        {
            if (X < minX)
                minX = X;
            else if (X > maxX)
                maxX = X;

            if (Y < minY)
                minY = Y;
            else if (Y > maxY)
                maxY = Y;

            state.SetAnalog("touchpad_x", ScaleInteger(X, minX, maxX, 0, 1024) / 1024.0f, X);
            state.SetAnalog("touchpad_y", ScaleInteger(Y, minY, maxY, 0, 16384) / 16384.0f, Y);
        }

        public static ControllerStateEventArgs ReadFromPacket(byte[] packet)
        {
            if (packet.Length == PACKET_SIZE)
            {

                byte[] polishedPacket = new byte[POLISHED_PACKET_SIZE];

                polishedPacket[0] = (byte)((packet[0] >> 4) | packet[1]);
                polishedPacket[1] = (byte)(packet[9] == 0 ? 0 : 1);
                polishedPacket[2] = (byte)(packet[17] == 0 ? 0 : 1);

                for (int i = 18; i < 29; ++i)
                {
                    polishedPacket[i - 15] = (byte)(packet[i] == 0 ? 0 : 1);
                }

                for (byte i = 0; i < 7; ++i)
                {
                    polishedPacket[14] |= (byte)((packet[i + 2] == 0 ? 0 : 1) << i);
                }

                for (byte i = 0; i < 7; ++i)
                {
                    polishedPacket[15] |= (byte)((packet[i + 10] == 0 ? 0 : 1) << i);
                }

                for (int i = 29; i < 61; i += 2)
                {
                    polishedPacket[(i / 2) + 2] = (byte)((packet[i] >> 4) | packet[i + 1]);
                }

                ControllerStateBuilder state = new ControllerStateBuilder();

                for (int i = 0; i < BUTTONS.Length; ++i)
                {
                    if (string.IsNullOrEmpty(BUTTONS[i]))
                    {
                        continue;
                    }

                    state.SetButton(BUTTONS[i], polishedPacket[i] == 0x00);
                }

                float y = ReadMouse(polishedPacket[14]);
                float x = ReadMouse(polishedPacket[15]);

                SignalTool.SetMouseProperties(x, y, polishedPacket[14], polishedPacket[15], state);

                for (int i = 0; i < KEYBOARD_CODES.Length; ++i)
                {
                    if (KEYBOARD_CODES[i] != null)
                    {
                        state.SetButton(KEYBOARD_CODES[i], (polishedPacket[(i / 8) + 16] & (1 << (i % 8))) != 0);
                    }
                }

                return state.Build();
            }
            else if (packet.Length == JOYSTICK_PACKET_SIZE)
            {
                ControllerStateBuilder state = new ControllerStateBuilder();

                byte x = 0, y = 0;

                for (byte i = 0; i < 8; ++i)
                {
                    x |= (byte)((packet[i + 2] == 0 ? 0 : 1) << i);
                }

                for (byte i = 0; i < 8; ++i)
                {
                    y |= (byte)((packet[i + 10] == 0 ? 0 : 1) << i);
                }

                for (int i = 0; i < JOYSTICK_BUTTONS.Length; ++i)
                {
                    if (string.IsNullOrEmpty(JOYSTICK_BUTTONS[i]))
                    {
                        continue;
                    }

                    state.SetButton(JOYSTICK_BUTTONS[i], packet[i + 18] == 0x00);
                }

                state.SetAnalog("x", ReadJoystick(x), x);
                state.SetAnalog("y", ReadJoystick(y), y);

                return state.Build();
            }
            else if (packet.Length == TABLET_PACKET_SIZE )
            {
                ControllerStateBuilder state = new ControllerStateBuilder();

                if (packet[34] != 0)
                {
                    byte x = 0, y = 0, x1 = 0, y1 = 0;

                    for (byte i = 0; i < 8; ++i)
                    {
                        x |= (byte)((packet[i + 2] == 0 ? 0 : 1) << i);
                    }

                    for (byte i = 0; i < 8; ++i)
                    {
                        x1 |= (byte)((packet[i + 10] == 0 ? 0 : 1) << i);
                    }

                    for (byte i = 0; i < 8; ++i)
                    {
                        y |= (byte)((packet[i + 18] == 0 ? 0 : 1) << i);
                    }

                    for (byte i = 0; i < 8; ++i)
                    {
                        y1 |= (byte)((packet[i + 26] == 0 ? 0 : 1) << i);
                    }

                    SetTablet(((x & 0b00011111) * 32) + (x1 >> 3),
                                ((y & 0b00011111) * 128) + (y1 >> 1), state);

                    state.SetButton("Button", (y & 0b10000000) != 0);
                }

                return state.Build();
            }    
            else
                return null;
        }
    }
}