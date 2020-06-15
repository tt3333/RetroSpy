using System;

namespace RetroSpy.Readers
{
    public static class SS3D
    {
        private const int PACKET_SIZE = 56;
        private const int POLISHED_PACKET_SIZE = 21;

        private static readonly string[] BUTTONS = {
            null, "right", "left", "down", "up", "start", "A", "C", "B", "R", "X", "Y", "Z", "L"
        };

        private static readonly string[] MOUSE_BUTTONS = {
            null, null, null, null, "start", "middle", "right", "left"
        };

        private static float ReadTrigger(byte input)
        {
            return (float)(input) / 256;
        }

        private static float ReadStick(byte input)
        {
            return (float)(input - 128) / 128;
        }

        private static float ReadMouse(bool sign, bool over, byte data)
        {
            float val;
            if (over)
            {
                val = 1.0f;
            }
            else if (sign)
            {
                val = 0xFF - data;
            }
            else
            {
                val = data;
            }

            return val * (sign ? -1 : 1) / 255;
        }

        public static ControllerStateEventArgs ReadFromPacket(byte[] packet)
        {
            if (packet == null)
                throw new NullReferenceException();

            if (packet.Length < PACKET_SIZE)
            {
                return null;
            }

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
                        polishedPacket[17 + i] |= (byte)((packet[24 + (i * 8 + k)] == 0 ? 0 : 1) << j);
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
                    state.SetAnalog("lstick_x", ReadStick(polishedPacket[17]));
                    state.SetAnalog("lstick_y", ReadStick(polishedPacket[18]));
                    state.SetAnalog("trig_r", ReadTrigger(polishedPacket[19]));
                    state.SetAnalog("trig_l", ReadTrigger(polishedPacket[20]));
                }
                else
                {
                    state.SetAnalog("lstick_x", ReadStick(128));
                    state.SetAnalog("lstick_y", ReadStick(128));
                    state.SetAnalog("trig_r", ReadTrigger(0));
                    state.SetAnalog("trig_l", ReadTrigger(0));
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

                state.SetAnalog("lstick_x", ReadStick(128));
                state.SetAnalog("lstick_y", ReadStick(128));
                state.SetAnalog("trig_r", ReadTrigger(0));
                state.SetAnalog("trig_l", ReadTrigger(0));

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

                SignalTool.SetMouseProperties(x, y, state);
            }

            return state.Build();
        }
    }
}