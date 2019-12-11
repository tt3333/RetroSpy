using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroSpy.Readers
{
    static public class MiSTerReader
    {
        const int PRO_PACKET_SIZE = 268;

        static readonly string[] BUTTONS = {
            "b", "a", "y", "x", "l", "r", "select", "start", "9", "10", "11"
        };


        static public ControllerState ReadFromPacket(byte[] packet)
        {
            if (packet.Length != PRO_PACKET_SIZE) return null;

            byte[] polishedPacket = new byte[BUTTONS.Length];
            int[] intPacket = new int[8];

            for (int i = 0; i < 11; ++i)
                polishedPacket[i] = (byte)((packet[i] == 0x31) ? 1 : 0);

            for (int i = 0; i < 8; ++i)
            {
                intPacket[i] = 0;
                for (byte j = 0; j < 32; ++j)
                {
                    intPacket[i] |= (int)((packet[11 + (i * 32 + j)] == 0x30 ? 0 : 1) << j);
                }
            }

            var outState = new ControllerStateBuilder();

            for (int i = 0; i < BUTTONS.Length; ++i)
            {
                if (string.IsNullOrEmpty(BUTTONS[i])) continue;
                outState.SetButton(BUTTONS[i], polishedPacket[i] != 0x00);
            }

            if (intPacket[6] < 0)
                outState.SetButton("left", true);
            else if (intPacket[6] > 0)
                outState.SetButton("right", true);
            else
            {
                outState.SetButton("left", false);
                outState.SetButton("right", false);
            }

            if (intPacket[7] < 0)
                outState.SetButton("up", true);
            else if (intPacket[7] > 0)
                outState.SetButton("down", true);
            else
            {
                outState.SetButton("up", false);
                outState.SetButton("down", false);
            }

            return outState.Build();

        }
    }
}
