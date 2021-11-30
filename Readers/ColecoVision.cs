using System;
using System.Globalization;

namespace RetroSpy.Readers
{
    public static class ColecoVision
    {
        private const int ROLLER_PACKET_SIZE = 18;
        private const int PACKET_SIZE = 11;

        private static float maxX = float.MinValue;
        private static float maxY = float.MinValue;

        private static readonly string[] ROLLER_BUTTONS = {
            null, null, null, null, "blue", null, null, null, null, "purple"
        };

        private static readonly string[] BUTTONS = {
            "up", "down", "left", "right", "L", null, null, null, null, "R"
        };

        private static readonly string[] Y_ROLLER_BUTTONS = {
            null, null, null, null, "blue_2", null, null, null, null, "purple_2"
        };

        private static readonly string[] Y_BUTTONS = {
            "up_2", "down_2", "left_2", "right_2", "L_2", null, null, null, null, "R_2"
        };

        private static byte[] y_packet;

        public static ControllerStateEventArgs ReadFromPacket(byte[] packet)
        {
            if (packet == null)
                throw new ArgumentNullException(nameof(packet));

            if (packet.Length < PACKET_SIZE)
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
            float x = 0;
            float y = 0;

            if (packet[3] != 0x00)
            {
                x = 1;
            }
            else if (packet[2] != 0x00)
            {
                x = -1;
            }

            if (packet[0] != 0x00)
            {
                y = 1;
            }
            else if (packet[1] != 0x00)
            {
                y = -1;
            }

            if (y != 0 || x != 0)
            {
                // point on the unit circle at the same angle
                double radian = Math.Atan2(y, x);
                float x1 = (float)Math.Cos(radian);
                float y1 = (float)Math.Sin(radian);

                // Don't let magnitude exceed the unit circle
                if (Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)) > 1.0)
                {
                    x = x1;
                    y = y1;
                }
            }

            state.SetAnalog("x", x, 0);
            state.SetAnalog("y", y, 0);

            state.SetButton("1", packet[5] == 0 && packet[6] == 0 && packet[7] == 0 && packet[8] != 0);
            state.SetButton("2", packet[5] == 0 && packet[6] == 0 && packet[7] != 0 && packet[8] == 0);
            state.SetButton("3", packet[5] != 0 && packet[6] == 0 && packet[7] == 0 && packet[8] != 0);
            state.SetButton("4", packet[5] != 0 && packet[6] != 0 && packet[7] != 0 && packet[8] == 0);
            state.SetButton("5", packet[5] == 0 && packet[6] != 0 && packet[7] != 0 && packet[8] == 0);
            state.SetButton("6", packet[5] != 0 && packet[6] == 0 && packet[7] == 0 && packet[8] == 0);
            state.SetButton("7", packet[5] == 0 && packet[6] == 0 && packet[7] != 0 && packet[8] != 0);
            state.SetButton("8", packet[5] == 0 && packet[6] != 0 && packet[7] != 0 && packet[8] != 0);
            state.SetButton("9", packet[5] == 0 && packet[6] != 0 && packet[7] == 0 && packet[8] == 0);
            state.SetButton("star", packet[5] == 0 && packet[6] != 0 && packet[7] == 0 && packet[8] != 0);
            state.SetButton("0", packet[5] != 0 && packet[6] != 0 && packet[7] == 0 && packet[8] == 0);
            state.SetButton("pound", packet[5] != 0 && packet[6] == 0 && packet[7] != 0 && packet[8] == 0);
            state.SetButton("purple", packet[5] != 0 && packet[6] != 0 && packet[7] == 0 && packet[8] != 0);
            state.SetButton("blue", packet[5] != 0 && packet[6] == 0 && packet[7] != 0 && packet[8] != 0);

            if (packet.Length == PACKET_SIZE)
            {

                for (int i = 0; i < 64; ++i)
                {
                    state.SetButton("E" + i.ToString(CultureInfo.CurrentCulture), i == (packet[10] - 11));
                }
            }
            else if (packet.Length == ROLLER_PACKET_SIZE)
            {
                for (int i = 0; i < ROLLER_BUTTONS.Length; ++i)
                {
                    if (string.IsNullOrEmpty(ROLLER_BUTTONS[i]))
                    {
                        continue;
                    }

                    state.SetButton(ROLLER_BUTTONS[i], packet[i] != 0x00);
                }

                if (y_packet != null)
                {
                    for (int i = 0; i < Y_BUTTONS.Length; ++i)
                    {
                        if (string.IsNullOrEmpty(Y_BUTTONS[i]))
                        {
                            continue;
                        }

                        state.SetButton(Y_BUTTONS[i], y_packet[i] != 0x00);
                    }

                    float x_2 = 0;
                    float y_2 = 0;

                    if (y_packet[3] != 0x00)
                    {
                        x_2 = 1;
                    }
                    else if (y_packet[2] != 0x00)
                    {
                        x_2 = -1;
                    }

                    if (y_packet[0] != 0x00)
                    {
                        y_2 = 1;
                    }
                    else if (y_packet[1] != 0x00)
                    {
                        y_2 = -1;
                    }

                    if (y_2 != 0 || x_2 != 0)
                    {
                        // point on the unit circle at the same angle
                        double radian = Math.Atan2(y_2, x_2);
                        float x1_2 = (float)Math.Cos(radian);
                        float y1_2 = (float)Math.Sin(radian);

                        // Don't let magnitude exceed the unit circle
                        if (Math.Sqrt(Math.Pow(x_2, 2) + Math.Pow(y_2, 2)) > 1.0)
                        {
                            x_2 = x1_2;
                            y_2 = y1_2;
                        }
                    }

                    state.SetAnalog("x_2", x_2, 0);
                    state.SetAnalog("y_2", y_2, 0);

                    state.SetButton("1_2", y_packet[5] == 0 && y_packet[6] == 0 && y_packet[7] == 0 && y_packet[8] != 0);
                    state.SetButton("2_2", y_packet[5] == 0 && y_packet[6] == 0 && y_packet[7] != 0 && y_packet[8] == 0);
                    state.SetButton("3_2", y_packet[5] != 0 && y_packet[6] == 0 && y_packet[7] == 0 && y_packet[8] != 0);
                    state.SetButton("4_2", y_packet[5] != 0 && y_packet[6] != 0 && y_packet[7] != 0 && y_packet[8] == 0);
                    state.SetButton("5_2", y_packet[5] == 0 && y_packet[6] != 0 && y_packet[7] != 0 && y_packet[8] == 0);
                    state.SetButton("6_2", y_packet[5] != 0 && y_packet[6] == 0 && y_packet[7] == 0 && y_packet[8] == 0);
                    state.SetButton("7_2", y_packet[5] == 0 && y_packet[6] == 0 && y_packet[7] != 0 && y_packet[8] != 0);
                    state.SetButton("8_2", y_packet[5] == 0 && y_packet[6] != 0 && y_packet[7] != 0 && y_packet[8] != 0);
                    state.SetButton("9_2", y_packet[5] == 0 && y_packet[6] != 0 && y_packet[7] == 0 && y_packet[8] == 0);
                    state.SetButton("star_2", y_packet[5] == 0 && y_packet[6] != 0 && y_packet[7] == 0 && y_packet[8] != 0);
                    state.SetButton("0_2", y_packet[5] != 0 && y_packet[6] != 0 && y_packet[7] == 0 && y_packet[8] == 0);
                    state.SetButton("pound_2", y_packet[5] != 0 && y_packet[6] == 0 && y_packet[7] != 0 && y_packet[8] == 0);
                    state.SetButton("purple_2", y_packet[5] != 0 && y_packet[6] != 0 && y_packet[7] == 0 && y_packet[8] != 0);
                    state.SetButton("blue_2", y_packet[5] != 0 && y_packet[6] == 0 && y_packet[7] != 0 && y_packet[8] != 0);

                    for (int i = 0; i < Y_ROLLER_BUTTONS.Length; ++i)
                    {
                        if (string.IsNullOrEmpty(Y_ROLLER_BUTTONS[i]))
                        {
                            continue;
                        }

                        state.SetButton(Y_ROLLER_BUTTONS[i], y_packet[i] != 0x00);
                    }
                }

                sbyte roller_y = y_packet != null ? (sbyte)SignalTool.ReadByteBackwards(y_packet, 10) : (sbyte)0;
                sbyte roller_x = (sbyte)SignalTool.ReadByteBackwards(packet, 10);

                if (Math.Abs(roller_x) != 0 && Math.Abs(roller_x) > maxX)
                    maxX = Math.Abs(roller_x);
                
                if (Math.Abs(roller_y) != 0 && Math.Abs(roller_y) > maxY)
                    maxY = Math.Abs(roller_y);

                SignalTool.SetMouseProperties(-1.0f * roller_x / maxX, -1.0f * roller_y / maxY, roller_x, roller_y, state);    
            }

            return state.Build();
        }

        public static ControllerStateEventArgs ReadFromSecondColecoVisionController(byte[] packet)
        {
            if (packet == null)
                throw new ArgumentNullException(nameof(packet));

            if (packet.Length != ROLLER_PACKET_SIZE)
            {
                return null;
            }

            y_packet = packet;

            return null;
        }
    }
}