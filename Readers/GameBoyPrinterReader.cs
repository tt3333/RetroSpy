using System;

namespace RetroSpy.Readers
{
    public sealed class GameBoyPrinterReader : IControllerReader, IDisposable
    {
        public event StateEventHandler ControllerStateChanged;

        public event EventHandler ControllerDisconnected;

        private readonly Func<byte[], ControllerStateEventArgs> _packetParser;
        private SerialMonitor _serialMonitor;

        public GameBoyPrinterReader(string portName, Func<byte[], ControllerStateEventArgs> packetParser)
        {
            _packetParser = packetParser;

            _serialMonitor = new SerialMonitor(portName, true);
            _serialMonitor.PacketReceived += SerialMonitor_PacketReceived;
            _serialMonitor.Disconnected += SerialMonitor_Disconnected;
            _serialMonitor.Start();
        }

        private void SerialMonitor_Disconnected(object sender, EventArgs e)
        {
            Finish();
            ControllerDisconnected?.Invoke(this, EventArgs.Empty);
        }

        public void Finish()
        {
            if (_serialMonitor != null)
            {
                _serialMonitor.Stop();
                _serialMonitor.Dispose();
                _serialMonitor = null;
            }
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                Finish();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void SerialMonitor_PacketReceived(object sender, PacketDataEventArgs packet)
        {
            if (ControllerStateChanged != null)
            {
                ControllerStateEventArgs state = _packetParser(packet.GetPacket());
                if (state != null)
                {
                    ControllerStateChanged(this, state);
                }
            }
        }
    }
}