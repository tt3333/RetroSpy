using System;

namespace RetroSpy.Readers
{
    public static class Xbox360Reader
    {
        private const int PACKET_SIZE = 96;
        private const int POLISHED_PACKET_SIZE = 18;

        private static readonly string[] BUTTONS = {
            "up", "down", "left", "right", "start", "back", "l3", "r3", "lb", "rb", "xbox", null, "a", "b", "x", "y"
        };

        private static float ReadTrigger(byte input)
        {
            return (float)input / 255;
        }

        private static float ReadStick(short input)
        {
            return (float)input / 32768;
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

            for (int i = 0; i < 16; ++i)
            {
                polishedPacket[i] = (byte)((packet[i] == 0x31) ? 1 : 0);
            }

            for (int i = 0; i < 2; ++i)
            {
                packet[16 + i] = 0;
                for (byte j = 0; j < 8; ++j)
                {
                    polishedPacket[16 + i] |= (byte)((packet[16 + (i * 8 + j)] == 0x30 ? 0 : 1) << j);
                }
            }

            short[] sticks = new short[4];

            for (int i = 0; i < 4; ++i)
            {
                sticks[i] = 0;
                for (byte j = 0; j < 16; ++j)
                {
                    sticks[i] |= (short)((packet[32 + (i * 16 + j)] == 0x30 ? 0 : 1) << j);
                }
            }

            ControllerStateBuilder outState = new ControllerStateBuilder();

            for (int i = 0; i < BUTTONS.Length; ++i)
            {
                if (string.IsNullOrEmpty(BUTTONS[i]))
                {
                    continue;
                }

                outState.SetButton(BUTTONS[i], polishedPacket[i] != 0x00);
            }

            outState.SetAnalog("trig_l", ReadTrigger(polishedPacket[16]));
            outState.SetAnalog("trig_r", ReadTrigger(polishedPacket[17]));

            outState.SetAnalog("rstick_x", ReadStick(sticks[2]));
            outState.SetAnalog("rstick_y", ReadStick(sticks[3]));
            outState.SetAnalog("lstick_x", ReadStick(sticks[0]));
            outState.SetAnalog("lstick_y", ReadStick(sticks[1]));

            return outState.Build();
        }
    }
}