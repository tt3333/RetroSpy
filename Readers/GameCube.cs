namespace RetroSpy.Readers
{
    public static class GameCube
    {
        private const int PACKET_SIZE = 64;

        private static readonly string[] BUTTONS = {
            null, null, null, "start", "y", "x", "b", "a", null, "l", "r", "z", "up", "down", "right", "left"
        };

        private static float ReadStick(byte input)
        {
            return (float)(input - 128) / 128;
        }

        private static float ReadTrigger(byte input)
        {
            return (float)(input) / 256;
        }

        public static ControllerState ReadFromPacket(byte[] packet)
        {
            if (packet.Length != PACKET_SIZE)
            {
                return null;
            }

            ControllerStateBuilder state = new ControllerStateBuilder();

            for (int i = 0; i < BUTTONS.Length; ++i)
            {
                if (string.IsNullOrEmpty(BUTTONS[i]))
                {
                    continue;
                }

                state.SetButton(BUTTONS[i], packet[i] != 0x00);
            }

            state.SetAnalog("lstick_x", ReadStick(SignalTool.ReadByte(packet, BUTTONS.Length)));
            state.SetAnalog("lstick_y", ReadStick(SignalTool.ReadByte(packet, BUTTONS.Length + 8)));
            state.SetAnalog("cstick_x", ReadStick(SignalTool.ReadByte(packet, BUTTONS.Length + 16)));
            state.SetAnalog("cstick_y", ReadStick(SignalTool.ReadByte(packet, BUTTONS.Length + 24)));
            state.SetAnalog("trig_l", ReadTrigger(SignalTool.ReadByte(packet, BUTTONS.Length + 32)));
            state.SetAnalog("trig_r", ReadTrigger(SignalTool.ReadByte(packet, BUTTONS.Length + 40)));

            return state.Build();
        }
    }
}