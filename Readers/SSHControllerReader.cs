using System;

namespace RetroSpy.Readers
{
    public sealed class SSHControllerReader : IControllerReader, IDisposable
    {
        public event StateEventHandler ControllerStateChanged;

        public event EventHandler ControllerDisconnected;

        private readonly Func<byte[], ControllerStateEventArgs> _packetParser;
        private SSHMonitor _serialMonitor;

        public SSHControllerReader(string hostname, string arguments, Func<byte[], ControllerStateEventArgs> packetParser,
            string username, string password, int delayInMilliseconds = 0)
        {
            _packetParser = packetParser;

            _serialMonitor = new SSHMonitor(hostname, arguments, username, password, delayInMilliseconds);
            _serialMonitor.PacketReceived += SerialMonitor_PacketReceived;
            _serialMonitor.Disconnected += SerialMonitor_Disconnected;
            _serialMonitor.Start();
        }

        private void SerialMonitor_Disconnected(object sender, EventArgs e)
        {
            Finish();
            ControllerDisconnected?.Invoke(this, EventArgs.Empty);
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