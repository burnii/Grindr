using Grindr.DTOs;
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
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern Int32 GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int Width, int Height, bool Repaint);

        public IntPtr? WindowHandle { get; set; }

        public Process Process { get; set; }

        public bool IsInitializing { get; set; } = false;

        public BotInstance i { get; set; }

        public Initializer(BotInstance instance = null)
        {
            this.i = instance;
        }

        public async Task LaunchTeams(params TeamVM[] teams)
        {
            //return Task.Run(() =>
            //{
            //    foreach (var team in teams)
            //    {
            //        foreach (var member in team.Member)
            //        {
            //            this.InitializeInternal(member);
            //        }
            //    }
            //});

                foreach (var team in teams)
                {
                    foreach (var member in team.Member)
                    {
                        await this.InitializeInternal(member);
                    }
                }
        }

        public async Task<bool> Initialize()
        {
            //return Task.Run(() =>
            //{
            //    this.InitializeInternal();
            //});

            return await this.InitializeInternal();
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

        public async Task<bool> InitializeInternal(MemberVM member = null)
        {
            bool initialized = false;

            // um abwärtskompatibel zu bleiben, da der multiboxer an dieser Stelle keine Botinstanz (i) hat, sondern auf den member zugegriffen wird.
            string username = member.AccName;
            string password = member.Password;

            if (member == null)
            {
                username = this.i.Profile.Settings.Username;
            }

            if (member == null)
            {
                password = this.i.Profile.Settings.Password;
            }
            //

            member.i.Logger.AddLogEntry("Initializing ...");
            IsInitializing = true;

            if (Process != null && Process.Responding == true)
            {
                member.i.Logger.AddLogEntry($"Kill WoW process with PID: {Process.Id}");
                Process.Kill();
            }

            Process = Process.Start(GlobalState.Instance.WowExePath);
            Process.WaitForInputIdle();
            WindowHandle = Process.MainWindowHandle;

            MoveWindow(WindowHandle.Value, 0, 0, 500, 500, false);

            // Always focus the window while initializing since "SendKeys" only sends the keys to the focused window
            //await Task.Run(() =>
            // {
            //     while (IsInitializing)
            //     {
            //         Thread.Sleep(100);
            //         SetForegroundWindow(WindowHandle.Value);
            //     }
            // });


            var found = await member.i.InputController.ClickAndFindTemplate(Properties.Resources.usernameField_484x461, WindowHandle.Value, username, true);

            if (!found)
            {
                throw new Exception("Couldnt insert Username");
            }

            found = await member.i.InputController.ClickAndFindTemplate(Properties.Resources.passwordField_484x461, WindowHandle.Value, password, true);

            if (!found)
            {
                throw new Exception("Couldnt insert Password");
            }

            found = await member.i.InputController.ClickAndFindTemplate(Properties.Resources.loginButton_484x461, WindowHandle.Value, string.Empty, true);

            if (!found)
            {
                throw new Exception("Couldnt insert Password");
            }


            found = await member.i.InputController.ClickAndFindTemplate(Properties.Resources.passwordWrong_484x461, WindowHandle.Value, string.Empty, false);

            if (found)
            {
                throw new Exception("Wrong password");
            }

            found = await member.i.InputController.ClickAndFindTemplate(Properties.Resources.cantFindBlizzAccount_484x461, WindowHandle.Value, string.Empty, false);

            if (found)
            {
                throw new Exception("No Blizz Acc found");
            }

            Thread.Sleep(5000);

            switch (member.WowAccIndex)
            {
                case 1:
                    found = await member.i.InputController.ClickAndFindTemplate(Properties.Resources.accountWow1_484x461, WindowHandle.Value, string.Empty, true);
                    break;
                case 2:
                    found = await member.i.InputController.ClickAndFindTemplate(Properties.Resources.accountWow2_484x461, WindowHandle.Value, string.Empty, true);
                    break;
                case 3:
                    found = await member.i.InputController.ClickAndFindTemplate(Properties.Resources.accountWow3_484x461, WindowHandle.Value, string.Empty, true);
                    break;
                case 4:
                    found = await member.i.InputController.ClickAndFindTemplate(Properties.Resources.accountWow4_484x461, WindowHandle.Value, string.Empty, true);
                    break;
                case 5:
                    found = await member.i.InputController.ClickAndFindTemplate(Properties.Resources.accountWow5_484x461, WindowHandle.Value, string.Empty, true);
                    break;
                case 6:
                    found = await member.i.InputController.ClickAndFindTemplate(Properties.Resources.accountWow6_484x461, WindowHandle.Value, string.Empty, true);
                    break;
                case 7:
                    found = await member.i.InputController.ClickAndFindTemplate(Properties.Resources.accountWow7_484x461, WindowHandle.Value, string.Empty, true);
                    break;
                case 8:
                    found = await member.i.InputController.ClickAndFindTemplate(Properties.Resources.accountWow8_484x461, WindowHandle.Value, string.Empty, true);
                    break;
                default:
                    break;
            }

            if (!found)
            {
                throw new Exception("Wow Account (1-8) not found");
            }

            found = await member.i.InputController.ClickAndFindTemplate(Properties.Resources.acceptAccount_484x461, WindowHandle.Value, string.Empty, true);

            if (!found)
            {
                throw new Exception("Cant accept wow account");
            }

            Thread.Sleep(10000);
            member.i.InputController.TapKey(Keys.Enter);

            member.i.Logger.AddLogEntry("Initialized");
            IsInitializing = false;
            initialized = true;

            return initialized;
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
