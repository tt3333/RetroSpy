using System;

namespace RetroSpy.Readers
{
    //public delegate void StateEventHandler(object sender, ControllerStateEventArgs e);

    public interface IControllerReader
    {
        event EventHandler<ControllerStateEventArgs> ControllerStateChanged;

        event EventHandler ControllerDisconnected;

        void Finish();
    }
}