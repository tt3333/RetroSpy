using System;

namespace RetroSpy.Readers
{
    public static class AtariKeyboard
    {
        private const int PACKET_SIZE = 1;

        private static readonly string?[] BUTTONS = {
            null, "1", "2", "3", "4", "5", "6", "7", "8", "9", "star", "0", "pound"
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

            ControllerStateBuilder controllerStateBuilder = new();
            ControllerStateBuilder state = controllerStateBuilder;

            for (int i = 0; i < BUTTONS.Length; ++i)
            {
                if (string.IsNullOrEmpty(BUTTONS[i]))
                {
                    continue;
                }

                state.SetButton(BUTTONS[i], (i + 65) == packet[0]);
            }

            return state.Build();
        }
    }
}