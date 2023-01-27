using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using MessageBox.Avalonia.Enums;
using Renci.SshNet;
using System;
using System.Threading;

namespace UsbUpdaterX2
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void UpdateThread()
        {
            try
            {
                string hostname = txtboxHostname.Text;
                string username = txtboxUserName.Text;
                string password = txtboxPassword.Text;


                Dispatcher.UIThread.Post(() =>
                {
                    this.goButton.IsEnabled = false;
                    txtboxData.Text = string.Empty;
                    txtboxData.Text += "\n";
                    txtboxData.Text += "\nLogging into " + (string.IsNullOrEmpty(hostname) ? "beaglebone.local" : hostname) + "...\n";
                    txtboxData.CaretIndex = int.MaxValue;
                });


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
                        string line = _data.Read();

                        Dispatcher.UIThread.Post(() =>
                        {
                            txtboxData.Text += line;
                            txtboxData.CaretIndex = int.MaxValue;
                        });

                        if (line.Contains("Device needs full image installed!"))
                        {
                            Dispatcher.UIThread.Post(async () =>
                            {
                                var m = MessageBox.Avalonia.MessageBoxManager
                                    .GetMessageBoxStandardWindow("RetroSpy", "Unfortunately, this device requires a full reimage.\nIf you are unsure how to do this please reach out to support@retro-spy.com.", ButtonEnum.Ok, MessageBox.Avalonia.Enums.Icon.Info);
                                await m.ShowDialog(this);
                                goButton.IsEnabled = true;
                            });

                            break;
                        }
                        else if (line.Contains("Installation complete!"))
                        {

                            Dispatcher.UIThread.Post(async () =>
                            {
                                var m = MessageBox.Avalonia.MessageBoxManager
                                    .GetMessageBoxStandardWindow("RetroSpy", "Installation complete! Please reboot your device.", ButtonEnum.Ok, MessageBox.Avalonia.Enums.Icon.Info);
                                await m.ShowDialog(this);
                                goButton.IsEnabled = true;
                            });

                            break;
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                Dispatcher.UIThread.Post(async() =>
                {
                    txtboxData.Text += "\nUpdater encountered an error.  Message: " + ex.Message + "\n";
                    var m = MessageBox.Avalonia.MessageBoxManager
                        .GetMessageBoxStandardWindow("RetroSpy", ex.Message, ButtonEnum.Ok, MessageBox.Avalonia.Enums.Icon.Error);
                    await m.ShowDialog(this);
                    goButton.IsEnabled = true;
                });


            }
        }

        private void GoButton_Click(object? sender, RoutedEventArgs? e)
        {
            Thread thread = new(UpdateThread);
            thread.Start();
        }
    }
}