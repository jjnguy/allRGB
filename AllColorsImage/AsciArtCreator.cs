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
        private string location;

        public AsciArtCreator(string ImageLocation)
        {
            this.location = ImageLocation;
        }

        private string[] pixels = new[]{
            " ",
            " ",
            ".,-",
            "_ivc=!/|\\~",
            "gjez2]/(YL)t[+T7Vf",
            "mdK4ZGbNDXY5P*Q",
            "W8KMA",
            "#%$"
        };
        Random r = new Random();
        public void Go(string saveLocation)
        {
            var image = new Bitmap(location);
            using (var fout = new StreamWriter(saveLocation))
            {
                var height = image.Height;
                var width = image.Width;

                for (var y = 0; y < height; y++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        var c = image.GetPixel(x, y);
                        var brightness = Math.Sqrt(c.R * c.R * .241 + c.G * c.G * .691 + c.B * c.B * .068) / 255 * (pixels.Length - 1);
                        var pxlGrp = pixels[pixels.Length - (int)Math.Round(brightness) - 1];
                        var pxl = pxlGrp[r.Next(0,pxlGrp.Length)];
                        fout.Write(pxl + "" + pxl);
                    }
                    fout.WriteLine();
                }
            }
        }
    }
}
