using System;

namespace RetroSpy.Readers
{
    public static class XboxReaderV2
    {
        private const int PACKET_SIZE = 41;

        private static readonly string[] BUTTONS = {
            "up", "down", "left", "right", "start", "back", "l3", "r3"
        };

        private static readonly string[] ANALOG_BUTTONS = {
            "a", "b", "x", "y", "black", "white", "trig_l", "trig_r"
        };

        private static readonly string[] STICKS = {
            "lstick_x", "lstick_y", "rstick_x", "rstick_y"
        };

        private static float ReadTrigger(byte input)
        {
            return (float)(input) / 256;
        }

        private static float ReadStick(short input)
        {
            return (float)input / short.MaxValue;
        }

        public static ControllerStateEventArgs ReadFromPacket(byte[] packet)
        {
            if (packet == null)
                throw new NullReferenceException();

            if (packet.Length != PACKET_SIZE)
            {
                return null;
            }

            string str = System.Text.Encoding.Default.GetString(packet, 0, 40);

            byte[] binaryPacket = new byte[20];

            try
            {
                for (int i = 0; i < str.Length; i += 2)
                {
                    binaryPacket[i / 2] = Convert.ToByte(str.Substring(i, 2), 16);
                }
            }
            catch (ArgumentException)
            {
                return null;
            }
            catch (FormatException)
            {
                return null;
            }
            catch (OverflowException)
            {
                return null;
            }

            ControllerStateBuilder outState = new ControllerStateBuilder();

            for (int i = 0; i < 8; ++i)
            {
                outState.SetButton(BUTTONS[i], (binaryPacket[2] & (1 << i)) != 0);
            }

            for (int i = 4; i < 12; ++i)
            {
                outState.SetButton(ANALOG_BUTTONS[i - 4], binaryPacket[i] > 0);
                outState.SetAnalog(ANALOG_BUTTONS[i - 4], ReadTrigger(binaryPacket[i]));
            }

            int j = 0;
            for (int i = 0; i < 4; ++i)
            {
                short val = binaryPacket[12 + j];
                val += (short)(binaryPacket[13 + j] << 8);
                outState.SetAnalog(STICKS[i], ReadStick(val));
                j += 2;
            }

            return outState.Build();
        }
    }
}