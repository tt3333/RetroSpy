using System;
using System.Collections.Generic;

namespace RetroSpy.Readers
{
    public class ControllerStateEventArgs : EventArgs
    {
        public static readonly ControllerStateEventArgs Zero = new ControllerStateEventArgs
            (new Dictionary<string, bool>(), new Dictionary<string, float>());

        public IReadOnlyDictionary<string, bool> Buttons { get; private set; }
        public IReadOnlyDictionary<string, float> Analogs { get; private set; }

        public ControllerStateEventArgs(IReadOnlyDictionary<string, bool> buttons, IReadOnlyDictionary<string, float> analogs)
        {
            Buttons = buttons;
            Analogs = analogs;
        }
    }
}