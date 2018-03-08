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
            RandomPixel.Run();
            //var allrgb = new AllRbgImageCreator(ImageCreatorConfig.c256_128);
            //allrgb.SaveImage(@"C:\Users\Justin\Desktop\newMethod.png");
        }
    }
}
