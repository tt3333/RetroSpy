using System;


namespace RetroSpy.Readers
{
    public static class Tg16
    {
        private const int PACKET_SIZE = 12;

        private static readonly string[] BUTTONS = {
            "up", "right", "down", "left", "1", "2", "select", "run", "3", "4", "5", "6"
        };

        public static ControllerStateEventArgs? ReadFromPacket(byte[]? packet)
        {
            if (packet == null)
            {
                throw new ArgumentNullException(nameof(packet));
            }

            if (packet.Length < PACKET_SIZE)
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

            return state.Build();
        }
    }
}