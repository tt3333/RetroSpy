using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using Avalonia.Threading;

namespace RetroSpy.Readers
{
    public sealed class XInputReader : IControllerReader
    {
        // ----- Interface implementations with backing state -------------------------------------------------------------

        public event EventHandler<ControllerStateEventArgs>? ControllerStateChanged;

        public event EventHandler? ControllerDisconnected;

        // ----- Interop with XInput DLL ----------------------------------------------------------------------------------

        [StructLayout(LayoutKind.Sequential)]
        private struct XInputState
        {
            public uint dwPacketNumber;
            public ushort wButtons;
            public byte bLeftTrigger;
            public byte bRightTrigger;
            public short sThumbLX;
            public short sThumbLY;
            public short sThumbRX;
            public short sThumbRY;
        }

        private static class NativeMethods
        {
            [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
            [DllImport("xinput9_1_0.dll")]
            public static extern uint XInputGetState(uint userIndex, ref XInputState inputState);
        }

        
        public static Collection<uint> GetDevices()
        {
            Collection<uint> result = new();
            XInputState dummy = new();
            for (uint i = 0; i < 4; i++) //Poll all 4 possible controllers to see which are connected, thats how it works :/
            {
                if (NativeMethods.XInputGetState(i, ref dummy) == 0)
                {
                    result.Add(i);
                }
            }
            return result;
        }

        // ----------------------------------------------------------------------------------------------------------------

        private const double TIMER_MS = 30;
        private DispatcherTimer? _timer;
        private readonly uint _id;

        
        public XInputReader(uint id = 0)
        {
            _id = id;
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(TIMER_MS)
            };
            _timer.Tick += Tick;
            _timer.Start();
        }

        private void Tick(object? sender, EventArgs e)
        {
            XInputState state = new();
            if (NativeMethods.XInputGetState(_id, ref state) > 0)
            {
                ControllerDisconnected?.Invoke(this, EventArgs.Empty);
                Finish();
                return;
            }

            ControllerStateBuilder outState = new();

            outState.SetButton("a", (state.wButtons & 0x1000) != 0);
            outState.SetButton("b", (state.wButtons & 0x2000) != 0);
            outState.SetButton("x", (state.wButtons & 0x4000) != 0);
            outState.SetButton("y", (state.wButtons & 0x8000) != 0);
            outState.SetButton("up", (state.wButtons & 0x0001) != 0);
            outState.SetButton("down", (state.wButtons & 0x0002) != 0);
            outState.SetButton("left", (state.wButtons & 0x0004) != 0);
            outState.SetButton("right", (state.wButtons & 0x0008) != 0);
            outState.SetButton("start", (state.wButtons & 0x0010) != 0);
            outState.SetButton("back", (state.wButtons & 0x0020) != 0);
            outState.SetButton("l3", (state.wButtons & 0x0040) != 0);
            outState.SetButton("r3", (state.wButtons & 0x0080) != 0);
            outState.SetButton("l", (state.wButtons & 0x0100) != 0);
            outState.SetButton("r", (state.wButtons & 0x0200) != 0);

            outState.SetAnalog("lstick_x", (float)state.sThumbLX / 32768, state.sThumbLX);
            outState.SetAnalog("lstick_y", (float)state.sThumbLY / 32768, state.sThumbLY);
            outState.SetAnalog("rstick_x", (float)state.sThumbRX / 32768, state.sThumbRX);
            outState.SetAnalog("rstick_y", (float)state.sThumbRY / 32768, state.sThumbRY);
            outState.SetAnalog("trig_l", (float)state.bLeftTrigger / 255, state.bLeftTrigger);
            outState.SetButton("trig_l_d", ((float)state.bLeftTrigger / 255) > 0);
            outState.SetAnalog("trig_r", (float)state.bRightTrigger / 255, state.bRightTrigger);
            outState.SetButton("trig_r_d", ((float)state.bRightTrigger / 255) > 0);

            ControllerStateChanged?.Invoke(this, outState.Build());
        }

        public void Finish()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer = null;
            }
        }

    }
}