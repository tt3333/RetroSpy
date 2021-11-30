using SharpDX.DirectInput;
using System;

namespace RetroSpy.Readers
{
    public static class CDi
    {
        private const int PACKET_SIZE = 7;
        private const int KEYBOARD_PACKET_SIZE = 10;

        private static readonly string[] BUTTONS = {
            "wired-up", "wired-down", "wired-left", "wired-right", "wired-1", "wired-2",
            "wireless-up", "wireless-down", "wireless-left", "wireless-right", "wireless-1", "wireless-2",
             "pause", "standby", "stop", "previous", "play", "next", "tv-cable", "volume-down", "volume-up"
        };

        private static readonly string[] KEYBOARD_CODES =
        {

            "Escape", 
            null, "D1", "D2", "D3", "D4", "D5", "D6", "D7",
            null, "D8", "D9", "D0", "Minus", "Equals", "Back", "Home",
            null, "A", "B", "C", "D", "E", "F", "G", 
            null, "H", "I", "J", "K", "L", "M", "N", 
            null, "O", "P", "Q", "R", "S", "T", "U", 
            null, "V", "W", "X", "Y", "Z", "Tab", "LeftBracket",
            null, "RightBracket", "Backslash", "Semicolon", "Apostrophe", "Return", "Comma", "Period",
            null, "Slash", "Up", "Grave", "Space", "Delete", "Left", "Down",
            null, "Right", "F1","F2","F3","F4","F5","F6",
            null, "F7","F8", "Capital", "LeftShift", "LeftWindowsKey"/* SuperShift*/, "LeftControl"
        };

        private static float ReadAnalogButton(byte input)
        {
            return (float)(input) / 256;
        }

        public static ControllerStateEventArgs ReadFromPacket(byte[] packet)
        {
            if (packet == null)
                throw new ArgumentNullException(nameof(packet));

            if (packet.Length != PACKET_SIZE && packet.Length != KEYBOARD_PACKET_SIZE)
            {
                return null;
            }

            if (packet.Length == PACKET_SIZE)
            {
                byte[] cleanedData = new byte[21];
                if ((packet[2] & 0b00001000) == 0x0)
                {
                    cleanedData[0] = packet[0];
                    cleanedData[1] = 0;
                }
                else
                {
                    cleanedData[1] = packet[0];
                    cleanedData[0] = 0;
                }

                if ((packet[2] & 0b00010000) == 0x0)
                {
                    cleanedData[2] = packet[1];
                    cleanedData[3] = 0;
                }
                else
                {
                    cleanedData[3] = packet[1];
                    cleanedData[2] = 0;
                }

                cleanedData[4] = (byte)((packet[2] & 0x01) != 0 ? 0x01 : 0x00);
                cleanedData[5] = (byte)((packet[2] & 0x04) != 0 ? 0x01 : 0x00);

                if ((packet[6] & 0b00100000) == 0x0)
                {
                    cleanedData[6] = packet[3];
                    cleanedData[7] = 0;
                }
                else
                {
                    cleanedData[7] = packet[3];
                    cleanedData[6] = 0;
                }

                if ((packet[6] & 0b01000000) == 0x0)
                {
                    cleanedData[8] = packet[4];
                    cleanedData[9] = 0;
                }
                else
                {
                    cleanedData[9] = packet[4];
                    cleanedData[8] = 0;
                }

                cleanedData[10] = (byte)((packet[5] & 0x01) != 0 ? 0x01 : 0x00);
                cleanedData[11] = (byte)((packet[5] & 0x04) != 0 ? 0x01 : 0x00);
                cleanedData[12] = (byte)((packet[5] & 0x08) != 0 ? 0x01 : 0x00);
                cleanedData[13] = (byte)((packet[5] & 0x10) != 0 ? 0x01 : 0x00);
                cleanedData[14] = (byte)((packet[5] & 0x20) != 0 ? 0x01 : 0x00);
                cleanedData[15] = (byte)((packet[5] & 0x40) != 0 ? 0x01 : 0x00);
                cleanedData[16] = (byte)((packet[5] & 0x80) != 0 ? 0x01 : 0x00);
                cleanedData[17] = (byte)((packet[6] & 0x01) != 0 ? 0x01 : 0x00);
                cleanedData[18] = (byte)((packet[6] & 0x04) != 0 ? 0x01 : 0x00);
                cleanedData[19] = (byte)((packet[6] & 0x08) != 0 ? 0x01 : 0x00);
                cleanedData[20] = (byte)((packet[6] & 0x10) != 0 ? 0x01 : 0x00);

                ControllerStateBuilder state = new ControllerStateBuilder();

                for (int i = 0; i < BUTTONS.Length; ++i)
                {
                    if (string.IsNullOrEmpty(BUTTONS[i]))
                    {
                        continue;
                    }

                    state.SetButton(BUTTONS[i], cleanedData[i] != 0x00);
                }

                // Set double buttons
                state.SetButton("wired-1a", cleanedData[4] != 0x00);
                state.SetButton("wired-2a", cleanedData[5] != 0x00);
                state.SetButton("wireless-1a", cleanedData[10] != 0x00);
                state.SetButton("wireless-2a", cleanedData[11] != 0x00);

                // Handle 3 overriding other pushes
                if (cleanedData[4] != 0x00 && cleanedData[5] != 0x00)
                {
                    state.SetButton("wired-3of3", true);
                    state.SetButton("wired-1of3", false);
                    state.SetButton("wired-1aof3", false);
                    state.SetButton("wired-2of3", false);
                }
                else
                {
                    state.SetButton("wired-3of3", false);
                    state.SetButton("wired-1of3", cleanedData[4] != 0x00);
                    state.SetButton("wired-1aof3", cleanedData[4] != 0x00);
                    state.SetButton("wired-2of3", cleanedData[5] != 0x00);
                }

                state.SetButton("wireless-1a", cleanedData[10] != 0x00);

                state.SetAnalog("wired-analog_right", ReadAnalogButton(cleanedData[3]), cleanedData[3]);
                state.SetAnalog("wired-analog_left", ReadAnalogButton(cleanedData[2]), cleanedData[2]);
                state.SetAnalog("wired-analog_up", ReadAnalogButton(cleanedData[0]), cleanedData[0]);
                state.SetAnalog("wired-analog_down", ReadAnalogButton(cleanedData[1]), cleanedData[1]);

                state.SetAnalog("wireless-analog_right", ReadAnalogButton(cleanedData[9]), cleanedData[9]);
                state.SetAnalog("wireless-analog_left", ReadAnalogButton(cleanedData[8]), cleanedData[8]);
                state.SetAnalog("wireless-analog_up", ReadAnalogButton(cleanedData[6]), cleanedData[6]);
                state.SetAnalog("wireless-analog_down", ReadAnalogButton(cleanedData[7]), cleanedData[7]);

                float x = 0, x_wireless = 0;
                float y = 0, y_wireless = 0;
                int xRaw = 0, x_wirelessRaw = 0;
                int yRaw = 0, y_wirelessRaw = 0;

                if (cleanedData[0] > 0)
                {
                    y = ReadAnalogButton(cleanedData[0]);
                    yRaw = cleanedData[0];
                }
                else if (cleanedData[1] > 0)
                {
                    y = -1 * ReadAnalogButton(cleanedData[1]);
                    yRaw = cleanedData[1];
                }
                if (cleanedData[2] > 0)
                {
                    x = -1 * ReadAnalogButton(cleanedData[2]);
                    xRaw = cleanedData[2];
                }
                else if (cleanedData[3] > 0)
                {
                    x = ReadAnalogButton(cleanedData[3]);
                    xRaw = cleanedData[3];
                }

                if (cleanedData[6] > 0)
                {
                    y_wireless = ReadAnalogButton(cleanedData[6]);
                    y_wirelessRaw = cleanedData[6];
                }
                else if (cleanedData[7] > 0)
                {
                    y_wireless = -1 * ReadAnalogButton(cleanedData[7]);
                    y_wirelessRaw = cleanedData[7];
                }

                if (cleanedData[8] > 0)
                {
                    x_wireless = -1 * ReadAnalogButton(cleanedData[8]);
                    x_wirelessRaw = cleanedData[8];
                }
                else if (cleanedData[9] > 0)
                {
                    x_wireless = ReadAnalogButton(cleanedData[9]);
                    x_wirelessRaw = cleanedData[9];
                }


                state.SetAnalog("wireless_stick_x", x_wireless, x_wirelessRaw);
                state.SetAnalog("wireless_stick_y", y_wireless, y_wirelessRaw);

                state.SetAnalog("stick_x", x, xRaw);
                state.SetAnalog("stick_y", y, yRaw);
                SignalTool.SetMouseProperties(x, y, xRaw, yRaw, state);

                return state.Build();
            }
            else
            {
                ControllerStateBuilder state = new ControllerStateBuilder();

                for (int i = 0; i < KEYBOARD_CODES.Length; ++i)
                {
                    if (KEYBOARD_CODES[i] != null)
                    {
                        state.SetButton(KEYBOARD_CODES[i], (packet[i / 8] & (1 << (i % 8))) != 0);
                    }
                }

                return state.Build();
            }
        }
    }
}