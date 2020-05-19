using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroSpy.Readers
{
    public delegate void StateEventHandler (object sender, ControllerState e);

    public interface IControllerReader
    {
        event StateEventHandler ControllerStateChanged;
        event EventHandler ControllerDisconnected;

        void Finish ();
    }
}
