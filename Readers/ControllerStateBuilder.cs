using System.Collections.Generic;

namespace RetroSpy.Readers
{
    public sealed class ControllerStateBuilder
    {
        private readonly Dictionary<string, bool> _buttons = new Dictionary<string, bool>();
        private readonly Dictionary<string, float> _analogs = new Dictionary<string, float>();

        public void SetButton(string name, bool value)
        {
            _buttons[name] = value;
            _buttons[name.ToLower()] = value;
            _buttons[name.ToUpper()] = value;
        }

        public void SetAnalog(string name, float value)
        {
            _analogs[name] = value;
        }

        public ControllerState Build()
        {
            return new ControllerState(_buttons, _analogs);
        }
    }
}