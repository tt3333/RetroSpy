using SharpDX;
using SharpDX.DirectInput;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows.Threading;

namespace RetroSpy.Readers
{
    public sealed class PCKeyboardReader : IControllerReader, IDisposable
    {
        public event StateEventHandler ControllerStateChanged;

        public event EventHandler ControllerDisconnected;

        private const double TIMER_MS = 1;
        private DirectInput _dinput;
        private DispatcherTimer _timer;
        private Keyboard _keyboard;

        public PCKeyboardReader()
        {
            _dinput = new DirectInput();

            _keyboard = new Keyboard(_dinput);

            ResourceManager stringManager = new ResourceManager("en-US", Assembly.GetExecutingAssembly());

            try
            {
                _keyboard.Acquire();
            }
            catch (Exception)
            {
                throw new IOException(stringManager.GetString("KeyboardCouldNotBeAcquired", CultureInfo.CurrentUICulture));
            }

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(TIMER_MS)
            };
            _timer.Tick += Tick;
            _timer.Start();
        }

        private void Tick(object sender, EventArgs e)
        {
            try
            {
                _keyboard.Poll();
            }
            catch (SharpDXException)
            {
                Finish();
                ControllerDisconnected?.Invoke(this, EventArgs.Empty);
                return;
            }

            ControllerStateBuilder outState = new ControllerStateBuilder();
            KeyboardState state = _keyboard.GetCurrentState();

            foreach (string key in Enum.GetNames(typeof(Key)))
            {
                outState.SetButton(key, false);
            }

            for (int i = 0; i < state.PressedKeys.Count; i++)
            {
                outState.SetButton(state.PressedKeys[i].ToString(), true);
            }

            ControllerStateChanged?.Invoke(this, outState.Build());
        }

        public void Finish()
        {
            if (_keyboard != null)
            {
                _keyboard.Unacquire();
                _keyboard.Dispose();
                _keyboard = null;
            }
            if (_dinput != null)
            {
                _dinput.Dispose();
                _dinput = null;
            }
            if (_timer != null)
            {
                _timer.Stop();
                _timer = null;
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