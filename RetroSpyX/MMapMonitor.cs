using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.IO;

namespace RetroSpy
{
    public class MMapMonitor : IDisposable
    {
        private const int TIMER_MS = 1;

        public event EventHandler<PacketDataEventArgs>? PacketReceived;

        public event EventHandler? Disconnected;

        private DispatcherTimer? _timer;
        private FileStream _file;
        private readonly List<byte> _localBuffer;
        private int _frameNum;
        private int _readDataLength;
        private int _frameSize;
        private int _globalOffset;

        public MMapMonitor(string frameNum, string filename, int readDataLength, int frameSize, int globalOffset)
        {
            _readDataLength= readDataLength;
            _frameSize= frameSize;
            _globalOffset= globalOffset;
            _frameNum = int.Parse(frameNum) - 1;

            _file = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            _localBuffer = new List<byte>();
        }

        public void Start()
        {
            if (_timer != null)
            {
                return;
            }

            _localBuffer.Clear();

            _timer = new DispatcherTimer(DispatcherPriority.Normal)
            {
                Interval = TimeSpan.FromMilliseconds(TIMER_MS)
            };
            _timer.Tick += Tick;
            _timer.Start();
        }

        public void Stop()
        {
            _file?.Close();

            if (_timer != null)
            {
                _timer.Stop();
                _timer = null;
            }
        }
            

        private void Tick(object? sender, EventArgs e)
        {
            if (_file == null || PacketReceived == null)
            {
                return;
            }

            // Try to read some data from the COM port and append it to our localBuffer.
            // If there's an IOException then the device has been disconnected.
            try
            { 

                byte[] readBuffer = new byte[_readDataLength];
                // If the device hasn't sent data in the last 5 seconds,
                // a timeout error (ec = IoTimedOut) will occur. 
                _file.Position = 0;
                _ = _file.Read(readBuffer, 0, _readDataLength);
                _file.Flush();

                byte[] newReadBuffer = new byte[(_frameSize * 8) + 1];

                int i = _frameNum;

                for (int j = 0; j < _frameSize; ++j)
                {
                    for (int k = 0; k < 8; ++k)
                    {
                        newReadBuffer[(j * 8) + k] = (byte)((readBuffer[(i * _frameSize) + _globalOffset + j] & (1 << k)) != 0 ? '1' : '0');
                    }
                }

                newReadBuffer[(_frameSize * 8)] = (byte)'\n';

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
