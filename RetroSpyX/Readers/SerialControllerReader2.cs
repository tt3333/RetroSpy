using System;

namespace RetroSpy.Readers
{
    public sealed class SerialControllerReader2 : IControllerReader, IDisposable
    {
        public event EventHandler<ControllerStateEventArgs>? ControllerStateChanged;

        public event EventHandler? ControllerDisconnected;

        public event EventHandler? ControllerDisconnected2;

        private readonly Func<byte[]?, ControllerStateEventArgs?>? _packetParser;
        private readonly Func<byte[]?, ControllerStateEventArgs?>? _packet2Parser;
        private SerialMonitor? _serialMonitor;
        private SerialMonitor? _serialMonitor2;

        public SerialControllerReader2(string? portName, string? port2Name, bool useLagFix, Func<byte[]?, ControllerStateEventArgs?>? packetParser, Func<byte[]?, ControllerStateEventArgs?>? packet2Parser)
        {
            _packetParser = packetParser;
            _packet2Parser = packet2Parser;

            _serialMonitor = new SerialMonitor(portName, useLagFix);
            _serialMonitor.PacketReceived += SerialMonitor_PacketReceived;
            _serialMonitor.Disconnected += SerialMonitor_Disconnected;
            _serialMonitor.Start();

            if (port2Name != "Not Connected")
            {
                _serialMonitor2 = new SerialMonitor(port2Name, useLagFix);
                _serialMonitor2.PacketReceived += SerialMonitor2_PacketReceived;
                _serialMonitor2.Disconnected += SerialMonitor2_Disconnected;
                _serialMonitor2.Start();
            }
            else
            {
                _serialMonitor2 = null;
            }
        }

        private void SerialMonitor_Disconnected(object? sender, EventArgs e)
        {
            Finish();
            ControllerDisconnected?.Invoke(this, EventArgs.Empty);
        }

        private void SerialMonitor_PacketReceived(object? sender, PacketDataEventArgs packet)
        {
            if (ControllerStateChanged != null)
            {
                ControllerStateEventArgs? state = _packetParser == null ? null : _packetParser(packet.GetPacket());
                if (state != null)
                {
                    ControllerStateChanged(this, state);
                }
            }
        }

        private void SerialMonitor2_Disconnected(object? sender, EventArgs e)
        {
            Finish();
            ControllerDisconnected2?.Invoke(this, EventArgs.Empty);
        }

        private void SerialMonitor2_PacketReceived(object? sender, PacketDataEventArgs packet)
        {
            _ = _packet2Parser == null ? null : _packet2Parser(packet.GetPacket());
        }

        public void Finish()
        {
            if (_serialMonitor != null)
            {
                _serialMonitor.Stop();
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