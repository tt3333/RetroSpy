using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsbUpdater
{
    class Program
    {
        static void Main(string[] args)
        {

            try {
                Console.WriteLine("=== RetroSpy USB Updater ===");

                Console.Write("Hostname/IP Address (beaglebone.local): ");
                var hostname = Console.ReadLine();
                Console.Write("Username (retrospy): ");
                var username = Console.ReadLine();
                Console.Write("Password (retrospy): ");
                var password = Console.ReadLine();

                Console.Write("");
                Console.WriteLine("\nLogging into " + (hostname == "" ? "beaglebone.local" : hostname) + "...");

                var _client = new SshClient(hostname == "" ? "beaglebone.local" : hostname, 
                                            username == "" ? "retrospy" : username, 
                                            password == "" ? "retrospy" : password);

                _client.Connect();
                var _data = _client.CreateShellStream("", 0, 0, 0, 0, 1000);

                _data.WriteLine("sudo /usr/local/bin/update-usb-retrospy.sh");

                while (true)
                {
                    while (!_data.DataAvailable) ;
                    var line = _data.ReadLine();
                    Console.WriteLine(line);
                    if (line == "Installation complete!")
                        break;
                }

                Console.WriteLine("\nPress Enter to Exit");
                Console.ReadLine();

            }
            catch (Exception ex)
            {
                Console.WriteLine("\nUpdater encountered an error.  Message: " + ex.Message);
                Console.WriteLine("");
                Console.WriteLine("Press Enter to Exit");
                Console.ReadLine();
            }
        }
    }
}
