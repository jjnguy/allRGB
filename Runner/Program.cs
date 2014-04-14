using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllColorsImage;

namespace Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            var asciiCreator = new AsciArtCreator(@"C:\Users\Justin\Desktop\example_images\reddit.bmp");
            asciiCreator.Go(@"C:\Users\Justin\Desktop\testOut.txt");
        }
    }
}
