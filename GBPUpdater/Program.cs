using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Management;
using System.IO.Ports;
using System.Diagnostics;

namespace GBPUpdater
{
    class Program
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

            String[] portNames = System.IO.Ports.SerialPort.GetPortNames();
            foreach (String s in portNames)
            {
                // s is like "COM14"
                COMPortInfo ci = new COMPortInfo
                {
                    PortName = s,
                    FriendlyName = s
                };
                comPortInformation.Add(ci);
            }

            String[] usbDevs = GetUSBCOMDevices();
            foreach (String s in usbDevs)
            {
                // Name will be like "USB Bridge (COM14)"
                int start = s.IndexOf("(COM", StringComparison.Ordinal) + 1;
                if (start >= 0)
                {
                    int end = s.IndexOf(")", start + 3, StringComparison.Ordinal);
                    if (end >= 0)
                    {
                        // cname is like "COM14"
                        String cname = s.Substring(start, end - start);
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
            foreach (var port in comPortInformation)
            {
                if(port.FriendlyName.Contains("Arduino"))
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

        static void Main(string[] args)
        {

            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);

            Console.WriteLine(tempDirectory);

            Console.Write("Downloading latest firmware...");
            WebClient webClient = new WebClient();
            webClient.DownloadFile("https://github.com/retrospy/RetroSpy/releases/latest/download/GBP_Firmware.zip", 
                Path.Combine(tempDirectory, "GBP_Firmware.zip"));
            Console.WriteLine("done.\n");

            Console.Write("Decompressing firmware package...");
            ZipFile.ExtractToDirectory(Path.Combine(tempDirectory, "GBP_Firmware.zip"), tempDirectory);
            Console.WriteLine("done.\n");

            Console.Write("Searching for GameBoy Printer Emulator...");
            
            SerialPort _serialPort = null;

            var arduinoPorts = SetupCOMPortInformation();
            string gbpemuPort = "";
            bool foundPort = false;

            foreach (var port in arduinoPorts)
            {
                _serialPort = new SerialPort(port, 115200, Parity.None, 8, StopBits.One);
                _serialPort.Handshake = Handshake.None;

                _serialPort.ReadTimeout = 500;
                _serialPort.WriteTimeout = 500;

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
                } while (result != null && (result.StartsWith("!") || result.StartsWith("#")));

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

            if (!foundPort)
            {
                Console.WriteLine("cannot find RetroSpy GameBoy Printer Emulator.\n");
            }
            else
            {
                Console.WriteLine("found on " + gbpemuPort + ".\n");


                Console.WriteLine("Updating firmware...");

                var processInfo = new ProcessStartInfo("cmd.exe", "/c avrdude.exe -Cavrdude.conf -v -patmega328p -carduino -P" + gbpemuPort + " -b115200 -D -Uflash:w:firmware.ino.hex:i")
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

                Console.WriteLine("...done.\n");

                Console.WriteLine("Update Complete!\n");
            }

            Console.WriteLine("Press Enter to Exit");
            Console.Read();


        }
    }
}
