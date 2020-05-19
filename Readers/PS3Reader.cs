using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroSpy.Readers
{
    static public class PS3Reader
    {
        const int PACKET_SIZE = 153;
        const int POLISHED_PACKET_SIZE = 40;

        static readonly string[] BUTTONS = {
            "select", "lstick", "rstick", "start", "up", "right", "down", "left", "l2", "r2", "l1", "r1", "triangle", "circle", "x", "square", "ps"
        };


        static float ReadAnalogButton(byte input)
        {
            return (float)input / 256;
        }
        static float ReadStick(byte input)
        {
            return (float)(input - 128) / 128;
        }


        static public ControllerState ReadFromPacket(byte[] packet)
        {
            if (packet.Length < PACKET_SIZE) return null;

            byte[] polishedPacket = new byte[POLISHED_PACKET_SIZE];

            for (int i = 0; i < 24; ++i)
                polishedPacket[i] = (byte)((packet[i] == 0x31) ? 1 : 0);

            for (int i = 0; i < 16; ++i)
            {
                polishedPacket[24 + i] = 0;
                for (byte j = 0; j < 8; ++j)
                {
                    polishedPacket[24 + i] |= (byte)((packet[24 + (i * 8 + j)] == 0x30 ? 0 : 1) << j);
                }
            }

            var outState = new ControllerStateBuilder();

            for (int i = 0; i < BUTTONS.Length; ++i)
            {
                if (string.IsNullOrEmpty(BUTTONS[i])) continue;
                outState.SetButton(BUTTONS[i], polishedPacket[i] != 0x00);
            }

            outState.SetAnalog("rstick_x", ReadStick(polishedPacket[26]));
            outState.SetAnalog("rstick_y", ReadStick(polishedPacket[27]));
            outState.SetAnalog("lstick_x", ReadStick(polishedPacket[24]));
            outState.SetAnalog("lstick_y", ReadStick(polishedPacket[25]));


            outState.SetAnalog("analog_up", ReadAnalogButton(polishedPacket[28]));
            outState.SetAnalog("analog_right", ReadAnalogButton(polishedPacket[29]));
            outState.SetAnalog("analog_down", ReadAnalogButton(polishedPacket[30]));
            outState.SetAnalog("analog_left", ReadAnalogButton(polishedPacket[31]));
            outState.SetAnalog("l_trig", ReadAnalogButton(polishedPacket[32]));
            outState.SetAnalog("r_trig", ReadAnalogButton(polishedPacket[33]));
            outState.SetAnalog("analog_l1", ReadAnalogButton(polishedPacket[34]));
            outState.SetAnalog("analog_r1", ReadAnalogButton(polishedPacket[35]));
            outState.SetAnalog("analog_triangle", ReadAnalogButton(polishedPacket[36]));
            outState.SetAnalog("analog_circle", ReadAnalogButton(polishedPacket[37]));
            outState.SetAnalog("analog_x", ReadAnalogButton(polishedPacket[38]));
            outState.SetAnalog("analog_square", ReadAnalogButton(polishedPacket[39]));


            return outState.Build();

        }
    }
}
