using Avalonia.Input;
using RetroSpy.Readers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;


namespace RetroSpy
{
    public class Keybindings
    {
        private class Binding
        {
            public readonly ushort OutputKey;
            public readonly IReadOnlyList<string> RequiredButtons;

            public bool CurrentlyDepressed;

            public Binding(ushort outputKey, IReadOnlyList<string> requiredButtons)
            {
                OutputKey = outputKey;
                RequiredButtons = requiredButtons;
            }
        }

        public const string XmlFilePath = "keybindings.xml";

        private readonly IControllerReader _reader;
        private readonly List<Binding> _bindings = new List<Binding>();

        public Keybindings(string xmlFilePath, IControllerReader reader)
        {
            string xmlPath = Path.Combine(Environment.CurrentDirectory, xmlFilePath);

            if (!File.Exists(xmlPath))
            {
                throw new ConfigParseException(string.Format(CultureInfo.CurrentCulture, "Could not find {0}", XmlFilePath));
            }

            XDocument doc = XDocument.Load(xmlPath);

            foreach (XElement binding in doc.Root.Elements("binding"))
            {
                ushort outputKey = ReadKeybinding(binding.Attribute("output-key").Value);
                if (outputKey == 0)
                {
                    continue;
                }

                List<string> requiredButtons = new List<string>();
                foreach (XElement input in binding.Elements("input"))
                {
                    requiredButtons.Add(input.Attribute("button").Value);
                }

                if (requiredButtons.Count < 1)
                {
                    continue;
                }

                _bindings.Add(new Binding(outputKey, requiredButtons));
            }

            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
            if (_reader.GetType() == typeof(DelayedControllerReader))
            {
                ((DelayedControllerReader)_reader).ControllerStateChangedNoDelay += Reader_ControllerStateChanged;
            }
            else
            {
                _reader.ControllerStateChanged += Reader_ControllerStateChanged;
            }
        }

        public void Finish()
        {
            _reader.ControllerStateChanged -= Reader_ControllerStateChanged;
        }

        private void Reader_ControllerStateChanged(object reader, ControllerStateEventArgs e)
        {
            foreach (Binding binding in _bindings)
            {
                bool allRequiredButtonsDown = true;

                foreach (string requiredButton in binding.RequiredButtons)
                {
                    allRequiredButtonsDown &= e.Buttons[requiredButton];
                }

                if (allRequiredButtonsDown && !binding.CurrentlyDepressed)
                {
                    SendKeys.PressKey(binding.OutputKey);
                    binding.CurrentlyDepressed = true;
                }
                else if (!allRequiredButtonsDown && binding.CurrentlyDepressed)
                {
                    SendKeys.ReleaseKey(binding.OutputKey);
                    binding.CurrentlyDepressed = false;
                }
            }
        }

        private static ushort ReadKeybinding(string name)
        {
            string upperName = name.ToUpperInvariant();

            return Regex.Match(upperName, "^[A-Z0-9]$").Success
                ? upperName.ToCharArray()[0]
                : VK_KEYWORDS.ContainsKey(upperName) ? VK_KEYWORDS[upperName] : (ushort)0;
        }

        private static ushort VkConvert(Key key)
        {
            return (ushort)KeyInterop.VirtualKeyFromKey(key);
        }

        private static readonly IReadOnlyDictionary<string, ushort> VK_KEYWORDS = new Dictionary<string, ushort> {
            { "ENTER", VkConvert (Key.Enter) },
            { "TAB", VkConvert (Key.Tab) },
            { "ESC", VkConvert (Key.Escape) },
            { "ESCAPE", VkConvert (Key.Escape) },
            { "HOME", VkConvert (Key.Home) },
            { "END", VkConvert (Key.End) },
            { "LEFT", VkConvert (Key.Left) },
            { "RIGHT", VkConvert (Key.Right) },
            { "UP", VkConvert (Key.Up) },
            { "DOWN", VkConvert (Key.Down) },
            { "PGUP", VkConvert (Key.Prior) },
            { "PGDN", VkConvert (Key.Next) },
            { "NUMLOCK", VkConvert (Key.NumLock) },
            { "SCROLLLOCK", VkConvert (Key.Scroll) },
            { "PRTSC", VkConvert (Key.PrintScreen) },
            { "BREAK", VkConvert (Key.Cancel) },
            { "BACKSPACE", VkConvert (Key.Back) },
            { "BKSP", VkConvert (Key.Back) },
            { "BS", VkConvert (Key.Back) },
            { "CLEAR", VkConvert (Key.Clear) },
            { "CAPSLOCK", VkConvert (Key.Capital) },
            { "INS", VkConvert (Key.Insert) },
            { "INSERT", VkConvert (Key.Insert) },
            { "DEL", VkConvert (Key.Delete) },
            { "DELETE", VkConvert (Key.Delete) },
            { "HELP", VkConvert (Key.Help) },
            { "F1", VkConvert (Key.F1) },
            { "F2", VkConvert (Key.F2) },
            { "F3", VkConvert (Key.F3) },
            { "F4", VkConvert (Key.F4) },
            { "F5", VkConvert (Key.F5) },
            { "F6", VkConvert (Key.F6) },
            { "F7", VkConvert (Key.F7) },
            { "F8", VkConvert (Key.F8) },
            { "F9", VkConvert (Key.F9) },
            { "F10", VkConvert (Key.F10) },
            { "F11", VkConvert (Key.F11) },
            { "F12", VkConvert (Key.F12) },
            { "F13", VkConvert (Key.F13) },
            { "F14", VkConvert (Key.F14) },
            { "F15", VkConvert (Key.F15) },
            { "F16", VkConvert (Key.F16) },
            { "NUMPAD0", VkConvert (Key.NumPad0) },
            { "NUMPAD1", VkConvert (Key.NumPad1) },
            { "NUMPAD2", VkConvert (Key.NumPad2) },
            { "NUMPAD3", VkConvert (Key.NumPad3) },
            { "NUMPAD4", VkConvert (Key.NumPad4) },
            { "NUMPAD5", VkConvert (Key.NumPad5) },
            { "NUMPAD6", VkConvert (Key.NumPad6) },
            { "NUMPAD7", VkConvert (Key.NumPad7) },
            { "NUMPAD8", VkConvert (Key.NumPad8) },
            { "NUMPAD9", VkConvert (Key.NumPad9) },
            { "MULTIPLY", VkConvert (Key.Multiply) },
            { "*", VkConvert (Key.Multiply) },
            { "ADD", VkConvert (Key.Add) },
            { "+", VkConvert (Key.Add) },
            { "SUBTRACT", VkConvert (Key.Subtract) },
            { "-", VkConvert (Key.Subtract) },
            { "DIVIDE", VkConvert (Key.Divide) },
            { "/", VkConvert (Key.Divide) }
        };
    }
}