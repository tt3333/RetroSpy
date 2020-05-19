			using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroSpy.Readers
{
    static public class PS4Reader
    {
        const int PACKET_SIZE = 153;
        const int PACKET_HEADER = 21;
        const int POLISHED_PACKET_SIZE = 34;

        static readonly string[] BUTTONS = {
            "up", "down", "left", "right", "x", "circle", "square", "triangle", "l1", "l2", "lstick", "r1", "r2", "rstick", "share", "options", "ps", "trackpad", "trackpad0_touch", "trackpad1_touch"
        };

        static float ReadStick(byte input)
        {
            return (float)(input - 128) / 128;
        }

        static float ReadAnalogButton(byte input)
        {
            return (float)input / 256;
        }

        static float ReadTouchPad(int input, int maxValue)
        {
            if (input > maxValue)
                return 1.0f;

            if (input < 0)
                return 0.0f;

            return input / (float)maxValue;
        }

        static public ControllerState ReadFromPacket(byte[] packet)
        {
            if (packet.Length < PACKET_SIZE) return null;

            byte[] polishedPacket = new byte[POLISHED_PACKET_SIZE];

            for (int i = 0; i < 20; ++i)
                polishedPacket[i] = (byte)((packet[PACKET_HEADER+i] == 0x31) ? 1 : 0);

            for (int i = 0; i < 14; ++i)
            {
                polishedPacket[20 + i] = 0;
                for (byte j = 0; j < 8; ++j)
                {
                    polishedPacket[20 + i] |= (byte)((packet[PACKET_HEADER + 20 + (i * 8 + j)] == 0x30 ? 0 : 1) << j);
                }
            }

            var outState = new ControllerStateBuilder();

            for (int i = 0; i < BUTTONS.Length; ++i)
            {
                if (string.IsNullOrEmpty(BUTTONS[i])) continue;
                outState.SetButton(BUTTONS[i], polishedPacket[i] != 0x00);
            }

            outState.SetAnalog("rstick_x", ReadStick(polishedPacket[22]));
            outState.SetAnalog("rstick_y", ReadStick(polishedPacket[23]));
            outState.SetAnalog("lstick_x", ReadStick(polishedPacket[20]));
            outState.SetAnalog("lstick_y", ReadStick(polishedPacket[21]));

            outState.SetAnalog("l_trig", ReadStick(polishedPacket[24]));
            outState.SetAnalog("r_trig", ReadStick(polishedPacket[25]));

            int touchpad_x1 = (polishedPacket[27] << 8) | polishedPacket[26];
            int touchpad_y1 = (polishedPacket[29] << 8) | polishedPacket[28];
            int touchpad_x2 = (polishedPacket[31] << 8) | polishedPacket[30];
            int touchpad_y2 = (polishedPacket[33] << 8) | polishedPacket[32];

            if (polishedPacket[18] == 1) // touch
            {
                if (polishedPacket[17] == 1) // click
                {
                    outState.SetAnalog("touchpad_x3", ReadTouchPad(touchpad_x1, 1920));
                    outState.SetAnalog("touchpad_y3", ReadTouchPad(touchpad_y1, 943));
                }
                else
                {
                    outState.SetAnalog("touchpad_x1", ReadTouchPad(touchpad_x1, 1920));
                    outState.SetAnalog("touchpad_y1", ReadTouchPad(touchpad_y1, 943));
                }
            }

            if (polishedPacket[19] == 1) // touch
            {
                if (polishedPacket[17] == 1) // click
                {
                    outState.SetAnalog("touchpad_x4", ReadTouchPad(touchpad_x2, 1920));
                    outState.SetAnalog("touchpad_y4", ReadTouchPad(touchpad_y2, 943));
                }
                else
                {
                    outState.SetAnalog("touchpad_x2", ReadTouchPad(touchpad_x2, 1920));
                    outState.SetAnalog("touchpad_y2", ReadTouchPad(touchpad_y2, 943));
                }
            }

            return outState.Build();
        }
    }
}
