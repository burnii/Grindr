using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using static Grindr.DataReader;
using static Grindr.ScreenRecorderHelper;

namespace Grindr
{
    class Initializer
    {
        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern Int32 GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        public static IntPtr WindowHandle { get; set; }

        public static Process Process { get; set; }

        public static string Username { get; set; } = "fritz.bauer@muell.icu";
        public static string Password { get; set; } = "Yh9tegjmg";

        public static bool IsInitializing { get; set; } = false;

        public static Task Initialize()
        {
            return Task.Run(() =>
            {
                Logger.AddLogEntry("Initializing ...");
                IsInitializing = true;

                if (Process != null)
                {
                    Logger.AddLogEntry($"Kill WoW process with PID: {Process.Id}");
                    Process.Kill();
                }

                Process = Process.Start(@"C:\Program Files (x86)\World of Warcraft\_retail_\Wow.exe");
                Process.WaitForInputIdle();
                WindowHandle = Process.MainWindowHandle;

                // Always focus the window while initializing since "SendKeys" only sends the keys to the focused window
                Task.Run(() =>
                {
                    while (IsInitializing)
                    {
                        Thread.Sleep(100);
                        SetForegroundWindow(WindowHandle);
                    }
                });

                Thread.Sleep(1000);
                SendKeys.SendWait(Username);
                SendKeys.SendWait("{Tab}");
                SendKeys.SendWait(Password);
                SendKeys.SendWait("{Enter}");
                Thread.Sleep(10000);
                SendKeys.SendWait("{Enter}");

                Logger.AddLogEntry("Initialized");
                IsInitializing = false;
            });
        }

        public static Task Attach()
        {
            return Task.Run(() =>
            {
                Logger.AddLogEntry("Try to attach to focused wow process");
                while (true)
                {
                    var windowHandle = GetForegroundWindow();

                    var process = GetProcessByHandle(windowHandle);
                    Logger.AddLogEntry($"Focused process name: {process.ProcessName}");
                    if (process.ProcessName == "Wow")
                    {
                        WindowHandle = windowHandle;
                        Process = process;
                        Logger.AddLogEntry($"Attached to Wow process");
                        break;
                    }
                    
                    Thread.Sleep(1000);
                }
            });
        }

        private static Process GetProcessByHandle(IntPtr hwnd)
        {
            try
            {
                uint processID;
                GetWindowThreadProcessId(hwnd, out processID);
                return Process.GetProcessById((int)processID);
            }
            catch
            { 
                return null; 
            }
        }
    }
}
