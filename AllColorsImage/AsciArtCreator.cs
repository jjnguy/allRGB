using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;

namespace AllColorsImage
{
    public class AsciArtCreator
    {
        private string _imageLocation;

        public AsciArtCreator(string ImageLocation)
        {
            _imageLocation = ImageLocation;
            var font = new Font("Courier New", 9);
            var printableChars = new List<string>();
            var brightness = new List<double>();

            for (int i = char.MinValue; i <= 100; i++)
            {
                var c = Convert.ToChar(i);
                if (!char.IsControl(c))
                {
                    printableChars.Add(c.ToString());
                }
            }
            var bmp = new Bitmap(1, 1);
            var stringMeasure = Graphics.FromImage(bmp);
            foreach (var printableChar in printableChars)
            {
                var foo = stringMeasure.MeasureString(printableChar, font);
                var width = foo.ToSize().Width;
                var height = foo.ToSize().Height;
                var bitmap = new Bitmap(width, height);
                var g = Graphics.FromImage(bitmap);
                g.DrawString(printableChar, font, new SolidBrush(Color.Black), new RectangleF(0, 0, width, height));
                double charbrightness = 0;
                for (var x = 0; x < width; x++)
                {
                    for (var y = 0; y < height; y++)
                    {
                        charbrightness += Brightness(bitmap.GetPixel(x, y));
                    }
                }
                brightness.Add(Math.Round(charbrightness / (width * height)));
            }

            var orderedZip = printableChars.Zip(brightness, (x, y) => new { x, y })
                      .OrderBy(pair => pair.y)
                      .ToList();
            var sortedchars = orderedZip.Select(pair => pair.x).ToList();

            _pixels = sortedchars.Aggregate("", (current, sortedchar) => current + sortedchar);
        }

        private string _pixels = " .-+*wGHM#&%";

        public void Go(string saveLocation)
        {
            var img = new Bitmap(_imageLocation);
            using (var wrtr = new StreamWriter(saveLocation))
            {
                for (var y = 0; y < img.Height; y++)
                {
                    for (var x = 0; x < img.Width; x++)
                    {
                        var color = img.GetPixel(x, y);
                        var brightness = Brightness(color);
                        var idx = brightness / 255 * (_pixels.Length - 1);
                        var pxl = _pixels[_pixels.Length - (int)Math.Round(idx) - 1];
                        wrtr.Write(pxl);
                        wrtr.Write(pxl);
                    }
                    wrtr.WriteLine();
                }
            }
        }

        private static double Brightness(Color c)
        {
            return Math.Sqrt(
               c.R * c.R * .241 +
               c.G * c.G * .691 +
               c.B * c.B * .068);
        }
    }
}
