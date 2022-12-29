using System;

namespace RetroSpy.Readers
{

    public static class Nuon
    {
        private const int PACKET_SIZE = 18;

        private static readonly string?[] BUTTONS =
        {
            "CD", "A", "start", "nuon", "down", "left", "up", "right", null, null, "LT", "RT", "B", "CL", "CU", "CR"
        };
        private static float ReadStick(byte input)
        {
            return (float)(sbyte)input / 128;
        }

        public static ControllerStateEventArgs? ReadFromPacket(byte[]? packet)
        {

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

                state.SetButton(BUTTONS[i], packet[i] != 0x00);
            }

            float x = ReadStick(packet[16]);
            float y = ReadStick(packet[17]);

            state.SetAnalog("stick_x", x, packet[16]);
            state.SetAnalog("stick_y", y, packet[17]);

            return state.Build();
        }
    }
}
