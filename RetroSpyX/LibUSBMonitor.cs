using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;

namespace RetroSpy
{
    public class LibUSBMonitor : IDisposable
    {
        private const int TIMER_MS = 1;

        public event EventHandler<PacketDataEventArgs>? PacketReceived;

        public event EventHandler? Disconnected;

        private readonly ReadEndpointID _eid;
        private DispatcherTimer? _timer;
        private readonly UsbContext _context;
        private IUsbDevice? _device;
        private UsbEndpointReader? _reader;
        private readonly List<byte> _localBuffer;
        private int _controllerId;

        public LibUSBMonitor(string controllerId, int vid, int pid, ReadEndpointID eid)
        {
            _controllerId = int.Parse(controllerId) - 1;
            _eid = eid;
            _context = new UsbContext();

            UsbDeviceFinder MyUsbFinder = new(vid, pid);
            _device = (UsbDevice)_context.Find(MyUsbFinder);

            if (_device == null) throw new Exception("LibUsb Device Not Found.");

            _localBuffer = new List<byte>();
        }

        public void Start()
        {
            if (_timer != null)
            {
                return;
            }

            _localBuffer.Clear();

            _device?.Open();
            if (_device != null)
            {
                IUsbDevice wholeUsbDevice = _device as IUsbDevice;
                if (wholeUsbDevice is not null)
                {
                    // This is a "whole" USB device. Before it can be used, 
                    // the desired configuration and interface must be selected.

                    // Select config #1
                    wholeUsbDevice.SetConfiguration(1);

                    // Claim interface #0.
                    wholeUsbDevice.ClaimInterface(0);
                }
            }

            _reader = _device?.OpenEndpointReader(_eid);

            _timer = new DispatcherTimer(DispatcherPriority.Normal)
            {
                Interval = TimeSpan.FromMilliseconds(TIMER_MS)
            };
            _timer.Tick += Tick;
            _timer.Start();
        }

        public void Stop()
        {
            if (_device != null)
            {
                if (_device.IsOpen)
                {
                    // If this is a "whole" usb device (libusb-win32, linux libusb-1.0)
                    // it exposes an IUsbDevice interface. If not (WinUSB) the 
                    // 'wholeUsbDevice' variable will be null indicating this is 
                    // an interface of a device; it does not require or support 
                    // configuration and interface selection.
                    IUsbDevice wholeUsbDevice = _device as IUsbDevice;
                    
                    // Release interface #0.
                    wholeUsbDevice?.ReleaseInterface(0);

                    _device.Close();
                }
                _device = null;

                _context.Dispose();

                if (_timer != null)
                {
                    _timer.Stop();
                    _timer = null;
                }
            }
        }

        private void Tick(object? sender, EventArgs e)
        {
            if (_reader == null || _device?.IsOpen == false || PacketReceived == null)
            {
                return;
            }

            // Try to read some data from the COM port and append it to our localBuffer.
            // If there's an IOException then the device has been disconnected.
            try
            { 

                byte[] readBuffer = new byte[37];
                // If the device hasn't sent data in the last 5 seconds,
                // a timeout error (ec = IoTimedOut) will occur. 
                _ = _reader.Read(readBuffer, 5000, out int bytesRead);
                byte[] newReadBuffer = new byte[61];

                int i = _controllerId;
                for (int j = 0; j < 8; ++j)
                    newReadBuffer[j] = (byte)((readBuffer[(i * 9) + 2] & (1 << j)) != 0 ? '1' : '0');

                for (int j = 0; j < 4; ++j)
                    newReadBuffer[8 + j] = (byte)((readBuffer[(i * 9) + 3] & (1 << j)) != 0 ? '1' : '0');

                for(int j = 0; j < 6; ++j)
                    for(int k = 0; k < 8; ++k)
                        newReadBuffer[12 + j*8 + k] = (byte)((readBuffer[(i * 9) + 4 + j] & (1 << k)) != 0 ? '1' : '0');
                
                newReadBuffer[60] = (byte)'\n';

                _localBuffer.AddRange(newReadBuffer);
                

            }
            catch (IOException)
            {
                Stop();
                Disconnected?.Invoke(this, EventArgs.Empty);
                return;
            }

            // Try and find 2 splitting characters in our buffer.
            int lastSplitIndex = _localBuffer.LastIndexOf(0x0A);
            if (lastSplitIndex <= 1)
            {
                return;
            }

            int sndLastSplitIndex = _localBuffer.LastIndexOf(0x0A, lastSplitIndex - 1);
            if (lastSplitIndex == -1)
            {
                return;
            }

            // Grab the latest packet out of the buffer and fire it off to the receive event listeners.
            int packetStart = sndLastSplitIndex + 1;
            int packetSize = lastSplitIndex - packetStart;


            PacketReceived(this, new PacketDataEventArgs(_localBuffer.GetRange(packetStart, packetSize).ToArray()));

            // Clear our buffer up until the last split character.
            _localBuffer.RemoveRange(0, lastSplitIndex);
           
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stop();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
