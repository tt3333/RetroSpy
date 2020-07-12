//using System;
//using System.Windows.Threading;

//namespace RetroSpy.Readers
//{
//    sealed public class MouseTester : IControllerReader
//    {
//        const double TIMER_MS = 7;

//        DispatcherTimer _timer;

//        public event StateEventHandler ControllerStateChanged;
//        public event EventHandler ControllerDisconnected;

//        public MouseTester(string portName)
//        {
//            _timer = new DispatcherTimer();
//            _timer.Interval = TimeSpan.FromMilliseconds(TIMER_MS);
//            _timer.Tick += tick;
//            _timer.Start();
//        }

//        bool buttonsOn = false;
//        float theta = 0;
//        float theta1 = 0;
//        bool inCenter = false;
//        int ticks = 0;


//        void tick(object sender, EventArgs e)
//        {
//            var outState = new ControllerStateBuilder();

//            if (ticks % 18 == 0)
//                buttonsOn = buttonsOn ? false : true;

//            ticks++;

//            if (!inCenter && (theta == 0 || theta == 90 || theta == 180 || theta == 270))
//                inCenter = true;
//            else if (inCenter)
//                inCenter = false;

//            float x = 0;
//            float y = 0;

//            if (!inCenter)
//            {
//                x = (float)Math.Cos(theta * Math.PI / 180);
//                y = (float)Math.Sin(theta * Math.PI / 180);
//                theta = theta + 5;
//                theta %= 360;
//            }

//            float x1 = (float)Math.Cos(theta * Math.PI / 180);
//            float y1 = (float)Math.Sin(theta * Math.PI / 180);
//            theta1 = theta1 + 5;
//            theta1 %= 360;

//            outState.SetButton("left", buttonsOn);
//            outState.SetButton("middle", buttonsOn);
//            outState.SetButton("right", buttonsOn);
//            outState.SetButton("wired-1", buttonsOn);
//            outState.SetButton("wired-1a", buttonsOn);
//            outState.SetButton("wired-2", buttonsOn);
//            outState.SetButton("a", buttonsOn);
//            outState.SetButton("b", buttonsOn);
//            outState.SetButton("c", buttonsOn);
//            outState.SetButton("start", buttonsOn);
//            outState.SetButton("select", buttonsOn);
//            outState.SetButton("thumb", buttonsOn);
//            outState.SetButton("scroll_up", buttonsOn);
//            outState.SetButton("scroll_down", buttonsOn);
//            outState.SetButton("1", buttonsOn);
//            outState.SetButton("2", buttonsOn);
//            outState.SetButton("menu", buttonsOn);
//            outState.SetButton("tr", buttonsOn);
//            outState.SetButton("tl", buttonsOn);
//            outState.SetButton("up", buttonsOn);
//            outState.SetButton("down", buttonsOn);

//            outState.SetButton("wired-1", buttonsOn);
//            outState.SetButton("wired-1a", buttonsOn);
//            outState.SetButton("wired-2", buttonsOn);
//            outState.SetButton("wired-2a", buttonsOn);


//            outState.SetButton("wireless-1", buttonsOn);
//            outState.SetButton("wireless-1a", buttonsOn);
//            outState.SetButton("wireless-2", buttonsOn);
//            outState.SetButton("wireless-2a", buttonsOn);

//            outState.SetButton("pause", buttonsOn);
//            outState.SetButton("previous", buttonsOn);
//            outState.SetButton("tv-cable", buttonsOn);
//            outState.SetButton("play", buttonsOn);

//            outState.SetButton("volume-down", buttonsOn);
//            outState.SetButton("stop", buttonsOn);
//            outState.SetButton("next", buttonsOn);
//            outState.SetButton("volume-up", buttonsOn);

//            outState.SetButton("mouse_center", true);

//            outState.SetAnalog("stick_x", x1);
//            outState.SetAnalog("stick_y", y1);

//            outState.SetAnalog("x", x1);
//            outState.SetAnalog("y", y1);

//            SignalTool.SetMouseProperties(x, y, outState);

//            if (ControllerStateChanged != null) ControllerStateChanged(this, outState.Build());
//        }

//        public void Finish()
//        {
//            if (_timer != null)
//            {
//                _timer.Stop();
//                _timer = null;
//            }
//        }
//    }
//}
