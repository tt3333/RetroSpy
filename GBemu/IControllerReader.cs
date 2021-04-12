using System;

namespace GBPemu
{
    public delegate void StateEventHandler(object sender, ControllerStateEventArgs e);

    public interface IControllerReader
    {
        event StateEventHandler ControllerStateChanged;

        event EventHandler ControllerDisconnected;

        void Finish();
    }
}