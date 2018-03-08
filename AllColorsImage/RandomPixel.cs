using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AllColorsImage
{
    public class RandomPixel
    {
        [DllImport("User32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("User32.dll")]
        public static extern void ReleaseDC(IntPtr hwnd, IntPtr dc);

        public static Random _r = new Random();

        public static void Run()
        {
            IntPtr desktopPtr = GetDC(IntPtr.Zero);
            Graphics graphics1 = Graphics.FromHdc(desktopPtr);
            var workingArea = Screen.PrimaryScreen.WorkingArea;
            var brushes = Enumerable.Range(0, 256)
                .SelectMany(r => Enumerable.Range(0, 256)
                    .SelectMany(g => Enumerable.Range(0, 256)
                        .Select(b => new SolidBrush(Color.FromArgb(r, g, b)))
                    )
                ).ToArray();

            var task1 = Task.Run(() =>
            {
                while (true)
                {
                    graphics1.FillRectangle(brushes[_r.Next(brushes.Length)], new Rectangle(_r.Next((int)workingArea.Width), _r.Next((int)workingArea.Height), 1, 1));
                }
            });

            Task.WaitAll(task1);

            graphics1.Dispose();
            ReleaseDC(IntPtr.Zero, desktopPtr);
        }
    }
}
