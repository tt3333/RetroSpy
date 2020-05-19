using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroSpy.Readers
{
    sealed public class SSHControllerReader : IControllerReader, IDisposable
    {
        public event StateEventHandler ControllerStateChanged;
        public event EventHandler ControllerDisconnected;

        readonly Func <byte[], ControllerState> _packetParser;
        SSHMonitor _serialMonitor;

        public SSHControllerReader(string hostname, string arguments, Func<byte[], ControllerState> packetParser, 
            string username = "retrospy", string password = "retrospy", int delayInMilliseconds = 0) 
        {
            _packetParser = packetParser;

            _serialMonitor = new SSHMonitor(hostname, arguments, username, password, delayInMilliseconds);
            _serialMonitor.PacketReceived += SerialMonitor_PacketReceived;
            _serialMonitor.Disconnected += SerialMonitor_Disconnected;
            _serialMonitor.Start ();
        }

        void SerialMonitor_Disconnected(object sender, EventArgs e)
        {
            Finish ();
            ControllerDisconnected?.Invoke(this, EventArgs.Empty);
        }

        void SerialMonitor_PacketReceived (object sender, PacketData packet)
        {
            if (ControllerStateChanged != null) {
                var state = _packetParser (packet._packet);
                if (state != null) {
                    ControllerStateChanged (this, state);
                }
            }
        }

        public void Finish ()
        {
            if (_serialMonitor != null) {
                _serialMonitor.Stop ();
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
