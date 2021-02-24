using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace GBPemu
{
    public class COMPortInfo
    {
        public string PortName { get; set; }
        public string FriendlyName { get; set; }
    }

    public class ListView<T>
    {
        private readonly List<T> _items;

        public CollectionView Items { get; private set; }
        public T SelectedItem { get; set; }

        public ListView()
        {
            _items = new List<T>();
            Items = new CollectionView(_items);
        }

        public void UpdateContents(IEnumerable<T> items)
        {
            _items.Clear();
            _items.AddRange(items);

            if (Items.Dispatcher.CheckAccess())
            {
                Items.Refresh();
            }
            else
            {
                Items.Dispatcher.Invoke(() =>
                {
                    Items.Refresh();
                });
            }
        }

        public void SelectFirst()
        {
            if (_items.Count > 0)
            {
                SelectedItem = _items[0];
            }
        }

        public void SelectId(int id)
        {
            if (_items.Count > 0 && id >= 0 && id < _items.Count)
            {
                SelectedItem = _items[id];
            }
            else
            {
                SelectFirst();
            }
        }

        public void SelectIdFromText(T text)
        {
            int index = _items.IndexOf(text);
            SelectId(index);
        }

        public int GetSelectedId()
        {
            if (SelectedItem != null)
            {
                return _items.IndexOf(SelectedItem);
            }
            return -1;
        }
    }

    public partial class SetupWindow : Window, INotifyPropertyChanged
    {
        private readonly SetupWindowViewModel _vm;
        private readonly DispatcherTimer _portListUpdateTimer;
        private readonly ResourceManager _resources;
        private bool isClosing;

    private void UpdatePortListThread()
        {
            Thread thread = new Thread(UpdatePortList);
            thread.Start();
        }

        public SetupWindow()
        {
            InitializeComponent();

            if (Properties.Settings.Default.UpgradeRequired)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default. UpgradeRequired = false;
                Properties.Settings.Default.Save();
            }


            isClosing = false;
            _vm = new SetupWindowViewModel();
            DataContext = _vm;
            _resources = Properties.Resources.ResourceManager;
            string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string strWorkPath = System.IO.Path.GetDirectoryName(strExeFilePath);

            _vm.FilterCOMPorts = Properties.Settings.Default.FilterCOMPorts;

            _portListUpdateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _portListUpdateTimer.Tick += (sender, e) => UpdatePortListThread();
            _portListUpdateTimer.Start();

            UpdatePortList();
            _vm.Ports.SelectIdFromText(Properties.Settings.Default.Port);

            //GoButton_Click(null, null);
        }

        private readonly object updatePortLock = new object();

        public event PropertyChangedEventHandler PropertyChanged;

        private int numPorts = 0;

        private void UpdatePortList()
        {
            if (!isClosing && Monitor.TryEnter(updatePortLock))
            {
                try
                {
                    var arduinoPorts = SetupCOMPortInformation();

                    foreach (var port in arduinoPorts)
                    {
                        SerialPort _serialPort = new SerialPort(port, 115200, Parity.None, 8, StopBits.One);
                        _serialPort.Handshake = Handshake.None;

                        _serialPort.ReadTimeout = 500;
                        _serialPort.WriteTimeout = 500;

                        try
                        {
                            _serialPort.Open();
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }

                        try
                        {
                            _serialPort.Write("\x88\x33\x0F\x00\x00\x00\x0F\x00\x00");
                        }
                        catch (Exception ex)
                        {
                            _serialPort.Close();
                            continue;
                        }

                        try
                        {
                            string result = null;
                            do
                            {
                                result = _serialPort.ReadLine();
                            } while (result != null && (result.StartsWith("!") || result.StartsWith("#")));

                            if (result == "parse_state:0\r")
                            {
                                _serialPort.Close();
                                Thread.Sleep(1000);

                                if (this.Dispatcher.CheckAccess())
                                {
                                    this.Hide();
                                }
                                else
                                {
                                    this.Dispatcher.Invoke(() =>
                                    {
                                        this.Hide();
                                    });
                                }

                                Properties.Settings.Default.Port = _vm.Ports.SelectedItem;
                                Properties.Settings.Default.FilterCOMPorts = _vm.FilterCOMPorts;
                                Properties.Settings.Default.Save();

                                

                                try
                                {
                                    if (this.Dispatcher.CheckAccess())
                                    {
                                        IControllerReader reader = InputSource.PRINTER.BuildReader(port);
                                        new GameBoyPrinterEmulatorWindow(reader).ShowDialog();
                                    }
                                    else
                                    {
                                        this.Dispatcher.Invoke(() =>
                                        {
                                            IControllerReader reader = InputSource.PRINTER.BuildReader(port);
                                            new GameBoyPrinterEmulatorWindow(reader).ShowDialog();
                                        });
                                    }


                                }
                                catch (UnauthorizedAccessException ex)
                                {
                                    MessageBox.Show(ex.Message, _resources.GetString("RetroSpy", CultureInfo.CurrentUICulture), MessageBoxButton.OK, MessageBoxImage.Error);
                                }

                                if (this.Dispatcher.CheckAccess())
                                {
                                    this.Show();
                                }
                                else
                                {
                                    this.Dispatcher.Invoke(() =>
                                    {
                                        this.Show();
                                    });
                                }

                            }
                            else
                            {
                                _serialPort.Close();
                                continue;
                            }
                        }
                        catch (Exception ex)
                        {
                            _serialPort.Close();
                            continue;
                        }

                    }

                }
                catch (TaskCanceledException)
                {
                    // Closing the window can cause this due to a race condition
                }
                finally
                {
                    Monitor.Exit(updatePortLock);
                }
            }
        }

        private static void GetTeensyPorts(List<string> arduinoPorts)
        {
            const uint vid = 0x16C0;
            const uint serPid = 0x483;
            string vidStr = "'%USB_VID[_]" + vid.ToString("X", CultureInfo.CurrentCulture) + "%'";
            using (var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PnPEntity WHERE DeviceID LIKE " + vidStr))
            {
                foreach (var mgmtObject in searcher.Get())
                {
                    var DeviceIdParts = ((string)mgmtObject["PNPDeviceID"]).Split("\\".ToArray());
                    if (DeviceIdParts[0] != "USB") break;

                    int start = DeviceIdParts[1].IndexOf("PID_", StringComparison.Ordinal) + 4;
                    uint pid = Convert.ToUInt32(DeviceIdParts[1].Substring(start, 4), 16);
                    if (pid == serPid)
                    {
                        //uint serNum = Convert.ToUInt32(DeviceIdParts[2], CultureInfo.CurrentCulture);
                        string port = (((string)mgmtObject["Caption"]).Split("()".ToArray()))[1];

                        var hwid = ((string[])mgmtObject["HardwareID"])[0];
                        switch (hwid.Substring(hwid.IndexOf("REV_", StringComparison.Ordinal) + 4, 4))
                        {
                            case "0273":
                                //board = PJRC_Board.Teensy_LC;
                                break;

                            case "0274":
                                //board = PJRC_Board.Teensy_30;
                                break;

                            case "0275":
                                //board = PJRC_Board.Teensy_31_2;
                                break;

                            case "0276":
                                arduinoPorts.Add(port + " (Teensy 3.5)");
                                break;

                            case "0277":
                                arduinoPorts.Add(port + " (Teensy 3.6)");
                                break;

                            case "0279":
                                //board = PJRC_Board.Teensy_40;
                                break;

                            default:
                                //board = PJRC_Board.unknown;
                                break;
                        }
                    }
                }
            }
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

        private List<string> SetupCOMPortInformation()
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
                if (_vm.FilterCOMPorts || port.FriendlyName.Contains("Arduino"))
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

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            Properties.Settings.Default.Port = _vm.Ports.SelectedItem;
            Properties.Settings.Default.FilterCOMPorts = _vm.FilterCOMPorts;
            Properties.Settings.Default.Save();

            IControllerReader reader = InputSource.PRINTER.BuildReader("COM16");

            try
            {
                new GameBoyPrinterEmulatorWindow(reader).ShowDialog();
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, _resources.GetString("RetroSpy", CultureInfo.CurrentUICulture), MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Show();
        }

        private void ComPortCombo_SelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdatePortList();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            isClosing = true;
        }
    }

    public class SetupWindowViewModel : INotifyPropertyChanged
    {
        public ListView<string> Ports { get; set; }
        public bool FilterCOMPorts { get; set; }

        private Visibility _comPortOptionVisibility;

        public Visibility ComPortOptionVisibility
        {
            get => _comPortOptionVisibility;
            set
            {
                _comPortOptionVisibility = value;
                NotifyPropertyChanged("ComPortOptionVisibility");
            }
        }


        public SetupWindowViewModel()
        {
            Ports = new ListView<string>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string prop)
        {
            if (PropertyChanged == null)
            {
                return;
            }

            PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

    }
}