using System;
using System.Collections.Generic;
using System.Globalization;

namespace GBPemu
{
    public sealed class ControllerStateBuilder
    {
        private readonly Dictionary<string, bool> _buttons = new Dictionary<string, bool>();
        private readonly Dictionary<string, float> _analogs = new Dictionary<string, float>();
        private string _gameboyPrinterData;
        public void SetButton(string name, bool value)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            _buttons[name] = value;
            _buttons[name.ToLower(CultureInfo.CurrentUICulture)] = value;
            _buttons[name.ToUpper(CultureInfo.CurrentUICulture)] = value;
        }

        public void SetAnalog(string name, float value)
        {
            _analogs[name] = value;
        }

        public void SetPrinterData(string data)
        {
            _gameboyPrinterData = data;
        }

        public ControllerStateEventArgs Build()
        {
            return new ControllerStateEventArgs(_buttons, _analogs, _gameboyPrinterData);
        }
    }
}