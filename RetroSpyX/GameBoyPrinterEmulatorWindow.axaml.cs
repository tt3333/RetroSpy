using Avalonia.Controls;
using System;
using RetroSpy.Readers;
using System.ComponentModel;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia;
using Avalonia.Media;
using Avalonia.Threading;
using System.Globalization;
using System.Collections.Generic;
using SixLabors.ImageSharp;

namespace RetroSpy
{
    public partial class GameBoyPrinterEmulatorWindow : Window
    {
        private readonly int TILE_PIXEL_WIDTH = 8;
        private readonly int TILE_PIXEL_HEIGHT = 8;
        private readonly int TILES_PER_LINE = 20; // Gameboy Printer Tile Constant

        private readonly byte[] colors_red = { 0xff, 0xaa, 0x55, 0x00 };
        private readonly byte[] colors_green = { 0xff, 0xaa, 0x55, 0x00 };
        private readonly byte[] colors_blue = { 0xff, 0xaa, 0x55, 0x00 };

        private readonly byte[] DMG_colors_red = { 0x9b, 0x8b, 0x30, 0x0f };
        private readonly byte[] DMG_colors_green = { 0xbc, 0xac, 0x62, 0x38 };
        private readonly byte[] DMG_colors_blue = { 0x0f, 0x0f, 0x30, 0x0f };

        private readonly Avalonia.Controls.Image _image;
        private BitmapPixelMaker _imageBuffer;
        private readonly IControllerReader _reader;

        public event PropertyChangedEventHandler PropertyChanged;

        BitmapPixelMaker.GBImage _wbitmap;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _DMGPaletteEnabled;

        public bool DMGPaletteEnabled
        {
            get => _DMGPaletteEnabled;
            set
            {
                _DMGPaletteEnabled = value;
                NotifyPropertyChanged(nameof(DMGPaletteEnabled));
            }
        }

        private void DMGPaletteEnabled_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem)
                DMGCheckbox.IsChecked = !DMGPaletteEnabled;

            for (int i = 0; i < 4; ++i)
            {
                if (DMGPaletteEnabled)
                {
                    _imageBuffer.ReplaceColor(DMG_colors_red[i], DMG_colors_green[i], DMG_colors_blue[i],
                                                colors_red[i], colors_green[i], colors_blue[i]);
                }
                else
                {
                    _imageBuffer.ReplaceColor(colors_red[i], colors_green[i], colors_blue[i],
                                                DMG_colors_red[i], DMG_colors_green[i], DMG_colors_blue[i]);
                }
            }
            _wbitmap = _imageBuffer.MakeBitmap(96, 96);
            _image.Source = _wbitmap._bitmap;

            DMGPaletteEnabled = !DMGPaletteEnabled;
        }

        private SetupWindow _sw;

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

        public GameBoyPrinterEmulatorWindow()
        {
            throw new NotImplementedException();
        }

        public GameBoyPrinterEmulatorWindow(SetupWindow sw, IControllerReader reader)
        {
            _sw = sw;

            InitializeComponent();
            DataContext = this;

            _reader = reader ?? throw new ArgumentNullException(nameof(reader));

            DMGPaletteEnabled = Properties.Settings.Default.DMGPaletteEnabled;

            _imageBuffer = new BitmapPixelMaker(480, 432);

            if (DMGPaletteEnabled)
            {
                _imageBuffer.SetColor(DMG_colors_red[3], DMG_colors_green[3], DMG_colors_blue[3], 255);
            }
            else
            {
                _imageBuffer.SetColor(colors_red[3], colors_green[3], colors_blue[3], 255);
            }

            _wbitmap = _imageBuffer.MakeBitmap(96, 96);

            // Create an Image to display the bitmap.
            _image = new Avalonia.Controls.Image
            {
                Stretch = Stretch.None,
                Margin = new Thickness(0)
            };

            GameBoyPrinterEmulatorWindowGrid.Children.Add(_image);
            _image.Source = _wbitmap._bitmap;

            _reader.ControllerStateChanged += Reader_ControllerStateChanged;
            _reader.ControllerDisconnected += Reader_ControllerDisconnected;
        }

        private void Reader_ControllerDisconnected(object sender, EventArgs e)
        {
            if (Dispatcher.UIThread.CheckAccess())
            {
                Close();
            }
            else
            {
                Dispatcher.UIThread.Post(() =>
                {
                    Close();
                });
            }
        }

        private void Reader_ControllerStateChanged(object reader, ControllerStateEventArgs e)
        {
            _imageBuffer.SetColor(0, 0, 0, 255);

            int square_width = 480 / (TILE_PIXEL_WIDTH * TILES_PER_LINE);
            int square_height = square_width;

            string[] tiles_rawBytes_array = e.RawPrinterData.Split('\n');

            int total_tile_count = 0;

            for (int tile_i = 0; tile_i < tiles_rawBytes_array.Length; tile_i++)
            {
                string tile_element = tiles_rawBytes_array[tile_i];

                // Check for invalid raw lines
                if (tile_element.Length == 0)
                {   // Skip lines with no bytes (can happen with .split() )
                    continue;
                }
                else if (tile_element.StartsWith("!", StringComparison.Ordinal))
                {   // Skip lines used for comments
                    continue;
                }
                else if (tile_element.StartsWith("#", StringComparison.Ordinal))
                {   // Skip lines used for comments
                    continue;
                }
                else if (tile_element.StartsWith("//", StringComparison.Ordinal))
                {   // Skip lines used for comments
                    continue;
                }
                else if (tile_element.StartsWith("{", StringComparison.Ordinal))
                {   // Skip lines used for comments
                    continue;
                }
                total_tile_count++;
            }

            int tile_height_count = total_tile_count / TILES_PER_LINE;

            _imageBuffer = new BitmapPixelMaker(square_width * TILE_PIXEL_WIDTH * TILES_PER_LINE, square_height * TILE_PIXEL_HEIGHT * tile_height_count);

            _image.Height = square_height * TILE_PIXEL_HEIGHT * tile_height_count;
            _image.Width = square_width * TILE_PIXEL_WIDTH * TILES_PER_LINE;
            GameBoyPrinterEmulatorWindowGrid.Height = square_height * TILE_PIXEL_HEIGHT * tile_height_count; ;
            GameBoyPrinterEmulatorWindowGrid.Width = square_width * TILE_PIXEL_WIDTH * TILES_PER_LINE;
            Height = square_height * TILE_PIXEL_HEIGHT * tile_height_count;
            Width = square_width * TILE_PIXEL_WIDTH * TILES_PER_LINE;

            int tile_count = 0;

            for (int tile_i = 0; tile_i < tiles_rawBytes_array.Length; tile_i++)
            {
                string tile_element = tiles_rawBytes_array[tile_i];

                // Check for invalid raw lines
                if (tile_element.Length == 0)
                {   // Skip lines with no bytes (can happen with .split() )
                    continue;
                }
                else if (tile_element.StartsWith("!", StringComparison.Ordinal))
                {   // Skip lines used for comments
                    continue;
                }
                else if (tile_element.StartsWith("#", StringComparison.Ordinal))
                {   // Skip lines used for comments
                    continue;
                }
                else if (tile_element.StartsWith("//", StringComparison.Ordinal))
                {   // Skip lines used for comments
                    continue;
                }
                else if (tile_element.StartsWith("{", StringComparison.Ordinal))
                {   // Skip lines used for comments
                    continue;
                }

                // Gameboy Tile Offset
                int tile_x_offset = tile_count % TILES_PER_LINE;
                int tile_y_offset = tile_count / TILES_PER_LINE;

                byte[] pixels = Decode(tile_element);

                if (pixels != null)
                {
                    Paint(_imageBuffer, pixels, square_width, square_height, tile_x_offset, tile_y_offset);
                }
                else
                {
                    //status = false;
                }


                // Increment Tile Count Tracker
                tile_count++;

            }

            //imageBuffer.SetColor(0, 0, 0);
            // Convert the pixel data into a WriteableBitmap.
            _wbitmap = _imageBuffer.MakeBitmap(96, 96);

            // Set the Image source.
            _image.Source = _wbitmap._bitmap;
        }

        private void Paint(BitmapPixelMaker canvas, byte[] pixels, int pixel_width, int pixel_height, int tile_x_offset, int tile_y_offset)
        {   // This paints the tile with a specified offset and pixel width

            int pixel_x_offset = TILE_PIXEL_WIDTH * tile_x_offset * pixel_width;
            int pixel_y_offset = TILE_PIXEL_HEIGHT * tile_y_offset * pixel_height;


            for (int i = 0; i < TILE_PIXEL_WIDTH; i++)
            {   // pixels along the tile's x axis
                for (int j = 0; j < TILE_PIXEL_HEIGHT; j++)
                {   // pixels along the tile's y axis

                    canvas.SetRect(pixel_x_offset + (i * pixel_width),
                            pixel_y_offset + (j * pixel_height),
                            pixel_width,
                            pixel_height,
                            DMGPaletteEnabled ? DMG_colors_red[pixels[(j * TILE_PIXEL_WIDTH) + i]] : colors_red[pixels[(j * TILE_PIXEL_WIDTH) + i]],
                            DMGPaletteEnabled ? DMG_colors_green[pixels[(j * TILE_PIXEL_WIDTH) + i]] : colors_green[pixels[(j * TILE_PIXEL_WIDTH) + i]],
                            DMGPaletteEnabled ? DMG_colors_blue[pixels[(j * TILE_PIXEL_WIDTH) + i]] : colors_blue[pixels[(j * TILE_PIXEL_WIDTH) + i]]);
                }
            }
        }

        private byte[] Decode(string rawBytes)
        {
            string bytes = rawBytes.Replace(" ", "").Replace("\r", "");
            if (bytes.Length != 32)
            {
                return null;
            }

            byte[] byteArray = new byte[16];
            for (int i = 0; i < byteArray.Length; i++)
            {
                byteArray[i] = byte.Parse(bytes.Substring(i * 2, 2), NumberStyles.HexNumber, CultureInfo.CurrentCulture);
            }

            byte[] pixels = new byte[TILE_PIXEL_WIDTH * TILE_PIXEL_HEIGHT];
            for (int j = 0; j < TILE_PIXEL_HEIGHT; j++)
            {
                for (int i = 0; i < TILE_PIXEL_WIDTH; i++)
                {
                    byte hiBit = (byte)((byteArray[(j * 2) + 1] >> (7 - i)) & 1);
                    byte loBit = (byte)((byteArray[j * 2] >> (7 - i)) & 1);
                    pixels[(j * TILE_PIXEL_WIDTH) + i] = (byte)((hiBit << 1) | loBit);
                }
            }
            return pixels;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
            _reader.Finish();
        }

        private async void SaveAs_Click(object sender, RoutedEventArgs e)
        {

            SaveFileDialog SaveFileBox = new SaveFileDialog();
            SaveFileBox.Title = "Save Picture As...";
            List<FileDialogFilter> Filters = new List<FileDialogFilter>();
            FileDialogFilter filter = new FileDialogFilter();
            List<string> extension = new List<string>();
            extension.Add("png");
            filter.Extensions = extension;
            filter.Name = "PNG";
            Filters.Add(filter);
            SaveFileBox.Filters = Filters;

            SaveFileBox.DefaultExtension = "png";

            var SettingsFileName = await SaveFileBox.ShowAsync(this);

            if(SettingsFileName != null)
            {
                _wbitmap._rawImage.SaveAsPng(SettingsFileName);
            }

        }

    }
}
