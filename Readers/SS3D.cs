using System;

namespace RetroSpy.Readers
{
    public static class SS3D
    {
        private const int PACKET_SIZE = 56;
        private const int POLISHED_PACKET_SIZE = 21;
        private const int KEYBOARD_PACKET_SIZE = 18 * 2;
        private const int KEYBOARD_POLISHED_PACKET_SIZE = 18;

        private static readonly string[] KEYS =
        {
            null, "F9", null, "F5", "F3", "F1", "F2", "F12",
            null, "F10", "F8", "F6", "F4", "Tab", "Grave", null,
            null, "LeftAlt", "LeftShift", "Katakana", "LeftControl", "Q", "D1", "RightAlt",
            "RightControl", "NumberPadEnter", "Z", "S", "A", "W", "D2", "LeftWindowsKey",

            null, "C", "X", "D", "E", "D4", "D3", "RightWindowsKey",
            null, "Space", "V", "F", "T", "R", "D5", "Applications",
            null, "N", "B", "H", "G", "Y", "D6", null,
            null, null, "M", "J", "U", "D7", "D8", null,

            null, "Comma", "K", "I", "O", "D0", "D9", null,
            null, "Period", "Slash", "L", "Semicolon", "P", "Minus", null,
            null, "JpSlash", "Apostrophe", null, "LeftBracket", "Equal", null, null,
            "Capital", "RightShift", "Return", "RightBracket", null, "LeftOfReturn", null, null,

            null, null, null, null, "Henkan", null, "Back", "Muhenkan",
            null, "NumberPad1", "Yen", "NumberPad4", "NumberPad7", null, null, null,
            "NumberPad0", "Decimal", "NumberPad2", "NumberPad5", "NumberPad6", "NumberPad8", "Escape", "NumberLock",
            "F11", "Add", "NumberPad3", "Subtract", "Multiply", "NumberPad9", "ScrollLock", null,

            "Divide", "Insert", "Pause", "F7", "PrintScreen", "Delete", "Left", "Home",
            "End", "Up", "Down", "PageUp", "PageDown", "Right", null, null,
        };

        private static readonly string[] BUTTONS = {
            null, "right", "left", "down", "up", "start", "A", "C", "B", "R", "X", "Y", "Z", "L"
        };

        private static readonly string[] MOUSE_BUTTONS = {
            null, null, null, null, "start", "middle", "right", "left"
        };

        private static float ReadTrigger(byte input)
        {
            return (float)input / 256;
        }

        private static float ReadStick(byte input)
        {
            return (float)(input - 128) / 128;
        }

        private static float ReadMouse(bool sign, bool over, byte data)
        {
            float val = over ? 1.0f : sign ? 0xFF - data : data;
            return val * (sign ? -1 : 1) / 255;
        }

#pragma warning disable CA1508 // Avoid dead conditional code
        public static ControllerStateEventArgs ReadFromPacket(byte[] packet)
        {
            if (packet == null)
            {
                throw new ArgumentNullException(nameof(packet));
            }

            if (packet.Length == PACKET_SIZE)
            {
                byte[] polishedPacket = new byte[POLISHED_PACKET_SIZE];

                polishedPacket[0] = 0;
                byte j = 8;
                for (byte i = 0; i < 8; ++i)
                {
                    j--;
                    polishedPacket[0] |= (byte)((packet[i] == 0 ? 0 : 1) << j);
                }

                if (polishedPacket[0] != 0x02 && polishedPacket[0] != 0x16 && polishedPacket[0] != 0xFF)

                {
                    return null;
                }

                ControllerStateBuilder state = new ControllerStateBuilder();

                if (polishedPacket[0] != 0xFF)
                {
                    for (byte i = 0; i < 16; ++i)
                    {
                        polishedPacket[i + 1] = packet[i + 8] == 0 ? (byte)1 : (byte)0;
                    }

                    byte numExtraBytes = 0;
                    if (polishedPacket[0] == 0x16)
                    {
                        numExtraBytes = 4;
                    }

                    for (int i = 0; i < numExtraBytes; ++i)
                    {
                        polishedPacket[17 + i] = 0;
                        j = 8;
                        for (byte k = 0; k < 8; ++k)
                        {
                            j--;
                            polishedPacket[17 + i] |= (byte)((packet[24 + (i * 8) + k] == 0 ? 0 : 1) << j);
                        }
                    }

                    if (polishedPacket.Length < POLISHED_PACKET_SIZE)
                    {
                        return null;
                    }

                    for (int i = 0; i < BUTTONS.Length; ++i)
                    {
                        if (string.IsNullOrEmpty(BUTTONS[i]))
                        {
                            continue;
                        }

                        state.SetButton(BUTTONS[i], polishedPacket[i] != 0x00);
                    }

                    if (polishedPacket[0] == 0x16)
                    {
                        state.SetAnalog("lstick_x", ReadStick(polishedPacket[17]), polishedPacket[17]);
                        state.SetAnalog("lstick_y", ReadStick(polishedPacket[18]), polishedPacket[18]);
                        state.SetAnalog("trig_r", ReadTrigger(polishedPacket[19]), polishedPacket[19]);
                        state.SetAnalog("trig_l", ReadTrigger(polishedPacket[20]), polishedPacket[20]);
                    }
                    else
                    {
                        state.SetAnalog("lstick_x", ReadStick(128), 128);
                        state.SetAnalog("lstick_y", ReadStick(128), 128);
                        state.SetAnalog("trig_r", ReadTrigger(0), 0);
                        state.SetAnalog("trig_l", ReadTrigger(0), 0);
                    }
                }
                else
                {
                    for (int i = 0; i < BUTTONS.Length; ++i)
                    {
                        if (string.IsNullOrEmpty(BUTTONS[i]))
                        {
                            continue;
                        }

                        state.SetButton(BUTTONS[i], false);
                    }

                    for (int i = 0; i < MOUSE_BUTTONS.Length; ++i)
                    {
                        if (string.IsNullOrEmpty(MOUSE_BUTTONS[i]))
                        {
                            continue;
                        }

                        state.SetButton(MOUSE_BUTTONS[i], packet[i + 8] != 0x00);
                    }

                    state.SetAnalog("lstick_x", ReadStick(128), 128);
                    state.SetAnalog("lstick_y", ReadStick(128), 128);
                    state.SetAnalog("trig_r", ReadTrigger(0), 0);
                    state.SetAnalog("trig_l", ReadTrigger(0), 0);

                    byte xVal = 0;
                    j = 8;
                    for (byte k = 0; k < 8; ++k)
                    {
                        j--;
                        xVal |= (byte)((packet[16 + k] == 0 ? 0 : 1) << j);
                    }
                    byte yVal = 0;
                    j = 8;
                    for (byte k = 0; k < 8; ++k)
                    {
                        j--;
                        yVal |= (byte)((packet[24 + k] == 0 ? 0 : 1) << j);
                    }

                    float x = ReadMouse(packet[11] != 0, packet[9] != 0, xVal);
                    float y = ReadMouse(packet[10] != 0, packet[8] != 0, yVal);

                    SignalTool.SetMouseProperties(x, y, xVal, yVal, state);
                }

                return state.Build();
            }
            else if (packet.Length == KEYBOARD_PACKET_SIZE)
            {
                ControllerStateBuilder state = new ControllerStateBuilder();

                int j = 0;
                byte[] reconstructedPacket = new byte[KEYBOARD_POLISHED_PACKET_SIZE];
                for (int i = 0; i < KEYBOARD_POLISHED_PACKET_SIZE; ++i)
                {
                    reconstructedPacket[i] = (byte)((packet[j] >> 4) | packet[j + 1]);
                    j += 2;
                }

                for (int i = 0; i < KEYS.Length; ++i)
                {
                    if (KEYS[i] != null)
                    {
                        state.SetButton(KEYS[i], (reconstructedPacket[i / 8] & (1 << (i % 8))) != 0);
                    }
                }

                return state.Build();
            }
            else
            {
                return null;
            }
        }
    }
}