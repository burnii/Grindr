using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        public bool IsInitializing { get; set; } = false;

        public BotInstance i { get; set; }

        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

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

                Process = Process.Start(this.i.Profile.Settings.WowExePath);
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
                SendKeys.SendWait(this.i.Profile.Settings.Username);
                SendKeys.SendWait("{Tab}");
                SendKeys.SendWait(this.i.Profile.Settings.Password);
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
                this.i.State.AttachState = Enums.AttachState.Attaching;
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
                        this.i.State.AttachState = Enums.AttachState.Detach;
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
            this.i.State.AttachState = Enums.AttachState.Attach;
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
