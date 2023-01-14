using System;

namespace RetroSpy.Readers
{
    public static class EVS
    {
        private const int PACKET_SIZE = 41;

        private static readonly string[] BUTTONS_1 = {
            "up", "down", "left", "right", "start", "select", "l2", "r2",
        };

        private static readonly string?[] BUTTONS_2 = {
            "l1", "r1", "ev", null, "a", "b", "x", "y"
        };

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

            string str = System.Text.Encoding.Default.GetString(packet, 0, 40);

            byte[] binaryPacket = new byte[20];

            try
            {
                for (int i = 0; i < str.Length; i += 2)
                {
                    binaryPacket[i / 2] = Convert.ToByte(str.Substring(i, 2), 16);
                }
            }
            catch (ArgumentException)
            {
                return null;
            }
            catch (FormatException)
            {
                return null;
            }
            catch (OverflowException)
            {
                return null;
            }

            ControllerStateBuilder outState = new();

            for (int i = 0; i < BUTTONS_2.Length; ++i)
            {
                if (string.IsNullOrEmpty(BUTTONS_2[i]))
                {
                    continue;
                }
                outState.SetButton(BUTTONS_2[i], (binaryPacket[3] & (1 << i)) != 0);
            }

            for (int i = 0; i < BUTTONS_1.Length; ++i)
            {
                if (string.IsNullOrEmpty(BUTTONS_1[i]))
                {
                    continue;
                }
                outState.SetButton(BUTTONS_1[i], (binaryPacket[2] & (1 << i)) != 0);
            }

            return outState.Build();

        }
    }
}