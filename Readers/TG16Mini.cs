using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroSpy.Readers
{
    static public class Tg16Mini
    {
        const int PACKET_SIZE = 8;

        static readonly string[] BUTTONS = {
            "2", "1", "select", "run", "up", "right", "down", "left"
        };

        static public ControllerStateEventArgs ReadFromPacket (byte[] packet)
        {
            if(packet == null)
                throw new NullReferenceException();

            if (packet.Length < PACKET_SIZE) return null;

            var state = new ControllerStateBuilder ();

            for (int i = 0 ; i < BUTTONS.Length ; ++i) {
                if (string.IsNullOrEmpty (BUTTONS [i])) continue;
                state.SetButton (BUTTONS[i], packet[i] == 0x31);
            }

            return state.Build ();
        }
    }
}
