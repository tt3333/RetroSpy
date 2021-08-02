using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Threading;

namespace RetroSpy
{
    //public delegate void PacketEventHandler (object sender, byte[] packet);

    public class SSHMonitor : IDisposable
    {
        private const int TIMER_MS = 1;

        public event PacketEventHandler PacketReceived;

        public event EventHandler Disconnected;

        private SshClient _client;
        private ShellStream _data;
        private readonly List<byte> _localBuffer;
        private readonly string _command;
        private readonly int _delayInMilliseconds;
        private DispatcherTimer _timer;

        public SSHMonitor(string hostname, string command, string username, string password, string commandSub, int delayInMilliseconds)
        {
            _localBuffer = new List<byte>();
            _client = new SshClient(hostname, username, password);
            if (!string.IsNullOrEmpty(commandSub))
                _command = String.Format(command, commandSub);
            else
                _command = command;
            _delayInMilliseconds = delayInMilliseconds;
        }

        public void Start()
        {
            if (_timer != null)
            {
                return;
            }

            _localBuffer.Clear();
            _client.Connect();
            _data = _client.CreateShellStream("", 0, 0, 0, 0, 0);
            if (_delayInMilliseconds > 0)
            {
                Thread.Sleep(_delayInMilliseconds);
            }

            _data.WriteLine(_command);

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(TIMER_MS)
            };
            _timer.Tick += Tick;
            _timer.Start();
        }

        public void Stop()
        {
            if (_data != null)
            {
                try
                { // If the device has been unplugged, Close will throw an IOException.  This is fine, we'll just keep cleaning up.
                    _data.Close();
                }
                catch (IOException) { }
                _data = null;
                if (_client != null)
                {
                    _client.Disconnect();
                    _client.Dispose();
                    _client = null;
                }
            }
            if (_timer != null)
            {
                _timer.Stop();
                _timer = null;
            }
        }

        private void Tick(object sender, EventArgs e)
        {
            if (_data == null || !_data.CanRead || PacketReceived == null)
            {
                return;
            }

            // Try to read some data from the COM port and append it to our localBuffer.
            // If there's an IOException then the device has been disconnected.
            try
            {
                int readCount = (int)_data.Length;
                if (readCount < 1)
                {
                    return;
                }

                byte[] readBuffer = new byte[readCount];
                _data.Read(readBuffer, 0, readCount);
                _localBuffer.AddRange(readBuffer);
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