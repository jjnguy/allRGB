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
    public class AllRbgImageCreator
    {
        private ImageCreatorConfig conf;
        public AllRbgImageCreator(ImageCreatorConfig conf)
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
                    var nextColor = GetAndRemoveCloseColor(PickNearColor(x2, y, pxls), random);
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
                    var nextColor = GetAndRemoveCloseColor(PickNearColor(x, y2, pxls), random);
                    pxls[x, y2] = nextColor;
                    x--;
                    y2++;
                }
            }

            // using the calculated pixels, fill in an image.
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

        private Color PickNearColor(int x, int y, Color[,] pxls)
        {
            var up = y == 0 ? conf.BorderColor : pxls[x, y - 1];
            var left = x == 0 ? conf.BorderColor : pxls[x - 1, y];
            return Avg(up, left);
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

        private static Color GetAndRemoveCloseColor(Color ToCompare, List<Color> TheList)
        {
            if (TheList.Count == 0) throw new Exception("Something went wrong");
            var threshold = 10;
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
        public static readonly ImageCreatorConfig c1024_2048 = new ImageCreatorConfig(1024, 2048, 2);
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
            return allColors;
        }
    }

    public static class Hlprs
    {
        // http://codegolf.stackexchange.com/a/22326/33
        // Max diff is 195075 between black and white
        public static int Diff(this Color c1, Color c2)
        {
            var r = c1.R - c2.R;
            var g = c1.G - c2.G;
            var b = c1.B - c2.B;
            return r * r + g * g + b * b;
        }
    }
}
