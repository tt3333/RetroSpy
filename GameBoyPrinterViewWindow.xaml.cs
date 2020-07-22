using howto_wpf_pixelmaker;
using RetroSpy.Readers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RetroSpy
{
    /// <summary>
    /// Interaction logic for GameBoyPrinterViewWindow.xaml
    /// </summary>
    public partial class GameBoyPrinterViewWindow : Window
    {
        int TILE_PIXEL_WIDTH = 8;
        int TILE_PIXEL_HEIGHT = 8;
        int TILES_PER_LINE = 20; // Gameboy Printer Tile Constant

        byte[] colors = { 0xff, 0xaa, 0x55, 0x00 };

        Image _image;
        BitmapPixelMaker _imageBuffer;
        private readonly IControllerReader _reader;
 
        public GameBoyPrinterViewWindow(IControllerReader reader)
        {
            InitializeComponent();
            
            _reader = reader;

            _imageBuffer = new BitmapPixelMaker(480, 432);
            _imageBuffer.SetColor(0, 0, 0, 255);

            Title = "RetroSpy GameBoy Printer Emulator";

            WriteableBitmap wbitmap = _imageBuffer.MakeBitmap(96, 96);

            // Create an Image to display the bitmap.
            _image = new Image();
            _image.Stretch = Stretch.None;
            _image.Margin = new Thickness(0);

            GameBoyPrinterViewWindowGrid.Children.Add(_image);
            _image.Source = wbitmap;

            _reader.ControllerStateChanged += Reader_ControllerStateChanged;
            _reader.ControllerDisconnected += Reader_ControllerDisconnected;
        }


        private void Reader_ControllerStateChanged(object reader, ControllerStateEventArgs e)
        {
            _imageBuffer.SetColor(0, 0, 0, 255);
            /* Determine size of each pixel in canvas */
            int square_width = 480 / (TILE_PIXEL_WIDTH * TILES_PER_LINE);
            int square_height = square_width;

            File.WriteAllText("out.txt", e.RawPrinterData);
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
                total_tile_count++;
            }

            // Increment Tile Count Tracker


            int tile_height_count = total_tile_count / 20;

            var canvas_width = square_width * TILE_PIXEL_WIDTH * TILES_PER_LINE;
            var canvas_height = square_height * TILE_PIXEL_HEIGHT * tile_height_count;

            int tile_count = 0, tile_x_offset = 0, tile_y_offset = 0;

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

                // Gameboy Tile Offset
                tile_x_offset = tile_count % TILES_PER_LINE;
                tile_y_offset = tile_count / TILES_PER_LINE;

                var pixels = decode(tile_element);

                if (pixels != null)
                {
                    paint(_imageBuffer, pixels, square_width, square_height, tile_x_offset, tile_y_offset);
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

        void paint(BitmapPixelMaker canvas, byte[] pixels, int pixel_width, int pixel_height, int tile_x_offset, int tile_y_offset)
        {   // This paints the tile with a specified offset and pixel width

            int tile_offset = tile_x_offset * tile_y_offset;
            int pixel_x_offset = TILE_PIXEL_WIDTH * tile_x_offset * pixel_width;
            int pixel_y_offset = TILE_PIXEL_HEIGHT * tile_y_offset * pixel_height;


            for (var i = 0; i < TILE_PIXEL_WIDTH; i++)
            {   // pixels along the tile's x axis
                for (var j = 0; j < TILE_PIXEL_HEIGHT; j++)
                {   // pixels along the tile's y axis

                    //canvas.SetColor(0, 0, 0, 0);
                    canvas.SetRect(pixel_x_offset + i * pixel_width,
                            pixel_y_offset + j * pixel_height,
                            pixel_width,
                            pixel_height, colors[pixels[j * TILE_PIXEL_WIDTH + i]]);

                }
            }
        }

        private byte[] decode(string rawBytes)
        {


            var bytes = rawBytes.Replace(" ", "").Replace("\r", "");
            if (bytes.Length != 32) return null;

            var byteArray = new byte[16];
            for (var i = 0; i < byteArray.Length; i++)
            {
                byteArray[i] = byte.Parse(bytes.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
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
    }
}
