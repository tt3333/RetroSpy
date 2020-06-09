using System.Collections.Generic;
using System;
using System.Globalization;

namespace RetroSpy.Readers
{
    public sealed class ControllerStateBuilder
    {
        private readonly Dictionary<string, bool> _buttons = new Dictionary<string, bool>();
        private readonly Dictionary<string, float> _analogs = new Dictionary<string, float>();

        public void SetButton(string name, bool value)
        {
            if (name == null)
                throw new NullReferenceException();

            _buttons[name] = value;
            _buttons[name.ToLower(CultureInfo.CurrentUICulture)] = value;
            _buttons[name.ToUpper(CultureInfo.CurrentUICulture)] = value;
        }

        public void SetAnalog(string name, float value)
        {
            _analogs[name] = value;
        }

        public ControllerStateEventArgs Build()
        {
            return new ControllerStateEventArgs(_buttons, _analogs);
        }
    }
}