using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Grindr
{
    public class ScreenRecorderHelper
    {
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        public Bitmap TakeScreenshot(BotInstance i)
        {
            Bitmap bmp = null;
            RECT srcRect;
            if (i.Initializer.Process != null && i.Initializer.WindowHandle != null)
            {
                if (GetWindowRect(i.Initializer.WindowHandle.Value, out srcRect))
                {
                    int width = srcRect.Right - srcRect.Left;
                    int height = srcRect.Bottom - srcRect.Top;

                    bmp = new Bitmap(width, height);

                    using (var screenG = Graphics.FromImage(bmp))
                    {
                        this.RecordScreen(
                            screenG,
                            srcRect.Top,
                            srcRect.Left,
                            bmp.Width,
                            bmp.Height
                        );
                    }
                }
            }

            return bmp;
        }

        public void RecordScreen(Graphics screenG, int top, int left, int width, int height)
        {
            screenG.CopyFromScreen(left, top,
                            0, 0, new Size(width, height),
                            CopyPixelOperation.SourceCopy);
        }
    }
}
