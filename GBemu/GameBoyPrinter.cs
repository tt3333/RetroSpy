using System.Text;

namespace GBPemu
{
    internal class GameBoyPrinter
    {

        public static ControllerStateEventArgs ReadFromPacket(byte[] packet)
        {
            ControllerStateBuilder state = new ControllerStateBuilder();

            state.SetPrinterData(Encoding.UTF8.GetString(packet, 0, packet.Length));

            return state.Build();
        }
    }
}