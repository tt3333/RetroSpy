using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroSpy.Readers
{
    public static class A500
    {
        private const int CONTROLLER_PACKET_SIZE = 17;
        private const int MOUSE_PACKET_SIZE = 9;

        private static readonly string[] BUTTONS_1 = {
            "backward", "forward", "menu", "home"
        };

        private static readonly string?[] BUTTONS_2 = {
            null, null, null, null, "yellow", "blue", "red", "green"
        };

        public static ControllerStateEventArgs? ReadFromPacket(byte[]? packet)
        {
            if (packet == null)
            {
                throw new ArgumentNullException(nameof(packet));
            }

            if (packet.Length != CONTROLLER_PACKET_SIZE && packet.Length != MOUSE_PACKET_SIZE)
            {
                return null;
            }

            string str = System.Text.Encoding.Default.GetString(packet, 0, packet.Length-1);

            byte[] binaryPacket = new byte[(packet.Length-1)/2];

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

            if (packet.Length == CONTROLLER_PACKET_SIZE)
            {

                for (int i = 0; i < BUTTONS_2.Length; ++i)
                {
                    if (string.IsNullOrEmpty(BUTTONS_2[i]))
                    {
                        continue;
                    }
                    outState.SetButton(BUTTONS_2[i], (binaryPacket[5] & (1 << i)) != 0);
                }

                for (int i = 0; i < BUTTONS_1.Length; ++i)
                {
                    if (string.IsNullOrEmpty(BUTTONS_1[i]))
                    {
                        continue;
                    }
                    outState.SetButton(BUTTONS_1[i], (binaryPacket[6] & (1 << i)) != 0);
                }

                outState.SetButton("left", binaryPacket[0] == 0);
                outState.SetButton("right", binaryPacket[0] == 255);
                outState.SetButton("up", binaryPacket[1] == 0);
                outState.SetButton("down", binaryPacket[1] == 255);
            }
            else
            {
                outState.SetButton("left_button", (binaryPacket[0] & 0x01) != 0x00);
                outState.SetButton("right_button", (binaryPacket[0] & 0x02) != 0x00);

                sbyte xVal = (sbyte)binaryPacket[1];
                sbyte yVal = (sbyte)binaryPacket[2];

                SignalTool.SetMouseProperties(xVal / 128.0f, yVal / -128.0f, xVal, yVal, outState);
            }

            return outState.Build();

        }
    }
}