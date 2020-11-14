using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Grindr
{
    public class InputController
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hWnd, ref Rect Rect);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private BotInstance i { get; set; }

        const uint WM_KEYDOWN = 0x100;
        const uint WM_KEYUP = 0x0101;
        const int WM_LBUTTONDOWN = 0x0201;
        const int WM_LBUTTONUP = 0x0202;
        const int WM_RBUTTONDOWN = 0x0204;
        const int WM_RBUTTONUP = 0x0205;

        public InputController(BotInstance instance)
        {
            this.i = instance;
        }

        public void PressKey(Keys key)
        {
            PostMessage(this.i.Initializer.WindowHandle.Value, WM_KEYDOWN, (IntPtr)key, IntPtr.Zero);
        }

        public void PressKey(Keys key, int msUntilRelease)
        {
            this.PressKey(key);

            Task.Run(() =>
            {
                Thread.Sleep(msUntilRelease);
                this.ReleaseKey(key);
            });
        }

        public void ReleaseKey(Keys key)
        {
            //Logger.AddLogEntry($"Releasing {key}");
            PostMessage(this.i.Initializer.WindowHandle.Value, WM_KEYUP, (IntPtr)key, IntPtr.Zero);
        }
        public static int MakeLParam(int x, int y) => (y << 16) | (x & 0xFFFF);
        public void LeftMouseClick(int x, int y)
        {
            Task.Run(() =>
            {
                PostMessage(this.i.Initializer.WindowHandle.Value, WM_LBUTTONDOWN, (IntPtr)0, (IntPtr)MakeLParam(x, y));
                var rand = new Random().Next(100, 1000);
                Task.Delay(rand);
                PostMessage(this.i.Initializer.WindowHandle.Value, WM_LBUTTONUP, (IntPtr)1, (IntPtr)MakeLParam(x, y));
            });
        }

        public void RightMouseClick(int x, int y)
        {
            Task.Run(() =>
            {
                PostMessage(this.i.Initializer.WindowHandle.Value, WM_RBUTTONDOWN, (IntPtr)0, (IntPtr)MakeLParam(x, y));
                PostMessage(this.i.Initializer.WindowHandle.Value, WM_RBUTTONUP, (IntPtr)1, (IntPtr)MakeLParam(x, y));
            });
        }

        public void TapKey(Keys key)
        {
            this.PressKey(key);
            this.ReleaseKey(key);
        }

        public async Task<bool> ClickAndFindTemplate(Bitmap template, IntPtr windowHandle, string text = null, bool doClick = false)
        {
            return await Task.Run(() =>
            {
                var ret = false;
                int foundX = 0;
                int foundY = 0;
                bool templateFound = false;
                int counter = 0;

                while (!templateFound && counter < 10)
                {
                    Thread.Sleep(100);
                    counter++;

                    this.AnalyseScreenshot(template, windowHandle, out foundX, out foundY, out templateFound);
                }

                if (templateFound)
                {
                    if (doClick)
                    {
                        SetForegroundWindow(windowHandle);
                        this.LeftMouseClick(foundX, foundY);

                        if(!string.IsNullOrEmpty(text))
                        {
                            Thread.Sleep(1000);
                            SendKeys.SendWait(text);
                        }
                    }
                    ret = true;
                }
                else
                {
                    Console.WriteLine("Cant find template");
                    // Write To log, message to user etc.
                }

                return ret;
            });
        }

        /// <summary>
        /// Analysiert den aktuellen Screenshot und vergleicht diesen mit einem template
        /// </summary>
        private void AnalyseScreenshot(Bitmap searchedTemplate, IntPtr windowHandle, out int foundX, out int foundY, out bool templateFound)
        {
            foundX = 0;
            foundY = 0;
            templateFound = false;

            try
            {
                var screenshot = this.CurrentScreenshotFromScreen(windowHandle);
                Bitmap temp = (Bitmap)screenshot.Clone();

                Image<Bgr, byte> clonedScreenshot = new Image<Bgr, byte>(temp);
                Image<Bgr, byte> template = new Image<Bgr, byte>(searchedTemplate);

                this.FindTemplate(template, clonedScreenshot, out foundX, out foundY, out templateFound);

                temp.Dispose();
            }
            catch (Exception ex)
            {
                // Write To Log
            }
        }

        private Bitmap CurrentScreenshotFromScreen(IntPtr windowHandle)
        {
            var rect = new Rect();
            InputController.GetWindowRect(windowHandle, ref rect);
            SetForegroundWindow(windowHandle);

            var offSetX = 14;
            var offSetY = 38;

            var width = rect.right - rect.left - offSetX;
            var height = rect.bottom - rect.top - offSetY;

            Bitmap bitmap = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(bitmap as Image);

            graphics.CopyFromScreen(rect.left + (offSetX / 2), rect.top + 32, 0, 0, bitmap.Size);
            graphics.Dispose();

            return bitmap;
        }

        private void FindTemplate(Image<Bgr, byte> template, Image<Bgr, byte> source, out int foundX, out int foundY, out bool templateFound)
        {
            templateFound = false;
            foundX = 0;
            foundY = 0;

            using (Image<Gray, float> result = source.MatchTemplate(template, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed))
            {
                this.CalculateTemplateCoeff(template, result, out foundX, out foundY, out templateFound);
            }
        }

        /// <summary>
        /// Kalkuliert den Koeffizienten --> Zu wie viel Anteil an % stimmt das Template mit dem zu vergleichenden Screenshot überein
        /// </summary>
        /// <param name="template"></param>
        /// <param name="result"></param>
        /// <param name="windowHandle"></param>
        /// <param name="foundX"></param>
        /// <param name="foundY"></param>
        /// <param name="templateFound"></param>
        /// <param name="imageToShow"></param>
        private void CalculateTemplateCoeff(
            Image<Bgr, byte> template,
            Image<Gray, float> result,
            out int foundX,
            out int foundY,
            out bool templateFound,
            Image<Bgr, byte> imageToShow = null
            )
        {
            foundX = 0;
            foundY = 0;

            double[] minValues, maxValues;
            Point[] minLocations, maxLocations;

            result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);

            // You can try different values of the threshold. I guess somewhere between 0.75 and 0.95 would be good.
            if (maxValues[0] > 0.9)
            {
                Rectangle match = new Rectangle(maxLocations[0], template.Size);
                // X und Y (centered) of the found template
                foundX = match.Left + match.Width / 2;
                foundY = match.Top + match.Height / 2;

                templateFound = true;
            }
            else
            {
                templateFound = false;
            }
        }
    }
}
