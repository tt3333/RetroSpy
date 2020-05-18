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
    sealed public class PCKeyboardReader : IControllerReader
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

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(TIMER_MS);
            _timer.Tick += tick;
            _timer.Start();
        }
        void tick(object sender, EventArgs e)
        {
            try
            {
                _keyboard.Poll();
            }
            catch (Exception)
            {
                Finish();
                if (ControllerDisconnected != null) ControllerDisconnected(this, EventArgs.Empty);
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

            if (ControllerStateChanged != null) ControllerStateChanged(this, outState.Build());
        }

        public void Finish()
        {
            if (_keyboard != null)
            {
                _keyboard.Unacquire();
                _keyboard.Dispose();
                _keyboard = null;
            }
            if (_timer != null)
            {
                _timer.Stop();
                _timer = null;
            }
        }
    }
}
