using Avalonia.Threading;
using ReactiveUI;
using SharpGen.Runtime;
using System;
using System.Globalization;
using System.IO;
using System.Resources;
using Vortice.DirectInput;

namespace RetroSpy.Readers
{
    public sealed class PCKeyboardReader : IControllerReader, IDisposable
    {
        public event EventHandler<ControllerStateEventArgs>? ControllerStateChanged;

        public event EventHandler? ControllerDisconnected;

        private const double TIMER_MS = 1;
        private IDirectInput8? _dinput;
        private DispatcherTimer? _timer;
        private IDirectInputDevice8? _keyboard;
        private IDirectInputDevice8? _mouse;

        private static readonly string[] MOUSE_BUTTONS = {
            "MouseLeft", "MouseRight", "MouseMiddle", "MouseXButton1", "MouseXButton2"
        };

        public PCKeyboardReader()
        {
            _dinput = DInput.DirectInput8Create();

            var keyboards = _dinput.GetDevices(DeviceClass.Keyboard, DeviceEnumerationFlags.AttachedOnly);

            if (keyboards.Count > 0)
            {
                _keyboard = _dinput.CreateDevice(keyboards[0].InstanceGuid);

                // Play the values back to see that buffersize is working correctly
                _keyboard.Properties.BufferSize = 16;

                if (_dinput.IsDeviceAttached(keyboards[0].InstanceGuid))
                {
                    _ = _keyboard.SetDataFormat<RawKeyboardState>();
                }
            }

            var pointers = _dinput.GetDevices(DeviceClass.Pointer, DeviceEnumerationFlags.AttachedOnly);

            if (pointers.Count > 0)
            {
                _mouse = _dinput.CreateDevice(pointers[0].InstanceGuid);

                // Play the values back to see that buffersize is working correctly
                _mouse.Properties.BufferSize = 16;

                if (_dinput.IsDeviceAttached(pointers[0].InstanceGuid))
                {
                    _ = _mouse.SetDataFormat<RawMouseState>();
                }
            }

            ResourceManager stringManager = Properties.Resources.ResourceManager;

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(TIMER_MS)
            };
            _timer.Tick += Tick;
            _timer.Start();
        }

        private void Tick(object? sender, EventArgs e)
        { 
            ControllerStateBuilder outState = new();

            if (_mouse != null)
            {
                Result result_ = _mouse.Poll();

                if (result_.Failure)
                {
                    result_ = _mouse.Acquire();

                    if (result_.Failure)
                    {
                        Finish();
                        ControllerDisconnected?.Invoke(this, EventArgs.Empty);
                        throw new UnhandledErrorException("Failed to Poll/Aquire DirectInput Mouse.");
                    }
                }

                MouseState mouseState = new();
                _mouse.GetCurrentMouseState(ref mouseState);

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

            if (_keyboard != null)
            {
                Result result_ = _keyboard.Poll();

                if (result_.Failure)
                {
                    result_ = _keyboard.Acquire();

                    if (result_.Failure)
                    {
                        Finish();
                        ControllerDisconnected?.Invoke(this, EventArgs.Empty);
                        throw new UnhandledErrorException("Failed to Poll/Aquire DirectInput Keyboard.");
                    }
                }

                KeyboardState state = new KeyboardState();
                _keyboard.GetCurrentKeyboardState(ref state);

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