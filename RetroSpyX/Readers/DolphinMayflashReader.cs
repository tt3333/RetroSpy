using System;

namespace RetroSpy.Readers
{
    public static class DolphinMayflashReader
    {
        private const int PACKET_SIZE = 72;

        private static readonly string?[] BUTTONS = {
            "a", "b", "x", "y", "left", "right", "down", "up", "start", "z", "r", "l", null, null, null, null
        };
        private static float ReadStick(byte input)
        {
            return (float)(input - 128) / 128;
        }

        private static float ReadTrigger(byte input, float maxVal = 256)
        {
            return (float)input / maxVal;
        }

        private static readonly byte[] keyboardData = new byte[3];

        public static ControllerStateEventArgs? ReadFromPacket(byte[]? packet)
        {
            int offset = 0;
            if (packet == null)
            {
                throw new ArgumentNullException(nameof(packet));
            }

            if (packet.Length != PACKET_SIZE)
            {
                return null;
            }

            ControllerStateBuilder state = new();

            for (int i = 0; i < BUTTONS.Length; ++i)
            {
                if (string.IsNullOrEmpty(BUTTONS[i]))
                {
                    continue;
                }

                state.SetButton(BUTTONS[i], packet[i+8] != (byte)'0');
            }

            state.SetAnalog("lstick_x", ReadStick(SignalTool.ReadByteBackwards(packet,8+BUTTONS.Length)), SignalTool.ReadByteBackwards(packet, 8+BUTTONS.Length));
            state.SetAnalog("lstick_y", ReadStick(SignalTool.ReadByteBackwards(packet,8+BUTTONS.Length + 8)), SignalTool.ReadByteBackwards(packet, 8+BUTTONS.Length + 8));
            state.SetAnalog("cstick_x", ReadStick(SignalTool.ReadByteBackwards(packet,8+BUTTONS.Length + 16)), SignalTool.ReadByteBackwards(packet,8+BUTTONS.Length + 16));
            state.SetAnalog("cstick_y", ReadStick(SignalTool.ReadByteBackwards(packet,8+BUTTONS.Length + 24)), SignalTool.ReadByteBackwards(packet, 8+BUTTONS.Length + 24));
            
            state.SetAnalog("trig_l", ReadTrigger(SignalTool.ReadByteBackwards(packet,8+BUTTONS.Length + 32)), SignalTool.ReadByteBackwards(packet, 8+BUTTONS.Length + 32));
            state.SetAnalog("trig_r", ReadTrigger(SignalTool.ReadByteBackwards(packet,8+BUTTONS.Length + 40)), SignalTool.ReadByteBackwards(packet, 8+BUTTONS.Length + 40));

            return state.Build();
        }
    }
}
