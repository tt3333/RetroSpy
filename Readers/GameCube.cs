using System;

namespace RetroSpy.Readers
{
    public static class GameCube
    {
        private const int PACKET_SIZE = 64;

        private static readonly string[] BUTTONS = {
            null, null, null, "start", "y", "x", "b", "a", null, "l", "r", "z", "up", "down", "right", "left"
        };

        private static readonly string[] KEYS =
        {
            null, null, null, null, null, null, "Home", "End",
            "PageUp", "PageDown", null, "ScrollLock", null, null, null, null,
            "K_A", "K_B", "C", "D", "E", "F", "G", "H",
            "I", "J", "K", "K_L", "M", "N", "O", "P",

            "Q", "K_R", "S", "T", "U", "V", "W", "K_X",
            "K_Y", "K_Z", "D1", "D2", "D3", "D4", "D5", "D6",
            "D7", "D8", "D9", "D0", "Minus", "Equals", "Yen", "LeftBracket",
            "RightBracket", "Semicolon", "Apostrophe", "LeftOfReturn", "Comma", "Period", "Slash", "JpSlash",

            "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8",
            "F9", "F10", "F11", "F12", "Escape", "Insert", "Delete", "Grave",
            "Back", "Tab", null, "Capital", "LeftShift", "RightShift", "LeftControl", "LeftAlt",
            "LeftWindowsKey", "Space", "RightWindowsKey", "Applications", "K_left", "K_down", "K_up", "K_right",

            null, "Return", null, null, null, null, null, null,
            null, null, null, null, null, null, null, null,
        };

        private static readonly string[] FUNCTION_KEYS =
        {
            null, null, null, null, null, null, "Function", "Function",
            "Function", "Function", null, "Function", null, null, null, null,
            null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null,

            null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null,

            null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null,

            null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null,

        };

        private static float ReadStick(byte input)
        {
            return (float)(input - 128) / 128;
        }

        private static float ReadTrigger(byte input)
        {
            return (float)input / 256;
        }

        private static readonly byte[] keyboardData = new byte[3];

        public static ControllerStateEventArgs ReadFromSecondPacket(byte[] packet)
        {
            if (packet == null)
            {
                throw new ArgumentNullException(nameof(packet));
            }

            if (packet.Length == 3)
            {
                for (int i = 0; i < 3; ++i)
                {
                    keyboardData[i] = packet[i];
                }
            }

            return null;
        }

        public static ControllerStateEventArgs ReadFromPacket(byte[] packet)
        {
            if (packet == null)
            {
                throw new ArgumentNullException(nameof(packet));
            }

            if (packet.Length == 3)
            {
                ControllerStateBuilder state1 = new ControllerStateBuilder();

                for (int i = 0; i < KEYS.Length; ++i)
                {
                    if (KEYS[i] != null)
                    {
                        state1.SetButton(KEYS[i], false);
                    }
                }

                for (int i = 0; i < packet.Length; ++i)
                {
                    if (KEYS[packet[i]] != null)
                    {
                        state1.SetButton(KEYS[packet[i]], true);
                    }
                }

                state1.SetButton("Function", false);
                for (int i = 0; i < packet.Length; ++i)
                {
                    if (FUNCTION_KEYS[packet[i]] != null)
                    {
                        state1.SetButton(FUNCTION_KEYS[packet[i]], true);
                    }
                }

                return state1.Build();
            }

            if (packet.Length != PACKET_SIZE)
            {
                return null;
            }

            ControllerStateBuilder state = new ControllerStateBuilder();

            for (int i = 0; i < BUTTONS.Length; ++i)
            {
                if (string.IsNullOrEmpty(BUTTONS[i]))
                {
                    continue;
                }

                state.SetButton(BUTTONS[i], packet[i] != 0x00);
            }

            for (int i = 0; i < KEYS.Length; ++i)
            {
                if (KEYS[i] != null)
                {
                    state.SetButton(KEYS[i], false);
                }
            }

            for (int i = 0; i < keyboardData.Length; ++i)
            {
                if (KEYS[keyboardData[i]] != null)
                {
                    state.SetButton(KEYS[keyboardData[i]], true);
                }
            }

            state.SetButton("Function", false);
            for (int i = 0; i < keyboardData.Length; ++i)
            {
                if (FUNCTION_KEYS[keyboardData[i]] != null)
                {
                    state.SetButton(FUNCTION_KEYS[keyboardData[i]], true);
                }
            }

            state.SetAnalog("lstick_x", ReadStick(SignalTool.ReadByte(packet, BUTTONS.Length)), SignalTool.ReadByte(packet, BUTTONS.Length));
            state.SetAnalog("lstick_y", ReadStick(SignalTool.ReadByte(packet, BUTTONS.Length + 8)), SignalTool.ReadByte(packet, BUTTONS.Length + 8));
            state.SetAnalog("cstick_x", ReadStick(SignalTool.ReadByte(packet, BUTTONS.Length + 16)), SignalTool.ReadByte(packet, BUTTONS.Length + 16));
            state.SetAnalog("cstick_y", ReadStick(SignalTool.ReadByte(packet, BUTTONS.Length + 24)), SignalTool.ReadByte(packet, BUTTONS.Length + 24));
            state.SetAnalog("trig_l", ReadTrigger(SignalTool.ReadByte(packet, BUTTONS.Length + 32)), SignalTool.ReadByte(packet, BUTTONS.Length + 32));
            state.SetAnalog("trig_r", ReadTrigger(SignalTool.ReadByte(packet, BUTTONS.Length + 40)), SignalTool.ReadByte(packet, BUTTONS.Length + 40));

            return state.Build();
        }
    }
}