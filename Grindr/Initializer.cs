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
    public class Initializer
    {
        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern Int32 GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        public IntPtr? WindowHandle { get; set; }

        public Process Process { get; set; }

        public static string Username { get; set; } = "fritz.bauer@muell.icu";
        public static string Password { get; set; } = "Yh9tegjmg";

        public bool IsInitializing { get; set; } = false;

        public BotInstance i { get; set; }

        public Initializer(BotInstance instance)
        {
            this.i = instance;
        }

        public Task Initialize()
        {
            return Task.Run(() =>
            {
                this.i.Logger.AddLogEntry("Initializing ...");
                IsInitializing = true;

                if (Process != null)
                {
                    this.i.Logger.AddLogEntry($"Kill WoW process with PID: {Process.Id}");
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
                        SetForegroundWindow(WindowHandle.Value);
                    }
                });

                Thread.Sleep(1000);
                SendKeys.SendWait(Username);
                SendKeys.SendWait("{Tab}");
                SendKeys.SendWait(Password);
                SendKeys.SendWait("{Enter}");
                Thread.Sleep(10000);
                SendKeys.SendWait("{Enter}");

                this.i.Logger.AddLogEntry("Initialized");
                IsInitializing = false;
            });
        }

        public Task Attach()
        {
            return Task.Run(() =>
            {
                this.i.Logger.AddLogEntry("Try to attach to focused wow process");
                while (true)
                {
                    var windowHandle = GetForegroundWindow();

                    var process = GetProcessByHandle(windowHandle);
                    this.i.Logger.AddLogEntry($"Focused process name: {process.ProcessName}");
                    if (process.ProcessName == "Wow")
                    {
                        WindowHandle = windowHandle;
                        Process = process;
                        State.IsAttached = true;
                        this.i.Logger.AddLogEntry($"Attached to Wow process");
                        break;
                    }
                    
                    Thread.Sleep(1000);
                }
            });
        }

        public void Detach()
        {
            this.i.Logger.AddLogEntry("Detached");
            State.IsAttached = false;
            this.Process = null;
            this.WindowHandle = null;
        }

        private Process GetProcessByHandle(IntPtr hwnd)
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
