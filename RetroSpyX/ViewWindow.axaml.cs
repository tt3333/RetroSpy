using Avalonia.Controls;
using System.Collections.Generic;
using System;
using RetroSpy.Readers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia.Media;
using Avalonia;
using Avalonia.Threading;
using MessageBox.Avalonia.Enums;
using System.Threading.Tasks;
using System.Threading;
using RetroSpy.Properties;
using System.Globalization;
using Avalonia.Interactivity;

namespace RetroSpy
{
    public partial class ViewWindow : Avalonia.Controls.Window
    {
        private readonly Skin _skin;
        private readonly IControllerReader _reader;
        private readonly Keybindings? _keybindings;
        private readonly BlinkReductionFilter _blinkFilter = new();
        private readonly List<Tuple<Detail, Image>> _detailsWithImages = new();
        private readonly List<Tuple<Button, Image>> _buttonsWithImages = new();
        private readonly List<Tuple<TouchPad, Image>> _touchPadWithImages = new();
        private readonly List<Tuple<RangeButton, Image>> _rangeButtonsWithImages = new ();
        private readonly List<Tuple<AnalogStick, Image>> _sticksWithImages = new();
        private readonly List<Tuple<AnalogText, Label>> _analogTextBoxes = new();

        private readonly Dictionary<string, List<Tuple<Button, Image>>> _dictOfButtonsWithImages = new();

        // The triggers images are embedded inside of a Grid element so that we can properly mask leftwards and upwards
        // without the image aligning to the top left of its element.
        private readonly List<Tuple<AnalogTrigger, Grid>> _triggersWithGridImages = new();

        public ViewWindow()
        {
            throw new NotImplementedException();
        }

        private readonly SetupWindow _sw;

        public ViewWindow(SetupWindow sw, Skin? skin, Background? skinBackground, IControllerReader? reader, bool staticViewerWindowName)
        {
            if (skin == null)
                throw new ArgumentNullException(nameof(skin));
            if (skinBackground == null)
                throw new ArgumentNullException(nameof(skinBackground));
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));
            
            _sw = sw;

            InitializeComponent();
            DataContext = this;

            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }
            else if (skin == null)
            {
                throw new ArgumentNullException(nameof(skin));
            }
            else if (skinBackground == null)
            {
                throw new ArgumentNullException(nameof(skinBackground));
            }

            _skin = skin;
            _reader = reader;

            Title = staticViewerWindowName ? "RetroSpy Viewer" : skin.Name;

            SolidColorBrush brush = new(skinBackground.Color);
            ControllerGrid.Background = brush;

            if (skinBackground.Image != null)
            {
                Image img = new()
                {
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left,
                    Source = skinBackground.Image,
                    Stretch = Stretch.Fill,
                    Margin = new Avalonia.Thickness(0, 0, 0, 0),
                    Width = skinBackground.Image.Size.Width,
                    Height = skinBackground.Image.Size.Height
                };

                ControllerGrid.Children.Add(img);
            }

            foreach (Detail detail in _skin.Details)
            {
                if (BgIsActive(skinBackground.Name, detail.Config?.TargetBackgrounds, detail.Config?.IgnoreBackgrounds))
                {
                    if (detail.Config != null)
                    {
                        detail.Config.X = detail.Config.OriginalX;
                        detail.Config.Y = detail.Config.OriginalY;
                        Image image = GetImageForElement(detail.Config);
                        _detailsWithImages.Add(new Tuple<Detail, Image>(detail, image));
                        ControllerGrid.Children.Add(image);
                    }
                }
            }

            foreach (AnalogTrigger trigger in _skin.AnalogTriggers)
            {
                if (BgIsActive(skinBackground.Name, trigger.Config?.TargetBackgrounds, trigger.Config?.IgnoreBackgrounds))
                {
                    if (trigger.Config != null)
                    {

                        trigger.Config.X = trigger.Config.OriginalX;
                        trigger.Config.Y = trigger.Config.OriginalY;
                        Grid grid = GetGridForAnalogTrigger(trigger);
                        _triggersWithGridImages.Add(new Tuple<AnalogTrigger, Grid>(trigger, grid));
                        ControllerGrid.Children.Add(grid);
                    }
                }
            }

            foreach (Button button in _skin.Buttons)
            {
                if (BgIsActive(skinBackground.Name, button.Config?.TargetBackgrounds, button.Config?.IgnoreBackgrounds))
                {
                    if (button.Config != null)
                    {
                        button.Config.X = button.Config.OriginalX;
                        button.Config.Y = button.Config.OriginalY;
                        Image image = GetImageForElement(button.Config);
                        Tuple<Button, Image> tuple = new(button, image);
                        _buttonsWithImages.Add(tuple);
                        if (_dictOfButtonsWithImages.ContainsKey(button.Name ?? string.Empty))
                        {
                            _dictOfButtonsWithImages[button.Name ?? string.Empty].Add(tuple);
                        }
                        else
                        {
                            List<Tuple<Button, Image>> list = new()
                        {
                            tuple
                        };
                            _dictOfButtonsWithImages.Add(button.Name ?? string.Empty, list);
                        }
                        image.IsVisible = false;
                        ControllerGrid.Children.Add(image);
                    }
                }
            }

            foreach (RangeButton button in _skin.RangeButtons)
            {
                if (BgIsActive(skinBackground.Name, button?.Config?.TargetBackgrounds, button?.Config?.IgnoreBackgrounds))
                {
                    if (button?.Config != null)
                    {
                        button.Config.X = button.Config.OriginalX;
                        button.Config.Y = button.Config.OriginalY;
                        Image image = GetImageForElement(button.Config);
                        _rangeButtonsWithImages.Add(new Tuple<RangeButton, Image>(button, image));
                        image.IsVisible = false;
                        ControllerGrid.Children.Add(image);
                    }
                }
            }

            foreach (AnalogStick stick in _skin.AnalogSticks)
            {
                if (BgIsActive(skinBackground.Name, stick.Config?.TargetBackgrounds, stick.Config?.IgnoreBackgrounds))
                {
                    if (stick.Config != null)
                    {
                        stick.Config.X = stick.Config.OriginalX;
                        stick.Config.Y = stick.Config.OriginalY;
                        stick.XRange = stick.OriginalXRange;
                        stick.YRange = stick.OriginalYRange;
                        Image image = GetImageForElement(stick.Config);
                        _sticksWithImages.Add(new Tuple<AnalogStick, Image>(stick, image));
                        if (stick?.VisibilityName?.Length > 0)
                        {
                            image.IsVisible = false;
                        }

                        ControllerGrid.Children.Add(image);
                    }
                }
            }

            foreach (TouchPad touchpad in _skin.TouchPads)
            {
                if (BgIsActive(skinBackground.Name, touchpad.Config?.TargetBackgrounds, touchpad.Config?.IgnoreBackgrounds))
                {
                    if (touchpad.Config != null)
                    {
                        touchpad.Config.X = touchpad.Config.OriginalX;
                        touchpad.Config.Y = touchpad.Config.OriginalY;
                        touchpad.XRange = touchpad.OriginalXRange;
                        touchpad.YRange = touchpad.OriginalYRange;
                        Image image = GetImageForElement(touchpad.Config);
                        _touchPadWithImages.Add(new Tuple<TouchPad, Image>(touchpad, image));
                        image.IsVisible = false;
                        ControllerGrid.Children.Add(image);
                    }
                }
            }

            foreach (AnalogText analogtext in _skin.AnalogTexts)
            {
                if (BgIsActive(skinBackground.Name, analogtext.TargetBackgrounds, analogtext.IgnoreBackgrounds))
                {
                    Label label = GetLabelForElement(analogtext);
                    _analogTextBoxes.Add(new Tuple<AnalogText, Label>(analogtext, label));
                    ControllerGrid.Children.Add(label);
                }
            }

            _reader.ControllerStateChanged += Reader_ControllerStateChanged;
            _reader.ControllerDisconnected += Reader_ControllerDisconnected;

            try
            {
                _keybindings = new Keybindings(Keybindings.XmlFilePath, _reader);
            }
            catch (ConfigParseException)
            {
                AvaloniaMessageBox("RetroSpy", "Error parsing keybindings.xml. Not binding any keys to gamepad inputs", ButtonEnum.Ok, MessageBox.Avalonia.Enums.Icon.Error);
            }

            MassBlinkReductionEnabled = Properties.Settings.Default.MassFilter;
            AnalogBlinkReductionEnabled = Properties.Settings.Default.AnalogFilter;
            ButtonBlinkReductionEnabled = Properties.Settings.Default.ButtonFilter;
            Topmost = Properties.Settings.Default.TopMost;
        }

        private static void AvaloniaMessageBox(string? title, string message, ButtonEnum buttonType, MessageBox.Avalonia.Enums.Icon iconType)
        {
            using var source = new CancellationTokenSource();
            _ = MessageBox.Avalonia.MessageBoxManager
            .GetMessageBoxStandardWindow(title ?? "Unknown Title Argument", message, buttonType, iconType)
                        .Show().ContinueWith(t => source.Cancel(), TaskScheduler.FromCurrentSynchronizationContext());
            Dispatcher.UIThread.MainLoop(source.Token);
        }

        private void Reader_ControllerDisconnected(object? sender, EventArgs e)
        {
            if (Dispatcher.UIThread.CheckAccess())
            {
                Close();
            }
            else
            {
                Dispatcher.UIThread.Post(Close);
            }
        }

        private static Label GetLabelForElement(AnalogText config)
        {
            Label label = new()
            {
                FontFamily = config.Font ?? new("Segoe UI"),
                FontSize = config.Size,
                Margin = new Thickness(config.X, config.Y, 0, 0),
                Foreground = config.Color,
            };
            return label;
        }

        private void Reader_ControllerStateChanged(object? reader, ControllerStateEventArgs e)
        {
            e = _blinkFilter.Process(e);

            // This assumes you can't press left/right and up/down at the same time.  The code gets more complicated otherwise.
            Dictionary<string, bool> compassDirectionStates = new();
            if (e.Buttons.ContainsKey("up") && e.Buttons.ContainsKey("left") && e.Buttons.ContainsKey("right") && e.Buttons.ContainsKey("down"))
            {
                string[] compassDirections = { "north", "northeast", "east", "southeast", "south", "southwest", "west", "northwest" };

                bool[] compassDirectionStatesTemp = new bool[8];
                compassDirectionStatesTemp[0] = e.Buttons["up"];
                compassDirectionStatesTemp[2] = e.Buttons["right"];
                compassDirectionStatesTemp[4] = e.Buttons["down"];
                compassDirectionStatesTemp[6] = e.Buttons["left"];

                if (compassDirectionStatesTemp[0] && compassDirectionStatesTemp[2])
                {
                    compassDirectionStatesTemp[1] = true;
                    compassDirectionStatesTemp[0] = compassDirectionStatesTemp[2] = false;
                }
                else if (compassDirectionStatesTemp[2] && compassDirectionStatesTemp[4])
                {
                    compassDirectionStatesTemp[3] = true;
                    compassDirectionStatesTemp[2] = compassDirectionStatesTemp[4] = false;
                }
                else if (compassDirectionStatesTemp[4] && compassDirectionStatesTemp[6])
                {
                    compassDirectionStatesTemp[5] = true;
                    compassDirectionStatesTemp[4] = compassDirectionStatesTemp[6] = false;
                }
                else if (compassDirectionStatesTemp[6] && compassDirectionStatesTemp[0])
                {
                    compassDirectionStatesTemp[7] = true;
                    compassDirectionStatesTemp[6] = compassDirectionStatesTemp[0] = false;
                }

                for (int i = 0; i < compassDirections.Length; ++i)
                {
                    compassDirectionStates[compassDirections[i]] = compassDirectionStatesTemp[i];
                }
            }

            foreach (Tuple<Button, Image> button in _buttonsWithImages)
            {
                if (e.Buttons.ContainsKey(button.Item1.Name ?? string.Empty))
                {
                    if (Dispatcher.UIThread.CheckAccess())
                    {
                        button.Item2.IsVisible = e.Buttons[button.Item1.Name ?? string.Empty];
                    }
                    else
                    {
                        Dispatcher.UIThread.Post(() =>
                        {
                            button.Item2.IsVisible = e.Buttons[button.Item1.Name ?? string.Empty];
                        });
                    }
                }
                else if (compassDirectionStates.ContainsKey(button.Item1.Name ?? string.Empty))
                {
                    if (Dispatcher.UIThread.CheckAccess())
                    {
                        button.Item2.IsVisible = compassDirectionStates[button.Item1.Name ?? string.Empty];
                    }
                    else
                    {
                        Dispatcher.UIThread.Post(() =>
                        {
                            button.Item2.IsVisible = compassDirectionStates[button.Item1.Name ?? string.Empty];
                        });
                    }
                }
            }

            foreach (Tuple<AnalogText, Label> text in _analogTextBoxes)
            {
                float value = 0;
                if (e.Analogs.ContainsKey(text.Item1.Name ?? string.Empty))
                {
                    value = e.Analogs[text.Item1.Name ?? string.Empty] * text.Item1.Range;
                }
                else if (e.RawAnalogs.ContainsKey(text.Item1.Name ?? string.Empty))
                {
                    value = e.RawAnalogs[text.Item1.Name ?? string.Empty] * text.Item1.Range;
                }

                text.Item2.Content = (int)value;
            }

            foreach (Tuple<RangeButton, Image> button in _rangeButtonsWithImages)
            {
                if (!e.Analogs.ContainsKey(button.Item1.Name ?? string.Empty))
                {
                    continue;
                }

                float value = e.Analogs[button.Item1.Name ?? string.Empty];
                bool visible = button.Item1.From <= value && value <= button.Item1.To;
                button.Item2.IsVisible = visible;
            }

            foreach (Tuple<AnalogStick, Image> stick in _sticksWithImages)
            {
                AnalogStick skin = stick.Item1;
                Image image = stick.Item2;

                float xrange = (skin.XReverse ? -1 : 1) * skin.XRange;
                float yrange = (skin.YReverse ? 1 : -1) * skin.YRange;

                if (skin.Config != null)
                {
                    float x = e.Analogs.ContainsKey(skin.XName ?? string.Empty)
                      ? skin.Config.X + (xrange * e.Analogs[skin.XName ?? string.Empty])
                      : skin.Config.X;

                    if (e.Analogs.ContainsKey(skin.XName ?? string.Empty) && Math.Abs(e.Analogs[skin.XName ?? string.Empty]) < skin.XPrecision)
                        x = skin.Config.X;

                    float y = e.Analogs.ContainsKey(skin.YName ?? string.Empty)
                          ? skin.Config.Y + (yrange * e.Analogs[skin.YName ?? string.Empty])
                          : skin.Config.Y;

                    if (e.Analogs.ContainsKey(skin.YName ?? string.Empty) && Math.Abs(e.Analogs[skin.YName ?? string.Empty]) < skin.YPrecision)
                        y = skin.Config.Y;

                    bool visibility = skin.VisibilityName?.Length == 0 || (e.Buttons.ContainsKey(skin.VisibilityName ?? string.Empty) && e.Buttons[skin.VisibilityName ?? string.Empty]);
                    if (Dispatcher.UIThread.CheckAccess())
                    {
                        image.Margin = new Thickness(x, y, 0, 0);
                        image.IsVisible = IsVisible;
                    }
                    else
                    {
                        Dispatcher.UIThread.Post(() =>
                        {
                            image.Margin = new Thickness(x, y, 0, 0);
                            image.IsVisible = IsVisible;
                        });
                    }
                }
            }

            foreach (Tuple<TouchPad, Image> touchpad in _touchPadWithImages)
            {
                TouchPad skin = touchpad.Item1;

                if (e.Analogs.ContainsKey(skin.XName ?? string.Empty) && e.Analogs.ContainsKey(skin.YName ?? string.Empty))
                {
                    if (skin.Config != null)
                    {
                        // Show
                        double x = (e.Analogs[skin.XName ?? string.Empty] * skin.XRange) + skin.Config.X - (touchpad.Item2.Width / 2);
                        double y = (e.Analogs[skin.YName ?? string.Empty] * skin.YRange) + skin.Config.Y - (touchpad.Item2.Height / 2);

                        if (Dispatcher.UIThread.CheckAccess())
                        {
                            touchpad.Item2.Margin = new Thickness(x, y, 0, 0);
                            touchpad.Item2.IsVisible = true;
                        }
                        else
                        {
                            Dispatcher.UIThread.Post(() =>
                            {
                                touchpad.Item2.Margin = new Thickness(x, y, 0, 0);
                                touchpad.Item2.IsVisible = true;
                            });
                        }
                    }
                }
                else
                {
                    if (Dispatcher.UIThread.CheckAccess())
                    {
                        touchpad.Item2.IsVisible = false;
                    }
                    else
                    {
                        Dispatcher.UIThread.Post(() =>
                        {
                            touchpad.Item2.IsVisible = false;
                        });
                    }
                }
            }

            foreach (Tuple<AnalogTrigger, Grid> trigger in _triggersWithGridImages)
            {
                AnalogTrigger skin = trigger.Item1;
                Grid grid = trigger.Item2;

                if (!e.Analogs.ContainsKey(skin.Name ?? string.Empty))
                {
                    continue;
                }

                float val = e.Analogs[skin.Name ?? string.Empty];
                if (skin.UseNegative)
                {
                    val *= -1;
                }

                if (skin.IsReversed)
                {
                    val = 1 - val;
                }

                if (val < 0)
                {
                    val = 0;
                }

                if (skin.Config != null)
                {
                    switch (skin.Direction)
                    {
                        case AnalogTrigger.DirectionValue.Right:
                            if (Dispatcher.UIThread.CheckAccess())
                            {
                                grid.Width = skin.Config.Width * val;
                            }
                            else
                            {
                                Dispatcher.UIThread.Post(() =>
                                {
                                    grid.Width = skin.Config.Width * val;
                                });
                            }
                            break;

                        case AnalogTrigger.DirectionValue.Left:
                            float width = skin.Config.Width * val;
                            float offx = skin.Config.Width - width;
                            if (Dispatcher.UIThread.CheckAccess())
                            {
                                grid.Margin = new Thickness(skin.Config.X + offx, skin.Config.Y, 0, 0);
                                grid.Width = width;
                            }
                            else
                            {
                                Dispatcher.UIThread.Post(() =>
                                {
                                    grid.Margin = new Thickness(skin.Config.X + offx, skin.Config.Y, 0, 0);
                                    grid.Width = width;
                                });
                            }
                            break;

                        case AnalogTrigger.DirectionValue.Down:
                            if (Dispatcher.UIThread.CheckAccess())
                            {
                                grid.Height = skin.Config.Height * val;
                            }
                            else
                            {
                                Dispatcher.UIThread.Post(() =>
                                {
                                    grid.Height = skin.Config.Height * val;
                                });
                            }
                            break;

                        case AnalogTrigger.DirectionValue.Up:
                            float height = skin.Config.Height * val;
                            float offy = skin.Config.Height - height;
                            if (Dispatcher.UIThread.CheckAccess())
                            {
                                grid.Margin = new Thickness(skin.Config.X, skin.Config.Y + offy, 0, 0);
                                grid.Height = height;
                            }
                            else
                            {
                                Dispatcher.UIThread.Post(() =>
                                {
                                    grid.Margin = new Thickness(skin.Config.X, skin.Config.Y + offy, 0, 0);
                                    grid.Height = height;
                                });
                            }
                            break;

                        case AnalogTrigger.DirectionValue.Fade:
                            if (Dispatcher.UIThread.CheckAccess())
                            {
                                grid.Height = skin.Config.Height;
                                grid.Width = skin.Config.Width;
                                grid.Opacity = val;
                            }
                            else
                            {
                                Dispatcher.UIThread.Post(() =>
                                {
                                    grid.Height = skin.Config.Height;
                                    grid.Width = skin.Config.Width;
                                    grid.Opacity = val;
                                });
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            foreach (KeyValuePair<string, float> analog in e.Analogs)
            {
                string[] buttonNames = { analog.Key + '-', analog.Key + '+' };
                bool[] buttonStates = { false, false };

                float posPrecision = _dictOfButtonsWithImages.ContainsKey(buttonNames[1]) ? _dictOfButtonsWithImages[buttonNames[1]].ToArray()[0].Item1.Precision : 0.0f;
                float negPrecision = -1.0f * (_dictOfButtonsWithImages.ContainsKey(buttonNames[0]) ? _dictOfButtonsWithImages[buttonNames[0]].ToArray()[0].Item1.Precision : 0.0f);

                if (analog.Value < negPrecision)
                {
                    buttonStates[0] = true;
                }
                else if (analog.Value > posPrecision)
                {
                    buttonStates[1] = true;
                }

                for (int i = 0; i < buttonNames.Length; ++i)
                {
                    if (_dictOfButtonsWithImages.ContainsKey(buttonNames[i]))
                    {
                        foreach (Tuple<Button, Image> button in _dictOfButtonsWithImages[buttonNames[i]])
                        {
                            if (Dispatcher.UIThread.CheckAccess())
                            {
                                button.Item2.IsVisible = buttonStates[i];
                            }
                            else
                            {
                                Dispatcher.UIThread.Post(() =>
                                {
                                    button.Item2.IsVisible = buttonStates[i];
                                });
                            }
                        }
                    }
                }
            }


        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
            _keybindings?.Finish();
            _reader.Finish();

        }

        bool _firstOpenHasHappened = false;

        private void Window_Open(object sender, EventArgs e)
        {
            // This is a terrible hack to get around 
            // not being able to have the parent hidden 
            // from a show dialog.
            if (!_firstOpenHasHappened)
            {
                _sw.Hide();
                Show();
                _firstOpenHasHappened = true;
            }
        }

        private static Grid GetGridForAnalogTrigger(AnalogTrigger trigger)
        {
            Image img = new()
            {
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top,

                HorizontalAlignment =
                  trigger.Direction == AnalogTrigger.DirectionValue.Left
                ? Avalonia.Layout.HorizontalAlignment.Right
                : Avalonia.Layout.HorizontalAlignment.Left
            };

            img.VerticalAlignment =
                  trigger.Direction == AnalogTrigger.DirectionValue.Up
                ? Avalonia.Layout.VerticalAlignment.Bottom
                : Avalonia.Layout.VerticalAlignment.Top;

            img.Source = trigger.Config?.Image;
            img.Stretch = Stretch.Fill;
            img.Margin = new Thickness(0, 0, 0, 0);
            img.Width = trigger.Config?.Width ?? 0;
            img.Height = trigger.Config?.Height ?? 0;

            Grid grid = new()
            {
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top,
                Margin = new Thickness(trigger.Config?.X ?? 0, trigger.Config?.Y ?? 0, 0, 0),
                Width = trigger.Config?.Width ?? 0,
                Height = trigger.Config?.Height ?? 0
            };

            grid.Children.Add(img);

            return grid;
        }
        private static Image GetImageForElement(ElementConfig config)
        {
            Image img = new()
            {
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left,
                Source = config.Image,
                Stretch = Stretch.Fill,
                Margin = new Thickness(config.X, config.Y, 0, 0),
                Width = config.Width,
                Height = config.Height
            };
            return img;
        }

        private static bool BgIsActive(string? bgName, Collection<string>? eligableBgs, Collection<string>? ignoreBgs)
        {
            return ignoreBgs?.Contains(bgName ?? string.Empty) == false && (eligableBgs?.Count == 0 || eligableBgs?.Contains(bgName ?? string.Empty) == true);
        }

        /// Expose the enabled status of the low-pass filter for data binding.
        public bool ButtonBlinkReductionEnabled
        {
            get => _blinkFilter.ButtonEnabled;
            set
            {
                _blinkFilter.ButtonEnabled = value;
                OnPropertyChanged(nameof(ButtonBlinkReductionEnabled));
            }
        }

        public bool MassBlinkReductionEnabled
        {
            get => _blinkFilter.MassEnabled;
            set
            {
                _blinkFilter.MassEnabled = value;
                OnPropertyChanged(nameof(MassBlinkReductionEnabled));
            }
        }

        public bool AnalogBlinkReductionEnabled
        {
            get => _blinkFilter.AnalogEnabled;
            set
            {
                _blinkFilter.AnalogEnabled = value;
                OnPropertyChanged(nameof(AnalogBlinkReductionEnabled));
            }
        }

        public bool AllBlinkReductionEnabled
        {
            get => ButtonBlinkReductionEnabled && AnalogBlinkReductionEnabled && MassBlinkReductionEnabled;
            set
            {
                ButtonBlinkReductionEnabled = AnalogBlinkReductionEnabled = MassBlinkReductionEnabled = value;
                OnPropertyChanged(nameof(AllBlinkReductionEnabled));
            }
        }

        private void AlwaysOnTop_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not CheckBox)
                Topmost = !Topmost;
            Properties.Settings.Default.TopMost = Topmost;
            OnTopCheckbox.IsChecked = Topmost;
        }

        public event PropertyChangedEventHandler? PropertyChangedEvent;
        private void OnPropertyChanged(string prop)
        {
            if (PropertyChangedEvent == null)
            {
                return;
            }

            PropertyChangedEvent(this, new PropertyChangedEventArgs(prop));
        }

        private void AllBlinkReductionEnabled_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not CheckBox)
                AllBlinkReductionEnabled = !ButtonBlinkReductionEnabled || !AnalogBlinkReductionEnabled || !MassBlinkReductionEnabled;
    
            AllBlinkCheckbox.IsChecked = AllBlinkReductionEnabled;
            Properties.Settings.Default.ButtonFilter = ButtonBlinkReductionEnabled;
            ButtonBlinkCheckbox.IsChecked = ButtonBlinkReductionEnabled;
            Properties.Settings.Default.AnalogFilter = AnalogBlinkReductionEnabled;
            AnalogBlinkCheckbox.IsChecked = AnalogBlinkReductionEnabled;
            Properties.Settings.Default.MassFilter = MassBlinkReductionEnabled;
            MassBlinkCheckbox.IsChecked = MassBlinkReductionEnabled;

        }

        private void ButtonBlinkReductionEnabled_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not CheckBox)
                ButtonBlinkReductionEnabled = !ButtonBlinkReductionEnabled;
            Properties.Settings.Default.ButtonFilter = ButtonBlinkReductionEnabled;
            ButtonBlinkCheckbox.IsChecked = ButtonBlinkReductionEnabled;
        }

        private void AnalogBlinkReductionEnabled_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not CheckBox)
                AnalogBlinkReductionEnabled = !AnalogBlinkReductionEnabled;
            Properties.Settings.Default.AnalogFilter = AnalogBlinkReductionEnabled;
            AnalogBlinkCheckbox.IsChecked = AnalogBlinkReductionEnabled;
        }

        private void MassBlinkReductionEnabled_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not CheckBox)
                MassBlinkReductionEnabled = !MassBlinkReductionEnabled;
            Properties.Settings.Default.MassFilter = MassBlinkReductionEnabled;
            MassBlinkCheckbox.IsChecked = MassBlinkReductionEnabled;
        }
    }
}
