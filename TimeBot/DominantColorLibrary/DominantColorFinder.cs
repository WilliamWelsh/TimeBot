using System;
using SixLabors.ImageSharp;
using System.Collections.Generic;
using SixLabors.ImageSharp.PixelFormats;
using TimeBot.DominantColorLibrary;

namespace TimeBot
{
    public static class DominantColorFinder
    {
        public static List<QuantizedColor> GetPalette(Image<Rgba32> sourceImage)
        {
            var colorMap = GetColorMap(GetPixelsFast(sourceImage), 5);
            return colorMap != null ? colorMap.GeneratePalette() : new List<QuantizedColor>();
        }

        private static CMap GetColorMap(byte[][] pixelArray, int colorCount)
        {
            if (colorCount > 0)
                --colorCount;
            return Mmcq.Quantize(pixelArray, colorCount);
        }

        private static byte[][] GetPixelsFast(Image<Rgba32> sourceImage) => ConvertPixels(GetIntFromPixel(sourceImage), sourceImage.Width * sourceImage.Height, 10);

        private static byte[] GetIntFromPixel(Image<Rgba32> bmp)
        {
            var numArray = new byte[bmp.Width * bmp.Height * 4];
            var index1 = 0;
            for (int x = 0; x < bmp.Width; ++x)
            {
                for (int y = 0; y < bmp.Height; ++y)
                {
                    var pixel = bmp[x, y];
                    numArray[index1] = pixel.B;
                    var index2 = index1 + 1;
                    numArray[index2] = pixel.G;
                    var index3 = index2 + 1;
                    numArray[index3] = pixel.R;
                    var index4 = index3 + 1;
                    numArray[index4] = pixel.A;
                    index1 = index4 + 1;
                }
            }
            return numArray;
        }

        private static byte[][] ConvertPixels(byte[] pixels, int pixelCount, int quality)
        {
            var num = pixelCount * 4;
            if (num != pixels.Length)
                throw new ArgumentException("(expectedDataLength = " + num + ") != (pixels.length = " + pixels.Length + ")");
            var length1 = (pixelCount + quality - 1) / quality;
            var length2 = 0;
            var numArray1 = new byte[length1][];
            for (int index1 = 0; index1 < pixelCount; index1 += quality)
            {
                var index2 = index1 * 4;
                var pixel1 = pixels[index2];
                var pixel2 = pixels[index2 + 1];
                var pixel3 = pixels[index2 + 2];
                if (pixels[index2 + 3] >= 125 && (pixel3 <= 250 || (pixel2 <= 250 || pixel1 <= 250)))
                {
                    numArray1[length2] = new[]
                    {
                        pixel3,
                        pixel2,
                        pixel1
                    };
                    ++length2;
                }
            }
            byte[][] numArray2 = new byte[length2][];
            Array.Copy(numArray1, numArray2, length2);
            return numArray2;
        }
    }
}