using System;

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GBPemu
{
    public struct Pixel : IEquatable<Pixel>
    {
        public Pixel(byte red, byte green, byte blue, byte alpha)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }
        public byte Alpha { get; set; }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Pixel p = (Pixel)obj;
                return (Red == p.Red) && (Green == p.Green) && (Blue == p.Blue) && (Alpha == p.Alpha);
            }
        }

        public override int GetHashCode()
        {
            return (Red.GetHashCode() * 17) + (Green.GetHashCode() * 17) + (Blue.GetHashCode() * 17) + (Alpha.GetHashCode() * 17);
        }

        public static bool operator ==(Pixel left, Pixel right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Pixel left, Pixel right)
        {
            return !(left == right);
        }

        public bool Equals(Pixel other)
        {
            return (Red == other.Red) && (Green == other.Green) && (Blue == other.Blue) && (Alpha == other.Alpha);
        }
    }

    public class BitmapPixelMaker
    {
        // The bitmap's size.
        private readonly int Width;
        private readonly int Height;

        // The pixel array.
        private readonly byte[] Pixels;

        // The number of bytes per row.
        private readonly int Stride;

        // Constructor. Width and height required.
        public BitmapPixelMaker(int width, int height)
        {
            // Save the width and height.
            Width = width;
            Height = height;

            // Create the pixel array.
            Pixels = new byte[width * height * 4];

            // Calculate the stride.
            Stride = width * 4;
        }

        // Get a pixel's value.
        public Pixel GetPixel(int x, int y)
        {
            Pixel retVal = new Pixel();

            int index = (y * Stride) + (x * 4);
            retVal.Blue = Pixels[index++];
            retVal.Green = Pixels[index++];
            retVal.Red = Pixels[index++];
            retVal.Alpha = Pixels[index];

            return retVal;
        }

        public byte GetBlue(int x, int y)
        {
            return Pixels[(y * Stride) + (x * 4)];
        }
        public byte GetGreen(int x, int y)
        {
            return Pixels[(y * Stride) + (x * 4) + 1];
        }
        public byte GetRed(int x, int y)
        {
            return Pixels[(y * Stride) + (x * 4) + 2];
        }
        public byte GetAlpha(int x, int y)
        {
            return Pixels[(y * Stride) + (x * 4) + 3];
        }

        // Set a pixel's value.
        public void SetPixel(int x, int y, Pixel color)
        {
            int index = (y * Stride) + (x * 4);
            Pixels[index++] = color.Blue;
            Pixels[index++] = color.Green;
            Pixels[index++] = color.Red;
            Pixels[index++] = color.Alpha;
        }
        public void SetBlue(int x, int y, byte blue)
        {
            Pixels[(y * Stride) + (x * 4)] = blue;
        }
        public void SetGreen(int x, int y, byte green)
        {
            Pixels[(y * Stride) + (x * 4) + 1] = green;
        }
        public void SetRed(int x, int y, byte red)
        {
            Pixels[(y * Stride) + (x * 4) + 2] = red;
        }
        public void SetAlpha(int x, int y, byte alpha)
        {
            Pixels[(y * Stride) + (x * 4) + 3] = alpha;
        }

        public void SetRect(int x1, int y1, int width, int height, byte red, byte green, byte blue)
        {
            for (int i = x1; i < width + x1; ++i)
            {
                for (int j = y1; j < height + y1; ++j)
                {
                    SetAlpha(i, j, 255);
                    SetRed(i, j, red);
                    SetGreen(i, j, green);
                    SetBlue(i, j, blue);

                }
            }
        }

        public void ReplaceColor(Pixel oldColor, Pixel newColor)
        {
            for (int i = 0; i < Width; ++i)
            {
                for (int j = 0; j < Height; ++j)
                {
                    Pixel pixel = GetPixel(i, j);
                    if (pixel == oldColor)
                    {
                        SetPixel(i, j, newColor);
                    }
                }
            }
        }

        // Set all pixels to a specific color.
        public void SetColor(byte red, byte green, byte blue, byte alpha)
        {
            int num_bytes = Width * Height * 4;
            int index = 0;
            while (index < num_bytes)
            {
                Pixels[index++] = blue;
                Pixels[index++] = green;
                Pixels[index++] = red;
                Pixels[index++] = alpha;
            }
        }

        // Set all pixels to a specific opaque color.
        public void SetColor(byte red, byte green, byte blue)
        {
            SetColor(red, green, blue, 255);
        }

        // Use the pixel data to create a WriteableBitmap.
        public WriteableBitmap MakeBitmap(double dpiX, double dpiY)
        {
            // Create the WriteableBitmap.
            WriteableBitmap wbitmap = new WriteableBitmap(
                Width, Height, dpiX, dpiY,
                PixelFormats.Bgra32, null);

            // Load the pixel data.
            Int32Rect rect = new Int32Rect(0, 0, Width, Height);
            wbitmap.WritePixels(rect, Pixels, Stride, 0);

            // Return the bitmap.
            return wbitmap;
        }
    }
}
