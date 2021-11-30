using SharpDX;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Windows.Threading;

namespace RetroSpy.Readers
{
    public sealed class GamepadReader : IControllerReader, IDisposable
    {
        public event EventHandler<ControllerStateEventArgs> ControllerStateChanged;

        public event EventHandler ControllerDisconnected;

        private const double TIMER_MS = 7;
        private const int RANGE = 1000;
        private DirectInput _dinput;
        private DispatcherTimer _timer;
        private Joystick _joystick;

        [CLSCompliant(false)]
        public static Collection<uint> GetDevices()
        {
            DirectInput input = new DirectInput();
            int amount = input.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly).Count;
            input.Dispose();
            Collection<uint> result = new Collection<uint>();

            for (uint i = 0; i < amount; i++)
            {
                result.Add(i);
            }
            return result;
        }

        public GamepadReader(int id = 0)
        {
            ResourceManager stringManager = Properties.Resources.ResourceManager;

            _dinput = new DirectInput();

            IList<DeviceInstance> devices = _dinput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly);
            if (devices.Count - 1 < id)
            {
                throw new IOException(stringManager.GetString("CouldNotConnectToGamePad", CultureInfo.CurrentUICulture));
            }
            _joystick = new Joystick(_dinput, devices[id].InstanceGuid);

            foreach (DeviceObjectInstance obj in _joystick.GetObjects())
            {
                if ((obj.ObjectId.Flags & DeviceObjectTypeFlags.Axis) != 0)
                {
                    _joystick.GetObjectPropertiesById(obj.ObjectId).Range = new InputRange(-RANGE, RANGE);
                }
            }

            try
            {
                _joystick.Acquire();
            }
            catch (SharpDXException)
            {
                throw new IOException(stringManager.GetString("GamepadCouldNotBeAcquired", CultureInfo.CurrentUICulture));
            }

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(TIMER_MS)
            };
            _timer.Tick += Tick;
            _timer.Start();
        }

        private static int OctantAngle(int octant)
        {
            return 2750 + (4500 * octant);
        }

        private void Tick(object sender, EventArgs e)
        {
            try
            {
                _joystick.Poll();
            }
            catch (SharpDXException)
            {
                Finish();
                ControllerDisconnected?.Invoke(this, EventArgs.Empty);
                return;
            }

            ControllerStateBuilder outState = new ControllerStateBuilder();
            JoystickState state = _joystick.GetCurrentState();

            for (int i = 0; i < _joystick.Capabilities.ButtonCount; ++i)
            {
                outState.SetButton("b" + i.ToString(CultureInfo.CurrentCulture), state.Buttons[i]);
            }

            int[] pov = state.PointOfViewControllers;

            outState.SetButton("up", false);
            outState.SetButton("right", false);
            outState.SetButton("down", false);
            outState.SetButton("left", false);

            if (pov != null && pov.Length > 0 && pov[0] >= 0)
            {
                outState.SetButton("up", pov[0] > OctantAngle(6) || pov[0] < OctantAngle(1));
                outState.SetButton("right", pov[0] > OctantAngle(0) && pov[0] < OctantAngle(3));
                outState.SetButton("down", pov[0] > OctantAngle(2) && pov[0] < OctantAngle(5));
                outState.SetButton("left", pov[0] > OctantAngle(4) && pov[0] < OctantAngle(7));
            }
            else  // For SN30
            {
                outState.SetButton("up", (float)state.Y / RANGE == -1.0);
                outState.SetButton("down", (float)state.Y / RANGE == 1.0);
                outState.SetButton("left", (float)state.X / RANGE == -1.0);
                outState.SetButton("right", (float)state.X / RANGE == 1.0);
            }

            outState.SetAnalog("x", (float)state.X / RANGE, state.X);
            outState.SetAnalog("y", (float)state.Y / RANGE, state.Y);
            outState.SetAnalog("z", (float)state.Z / RANGE, state.Z);
            outState.SetAnalog("rx", (float)state.RotationX / RANGE, state.RotationX);
            outState.SetAnalog("ry", (float)state.RotationY / RANGE, state.RotationY);
            outState.SetAnalog("rz", (float)state.RotationZ / RANGE, state.RotationZ);

            ControllerStateChanged?.Invoke(this, outState.Build());
        }

        public void Finish()
        {
            if (_joystick != null)
            {
                _joystick.Unacquire();
                _joystick.Dispose();
                _joystick = null;
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