using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Grindr
{
    public class InputController
    {
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
                PostMessage(this.i.Initializer.WindowHandle.Value, WM_LBUTTONDOWN, (IntPtr)0, (IntPtr)MakeLParam(x,y));
                PostMessage(this.i.Initializer.WindowHandle.Value, WM_LBUTTONUP, (IntPtr)1, (IntPtr)MakeLParam(x,y));
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
    }
}
