using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.IO;
using SharpDX.DirectInput;

namespace RetroSpy.Readers
{
    sealed public class PCKeyboardReader : IControllerReader, IDisposable
    {
        public event StateEventHandler ControllerStateChanged;
        public event EventHandler ControllerDisconnected;

        const double TIMER_MS = 1;

        DirectInput _dinput;
        DispatcherTimer _timer;
        Keyboard _keyboard;

        public PCKeyboardReader(int dummy = 0)
        {
            _dinput = new DirectInput();

            _keyboard = new Keyboard(_dinput);

            try
            {
                _keyboard.Acquire();
            }
            catch (Exception)
            {
                throw new IOException("Connected keyboard could not be acquired.");
            }

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(TIMER_MS)
            };
            _timer.Tick += Tick;
            _timer.Start();
        }
        void Tick(object sender, EventArgs e)
        {
            try
            {
                _keyboard.Poll();
            }
            catch (Exception)
            {
                Finish();
                ControllerDisconnected?.Invoke(this, EventArgs.Empty);
                return;
            }

            var outState = new ControllerStateBuilder();
            var state = _keyboard.GetCurrentState();

            foreach (var key in Enum.GetNames(typeof(Key)))
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
            if(_dinput != null)
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
