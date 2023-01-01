using Renci.SshNet;
using System.Globalization;

namespace UsbUpdater
{
    class Program
    {
        private static void Main()
        {

            try
            {
                Console.WriteLine(Properties.Resources.ResourceManager.GetString("Title", CultureInfo.CurrentUICulture));

                Console.Write(Properties.Resources.ResourceManager.GetString("HostnamePrompt", CultureInfo.CurrentUICulture));
                string hostname = Console.ReadLine() ?? "beaglebone.local";
                Console.Write(Properties.Resources.ResourceManager.GetString("UsernamePrompt", CultureInfo.CurrentUICulture));
                string username = Console.ReadLine() ?? "retrospy";
                Console.Write(Properties.Resources.ResourceManager.GetString("PasswordPrompt", CultureInfo.CurrentUICulture));
                string password = Console.ReadLine() ?? "retrospy";

                Console.Write("");
                Console.WriteLine("\nLogging into " + (string.IsNullOrEmpty(hostname) ? "beaglebone.local" : hostname) + "...");

                using (SshClient _client = new(string.IsNullOrEmpty(hostname) ? "beaglebone.local" : hostname,
                                            string.IsNullOrEmpty(username) ? "retrospy" : username,
                                            string.IsNullOrEmpty(password) ? "retrospy" : password))
                {
                    _client.Connect();
                    ShellStream _data = _client.CreateShellStream("", 0, 0, 0, 0, 1000);

                    _data.WriteLine("sudo /usr/local/bin/update-usb-retrospy.sh");

                    while (true)
                    {
                        while (!_data.DataAvailable) { };
                        string line = _data.ReadLine();
                        Console.WriteLine(line);
                        if (line == "Installation complete!")
                        {
                            break;
                        }
                    }
                }

                Console.WriteLine("");
                Console.WriteLine(Properties.Resources.ResourceManager.GetString("Exit", CultureInfo.CurrentUICulture));

                _ = Console.ReadLine();


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
