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

namespace Grindr
{
    public class Initializer
    {
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

                if (Process != null && Process.Responding == true)
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


                var found = this.i.InputController.ClickAndFindTemplate(Properties.Resources.usernameField_484x461, WindowHandle.Value,true);

                var counter = 10;
                while(found.Result != true)
                {
                    Thread.Sleep(1000);

                    if(counter == 0)
                    {
                        throw new Exception("Cant login after 10 seconds");
                    }

                    counter--;
                }

                SendKeys.SendWait(this.i.Profile.Settings.Username);
                Thread.Sleep(100);

                found = this.i.InputController.ClickAndFindTemplate(Properties.Resources.passwordField_484x461, WindowHandle.Value, true);

                if (found.Result == true)
                {
                    SendKeys.SendWait(this.i.Profile.Settings.Password);
                    Thread.Sleep(100);
                }

                found = this.i.InputController.ClickAndFindTemplate(Properties.Resources.loginButton_484x461, WindowHandle.Value,false);

                if (found.Result == true)
                {
                    SendKeys.SendWait("{Enter}");
                    Thread.Sleep(100);
                }

                found = this.i.InputController.ClickAndFindTemplate(Properties.Resources.passwordWrong_484x461, WindowHandle.Value, false);

                if (found.Result == true)
                {
                    IsInitializing = false;
                    throw new Exception("Wrong password");
                }

                found = this.i.InputController.ClickAndFindTemplate(Properties.Resources.cantFindBlizzAccount_484x461, WindowHandle.Value, false);

                if (found.Result == true)
                {
                    IsInitializing = false;
                    throw new Exception("No Blizzard-Account found");
                }

                switch (this.i.Profile.Settings.BlizzAccountIndex)
                {
                    case 1:
                        found = this.i.InputController.ClickAndFindTemplate(Properties.Resources.accountWow1_484x461, WindowHandle.Value, true);
                        break;
                    case 2:
                        found = this.i.InputController.ClickAndFindTemplate(Properties.Resources.accountWow2_484x461, WindowHandle.Value, true);
                        break;
                    case 3:
                        found = this.i.InputController.ClickAndFindTemplate(Properties.Resources.accountWow3_484x461, WindowHandle.Value, true);
                        break;
                    case 4:
                        found = this.i.InputController.ClickAndFindTemplate(Properties.Resources.accountWow4_484x461, WindowHandle.Value, true);
                        break;
                    case 5:
                        found = this.i.InputController.ClickAndFindTemplate(Properties.Resources.accountWow5_484x461, WindowHandle.Value, true);
                        break;
                    case 6:
                        found = this.i.InputController.ClickAndFindTemplate(Properties.Resources.accountWow6_484x461, WindowHandle.Value, true);
                        break;
                    case 7:
                        found = this.i.InputController.ClickAndFindTemplate(Properties.Resources.accountWow7_484x461, WindowHandle.Value, true);
                        break;
                    default:
                        break;
                }

                while (!found.IsCompleted || found.Result == false)
                {
                    Console.WriteLine($" IsCompleted: {found.IsCompleted}");
                    Console.WriteLine($" Result     : {found.Result}");
                    if (found.Result && found.IsCompleted)
                    {
                        IsInitializing = false;
                        Thread.Sleep(1200);
                        // Write to Log (WowAccount 1 ausgewählt)
                    }
                    else if(found.IsFaulted || found.IsCanceled)
                    {
                        // Write to Log
                    }
                }

                found = this.i.InputController.ClickAndFindTemplate(Properties.Resources.acceptAccount_484x461, WindowHandle.Value, true);

                if (found.Result == true)
                {
                    IsInitializing = false;
                    Thread.Sleep(100);
                    // Write to Log (accept)
                }

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
