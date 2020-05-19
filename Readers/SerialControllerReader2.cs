using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroSpy.Readers
{
    sealed public class SerialControllerReader2 : IControllerReader, IDisposable
    {
        public event StateEventHandler ControllerStateChanged;
        public event EventHandler ControllerDisconnected;

        public event EventHandler ControllerDisconnected2;

        readonly Func <byte[], ControllerState> _packetParser;
        readonly Func<byte[], ControllerState> _packet2Parser;
        SerialMonitor _serialMonitor;
        SerialMonitor _serialMonitor2;

        public SerialControllerReader2 (string portName, string port2Name, Func <byte[], ControllerState> packetParser, Func<byte[], ControllerState> packet2Parser) 
        {
            _packetParser = packetParser;
            _packet2Parser = packet2Parser;

            _serialMonitor = new SerialMonitor (portName);
            _serialMonitor.PacketReceived += SerialMonitor_PacketReceived;
            _serialMonitor.Disconnected += SerialMonitor_Disconnected;
            _serialMonitor.Start();

            if (port2Name != "Not Connected")
            {
                _serialMonitor2 = new SerialMonitor(port2Name);
                _serialMonitor2.PacketReceived += SerialMonitor2_PacketReceived;
                _serialMonitor2.Disconnected += SerialMonitor2_Disconnected;
                _serialMonitor2.Start();

            }
            else
                _serialMonitor2 = null;                
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

        void SerialMonitor2_Disconnected(object sender, EventArgs e)
        {
            Finish();
            ControllerDisconnected2?.Invoke(this, EventArgs.Empty);
        }

        void SerialMonitor2_PacketReceived(object sender, PacketData packet)
        {
            _packet2Parser(packet._packet);
        }

        public void Finish ()
        {
            if (_serialMonitor != null) {
                _serialMonitor.Stop ();
                _serialMonitor.Dispose();
                _serialMonitor = null;
            }

            if (_serialMonitor2 != null)
            {
                _serialMonitor2.Stop();
                _serialMonitor2.Dispose();
                _serialMonitor2 = null;
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
