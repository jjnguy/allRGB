using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace AllColorsImage
{
    public class ImageCreator
    {
        private ImageCreatorConfig conf;
        public ImageCreator(ImageCreatorConfig conf)
        {
            this.conf = conf;
        }

        public void SaveImage(string location)
        {
            var pxls = new Color[conf.Width, conf.Height];

            var random = conf.GenerateAllColors().OrderBy(c => Guid.NewGuid()).ToList();

            for (var x = 0; x < conf.Width; x++)
            {
                var x2 = x;
                var y = 0;
                while (x2 >= 0 && y < conf.Height)
                {
                    var nextColor = GetAndRemoveCloseColor(GetColorAbove(x2, y, pxls), random);
                    pxls[x2, y] = nextColor;
                    x2--;
                    y++;
                }
            }
            for (var y = 1; y < conf.Height; y++)
            {
                var y2 = y;
                var x = conf.Width - 1;
                while (x > conf.Width - conf.Height && y2 < conf.Height)
                {
                    var nextColor = GetAndRemoveCloseColor(GetColorAbove(x, y2, pxls), random);
                    pxls[x, y2] = nextColor;
                    x--;
                    y2++;
                }
            }
            using (var image = new Bitmap(conf.Width, conf.Height, PixelFormat.Format24bppRgb))
            {
                for (var x = 0; x < conf.Width; x++)
                {
                    for (var y = 0; y < conf.Height; y++)
                    {
                        image.SetPixel(x, y, pxls[x, y]);
                    }
                }

                image.Save(location);
            }
        }

        private Color GetColorAbove1(int x, int y, Color[,] pxls)
        {
            var up = y == 0 ? conf.BorderColor : pxls[x, y - 1];
            var twoUp = y < 2 ? conf.BorderColor : pxls[x, y - 2];
            var left = x == 0 ? conf.BorderColor : pxls[x - 1, y];
            var twoLeft = x < 2 ? conf.BorderColor : pxls[x - 2, y];
            var upLeft = y == 0 || x == 0 ? conf.BorderColor : pxls[x - 1, y - 1];
            return Avg(up, twoUp, left, twoLeft, upLeft);
        }

        private static Random r = new Random();
        private Color GetColorAbove2(int x, int y, Color[,] pxls)
        {
            var up = y == 0 ? conf.BorderColor : pxls[x, y - 1];
            var left = x == 0 ? conf.BorderColor : pxls[x - 1, y];
            var random = x < 1 || y < 1 ? conf.BorderColor : pxls[r.Next(Math.Max(0, 0), x - 1), r.Next(Math.Max(0, y - 2 * y), y - 1)];
            return Avg(up, up, left, left, random);
        }

        private Color GetColorAbove(int x, int y, Color[,] pxls)
        {
            var up = y == 0 ? Color.Blue : pxls[x, y - 1];
            var left = x == 0 ? Color.Blue : pxls[x - 1, y];
            var upLeft = y == 0 || x == 0 ? Color.Blue : pxls[x - 1, y - 1];
            return Avg(up, left, upLeft.Invert());
        }

        private Color GetColorAbove3(int x, int y, Color[,] pxls)
        {
            var up = y == 0 ? conf.BorderColor : pxls[x, y - 1];
            var left = x == 0 ? conf.BorderColor : pxls[x - 1, y];
            var upLeft = y == 0 || x == 0 ? conf.BorderColor : pxls[x - 1, y - 1];
            return Avg(up, left.Invert(), left, left, left, upLeft, upLeft, upLeft, upLeft, upLeft, upLeft);
        }

        private static Color Avg(params Color[] clrs)
        {
            var rs = 0;
            var gs = 0;
            var bs = 0;
            foreach (var clr in clrs)
            {
                rs += clr.R;
                gs += clr.G;
                bs += clr.B;
            }
            return Color.FromArgb(rs/clrs.Length, gs/clrs.Length, bs/clrs.Length);
        }

        private static byte Avg(params int[] bytes)
        {
            return (byte) Math.Round(bytes.Sum() / (double) bytes.Length);
        }

        private static Color GetAndRemoveCloseColor(Color ToCompare, List<Color> TheList)
        {
            if (TheList.Count == 0) throw new Exception("Something went wrong");
            var threshold = 0;
            var minDiff = int.MaxValue;
            while (true)
            {
                for (var i = 0; i < TheList.Count; i++)
                {
                    var diff = ToCompare.Diff(TheList[i]);
                    if (diff <= threshold)
                    {
                        var result = TheList[i];
                        TheList.RemoveAt(i);
                        return result;
                    }
                    if (diff < minDiff)
                    {
                        minDiff = diff;
                    }
                }
                threshold = minDiff;
            }
        }
    }

    public class ImageCreatorConfig
    {
        public static readonly ImageCreatorConfig c256_128 = new ImageCreatorConfig(128, 256, 8);
        public static readonly ImageCreatorConfig c512_512 = new ImageCreatorConfig(512, 4);
        public static readonly ImageCreatorConfig c2048_2048 = new ImageCreatorConfig(1024, 2048, 2);
        public static readonly ImageCreatorConfig c4096_4096 = new ImageCreatorConfig(4096, 1);

        public readonly int Height;
        public readonly int Width;
        public readonly int ColorGap;
        public readonly Color BorderColor = Color.Black;

        private ImageCreatorConfig(int Height, int ColorGap) : this(Height, Height, ColorGap) { }

        private ImageCreatorConfig(int Height, int Width, int ColorGap)
        {
            this.ColorGap = ColorGap;
            this.Height = Height;
            this.Width = Width;
        }

        public List<Color> GenerateAllColors()
        {
            var allColors = new List<Color>();
            for (var r = 0; r < 256; r += ColorGap)
            {
                for (var g = 0; g < 256; g += ColorGap)
                {
                    for (var b = 0; b < 256; b += ColorGap)
                    {
                        allColors.Add(Color.FromArgb(r, b, g));
                    }
                }
            }
            if (allColors.Count != Height * Width)
            {
                throw new Exception("Bad config");
            }
            return allColors;
        }
    }

    public static class Hlprs
    {
        public static int Diff(this Color c1, Color c2)
        {
            var r = c1.R - c2.R;
            var g = c1.G - c2.G;
            var b = c1.B - c2.B;
            return r * r + g * g + b * b;
        }

        public static Color Invert(this Color c)
        {
            return Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B);
        }
    }
}
