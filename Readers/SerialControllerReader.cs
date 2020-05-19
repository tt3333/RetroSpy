using System;

namespace RetroSpy.Readers
{
    public sealed class SerialControllerReader : IControllerReader, IDisposable
    {
        public event StateEventHandler ControllerStateChanged;

        public event EventHandler ControllerDisconnected;

        private readonly Func<byte[], ControllerState> _packetParser;
        private SerialMonitor _serialMonitor;

        public SerialControllerReader(string portName, Func<byte[], ControllerState> packetParser)
        {
            _packetParser = packetParser;

            _serialMonitor = new SerialMonitor(portName);
            _serialMonitor.PacketReceived += SerialMonitor_PacketReceived;
            _serialMonitor.Disconnected += SerialMonitor_Disconnected;
            _serialMonitor.Start();
        }

        private void SerialMonitor_Disconnected(object sender, EventArgs e)
        {
            Finish();
            ControllerDisconnected?.Invoke(this, EventArgs.Empty);
        }

        private void SerialMonitor_PacketReceived(object sender, PacketData packet)
        {
            if (ControllerStateChanged != null)
            {
                ControllerState state = _packetParser(packet._packet);
                if (state != null)
                {
                    ControllerStateChanged(this, state);
                }
            }
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
    }
}