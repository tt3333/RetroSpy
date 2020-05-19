using System;

namespace RetroSpy.Readers
{
    public delegate void StateEventHandler(object sender, ControllerState e);

    public interface IControllerReader
    {
        event StateEventHandler ControllerStateChanged;

        event EventHandler ControllerDisconnected;

        void Finish();
    }
}