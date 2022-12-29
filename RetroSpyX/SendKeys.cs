using Avalonia.Input;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace RetroSpy
{
    // Keycodes: http://msdn.microsoft.com/en-us/library/windows/desktop/dd375731(v=vs.85).aspx
    // Letter keys map to 0x41.. etc. (i.e. capital ASCII letters)

    static class KeyInterop
    {
        private static readonly Dictionary<Key, int> s_virtualKeyFromKey = new()
        {
            { Key.None, 0 },
            { Key.Cancel, 3 },
            { Key.Back, 8 },
            { Key.Tab, 9 },
            { Key.LineFeed, 0 },
            { Key.Clear, 12 },
            { Key.Return, 13 },
            { Key.Pause, 19 },
            { Key.Capital, 20 },
            { Key.KanaMode, 21 },
            { Key.JunjaMode, 23 },
            { Key.FinalMode, 24 },
            { Key.HanjaMode, 25 },
            { Key.Escape, 27 },
            { Key.ImeConvert, 28 },
            { Key.ImeNonConvert, 29 },
            { Key.ImeAccept, 30 },
            { Key.ImeModeChange, 31 },
            { Key.Space, 32 },
            { Key.PageUp, 33 },
            { Key.Next, 34 },
            { Key.End, 35 },
            { Key.Home, 36 },
            { Key.Left, 37 },
            { Key.Up, 38 },
            { Key.Right, 39 },
            { Key.Down, 40 },
            { Key.Select, 41 },
            { Key.Print, 42 },
            { Key.Execute, 43 },
            { Key.Snapshot, 44 },
            { Key.Insert, 45 },
            { Key.Delete, 46 },
            { Key.Help, 47 },
            { Key.D0, 48 },
            { Key.D1, 49 },
            { Key.D2, 50 },
            { Key.D3, 51 },
            { Key.D4, 52 },
            { Key.D5, 53 },
            { Key.D6, 54 },
            { Key.D7, 55 },
            { Key.D8, 56 },
            { Key.D9, 57 },
            { Key.A, 65 },
            { Key.B, 66 },
            { Key.C, 67 },
            { Key.D, 68 },
            { Key.E, 69 },
            { Key.F, 70 },
            { Key.G, 71 },
            { Key.H, 72 },
            { Key.I, 73 },
            { Key.J, 74 },
            { Key.K, 75 },
            { Key.L, 76 },
            { Key.M, 77 },
            { Key.N, 78 },
            { Key.O, 79 },
            { Key.P, 80 },
            { Key.Q, 81 },
            { Key.R, 82 },
            { Key.S, 83 },
            { Key.T, 84 },
            { Key.U, 85 },
            { Key.V, 86 },
            { Key.W, 87 },
            { Key.X, 88 },
            { Key.Y, 89 },
            { Key.Z, 90 },
            { Key.LWin, 91 },
            { Key.RWin, 92 },
            { Key.Apps, 93 },
            { Key.Sleep, 95 },
            { Key.NumPad0, 96 },
            { Key.NumPad1, 97 },
            { Key.NumPad2, 98 },
            { Key.NumPad3, 99 },
            { Key.NumPad4, 100 },
            { Key.NumPad5, 101 },
            { Key.NumPad6, 102 },
            { Key.NumPad7, 103 },
            { Key.NumPad8, 104 },
            { Key.NumPad9, 105 },
            { Key.Multiply, 106 },
            { Key.Add, 107 },
            { Key.Separator, 108 },
            { Key.Subtract, 109 },
            { Key.Decimal, 110 },
            { Key.Divide, 111 },
            { Key.F1, 112 },
            { Key.F2, 113 },
            { Key.F3, 114 },
            { Key.F4, 115 },
            { Key.F5, 116 },
            { Key.F6, 117 },
            { Key.F7, 118 },
            { Key.F8, 119 },
            { Key.F9, 120 },
            { Key.F10, 121 },
            { Key.F11, 122 },
            { Key.F12, 123 },
            { Key.F13, 124 },
            { Key.F14, 125 },
            { Key.F15, 126 },
            { Key.F16, 127 },
            { Key.F17, 128 },
            { Key.F18, 129 },
            { Key.F19, 130 },
            { Key.F20, 131 },
            { Key.F21, 132 },
            { Key.F22, 133 },
            { Key.F23, 134 },
            { Key.F24, 135 },
            { Key.NumLock, 144 },
            { Key.Scroll, 145 },
            { Key.LeftShift, 160 },
            { Key.RightShift, 161 },
            { Key.LeftCtrl, 162 },
            { Key.RightCtrl, 163 },
            { Key.LeftAlt, 164 },
            { Key.RightAlt, 165 },
            { Key.BrowserBack, 166 },
            { Key.BrowserForward, 167 },
            { Key.BrowserRefresh, 168 },
            { Key.BrowserStop, 169 },
            { Key.BrowserSearch, 170 },
            { Key.BrowserFavorites, 171 },
            { Key.BrowserHome, 172 },
            { Key.VolumeMute, 173 },
            { Key.VolumeDown, 174 },
            { Key.VolumeUp, 175 },
            { Key.MediaNextTrack, 176 },
            { Key.MediaPreviousTrack, 177 },
            { Key.MediaStop, 178 },
            { Key.MediaPlayPause, 179 },
            { Key.LaunchMail, 180 },
            { Key.SelectMedia, 181 },
            { Key.LaunchApplication1, 182 },
            { Key.LaunchApplication2, 183 },
            { Key.Oem1, 186 },
            { Key.OemPlus, 187 },
            { Key.OemComma, 188 },
            { Key.OemMinus, 189 },
            { Key.OemPeriod, 190 },
            { Key.OemQuestion, 191 },
            { Key.Oem3, 192 },
            { Key.AbntC1, 193 },
            { Key.AbntC2, 194 },
            { Key.OemOpenBrackets, 219 },
            { Key.Oem5, 220 },
            { Key.Oem6, 221 },
            { Key.OemQuotes, 222 },
            { Key.Oem8, 223 },
            { Key.OemBackslash, 226 },
            { Key.ImeProcessed, 229 },
            { Key.System, 0 },
            { Key.OemAttn, 240 },
            { Key.OemFinish, 241 },
            { Key.OemCopy, 242 },
            { Key.DbeSbcsChar, 243 },
            { Key.OemEnlw, 244 },
            { Key.OemBackTab, 245 },
            { Key.DbeNoRoman, 246 },
            { Key.DbeEnterWordRegisterMode, 247 },
            { Key.DbeEnterImeConfigureMode, 248 },
            { Key.EraseEof, 249 },
            { Key.Play, 250 },
            { Key.DbeNoCodeInput, 251 },
            { Key.NoName, 252 },
            { Key.Pa1, 253 },
            { Key.OemClear, 254 },
            { Key.DeadCharProcessed, 0 },
        };

        public static int VirtualKeyFromKey(Key key)
        {
            s_virtualKeyFromKey.TryGetValue(key, out var result);

            return result;
        }
    }

    public static partial class SendKeys
    {
        //const int INPUT_MOUSE = 0;
        private const int INPUT_KEYBOARD = 1;
        //const int INPUT_HARDWARE = 2;
        //const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        //const uint KEYEVENTF_UNICODE = 0x0004;
        //const uint KEYEVENTF_SCANCODE = 0x0008;

        private struct INPUT
        {
            public int type;
            public InputUnion u;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct InputUnion
        {
            [FieldOffset(0)]
            public MOUSEINPUT mi;
            [FieldOffset(0)]
            public KEYBDINPUT ki;
            [FieldOffset(0)]
            public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        private static partial class NativeMethods
        {
            [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
            [LibraryImport("user32.dll")]
            public static partial IntPtr GetMessageExtraInfo();

            [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
            [LibraryImport("user32.dll", SetLastError = true)]
            public static partial uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);
        }

        private static INPUT InputForKey(ushort key, bool releasing)
        {
            return new INPUT
            {
                type = INPUT_KEYBOARD,
                u = new InputUnion
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = key,
                        wScan = 0,
                        dwFlags = releasing ? KEYEVENTF_KEYUP : 0,
                        dwExtraInfo = NativeMethods.GetMessageExtraInfo(),
                    }
                }
            };
        }

        private static void SendInputs(params INPUT[] inputs)
        {
            _ = NativeMethods.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        public static void PressKey(ushort key)
        {
            SendInputs(InputForKey(key, false));
        }

        public static void ReleaseKey(ushort key)
        {
            SendInputs(InputForKey(key, true));
        }

        public static void PressAndReleaseKey(ushort key)
        {
            SendInputs(
                InputForKey(key, false),
                InputForKey(key, true)
            );
        }
    }
}
