using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GBPemu
{
    /// <summary>
    /// Interaction logic for GameBoyPrinterEmulatorWindow.xaml
    /// </summary>
    public partial class GameBoyPrinterEmulatorWindow : Window, INotifyPropertyChanged
    {
        private readonly int TILE_PIXEL_WIDTH = 8;
        private readonly int TILE_PIXEL_HEIGHT = 8;
        private readonly int TILES_PER_LINE = 20; // Gameboy Printer Tile Constant

        private readonly byte[,,] palettes = {
                                                {{ 0xff, 0xaa, 0x55, 0x00 }, { 0xff, 0xaa, 0x55, 0x00 }, { 0xff, 0xaa, 0x55, 0x00 }},   // Grayscale
                                                {{ 0x9b, 0x77, 0x30, 0x0f }, { 0xbc, 0xa1, 0x62, 0x38 }, { 0x0f, 0x12, 0x30, 0x0f }},   // DMG
                                                {{ 0xc4, 0x8b, 0x4d, 0x1f }, { 0xcf, 0x95, 0x53, 0x1f }, { 0xa1, 0x6d, 0x3c, 0x1f }},   // GameBoy Pocket
                                                {{ 0xff, 0x7b, 0x01, 0x00 }, { 0xff, 0xff, 0x63, 0x00 }, { 0xff, 0x30, 0xc6, 0x00 }},   // GameBoy Color EU/US
                                                {{ 0xff, 0xff, 0x83, 0x00 }, { 0xff, 0xad, 0x31, 0x00 }, { 0xff, 0x63, 0x00, 0x00 }},   // GameBoy Color JP
                                                {{ 0xe0, 0x88, 0x34, 0x08 }, { 0xf8, 0xc0, 0x68, 0x18 }, { 0xd0, 0x70, 0x56, 0x20 }},   // BGB
                                                {{ 0xe0, 0xa8, 0x70, 0x2b }, { 0xdb, 0x9f, 0x6b, 0x2b }, { 0xcd, 0x94, 0x66, 0x26 }},   // GraphixKid Gray
                                                {{ 0x7e, 0xab, 0x7b, 0x4c }, { 0x84, 0xc3, 0x92, 0x62 }, { 0xb4, 0x96, 0x78, 0x5a }},   // GraphixKid Green
                                                {{ 0x7e, 0x57, 0x38, 0x2e }, { 0x84, 0x7b, 0x5d, 0x46 }, { 0x16, 0x46, 0x49, 0x3d }}    // Black Zero
        };

        private readonly byte[] colors_red = { 0xff, 0xaa, 0x55, 0x00 };
        private readonly byte[] colors_green = { 0xff, 0xaa, 0x55, 0x00 };
        private readonly byte[] colors_blue = { 0xff, 0xaa, 0x55, 0x00 };

        private readonly byte[] DMG_colors_red = { 0x9b, 0x8b, 0x30, 0x0f };
        private readonly byte[] DMG_colors_green = { 0xbc, 0xac, 0x62, 0x38 };
        private readonly byte[] DMG_colors_blue = { 0x0f, 0x0f, 0x30, 0x0f };

        private readonly System.Windows.Controls.Image _image;
        private BitmapPixelMaker _imageBuffer;
        private readonly IControllerReader _reader;

        public event PropertyChangedEventHandler PropertyChanged;

        private int SelectedPalette = 0;

        private void checkPalette(int paletteId)
        {
            Palette_Grayscale.IsChecked = paletteId == 0;
            Palette_DMG.IsChecked = paletteId == 1;
            Palette_GBPocket.IsChecked = paletteId == 2;
            Palette_GBCUS.IsChecked = paletteId == 3;
            Palette_GBCJP.IsChecked = paletteId == 4;
            Palette_BGB.IsChecked = paletteId == 5;
            Palette_GKGray.IsChecked = paletteId == 6;
            Palette_GKGreen.IsChecked = paletteId == 7;
            Palette_BZ.IsChecked = paletteId == 8;
        }

        private void Palette_Click(Object sender, EventArgs e)
        {
            int newPalette = 0;

            if (sender == Palette_DMG)
            {
                newPalette = 1;
            }
            else if (sender == Palette_GBPocket)
            {
                newPalette = 2;
            }
            else if (sender == Palette_GBCUS)
            {
                newPalette = 3;
            }
            else if (sender == Palette_GBCJP)
            {
                newPalette = 4;
            }
            else if (sender == Palette_BGB)
            {
                newPalette = 5;
            }
            else if (sender == Palette_GKGray)
            {
                newPalette = 6;
            }
            else if (sender == Palette_GKGreen)
            {
                newPalette = 7;
            }
            else if (sender == Palette_BZ)
            {
                newPalette = 8;
            }

            checkPalette(newPalette);

            for (int i = 0; i < 4; ++i)
            {
                _imageBuffer.ReplaceColor(palettes[SelectedPalette, 0, i], palettes[SelectedPalette, 1, i], palettes[SelectedPalette, 2, i],
                                          palettes[newPalette, 0, i], palettes[newPalette, 1, i], palettes[newPalette, 2, i]);
            }
            WriteableBitmap wbitmap = _imageBuffer.MakeBitmap(96, 96);
            _image.Source = wbitmap;

            SelectedPalette = newPalette;
            Properties.Settings.Default.SelectedPalette = SelectedPalette;
        }

        private void DisplayError()
        {
            SelectedPalette = Properties.Settings.Default.SelectedPalette;

            var bmp = new Bitmap(Properties.Resources.ErrorImage);

            _imageBuffer = new BitmapPixelMaker(480, 432);

            _imageBuffer.SetColor(palettes[SelectedPalette, 0, 3], palettes[SelectedPalette, 1, 3], palettes[SelectedPalette, 2, 3]);

            for (int i = 0; i < bmp.Width; ++i)
            {
                for (int j = 0; j < bmp.Height; ++j)
                {
                    var pixel = bmp.GetPixel(i, j);
                    if (pixel.R == 255 && pixel.G == 255 && pixel.B == 255)
                    {
                        _imageBuffer.SetRed(i, j, palettes[SelectedPalette, 0, 0]);
                        _imageBuffer.SetGreen(i, j, palettes[SelectedPalette, 1, 0]);
                        _imageBuffer.SetBlue(i, j, palettes[SelectedPalette, 2, 0]);
                    }
                }
            }


            //imageBuffer.SetColor(0, 0, 0);
            // Convert the pixel data into a WriteableBitmap.
            WriteableBitmap wbitmap = _imageBuffer.MakeBitmap(96, 96);

            // Set the Image source.
            _image.Source = wbitmap;

            GameBoyPrinterEmulatorWindowGrid.Height = 432;
            GameBoyPrinterEmulatorWindowGrid.Width = 480;
            this.Height = 432;
            this.Width = 480;
        }
    
        public GameBoyPrinterEmulatorWindow(IControllerReader reader)
        {
            InitializeComponent();
            DataContext = this;

            _reader = reader ?? throw new NullReferenceException();

            SelectedPalette = Properties.Settings.Default.SelectedPalette;

            var bmp = new Bitmap(Properties.Resources.PrintImage);

            _imageBuffer = new BitmapPixelMaker(480, 432);

            _imageBuffer.SetColor(palettes[SelectedPalette, 0, 3], palettes[SelectedPalette, 1, 3], palettes[SelectedPalette, 2, 3]);

            for (int i = 0; i < bmp.Width; ++i)
            {
                for(int j = 0; j < bmp.Height; ++j)
                {
                    var pixel = bmp.GetPixel(i, j);
                    if (pixel.R == 255 && pixel.G == 255 && pixel.B == 255)
                    {
                        _imageBuffer.SetRed(i, j, palettes[SelectedPalette, 0, 0]);
                        _imageBuffer.SetGreen(i, j, palettes[SelectedPalette, 1, 0]);
                        _imageBuffer.SetBlue(i, j, palettes[SelectedPalette, 2, 0]);
                    }    
                }
            }

            WriteableBitmap wbitmap = _imageBuffer.MakeBitmap(96, 96);

            // Create an Image to display the bitmap.
            _image = new System.Windows.Controls.Image
            {
                Stretch = Stretch.None,
                Margin = new Thickness(0)
            };

            GameBoyPrinterEmulatorWindowGrid.Children.Add(_image);
            _image.Source = wbitmap;

            _reader.ControllerStateChanged += Reader_ControllerStateChanged;
            _reader.ControllerDisconnected += Reader_ControllerDisconnected;

            checkPalette(SelectedPalette);

        }

        private void Reader_ControllerDisconnected(object sender, EventArgs e)
        {
            if (Dispatcher.CheckAccess())
            {
                Hide();
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    Hide();
                });
            }
        }

        private void Reader_ControllerStateChanged(object reader, ControllerStateEventArgs e)
        {
            try
            {
                _imageBuffer.SetColor(0, 0, 0, 255);

                int square_width = 480 / (TILE_PIXEL_WIDTH * TILES_PER_LINE);
                int square_height = square_width;

                var tiles_rawBytes_array = e.RawPrinterData.Split('\n');

                var total_tile_count = 0;

                for (var tile_i = 0; tile_i < tiles_rawBytes_array.Length; tile_i++)
                {
                    var tile_element = tiles_rawBytes_array[tile_i];

                    // Check for invalid raw lines
                    if (tile_element.Length == 0)
                    {   // Skip lines with no bytes (can happen with .split() )
                        continue;
                    }
                    else if (tile_element.StartsWith("!", StringComparison.Ordinal) == true)
                    {   // Skip lines used for comments
                        continue;
                    }
                    else if (tile_element.StartsWith("#", StringComparison.Ordinal) == true)
                    {   // Skip lines used for comments
                        continue;
                    }
                    else if (tile_element.StartsWith("//", StringComparison.Ordinal) == true)
                    {   // Skip lines used for comments
                        continue;
                    }
                    else if (tile_element.StartsWith("{", StringComparison.Ordinal) == true)
                    {   // Skip lines used for comments
                        continue;
                    }
                    total_tile_count++;
                }

                var tile_height_count = total_tile_count / TILES_PER_LINE;

                if (tile_height_count == 0)
                {
                    DisplayError();
                    return;
                }

                _imageBuffer = new BitmapPixelMaker(square_width * TILE_PIXEL_WIDTH * TILES_PER_LINE, square_height * TILE_PIXEL_HEIGHT * tile_height_count);

                _image.Height = square_height * TILE_PIXEL_HEIGHT * tile_height_count;
                _image.Width = square_width * TILE_PIXEL_WIDTH * TILES_PER_LINE;
                GameBoyPrinterEmulatorWindowGrid.Height = square_height * TILE_PIXEL_HEIGHT * tile_height_count; ;
                GameBoyPrinterEmulatorWindowGrid.Width = square_width * TILE_PIXEL_WIDTH * TILES_PER_LINE;
                this.Height = square_height * TILE_PIXEL_HEIGHT * tile_height_count;
                this.Width = square_width * TILE_PIXEL_WIDTH * TILES_PER_LINE;

                int tile_count = 0;

                for (var tile_i = 0; tile_i < tiles_rawBytes_array.Length; tile_i++)
                {
                    var tile_element = tiles_rawBytes_array[tile_i];

                    // Check for invalid raw lines
                    if (tile_element.Length == 0)
                    {   // Skip lines with no bytes (can happen with .split() )
                        continue;
                    }
                    else if (tile_element.StartsWith("!", StringComparison.Ordinal) == true)
                    {   // Skip lines used for comments
                        continue;
                    }
                    else if (tile_element.StartsWith("#", StringComparison.Ordinal) == true)
                    {   // Skip lines used for comments
                        continue;
                    }
                    else if (tile_element.StartsWith("//", StringComparison.Ordinal) == true)
                    {   // Skip lines used for comments
                        continue;
                    }
                    else if (tile_element.StartsWith("{", StringComparison.Ordinal) == true)
                    {   // Skip lines used for comments
                        continue;
                    }

                    // Gameboy Tile Offset
                    int tile_x_offset = tile_count % TILES_PER_LINE;
                    int tile_y_offset = tile_count / TILES_PER_LINE;

                    var pixels = Decode(tile_element);

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
                WriteableBitmap wbitmap = _imageBuffer.MakeBitmap(96, 96);

                // Set the Image source.
                _image.Source = wbitmap;
            }
            catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        void Paint(BitmapPixelMaker canvas, byte[] pixels, int pixel_width, int pixel_height, int tile_x_offset, int tile_y_offset)
        {   // This paints the tile with a specified offset and pixel width

            int pixel_x_offset = TILE_PIXEL_WIDTH * tile_x_offset * pixel_width;
            int pixel_y_offset = TILE_PIXEL_HEIGHT * tile_y_offset * pixel_height;


            for (var i = 0; i < TILE_PIXEL_WIDTH; i++)
            {   // pixels along the tile's x axis
                for (var j = 0; j < TILE_PIXEL_HEIGHT; j++)
                {   // pixels along the tile's y axis

                    canvas.SetRect(pixel_x_offset + i * pixel_width,
                            pixel_y_offset + j * pixel_height,
                            pixel_width,
                            pixel_height,
                            palettes[SelectedPalette, 0, pixels[j * TILE_PIXEL_WIDTH + i]],
                            palettes[SelectedPalette, 1, pixels[j * TILE_PIXEL_WIDTH + i]],
                            palettes[SelectedPalette, 2, pixels[j * TILE_PIXEL_WIDTH + i]]);
                }
            }
        }

        private byte[] Decode(string rawBytes)
        {
            var bytes = rawBytes.Replace(" ", "").Replace("\r", "");
            if (bytes.Length != 32) return null;

            var byteArray = new byte[16];
            for (var i = 0; i < byteArray.Length; i++)
            {
                byteArray[i] = byte.Parse(bytes.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber, CultureInfo.CurrentCulture);
            }

            var pixels = new byte[TILE_PIXEL_WIDTH * TILE_PIXEL_HEIGHT];
            for (var j = 0; j < TILE_PIXEL_HEIGHT; j++)
            {
                for (var i = 0; i < TILE_PIXEL_WIDTH; i++)
                {
                    byte hiBit = (byte)((byteArray[j * 2 + 1] >> (7 - i)) & 1);
                    byte loBit = (byte)((byteArray[j * 2] >> (7 - i)) & 1);
                    pixels[j * TILE_PIXEL_WIDTH + i] = (byte)((hiBit << 1) | loBit);
                }
            }

            return pixels;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
            _reader.Finish();
            System.Environment.Exit(0);  
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                AddExtension = true,
                Filter = Properties.Resources.ResourceManager.GetString("PNGFilter", CultureInfo.CurrentUICulture)
            };
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var encoder = new PngBitmapEncoder();
                SaveUsingEncoder(_image, saveFileDialog.FileName, encoder);
            }
            saveFileDialog.Dispose();
        }

        private void SaveUsingEncoder(FrameworkElement visual, string fileName, BitmapEncoder encoder)
        {
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)visual.ActualWidth, (int)visual.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            BitmapFrame frame = BitmapFrame.Create(bitmap);
            encoder.Frames.Add(frame);

            using (var stream = File.Create(fileName))
            {
                encoder.Save(stream);
            }
        }

    }
}
