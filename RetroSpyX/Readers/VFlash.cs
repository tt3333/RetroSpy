using System;

namespace RetroSpy.Readers
{

    public static class VFlash
    {
        private const int PACKET_SIZE = 17;

        private static readonly string?[] BUTTONS = {
            "l", "r", "enter", "stick", "a", "b", "c", "d", "help", "menu", "exit", "yellow", "green", "blue", "red", null, null
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

            if (packet.Length == PACKET_SIZE)
            {
                ControllerStateBuilder state = new();

                for (int i = 0; i < BUTTONS.Length; ++i)
                {
                    if (string.IsNullOrEmpty(BUTTONS[i]))
                    {
                        continue;
                    }

                    state.SetButton(BUTTONS[i], packet[i] != 0x00);
                }

                state.SetAnalog("lstick_x", ReadStick(packet[15]), packet[15]);
                state.SetAnalog("lstick_y", ReadStick(packet[16]), packet[16]);

                return state.Build();
            }

            return null;
        }
    }
}
