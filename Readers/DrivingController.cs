using System;

namespace RetroSpy.Readers
{
    public static class DrivingController
    {
        private const int PACKET_SIZE = 2;

        private static readonly string[] BUTTONS = {
            "fire", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15"
        };

        public static ControllerStateEventArgs ReadFromPacket(byte[] packet)
        {
            if (packet == null)
                throw new NullReferenceException();

            if (packet.Length < PACKET_SIZE)
            {
                return null;
            }

            ControllerStateBuilder state = new ControllerStateBuilder();

            state.SetButton(BUTTONS[0], packet[0] != 0);

            for (int i = 1; i < BUTTONS.Length; ++i)
            {
                if (string.IsNullOrEmpty(BUTTONS[i]))
                {
                    continue;
                }

                state.SetButton(BUTTONS[i], (i - 1 + 65) == packet[1]);
            }

            return state.Build();
        }
    }
}