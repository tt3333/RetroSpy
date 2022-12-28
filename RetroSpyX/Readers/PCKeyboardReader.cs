using SharpDX;
using SharpDX.DirectInput;
using System;
using System.Globalization;
using System.IO;
using System.Resources;
using Avalonia.Threading;

namespace RetroSpy.Readers
{
    public sealed class PCKeyboardReader : IControllerReader, IDisposable
    {
        public event EventHandler<ControllerStateEventArgs>? ControllerStateChanged;

        public event EventHandler? ControllerDisconnected;

        private const double TIMER_MS = 1;
        private DirectInput? _dinput;
        private DispatcherTimer? _timer;
        private Keyboard? _keyboard;
        private Mouse? _mouse;

        private static readonly string[] MOUSE_BUTTONS = {
            "MouseLeft", "MouseRight", "MouseMiddle", "MouseXButton1", "MouseXButton2"
        };

        public PCKeyboardReader()
        {
            _dinput = new DirectInput();

            _keyboard = new Keyboard(_dinput);
            _mouse = new Mouse(_dinput);

            ResourceManager stringManager = Properties.Resources.ResourceManager;

            try
            {
                _keyboard.Acquire();
            }
            catch (SharpDXException)
            {
                throw new IOException(stringManager.GetString("KeyboardCouldNotBeAcquired", CultureInfo.CurrentUICulture));
            }

            try
            {
                _mouse.Acquire();
            }
            catch (SharpDXException)
            {
                throw new IOException(stringManager.GetString("MouseCouldNotBeAcquired", CultureInfo.CurrentUICulture));
            }

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(TIMER_MS)
            };
            _timer.Tick += Tick;
            _timer.Start();
        }

        private void Tick(object? sender, EventArgs e)
        {
            try
            {
                _keyboard?.Poll();
                _mouse?.Poll();
            }
            catch (SharpDXException)
            {
                Finish();
                ControllerDisconnected?.Invoke(this, EventArgs.Empty);
                return;
            }

            ControllerStateBuilder outState = new();
            KeyboardState? state = _keyboard?.GetCurrentState();
            MouseState? mouseState = _mouse?.GetCurrentState();

            if (mouseState != null)
            {
                SignalTool.SetPCMouseProperties(mouseState.X / 255.0f, -mouseState.Y / 255.0f, mouseState.X, mouseState.Y, outState, 1.0f);

                if (mouseState.Z > 0)
                {
                    outState.SetButton("MouseScrollUp", true);
                    outState.SetButton("MouseScrollDown", false);
                }
                else if (mouseState.Z < 0)
                {
                    outState.SetButton("MouseScrollDown", true);
                    outState.SetButton("MouseScrollUp", false);
                }
                else
                {
                    outState.SetButton("MouseScrollDown", false);
                    outState.SetButton("MouseScrollUp", false);
                }

                for (int i = 0; i < MOUSE_BUTTONS.Length; ++i)
                {
                    outState.SetButton(MOUSE_BUTTONS[i], mouseState.Buttons[i]);
                }
            }

            if (state != null)
            {
                foreach (string key in Enum.GetNames(typeof(Key)))
                {
                    outState.SetButton(key, false);
                }

                for (int i = 0; i < state.PressedKeys.Count; i++)
                {
                    outState.SetButton(state.PressedKeys[i].ToString(), true);
                }
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
            if (_mouse != null)
            {
                _mouse.Unacquire();
                _mouse.Dispose();
                _mouse = null;
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