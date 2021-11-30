using System;

namespace RetroSpy.Readers
{
    public static class Paddles
    {
        private const int PACKET_SIZE = 6;

        private static float ReadPaddle(ushort input)
        {
            return (float)(input) / 256;
        }

        private static bool SecondButton;
        private static float SecondPaddle;
        private static int SecondPaddleRaw;

        public static ControllerStateEventArgs ReadFromPacket(byte[] packet)
        {
            if (packet == null)
                throw new ArgumentNullException(nameof(packet));

            if (packet.Length < PACKET_SIZE)
            {
                return null;
            }

            if (packet[4] != 5)
            {
                return null;
            }

            ControllerStateBuilder state = new ControllerStateBuilder();

            state.SetAnalog("paddle", ReadPaddle(packet[2]), packet[2]);
            state.SetAnalog("paddle2", SecondPaddle, SecondPaddleRaw);
            state.SetButton("fire2", SecondButton);
            state.SetButton("fire", (packet[1] != 0x00));

            return state.Build();
        }

        public static ControllerStateEventArgs ReadFromSecondPacket(byte[] packet)
        {
            if (packet == null)
                throw new ArgumentNullException(nameof(packet));

            if (packet.Length < PACKET_SIZE)
            {
                return null;
            }

            if (packet[4] != 5)
            {
                return null;
            }

            SecondButton = (packet[1] != 0x00);
            SecondPaddle = ReadPaddle(packet[2]);
            SecondPaddleRaw = packet[2];

            return null;
        }
    }
}