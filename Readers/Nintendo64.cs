namespace RetroSpy.Readers
{
    public static class Nintendo64
    {
        private const int PACKET_SIZE = 32;

        private static readonly string[] BUTTONS = {
            "a", "b", "z", "start", "up", "down", "left", "right", null, null, "l", "r", "cup", "cdown", "cleft", "cright"
        };

        private static float ReadStick(byte input)
        {
            return (float)((sbyte)input) / 128;
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

            float x = ReadStick(SignalTool.ReadByte(packet, BUTTONS.Length));
            float y = ReadStick(SignalTool.ReadByte(packet, BUTTONS.Length + 8));
            state.SetAnalog("stick_x", x);
            state.SetAnalog("stick_y", y);

            SignalTool.SetMouseProperties(x, y, state);

            return state.Build();
        }
    }
}