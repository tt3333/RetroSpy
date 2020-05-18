using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroSpy.Readers
{
    static public class GameCube
    {
        const int PACKET_SIZE = 64;

        static readonly string[] BUTTONS = {
            null, null, null, "start", "y", "x", "b", "a", null, "l", "r", "z", "up", "down", "right", "left"
        };

        static float ReadStick (byte input) {
            return (float)(input - 128) / 128;
        }

        static float ReadTrigger (byte input) {
            return (float)(input) / 256;
        }

        static public ControllerState ReadFromPacket (byte[] packet)
        {
            if (packet.Length != PACKET_SIZE) return null;

            var state = new ControllerStateBuilder ();

            for (int i = 0 ; i < BUTTONS.Length ; ++i) {
                if (string.IsNullOrEmpty (BUTTONS [i])) continue;
                state.SetButton (BUTTONS[i], packet[i] != 0x00);
            }

            state.SetAnalog ("lstick_x", ReadStick (SignalTool.ReadByte (packet, BUTTONS.Length     )));
            state.SetAnalog ("lstick_y", ReadStick (SignalTool.ReadByte (packet, BUTTONS.Length +  8)));
            state.SetAnalog ("cstick_x", ReadStick (SignalTool.ReadByte (packet, BUTTONS.Length + 16)));
            state.SetAnalog ("cstick_y", ReadStick (SignalTool.ReadByte (packet, BUTTONS.Length + 24)));
            state.SetAnalog ("trig_l", ReadTrigger (SignalTool.ReadByte (packet, BUTTONS.Length + 32)));
            state.SetAnalog ("trig_r", ReadTrigger (SignalTool.ReadByte (packet, BUTTONS.Length + 40)));

            return state.Build ();
        }
    }
}
