using System;

namespace RetroSpy.Readers
{
    public static class VCS
    {
        private const int PACKET_SIZE_CLASSIC = 11;

        private const int PACKET_SIZE_MODERN = 31;

        private static readonly string[] BUTTONS_1 = {
            "a", "b", "x", "y", "l", "r", "l3", "r3",
        };

        private static readonly string?[] BUTTONS_2 = {
            "back", "menu", "atari", null, null, null, null, null
        };

        private static readonly string[] ANALOGS = {
            "lstick_x", "lstick_y", "rstick_x", "rstick_y", "trig_l", "trig_r"
        };

        private static float ReadAnalog(short input, short maxValue = short.MaxValue)
        {
            return (float)input / maxValue;
        }

        public static ControllerStateEventArgs? ReadFromPacket(byte[]? packet)
        {
            if (packet == null)
            {
                throw new ArgumentNullException(nameof(packet));
            }

            if (packet.Length != PACKET_SIZE_MODERN && packet.Length != PACKET_SIZE_CLASSIC)
            {
                return null;
            }

            if (packet.Length == PACKET_SIZE_CLASSIC)
            {
                string str = System.Text.Encoding.Default.GetString(packet, 0, 10);

                byte[] binaryPacket = new byte[5];

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

                ControllerStateBuilder outState = new();

                for (int i = 0; i < BUTTONS_2.Length; ++i)
                {
                    if (string.IsNullOrEmpty(BUTTONS_2[i]))
                    {
                        continue;
                    }
                    outState.SetButton(BUTTONS_2[i], (binaryPacket[2] & (1 << i)) != 0);
                }

                for (int i = 0; i < BUTTONS_1.Length; ++i)
                {
                    if (string.IsNullOrEmpty(BUTTONS_1[i]))
                    {
                        continue;
                    }
                    outState.SetButton(BUTTONS_1[i], (binaryPacket[1] & (1 << i)) != 0);
                }

                GenerateDigitalDirections(binaryPacket, outState);

                return outState.Build();
            }
            else
            {
                string str = System.Text.Encoding.Default.GetString(packet, 0, 30);

                byte[] binaryPacket = new byte[15];

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

                ControllerStateBuilder outState = new();

                for (int i = 0; i < BUTTONS_2.Length; ++i)
                {
                    if (string.IsNullOrEmpty(BUTTONS_2[i]))
                    {
                        continue;
                    }
                    outState.SetButton(BUTTONS_2[i], (binaryPacket[2] & (1 << i)) != 0);
                }

                for (int i = 0; i < BUTTONS_1.Length; ++i)
                {
                    if (string.IsNullOrEmpty(BUTTONS_1[i]))
                    {
                        continue;
                    }
                    outState.SetButton(BUTTONS_1[i], (binaryPacket[1] & (1 << i)) != 0);
                }

                GenerateDigitalDirections(binaryPacket, outState);

                int j = 0;
                for (int i = 0; i < 6; ++i)
                {
                    short val = binaryPacket[3 + j];
                    val += (short)(binaryPacket[4 + j] << 8);
                    outState.SetAnalog(ANALOGS[i], i < 4 ? ReadAnalog(val) : ReadAnalog(val, 0x3ff), val);
                    j += 2;
                }

                return outState.Build();
            }
        }

        private static void GenerateDigitalDirections(byte[] binaryPacket, ControllerStateBuilder outState)
        {
            byte directions = (byte)(binaryPacket[2] >> 4);
            bool up = false;
            bool left = false;
            bool right = false;
            bool down = false;
            switch (directions)
            {
                case 1:
                    up = true;
                    left = false;
                    down = false;
                    right = false;
                    break;
                case 2:
                    up = true;
                    left = false;
                    down = false;
                    right = true;
                    break;
                case 3:
                    up = false;
                    left = false;
                    down = false;
                    right = true;
                    break;
                case 4:
                    up = false;
                    left = false;
                    down = true;
                    right = true;
                    break;
                case 5:
                    up = false;
                    left = false;
                    down = true;
                    right = false;
                    break;
                case 6:
                    up = false;
                    left = true;
                    down = true;
                    right = false;
                    break;
                case 7:
                    up = false;
                    left = true;
                    down = false;
                    right = false;
                    break;
                case 8:
                    up = true;
                    left = true;
                    down = false;
                    right = false;
                    break;
            }

            outState.SetButton("up", up);
            outState.SetButton("left", left);
            outState.SetButton("down", down);
            outState.SetButton("right", right);

            SignalTool.GenerateFakeStick(outState, "x_stick", "y_stick", up, down, left, right);
        }
    }
}
