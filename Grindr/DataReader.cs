using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
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

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int Width, int Height, bool Repaint);

        private static readonly string rootColorHex = "ffff0000";

        private static int stride = 0;

        public static void Start()
        {
            Task.Run(() =>
            {
                var inputController = new InputController(Initializer.WindowHandle.Value);
                MoveWindow(Initializer.WindowHandle.Value, 0, 0, 500, 500, false);
                GetRootCoordinate(out int x, out int y);

                RECT srcRect;
                if (Initializer.Process != null && Initializer.WindowHandle != null)
                {
                    if (GetWindowRect(Initializer.WindowHandle.Value, out srcRect))
                    {
                        int width = srcRect.Right - srcRect.Left;
                        int height = srcRect.Bottom - srcRect.Top;

                        Bitmap bmp = new Bitmap(width, height);

                        var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);                    

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

                                var bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                                stride = bmpData.Stride;

                                int bytes = bmpData.Stride * bmp.Height;

                                byte[] rgbValues = new byte[bytes];

                                IntPtr ptr = bmpData.Scan0;

                                Marshal.Copy(ptr, rgbValues, 0, bytes);

                                Data.IsInInstance = GetBoolPixelValue(bmp, x + 24, y, rgbValues);

                                if (Data.IsInInstance == true)
                                {
                                    
                                    if (Data.PlayerXCoordinate == int.MaxValue || Data.PlayerYCoordinate == int.MaxValue)
                                    {
                                        inputController.TapKey(Keys.M);
                                    }
                                    var playerCoordinate = GetPixelCoordinate(rgbValues, bmpData, out var angle);

                                    Data.PlayerXCoordinate = playerCoordinate.X;
                                    Data.PlayerYCoordinate = playerCoordinate.Y;
                                    Data.PlayerFacing = angle;
                                }
                                else
                                {
                                    Data.PlayerXCoordinate = GetDoublePixelValue(bmp, x + 3, y, rgbValues) * 10;
                                    Data.PlayerYCoordinate = GetDoublePixelValue(bmp, x + 6, y, rgbValues) * 10;
                                    Data.PlayerFacing = GetDoublePixelValue(bmp, x + 9, y, rgbValues);
                                }
                                
                                Data.PlayerIsInCombat = GetBoolPixelValue(bmp, x + 12, y, rgbValues);
                                Data.IsTargetDead = GetBoolPixelValue(bmp, x + 15, y, rgbValues);
                                Data.PlayerHasTarget = GetBoolPixelValue(bmp, x + 18, y, rgbValues);
                                Data.PlayerZone = GetStringPixelValues(bmp, y, rgbValues, x + 21/*, x + 90, x + 100, x + 110*/);

                                bmp.UnlockBits(bmpData);
                                Thread.Sleep(1);
                            }
                            Logger.AddLogEntry("Data reading stopped");
                        }
                    }
                }
            });
        }

        private static Coordinate GetPixelCoordinate(byte[] rgbValues, BitmapData bmpData,out double angle)
        {
            var stride = bmpData.Stride;
            int count = 0;

            var maxY = int.MinValue;
            var minY = int.MaxValue;
            var maxX = int.MinValue;
            var minX = int.MaxValue;
            var maxXFront = int.MinValue;
            var maxYFront = int.MinValue;
            var minXFront = int.MaxValue;
            var minYFront = int.MaxValue;



            for (var column = 0; column < bmpData.Height; column++)
            {
                for (int row = 0; row < bmpData.Width; row++)
                {
                    var b = rgbValues[(column * stride) + (row * 3)];
                    var g = rgbValues[(column * stride) + (row * 3) + 1];
                    var r = rgbValues[(column * stride) + (row * 3) + 2];

                    if (r == 0 && b == 0 && g == 255)
                    {
                        maxY = Math.Max(maxY, column);
                        maxX = Math.Max(maxX, row);
                        minY = Math.Min(minY, column);
                        minX = Math.Min(minX, row);
                    }

                    if (r == 255 && b == 255 && g == 0)
                    {
                        maxY = Math.Max(maxY, column);
                        maxX = Math.Max(maxX, row);
                        minY = Math.Min(minY, column);
                        minX = Math.Min(minX, row);
                        maxYFront = Math.Max(maxYFront, column);
                        maxXFront = Math.Max(maxXFront, row);
                        minYFront = Math.Min(minYFront, column);
                        minXFront = Math.Min(minXFront, row);
                    }

                    count++;
                }
            }

            var centerX = minX + ((maxX - minX) / 2);
            var centerY = minY + ((maxY - minY) / 2);

            var centerFrontX = minXFront + ((maxXFront - minXFront) / 2);
            var centerFrontY = minYFront + ((maxYFront - minYFront) / 2);

            angle = CalculationHelper.GetAngle(centerX, centerY - 5, centerFrontX, centerFrontY, centerX, centerY);

            return new Coordinate(centerX, centerY);
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

        private static string RgbToHex(int x, int y, byte[] rgbValues)
        {
            var b = rgbValues[(y * stride) + (x * 3)];
            var g = rgbValues[(y * stride) + (x * 3) + 1];
            var r = rgbValues[(y * stride) + (x * 3) + 2];

            var color = Color.FromArgb(r, g, b);

            return color.Name.Substring(2);
        }

        private static bool GetBoolPixelValue(Bitmap bmp, int x, int y, byte[] rgbValues)
        {
            var hex = RgbToHex(x, y, rgbValues);

            return Convert.ToInt32(hex, 16) == 100 ? false : true;
        }

        private static double GetDoublePixelValue(Bitmap bmp, int x, int y, byte[] rgbValues)
        {
            var hex = RgbToHex(x, y, rgbValues);
            return (double)Convert.ToInt32(hex, 16) / 100000;
        }

        private static string GetStringPixelValues(Bitmap bmp, int y, byte[] rgbValues, params int[] xs)
        {
            var locationString = "";

            foreach (var x in xs)
            {
                var hex = RgbToHex(x, y, rgbValues);
                var hexInt = Convert.ToInt32(hex, 16);
                var intString = hexInt.ToString();

                if (intString.Length == 6)
                {
                    for (int i = 0; i < 6; i += 2)
                    {
                        var c = char.ConvertFromUtf32(Convert.ToInt32(intString.Substring(i, 2)));
                        locationString += c;
                    }
                }
            }

            return locationString;
        }

        private static int GetIntPixelValue(Bitmap bmp, int x, int y, byte[] rgbValues)
        {
            var hex = RgbToHex(x, y, rgbValues);

            return Convert.ToInt32(hex, 16);
        }

        private static string GetPixelValueAt(Bitmap bmp, int x, int y)
        {
            return bmp.GetPixel(x, y).Name;
        }
    }
}
