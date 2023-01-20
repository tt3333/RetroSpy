using Desktop.Robot;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace RetroSpy
{

    public static partial class SendKeys
    {
        static readonly Robot _robot = new();

        public static void PressKey(Key key)
        {
            _robot.KeyDown(key);
        }

        public static void ReleaseKey(Key key)
        {
            _robot.KeyUp(key);
        }

        public static void PressAndReleaseKey(Key key)
        {
            _robot.KeyDown(key);
            _robot.KeyUp(key);
        }
    }
}
