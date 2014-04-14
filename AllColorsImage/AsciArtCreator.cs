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
        private string imageLocation;

        public AsciArtCreator(string ImageLocation)
        {
            this.imageLocation = ImageLocation;
        }

        private string pixels = " .-+*wGHM#&%";

        public void Go(string saveLocation)
        {
            var img = new Bitmap(imageLocation);
            using (var wrtr = new StreamWriter(saveLocation))
            {
                for (var y = 0; y < img.Height; y++)
                {
                    for (var x = 0; x < img.Width; x++)
                    {
                        var color = img.GetPixel(x, y);
                        var brightness = Brightness(color);
                        var idx = brightness / 255 * (pixels.Length - 1);
                        var pxl = pixels[pixels.Length - (int)Math.Round(idx) - 1];
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
