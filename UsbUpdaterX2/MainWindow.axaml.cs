using Avalonia.Controls;
using Avalonia.Interactivity;
using Renci.SshNet;
using System;
using System.Threading;
using Avalonia.Threading;
using MessageBox.Avalonia.Enums;
using System.Threading.Tasks;

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

                if (Dispatcher.UIThread.CheckAccess())
                {
                    this.goButton.IsEnabled = false;
                    txtboxData.Text = string.Empty;
                    txtboxData.Text += "\n";
                    txtboxData.Text += "\nLogging into " + (string.IsNullOrEmpty(hostname) ? "beaglebone.local" : hostname) + "...\n";
                    txtboxData.CaretIndex = int.MaxValue;
                }
                else
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        this.goButton.IsEnabled = false;
                        txtboxData.Text = string.Empty;
                        txtboxData.Text += "\n";
                        txtboxData.Text += "\nLogging into " + (string.IsNullOrEmpty(hostname) ? "beaglebone.local" : hostname) + "...\n";
                        txtboxData.CaretIndex = int.MaxValue;
                    });
                }

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
                        if (Dispatcher.UIThread.CheckAccess())
                        {
                            txtboxData.Text += line;
                            txtboxData.CaretIndex = int.MaxValue;
                        }
                        else
                        {
                            Dispatcher.UIThread.Post(() =>
                            {
                                txtboxData.Text += line;
                                txtboxData.CaretIndex = int.MaxValue;
                            });
                        }

                        if (line.Contains("Installation complete!"))
                        {
                            if (Dispatcher.UIThread.CheckAccess())
                            {
                                AvaloniaMessageBox("RetroSpy", "Installation complete! Please reboot your device.", ButtonEnum.Ok, MessageBox.Avalonia.Enums.Icon.Info);
                                this.goButton.IsEnabled = true;
                            }
                            else
                            {
                                Dispatcher.UIThread.Post(() =>
                                {
                                    AvaloniaMessageBox("RetroSpy", "Installation complete! Please reboot your device.", ButtonEnum.Ok, MessageBox.Avalonia.Enums.Icon.Info);
                                    this.goButton.IsEnabled = true;
                                });
                            }
                            break;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                if (Dispatcher.UIThread.CheckAccess())
                {

                    txtboxData.Text += "\nUpdater encountered an error.  Message: " + ex.Message + "\n";
                    AvaloniaMessageBox("RetroSpy", ex.Message, ButtonEnum.Ok, MessageBox.Avalonia.Enums.Icon.Error);
                    this.goButton.IsEnabled = true;
                }
                else
                {
                    Dispatcher.UIThread.Post(() =>
                    {

                        txtboxData.Text += "\nUpdater encountered an error.  Message: " + ex.Message + "\n";
                        AvaloniaMessageBox("RetroSpy", ex.Message, ButtonEnum.Ok, MessageBox.Avalonia.Enums.Icon.Error);
                        this.goButton.IsEnabled = true;
                    });
                }

            }


        }

        private void GoButton_Click(object? sender, RoutedEventArgs? e)
        {
            Thread thread = new(UpdateThread);
            thread.Start();
        }

        private static void AvaloniaMessageBox(string? title, string message, ButtonEnum buttonType, MessageBox.Avalonia.Enums.Icon iconType)
        {
            using var source = new CancellationTokenSource();
            _ = MessageBox.Avalonia.MessageBoxManager
            .GetMessageBoxStandardWindow(title ?? "Unknown Title Argument", message, buttonType, iconType)
                        .Show().ContinueWith(t => source.Cancel(), TaskScheduler.FromCurrentSynchronizationContext());
            Dispatcher.UIThread.MainLoop(source.Token);
        }
    }
}