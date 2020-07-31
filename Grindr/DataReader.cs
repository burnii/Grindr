using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Grindr.ScreenRecorderHelper;

namespace Grindr
{
    public class DataReader
    {
        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        private static readonly string rootColorHex = "ffff0000";

        public static void Start()
        {
            Task.Run(() =>
            {
                GetRootCoordinate(out int x, out int y);

                RECT srcRect;
                if (Initializer.Process != null && Initializer.WindowHandle != null)
                {
                    if (GetWindowRect(Initializer.WindowHandle.Value, out srcRect))
                    {
                        
                        int width = srcRect.Right - srcRect.Left;
                        int height = srcRect.Bottom - srcRect.Top;

                        Bitmap bmp = new Bitmap(width, height);

                        using (var screenG = Graphics.FromImage(bmp))
                        {
                            Logger.AddLogEntry("Data reading started");
                            while (State.IsAttached)
                            {

                                GetWindowRect(Initializer.WindowHandle.Value, out srcRect);
                                ScreenRecorderHelper.RecordScreen(
                                    screenG,
                                    srcRect.Top,
                                    srcRect.Left,
                                    bmp.Width,
                                    bmp.Height
                                );

                                Data.PlayerXCoordinate = GetDoublePixelValue(bmp, x + 1, y) * 10;

                                Data.PlayerYCoordinate = GetDoublePixelValue(bmp, x + 2, y) * 10;

                                Data.PlayerFacing = GetDoublePixelValue(bmp, x + 3, y);

                                Data.PlayerIsInCombat = GetBoolPixelValue(bmp, x + 4, y);

                                Data.PlayerHasTarget = GetBoolPixelValue(bmp, x + 5, y);

                                Data.IsTargetDead = GetBoolPixelValue(bmp, x + 6, y);
                            }
                            Logger.AddLogEntry("Data reading stopped");
                        }
                    }
                }
            });
        }

        private static void GetRootCoordinate(out int x, out int y) 
        {
            int attempts = 0;
            while (true)
            {
                Logger.AddLogEntry($"Try to get root coordinates ({attempts})");

                var bmp = ScreenRecorderHelper.TakeScreenshot();

                for (x = 0; x < bmp.Width; x++)
                {
                    for (y = 0; y < bmp.Height; y++)
                    {
                        var value = GetPixelValueAt(bmp, x, y);

                        if (value == rootColorHex)
                        {
                            Logger.AddLogEntry($"Root coordinates: x: {x}, y: {y}");
                            return;
                        }
                    }
                }

                Thread.Sleep(3000);
                attempts++;
            }
        }

        private static bool GetBoolPixelValue(Bitmap bmp, int x, int y)
        {
            var hex = GetPixelValueAt(bmp, x, y);
            return Convert.ToInt32(hex.Substring(2), 16) == 100 ? false : true;
        }

        private static double GetDoublePixelValue(Bitmap bmp, int x, int y)
        {
            var hex = GetPixelValueAt(bmp, x, y);
            return (double)Convert.ToInt32(hex.Substring(2), 16) / 100000;
        }

        private static string GetPixelValueAt(Bitmap bmp, int x, int y)
        {
            return bmp.GetPixel(x, y).Name;
        }
    }
}
