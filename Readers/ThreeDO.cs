using System;

namespace RetroSpy.Readers
{
    public static class ThreeDO
    {
        private const int PACKET_SIZE = 16;
        private const int MOUSE_PACKET_SIZE = 32;

        private static readonly string[] BUTTONS = {
            null, null,"down", "up", "right", "left", "a", "b", "c","p", "x","r", "l", null, null, null
        };

        private static readonly string[] MOUSE_BUTTONS = {
            null, null, null, null, null, null, null, "left", "middle", "right", null
        };

        private static float ReadMouse(bool sign, byte data)
        {
            float val;
            if (sign)
            {
                val = -1 * (0x7F - data);
            }
            else
            {
                val = data;
            }

            return val / 127;
        }

        public static ControllerStateEventArgs ReadFromPacket(byte[] packet)
        {
            if (packet == null)
                throw new NullReferenceException();

            if (packet.Length < 1)
            {
                return null;
            }

            ControllerStateBuilder state = new ControllerStateBuilder();

            if (packet[0] == 0)
            {
                if (packet.Length < PACKET_SIZE)
                {
                    return null;
                }

                for (int i = 0; i < BUTTONS.Length; ++i)
                {
                    if (string.IsNullOrEmpty(BUTTONS[i]))
                    {
                        continue;
                    }

                    state.SetButton(BUTTONS[i], packet[i] != 0x00);
                }
            }
            else
            {
                if (packet.Length < MOUSE_PACKET_SIZE)
                {
                    return null;
                }

                for (int i = 0; i < MOUSE_BUTTONS.Length; ++i)
                {
                    if (string.IsNullOrEmpty(MOUSE_BUTTONS[i]))
                    {
                        continue;
                    }

                    state.SetButton(MOUSE_BUTTONS[i], packet[i] != 0x00);
                }

                bool ySign = packet[11] != 0;
                byte yVal = SignalTool.ReadByte(packet, 14, 7);
                bool xSign = packet[21] != 0;
                byte xVal = SignalTool.ReadByte(packet, 24, 7);

                float x = ReadMouse(xSign, xVal);
                float y = ReadMouse(ySign, yVal);

                SignalTool.SetMouseProperties(x, y, state);
            }
            return state.Build();
        }
    }
}