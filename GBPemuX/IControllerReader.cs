using System;

namespace GBPemu
{
    //public delegate void StateEventHandler(object sender, ControllerStateEventArgs e);

    public interface IControllerReader
    {
        event EventHandler<ControllerStateEventArgs> ControllerStateChanged;
        //event StateEventHandler ControllerStateChanged;

        event EventHandler ControllerDisconnected;

        void Finish();
    }
}