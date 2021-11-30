using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;

namespace GBPUpdater
{
    internal class Program
    {
        public class COMPortInfo
        {
            public string PortName { get; set; }
            public string FriendlyName { get; set; }
        }

        private static string[] GetUSBCOMDevices()
        {
            List<string> list = new List<string>();

            ManagementObjectSearcher searcher2 = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity");
            foreach (ManagementObject mo2 in searcher2.Get())
            {
                if (mo2["Name"] != null)
                {
                    string name = mo2["Name"].ToString();
                    // Name will have a substring like "(COM12)" in it.
                    if (name.Contains("(COM"))
                    {
                        list.Add(name);
                    }
                }
            }
            searcher2.Dispose();
            // remove duplicates, sort alphabetically and convert to array
            string[] usbDevices = list.Distinct().OrderBy(s => s).ToArray();
            return usbDevices;
        }

        private static List<string> SetupCOMPortInformation()
        {
            List<COMPortInfo> comPortInformation = new List<COMPortInfo>();

            string[] portNames = SerialPort.GetPortNames();
            foreach (string s in portNames)
            {
                // s is like "COM14"
                COMPortInfo ci = new COMPortInfo
                {
                    PortName = s,
                    FriendlyName = s
                };
                comPortInformation.Add(ci);
            }

            string[] usbDevs = GetUSBCOMDevices();
            foreach (string s in usbDevs)
            {
                // Name will be like "USB Bridge (COM14)"
                int start = s.IndexOf("(COM", StringComparison.Ordinal) + 1;
                if (start >= 0)
                {
                    int end = s.IndexOf(")", start + 3, StringComparison.Ordinal);
                    if (end >= 0)
                    {
                        // cname is like "COM14"
                        string cname = s.Substring(start, end - start);
                        for (int i = 0; i < comPortInformation.Count; i++)
                        {
                            if (comPortInformation[i].PortName == cname)
                            {
                                comPortInformation[i].FriendlyName = s.Remove(start - 1).TrimEnd();
                            }
                        }
                    }
                }
            }

            List<string> ports = new List<string>();
            foreach (COMPortInfo port in comPortInformation)
            {
                if (port.FriendlyName.Contains("Arduino"))
                {
                    ports.Add(port.PortName);
                }
                else if (port.FriendlyName.Contains("CH340") || port.FriendlyName.Contains("CH341"))
                {
                    ports.Add(port.PortName);
                }
            }

            return ports;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "I am fishing for the GBP, so I expect failures. I also want to report any errors that occur.")]
        private static void Main()
        {

            try
            {
                string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                _ = Directory.CreateDirectory(tempDirectory);

                Console.WriteLine(tempDirectory);

                Console.Write(Properties.Resources.ResourceManager.GetString("Downloading", CultureInfo.CurrentUICulture));
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadFile("https://github.com/retrospy/RetroSpy/releases/latest/download/GBP_Firmware.zip",
                        Path.Combine(tempDirectory, "GBP_Firmware.zip"));
                }
                Console.WriteLine(Properties.Resources.ResourceManager.GetString("Done", CultureInfo.CurrentUICulture));

                Console.Write(Properties.Resources.ResourceManager.GetString("Decompressing", CultureInfo.CurrentUICulture));
                ZipFile.ExtractToDirectory(Path.Combine(tempDirectory, "GBP_Firmware.zip"), tempDirectory);
                Console.WriteLine(Properties.Resources.ResourceManager.GetString("Done", CultureInfo.CurrentUICulture));

                Console.Write(Properties.Resources.ResourceManager.GetString("Searching", CultureInfo.CurrentUICulture));

                SerialPort _serialPort = null;

                List<string> arduinoPorts = SetupCOMPortInformation();
                string gbpemuPort = "";
                bool foundPort = false;

                foreach (string port in arduinoPorts)
                {
                    using (_serialPort = new SerialPort(port, 115200, Parity.None, 8, StopBits.One)
                    {
                        Handshake = Handshake.None,

                        ReadTimeout = 500,
                        WriteTimeout = 500
                    })
                    {

                        try
                        {
                            _serialPort.Open();
                        }
                        catch (Exception)
                        {
                            continue;
                        }

                        try
                        {
                            _serialPort.Write("\x88\x33\x0F\x00\x00\x00\x0F\x00\x00");
                        }
                        catch (Exception)
                        {
                            _serialPort.Close();
                            continue;
                        }

                        string result = null;
                        do
                        {
                            result = _serialPort.ReadLine();
                        } while (result != null && (result.StartsWith("!", StringComparison.Ordinal) || result.StartsWith("#", StringComparison.Ordinal)));

                        if (result == "parse_state:0\r" || result.Contains("d=debug"))
                        {
                            foundPort = true;
                            gbpemuPort = port;
                            _serialPort.Close();
                        }
                        else
                        {
                            _serialPort.Close();
                            continue;
                        }
                    }
                }

                if (!foundPort)
                {
                    Console.WriteLine(Properties.Resources.ResourceManager.GetString("NotFound", CultureInfo.CurrentUICulture));
                }
                else
                {
                    Console.WriteLine("found on " + gbpemuPort + ".\n");


                    Console.WriteLine(Properties.Resources.ResourceManager.GetString("Updating", CultureInfo.CurrentUICulture));

                    ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", "/c avrdude.exe -Cavrdude.conf -v -patmega328p -carduino -P" + gbpemuPort + " -b115200 -D -Uflash:w:firmware.ino.hex:i")
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        WorkingDirectory = tempDirectory
                    };

                    StringBuilder sb = new StringBuilder();
                    Process p = Process.Start(processInfo);
                    p.OutputDataReceived += (sender, args1) => sb.AppendLine(args1.Data);
                    p.ErrorDataReceived += (sender, args1) => sb.AppendLine(args1.Data);
                    p.BeginOutputReadLine();
                    p.BeginErrorReadLine();
                    p.WaitForExit();
                    Console.WriteLine(sb.ToString());

                    Console.Write(Properties.Resources.ResourceManager.GetString("Dots", CultureInfo.CurrentUICulture));
                    Console.WriteLine(Properties.Resources.ResourceManager.GetString("Done", CultureInfo.CurrentUICulture));

                    Console.WriteLine(Properties.Resources.ResourceManager.GetString("Complete", CultureInfo.CurrentUICulture));
                }

                Console.WriteLine(Properties.Resources.ResourceManager.GetString("Exit", CultureInfo.CurrentUICulture));
                _ = Console.Read();

            }
            catch (Exception ex)
            {
                Console.WriteLine("\nUpdater encountered an error.  Message: " + ex.Message);
                Console.WriteLine("");
                Console.WriteLine(Properties.Resources.ResourceManager.GetString("Exit", CultureInfo.CurrentUICulture));
                _ = Console.ReadLine();
            }

        }
    }
}
