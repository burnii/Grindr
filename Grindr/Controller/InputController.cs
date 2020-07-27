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

namespace Notepad
{
    public class InputController
    {
        [DllImport("user32.dll")]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public IntPtr WindowHandle { get; set; }

        const uint WM_KEYDOWN = 0x100;
        const uint WM_KEYUP = 0x0101;

        public InputController(IntPtr windowHandle)
        {
            this.WindowHandle = windowHandle;
        }

        public void PressKey(Keys key)
        {
            //Logger.AddLogEntry($"Pressing {key}");
            PostMessage(this.WindowHandle, WM_KEYDOWN, (IntPtr)key, IntPtr.Zero);
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
            PostMessage(this.WindowHandle, WM_KEYUP, (IntPtr)key, IntPtr.Zero);
        }

        public void TapKey(Keys key)
        {
            this.PressKey(key);
            this.ReleaseKey(key);
        }
    }
}
