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
            var asciiCreator = new AsciArtCreator(@"C:\Users\Justin\Desktop\testIn.jpg");
            asciiCreator.Go(@"C:\Users\Justin\Desktop\testOut.txt");
            return;
            var i = new AllRbgImageCreator(ImageCreatorConfig.c256_128);

            var startTime = DateTime.UtcNow;
            i.SaveImage(@"C:\Users\Justin\Desktop\newImage.png");
            var endTime = DateTime.UtcNow;

            Console.WriteLine(endTime.Subtract(startTime).TotalSeconds);
            Console.ReadKey();
        }
    }
}
