using LibUsbDotNet.Main;
using System;

namespace RetroSpy.Readers
{
    public sealed class LibUsbControllerReader : IControllerReader, IDisposable
    {
        public event EventHandler<ControllerStateEventArgs>? ControllerStateChanged;

        public event EventHandler? ControllerDisconnected;

        private readonly Func<byte[]?, ControllerStateEventArgs?> _packetParser;
        private LibUSBMonitor? _libusbMonitor;

        public LibUsbControllerReader(string controllerId, int vid, int pid, ReadEndpointID eid, Func<byte[]?, ControllerStateEventArgs?> packetParser)
        {
            _packetParser = packetParser;

            _libusbMonitor = new LibUSBMonitor(controllerId, vid, pid, eid);
            _libusbMonitor.PacketReceived += SerialMonitor_PacketReceived;
            _libusbMonitor.Disconnected += SerialMonitor_Disconnected;
            _libusbMonitor.Start();
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
                ControllerStateEventArgs? state = _packetParser(packet.GetPacket());
                if (state != null)
                {
                    ControllerStateChanged(this, state);
                }
            }
        }

        public void Finish()
        {
            if (_libusbMonitor != null)
            {
                _libusbMonitor.Stop();
                _libusbMonitor.Dispose();
                _libusbMonitor = null;
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