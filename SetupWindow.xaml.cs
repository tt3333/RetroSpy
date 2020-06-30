using RetroSpy.Readers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace RetroSpy
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

    public partial class SetupWindow : Window
    {
        private readonly SetupWindowViewModel _vm;
        private readonly DispatcherTimer _portListUpdateTimer;
        private readonly DispatcherTimer _xiAndGamepadListUpdateTimer;
        private readonly List<Skin> _skins;
        private readonly List<string> _excludedSources;
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
            isClosing = false;
            _vm = new SetupWindowViewModel();
            DataContext = _vm;
            _excludedSources = new List<string>();
            _resources = Properties.Resources.ResourceManager;
            string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string strWorkPath = System.IO.Path.GetDirectoryName(strExeFilePath);

            string skinsDirectory = Path.Combine(strWorkPath, "skins");

            if (!Directory.Exists(skinsDirectory))
            {
                MessageBox.Show("Could not find skins folder!", _resources.GetString("RetroSpy", CultureInfo.CurrentUICulture), MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            LoadResults results = Skin.LoadAllSkinsFromParentFolder(skinsDirectory);
            _skins = results.SkinsLoaded;

            _vm.Skins.UpdateContents(_skins.Where(x => x.Type == InputSource.DEFAULT));

            string[] hiddenConsoles = Properties.Settings.Default.HiddleConsoleList.Split(';');
            foreach (string source in hiddenConsoles)
            {
                if (source.Length > 0)
                {
                    _excludedSources.Add(source);
                }
            }

            PopulateSources();

            _vm.DelayInMilliseconds = Properties.Settings.Default.Delay;

            _vm.StaticViewerWindowName = Properties.Settings.Default.StaticViewerWindowName;

            _vm.LegacyKeybindingBehavior = Properties.Settings.Default.LegacyKeybindingBehavior;

            _vm.FilterCOMPorts = Properties.Settings.Default.FilterCOMPorts;

            _portListUpdateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _portListUpdateTimer.Tick += (sender, e) => UpdatePortListThread();
            _portListUpdateTimer.Start();

            _xiAndGamepadListUpdateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            _xiAndGamepadListUpdateTimer.Tick += (sender, e) =>
            {
                if (_vm.Sources.SelectedItem == InputSource.PAD)
                {
                    UpdateGamepadList();
                }
                else if (_vm.Sources.SelectedItem == InputSource.PC360)
                {
                    UpdateXIList();
                }
                //else if (_vm.Sources.SelectedItem == InputSource.XBOX)
                //{
                //    updateBeagleList();
                //}
                //else if (_vm.Sources.SelectedItem == InputSource.WII)
                //{
                //    updateBeagleI2CList();
                //}
            };
            _xiAndGamepadListUpdateTimer.Start();

            UpdatePortList();
            _vm.Ports.SelectIdFromText(Properties.Settings.Default.Port);
            _vm.Ports2.SelectIdFromText(Properties.Settings.Default.Port2);
            _vm.XIAndGamepad.SelectFirst();
            _vm.Sources.SelectId(Properties.Settings.Default.Source);
            _vm.Skins.SelectId(Properties.Settings.Default.Skin);
            _vm.Hostname = Properties.Settings.Default.Hostname;

            if (results.ParseErrors.Count > 0)
            {
                ShowSkinParseErrors(results.ParseErrors);
            }
        }

        private void PopulateSources()
        {
            List<InputSource> prunedSources = new List<InputSource>();
            foreach (InputSource source in InputSource.ALL)
            {
                if (!_excludedSources.Contains(source.Name))
                {
                    prunedSources.Add(source);
                }
            }
            _vm.Sources.UpdateContents(prunedSources);
        }

        private void ShowSkinParseErrors(List<string> errs)
        {
            StringBuilder msg = new StringBuilder();
            msg.AppendLine("Some skins were unable to be parsed:");
            foreach (string err in errs)
            {
                msg.AppendLine(err);
            }

            MessageBox.Show(msg.ToString(), _resources.GetString("RetroSpy", CultureInfo.CurrentUICulture), MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private readonly object updatePortLock = new object();

        private void UpdatePortList()
        {
            if (!isClosing && Monitor.TryEnter(updatePortLock))
            {
                try
                {
                    var arduinoPorts = SetupCOMPortInformation();
                    //List<string> arduinoPorts = GetArduinoPorts();
                    GetTeensyPorts(arduinoPorts);

                    arduinoPorts.Sort();

                    string[] ports = arduinoPorts.ToArray<string>();

                    if (ports.Length == 0)
                    {
                        ports = new string[1];
                        ports[0] = "No Arduino/Teensy Found";
                        _vm.Ports.UpdateContents(ports);
                        _vm.Ports2.UpdateContents(ports);
                    }
                    else
                    {
                        _vm.Ports.UpdateContents(ports);
                        string[] ports2 = new string[ports.Length + 1];
                        ports2[0] = "Not Connected";
                        for (int i = 0; i < ports.Length; ++i)
                        {
                            ports2[i + 1] = ports[i];
                        }
                        _vm.Ports2.UpdateContents(ports2);
                    }
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
                    ports.Add(String.Format(CultureInfo.CurrentCulture, "{0} ({1})", port.PortName, port.FriendlyName));
                }
                else if (port.FriendlyName.Contains("CH340") || port.FriendlyName.Contains("CH341"))
                {
                    ports.Add(String.Format(CultureInfo.CurrentCulture, "{0} (Generic Arduino)", port.PortName));
                }
            }

            return ports;
        }

        private void UpdateGamepadList()
        {
            _vm.XIAndGamepad.UpdateContents(GamepadReader.GetDevices());
        }

        private void UpdateXIList()
        {
            _vm.XIAndGamepad.UpdateContents(XInputReader.GetDevices());
        }

        private void UpdateBeagleList()
        {
            //_vm.XIAndGamepad.UpdateContents(XboxReader.GetDevices());
        }

        private void UpdateBeagleI2CList()
        {
            //_vm.XIAndGamepad.UpdateContents(WiiReaderV1.GetDevices());
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            Properties.Settings.Default.Port = _vm.Ports.SelectedItem;
            Properties.Settings.Default.Port2 = _vm.Ports2.SelectedItem;
            Properties.Settings.Default.Source = _vm.Sources.GetSelectedId();
            Properties.Settings.Default.Skin = _vm.Skins.GetSelectedId();
            Properties.Settings.Default.Delay = _vm.DelayInMilliseconds;
            Properties.Settings.Default.Background = _vm.Backgrounds.GetSelectedId();
            Properties.Settings.Default.Hostname = _vm.Hostname;
            Properties.Settings.Default.StaticViewerWindowName = _vm.StaticViewerWindowName;
            Properties.Settings.Default.LegacyKeybindingBehavior = _vm.LegacyKeybindingBehavior;
            Properties.Settings.Default.FilterCOMPorts = _vm.FilterCOMPorts;
            Properties.Settings.Default.Save();
            try
            {
                IControllerReader reader;
                if (_vm.Sources.SelectedItem == InputSource.PAD)
                {
                    reader = _vm.Sources.SelectedItem.BuildReader(_vm.XIAndGamepad.SelectedItem.ToString(CultureInfo.CurrentCulture));
                }
                else if (_vm.Sources.SelectedItem == InputSource.PC360)
                {
                    reader = _vm.Sources.SelectedItem.BuildReader(_vm.XIAndGamepad.SelectedItem.ToString(CultureInfo.CurrentCulture));
                }
                else if (_vm.Sources.SelectedItem == InputSource.PCKEYBOARD)
                {
                    reader = _vm.Sources.SelectedItem.BuildReader3;
                }
                else if (_vm.Sources.SelectedItem == InputSource.XBOX || _vm.Sources.SelectedItem == InputSource.PSCLASSIC ||
                         _vm.Sources.SelectedItem == InputSource.SWITCH || _vm.Sources.SelectedItem == InputSource.XBOX360 ||
                         _vm.Sources.SelectedItem == InputSource.GENMINI || _vm.Sources.SelectedItem == InputSource.C64MINI ||
                         _vm.Sources.SelectedItem == InputSource.NEOGEOMINI || _vm.Sources.SelectedItem == InputSource.PS3 
                         || _vm.Sources.SelectedItem == InputSource.PS4 || _vm.Sources.SelectedItem == InputSource.MISTER
                         || _vm.Sources.SelectedItem == InputSource.TG16MINI)
                {
                    reader = _vm.Sources.SelectedItem.BuildReader(txtHostname.Text);
                }
                else if (_vm.Sources.SelectedItem == InputSource.PADDLES || _vm.Sources.SelectedItem == InputSource.CD32 || _vm.Sources.SelectedItem == InputSource.ATARI5200)
                {
                    if (_vm.Ports.SelectedItem == _vm.Ports2.SelectedItem)
                    {
                        throw new ConfigParseException(_resources.GetString("Port1And2CannotBeTheSame", CultureInfo.CurrentUICulture));
                    }

                    reader = _vm.Sources.SelectedItem.BuildReader2(_vm.Ports.SelectedItem, _vm.Ports2.SelectedItem);
                }
                //else if (_vm.Sources.SelectedItem == InputSource.XBOX)
                //{
                //    reader = _vm.Sources.SelectedItem.BuildReader(_vm.XIAndGamepad.SelectedItem.ToString());
                //}
                //else if (_vm.Sources.SelectedItem == InputSource.WII)
                //{
                //    reader = _vm.Sources.SelectedItem.BuildReader(_vm.XIAndGamepad.SelectedItem.ToString());
                //}
                else
                {
                    reader = _vm.Sources.SelectedItem.BuildReader(_vm.Ports.SelectedItem);
                }
                if (_vm.DelayInMilliseconds > 0)
                {
#pragma warning disable CA2000 // Dispose objects before losing scope
                    reader = new DelayedControllerReader(reader, _vm.DelayInMilliseconds, _vm.LegacyKeybindingBehavior);
#pragma warning restore CA2000 // Dispose objects before losing scope
                }

                new ViewWindow(_vm.Skins.SelectedItem,
                                _vm.Backgrounds.SelectedItem,
                                reader, _vm.StaticViewerWindowName)
                    .ShowDialog();
            }
            catch (ConfigParseException ex)
            {
                MessageBox.Show(ex.Message, _resources.GetString("RetroSpy", CultureInfo.CurrentUICulture), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (System.Net.Sockets.SocketException)
            {
                MessageBox.Show(String.Format(new CultureInfo("en-US"), "Cannot connect to {0}.", txtHostname.Text), _resources.GetString("RetroSpy", CultureInfo.CurrentUICulture), MessageBoxButton.OK, MessageBoxImage.Error);
                   
            }

            Show();
        }

        private void SourceSelectComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_vm.Sources.SelectedItem == null)
            {
                return;
            }

            _vm.ComPortOptionVisibility = _vm.Sources.SelectedItem.RequiresComPort ? Visibility.Visible : Visibility.Hidden;
            _vm.ComPort2OptionVisibility = _vm.Sources.SelectedItem.RequiresComPort2 ? Visibility.Visible : Visibility.Hidden;
            _vm.XIAndGamepadOptionVisibility = _vm.Sources.SelectedItem.RequiresId ? Visibility.Visible : Visibility.Hidden;
            _vm.SSHOptionVisibility = _vm.Sources.SelectedItem.RequiresHostname ? Visibility.Visible : Visibility.Hidden;
            UpdateGamepadList();
            UpdateXIList();
            UpdatePortListThread();
            UpdateBeagleList();
            UpdateBeagleI2CList();
            _vm.Skins.UpdateContents(_skins.Where(x => x.Type == _vm.Sources.SelectedItem));
            _vm.Skins.SelectFirst();
            if (_vm.Sources.GetSelectedId() == Properties.Settings.Default.Source)
            {
                _vm.Skins.SelectId(Properties.Settings.Default.Skin);
            }
        }

        private void Skin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_vm.Skins.SelectedItem == null)
            {
                return;
            }

            _vm.Backgrounds.UpdateContents(_vm.Skins.SelectedItem.Backgrounds);
            _vm.Backgrounds.SelectFirst();
            if (_vm.Skins.GetSelectedId() == Properties.Settings.Default.Skin)
            {
                _vm.Backgrounds.SelectId(Properties.Settings.Default.Background);
            }
        }

        private void ComPortCombo_SelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdatePortList();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(string.Format(CultureInfo.CurrentCulture, " RetroSpy Version {0}\n\nThis Release Supported By\n\n     40wattRange\n     Coltaho\n     sk84uhlivin\n     watsonpunk", Assembly.GetEntryAssembly().GetName().Version), _resources.GetString("About", CultureInfo.CurrentUICulture),
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void FilterCOM_Checked(object sender, RoutedEventArgs e)
        {
            _vm.FilterCOMPorts = FilterCOM.IsChecked;
            Properties.Settings.Default.FilterCOMPorts = FilterCOM.IsChecked;
        }

        private void AddRemove_Click(object sender, RoutedEventArgs e)
        {
            new AddRemoveWindow(InputSource.ALL, _excludedSources).ShowDialog();

            PopulateSources();

            string hiddenConsoleList = "";
            foreach (string source in _excludedSources)
            {
                hiddenConsoleList += source + ";";
            }

            Properties.Settings.Default.HiddleConsoleList = hiddenConsoleList;
            Properties.Settings.Default.Save();
        }

        private void KeybindingBehavior_Checked(object sender, RoutedEventArgs e)
        {
            _vm.LegacyKeybindingBehavior = KeybindingBehavior.IsChecked;
            Properties.Settings.Default.LegacyKeybindingBehavior = KeybindingBehavior.IsChecked;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            isClosing = true;
        }
    }

    public class SetupWindowViewModel : INotifyPropertyChanged
    {
        public ListView<string> Ports { get; set; }
        public ListView<string> Ports2 { get; set; }
        public ListView<uint> XIAndGamepad { get; set; }
        public ListView<Skin> Skins { get; set; }
        public ListView<Background> Backgrounds { get; set; }
        public ListView<InputSource> Sources { get; set; }
        public int DelayInMilliseconds { get; set; }
        public bool StaticViewerWindowName { get; set; }
        public bool LegacyKeybindingBehavior { get; set; }
        public string Hostname { get; set; }
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

        private Visibility _comPort2OptionVisibility;

        public Visibility ComPort2OptionVisibility
        {
            get => _comPort2OptionVisibility;
            set
            {
                _comPort2OptionVisibility = value;
                NotifyPropertyChanged("ComPort2OptionVisibility");
            }
        }

        private Visibility _XIAndGamepadOptionVisibility;

        public Visibility XIAndGamepadOptionVisibility
        {
            get => _XIAndGamepadOptionVisibility;
            set
            {
                _XIAndGamepadOptionVisibility = value;
                NotifyPropertyChanged("XIAndGamepadOptionVisibility");
            }
        }

        private Visibility _SSHOptionVisibility;

        public Visibility SSHOptionVisibility
        {
            get => _SSHOptionVisibility;
            set
            {
                _SSHOptionVisibility = value;
                NotifyPropertyChanged("SSHOptionVisibility");
            }
        }

        public SetupWindowViewModel()
        {
            Ports = new ListView<string>();
            Ports2 = new ListView<string>();
            XIAndGamepad = new ListView<uint>();
            Skins = new ListView<Skin>();
            Sources = new ListView<InputSource>();
            Backgrounds = new ListView<Background>();
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