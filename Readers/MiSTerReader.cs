using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroSpy.Readers
{
    static public class MiSTerReader
    {

        static readonly string[] AXES_NAMES = {
            "x", "y", "z", "rx", "ry", "rz", "s0", "s1"
        };


        static public ControllerState ReadFromPacket(byte[] packet)
        {
            if (packet.Length < 16) return null;

            int axes = 0;
            for (byte j = 0; j < 8; ++j)
            {
                axes |= (int)((packet[j] == 0x30 ? 0 : 1) << j);
            }

            int buttons = 0;
            for (byte j = 0; j < 8; ++j)
            {
                buttons |= (int)((packet[8+j] == 0x30 ? 0 : 1) << j);
            }

            int packetSize = 2 + (axes * 32) + buttons;

            if (packet.Length != packetSize) return null;

            byte[] buttonValues = new byte[buttons];
            int[] axesValues = new int[axes];

            for (int i = 0; i < buttons; ++i)
                buttonValues[i] = (byte)((packet[i] == 0x31) ? 1 : 0);

            for (int i = 0; i < axes; ++i)
            {
                axesValues[i] = 0;
                for (byte j = 0; j < 32; ++j)
                {
                    axesValues[i] |= (packet[buttons + i * 32 + j] == 0x30 ? 0 : 1) << j;
                }
            }

            var outState = new ControllerStateBuilder();

            for (int i = 0; i < buttonValues.Length; ++i)
            {
                outState.SetButton("b" + i.ToString(), buttonValues[i] != 0x00);
            }

            for(int i = 0; i < axesValues.Length; ++i)
            {
                if (i >= AXES_NAMES.Length)
                    outState.SetAnalog(AXES_NAMES[i], axesValues[i] / short.MaxValue);

                outState.SetAnalog("a" + i, axesValues[i] / short.MaxValue);
            }

            if (axes >= 2)
            {
                if (axesValues[axes - 2] < 0)
                    outState.SetButton("left", true);
                else if (axesValues[axes - 2] > 0)
                    outState.SetButton("right", true);
                else
                {
                    outState.SetButton("left", false);
                    outState.SetButton("right", false);
                }

                if (axesValues[axes - 1] < 0)
                    outState.SetButton("up", true);
                else if (axesValues[axes - 1] > 0)
                    outState.SetButton("down", true);
                else
                {
                    outState.SetButton("up", false);
                    outState.SetButton("down", false);
                }
            }
            else
            {
                outState.SetButton("up", false);
                outState.SetButton("down", false);
                outState.SetButton("left", false);
                outState.SetButton("right", false);
            }

            return outState.Build();

        }
    }
}
