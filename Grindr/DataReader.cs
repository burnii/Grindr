using Grindr.Enums;
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

namespace Grindr
{
    public class DataReader
    {
        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int Width, int Height, bool Repaint);

        private readonly string rootColorHex = "ffff0000";

        private int stride = 0;

        private readonly int width1 = 500;
        private readonly int height1 = 500;
        private readonly int maxWidth = 2000;

        private ScreenRecorderHelper recorder = new ScreenRecorderHelper();

        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public BotInstance i { get; set; }

        public DataReader(BotInstance instance)
        {
            this.i = instance;
        }

        private void AlignWindow()
        {
            var row = 0;

            if ((this.i.BotIndex + 1) * height1 > maxWidth)
            {
                row++;    
            }

            if (this.i.BotIndex * height1 > maxWidth * 2)
            {
                row++;
            }

            //MoveWindow(this.i.Initializer.WindowHandle.Value, 1, 1, width1, height1, false);
            MoveWindow(this.i.Initializer.WindowHandle.Value, this.i.BotIndex * width1 - (row * maxWidth), row * height1, width1, height1, false);
        }

        public void Start()
        {
            Task.Run(() =>
            {
                GetRootCoordinate(out int x, out int y);

                RECT srcRect;
                if (this.i.Initializer.Process != null && this.i.Initializer.WindowHandle != null)
                {
                    this.AlignWindow();

                    if (GetWindowRect(this.i.Initializer.WindowHandle.Value, out srcRect))
                    {
                        int width = srcRect.Right - srcRect.Left;
                        int height = srcRect.Bottom - srcRect.Top;

                        Bitmap bmp = new Bitmap(width, height);

                        var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);                    

                        using (var screenG = Graphics.FromImage(bmp))
                        {
                            this.i.Logger.AddLogEntry("Data reading started");
                            
                            while (this.i.State.AttachState == Enums.AttachState.Detach)
                            {
                                Point p = Cursor.Position;

                                if (rect.Contains(p))
                                {
                                    var x1 = Cursor.Position.X - rect.Left;
                                    var y1 = Cursor.Position.Y - rect.Top;
                                    Console.WriteLine(x1);
                                    Console.WriteLine(y1);
                                    //new InputController(Initializer.WindowHandle.Value).MouseClick(x1, y1);

                                }

                                GetWindowRect(this.i.Initializer.WindowHandle.Value, out srcRect);
                                recorder.RecordScreen(
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

                                this.i.Data.IsInInstance = GetBoolPixelValue(bmp, x + 24, y, rgbValues);
                                this.i.Data.IsMapOpened = GetBoolPixelValue(bmp, x + 27, y, rgbValues);

                                if (this.i.Data.IsInInstance == true)
                                {

                                    //if ((Data.IsMapOpened == false) && State.IsRunning)
                                    //{
                                        
                                    //        inputController.TapKey(Keys.M);
                                    //        Thread.Sleep(1000);
                                        
                                    //}
                                    var playerCoordinate = GetPixelCoordinate(rgbValues, bmpData, out var angle);

                                    this.i.Data.PlayerXCoordinate = playerCoordinate.X;
                                    this.i.Data.PlayerYCoordinate = playerCoordinate.Y;
                                    this.i.Data.PlayerFacing = angle;
                                }
                                else
                                {
                                    this.i.Data.PlayerXCoordinate = GetDoublePixelValue(bmp, x + 3, y, rgbValues) * 10;
                                    this.i.Data.PlayerYCoordinate = GetDoublePixelValue(bmp, x + 6, y, rgbValues) * 10;
                                    this.i.Data.PlayerFacing = GetDoublePixelValue(bmp, x + 9, y, rgbValues);
                                }
                                this.i.Data.PlayerIsInCombat = GetBoolPixelValue(bmp, x + 12, y, rgbValues);
                                this.i.Data.IsTargetDead = GetBoolPixelValue(bmp, x + 15, y, rgbValues);
                                this.i.Data.PlayerHasTarget = GetBoolPixelValue(bmp, x + 18, y, rgbValues);
                                this.i.Data.PlayerZone = GetStringPixelValues(bmp, y, rgbValues, x + 21/*, x + 90, x + 100, x + 110*/);
                                this.i.Data.IsPlayerDead = GetBoolPixelValue(bmp, x + 30, y, rgbValues);
                                this.i.Data.TargetIsInInteractRange = GetBoolPixelValue(bmp, x + 33, y, rgbValues);
                                this.i.Data.IsTargetAttackingPlayer = GetBoolPixelValue(bmp, x + 36, y, rgbValues);
                                this.i.Data.IsOutDoors = GetBoolPixelValue(bmp, x + 39, y, rgbValues);
                                this.i.Data.FreeBagSlots = GetIntPixelValue(bmp, x + 42, y, rgbValues);
                                this.i.Data.IsMounted = GetBoolPixelValue(bmp, x + 45, y, rgbValues);
                                this.i.Data.PlayerHealth = GetIntPixelValue(bmp, x + 48, y, rgbValues);
                                var playerName = "";
                                playerName += GetStringPixelValues(bmp, y, rgbValues, x + 51/*, x + 90, x + 100, x + 110*/);
                                playerName += GetStringPixelValues(bmp, y, rgbValues, x + 54/*, x + 90, x + 100, x + 110*/);
                                this.i.Data.PlayerName = playerName;
                                this.i.Data.PlayerIsInGroup = GetBoolPixelValue(bmp, x + 57, y, rgbValues);
                                this.i.Data.DruidShapeshiftForm = (DruidShapeshiftForm)GetIntPixelValue(bmp, x + 60, y, rgbValues);
                                this.i.Data.Party1Health = GetIntPixelValue(bmp, x + 63, y, rgbValues);
                                this.i.Data.Party2Health = GetIntPixelValue(bmp, x + 66, y, rgbValues);
                                this.i.Data.Party3Health = GetIntPixelValue(bmp, x + 69, y, rgbValues);
                                this.i.Data.Party4Health = GetIntPixelValue(bmp, x + 72, y, rgbValues);
                                this.i.Data.IsStealthed = GetBoolPixelValue(bmp, x + 75, y, rgbValues);

                                bmp.UnlockBits(bmpData);
                                Thread.Sleep(50);
                            }
                            this.i.Logger.AddLogEntry("Data reading stopped");
                        }
                    }
                }
            });
        }

        private Coordinate GetPixelCoordinate(byte[] rgbValues, BitmapData bmpData,out double angle)
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



        private void GetRootCoordinate(out int x, out int y)
        {
            int attempts = 0;
            while (true)
            {
                this.i.Logger.AddLogEntry($"Try to get root coordinates ({attempts})");

                var bmp = recorder.TakeScreenshot(this.i);

                for (x = 0; x < bmp.Width; x++)
                {
                    for (y = 0; y < bmp.Height; y++)
                    {
                        var value = GetPixelValueAt(bmp, x, y);

                        if (value == rootColorHex)
                        {
                            this.i.Logger.AddLogEntry($"Root coordinates: x: {x}, y: {y}");
                            return;
                        }
                    }
                }

                Thread.Sleep(3000);
                attempts++;
            }
        }

        private string RgbToHex(int x, int y, byte[] rgbValues)
        {
            var b = rgbValues[(y * this.stride) + (x * 3)];
            var g = rgbValues[(y * this.stride) + (x * 3) + 1];
            var r = rgbValues[(y * this.stride) + (x * 3) + 2];

            var color = Color.FromArgb(r, g, b);

            return color.Name.Substring(2);
        }

        private bool GetBoolPixelValue(Bitmap bmp, int x, int y, byte[] rgbValues)
        {
            var hex = RgbToHex(x, y, rgbValues);

            return Convert.ToInt32(hex, 16) == 100 ? false : true;
        }

        private double GetDoublePixelValue(Bitmap bmp, int x, int y, byte[] rgbValues)
        {
            var hex = RgbToHex(x, y, rgbValues);
            return (double)Convert.ToInt32(hex, 16) / 100000;
        }

        private string GetStringPixelValues(Bitmap bmp, int y, byte[] rgbValues, params int[] xs)
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

        private  int GetIntPixelValue(Bitmap bmp, int x, int y, byte[] rgbValues)
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
