using System;

namespace RetroSpy.Readers
{
    public static class CDi
    {
        private const int PACKET_SIZE = 24;

        private static readonly string[] BUTTONS = {
            "wired-up", "wired-down", "wired-left", "wired-right", "wired-1", "wired-2",
            "wireless-up", "wireless-down", "wireless-left", "wireless-right", "wireless-1", "wireless-2"
        };

        private static float ReadAnalogButton(byte input)
        {
            return (float)(input) / 256;
        }

        public static ControllerStateEventArgs ReadFromPacket(byte[] packet)
        {
            if (packet == null)
                throw new NullReferenceException();

            if (packet.Length < PACKET_SIZE)
            {
                return null;
            }

            byte[] cleanedData = new byte[12];
            for (int i = 0; i < 24; i += 2)
            {
                cleanedData[i / 2] = (byte)((packet[i]) | ((packet[i + 1]) >> 4));
            }

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

            state.SetAnalog("wired-analog_right", ReadAnalogButton(cleanedData[3]));
            state.SetAnalog("wired-analog_left", ReadAnalogButton(cleanedData[2]));
            state.SetAnalog("wired-analog_up", ReadAnalogButton(cleanedData[0]));
            state.SetAnalog("wired-analog_down", ReadAnalogButton(cleanedData[1]));

            state.SetAnalog("wireless-analog_right", ReadAnalogButton(cleanedData[9]));
            state.SetAnalog("wireless-analog_left", ReadAnalogButton(cleanedData[8]));
            state.SetAnalog("wireless-analog_up", ReadAnalogButton(cleanedData[6]));
            state.SetAnalog("wireless-analog_down", ReadAnalogButton(cleanedData[7]));

            float x = 0;
            float y = 0;

            if (cleanedData[2] > 0)
            {
                x = -1 * ReadAnalogButton(cleanedData[2]);
            }
            else if (cleanedData[3] > 0)
            {
                x = ReadAnalogButton(cleanedData[3]);
            }

            if (cleanedData[0] > 0)
            {
                y = ReadAnalogButton(cleanedData[0]);
            }
            else if (cleanedData[1] > 0)
            {
                y = -1 * ReadAnalogButton(cleanedData[1]);
            }

            state.SetAnalog("stick_x", x);
            state.SetAnalog("stick_y", y);
            SignalTool.SetMouseProperties(x, y, state);

            return state.Build();
        }
    }
}