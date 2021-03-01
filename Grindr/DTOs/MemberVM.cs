using Grindr.CombaRoutine;
using Grindr.VM;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Grindr.DTOs
{
    public class MemberVM : BaseViewModel
    {
        [JsonIgnore]
        public BotInstance i { get; set; }

        private string accName;
        public string AccName
        {
            get
            {
                return accName;
            }
            set
            {
                accName = value;
                OnPropertyChanged("AccName");
            }
        }

        private string password;
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
                OnPropertyChanged("Password");
            }
        }

        private int wowAccIndex;
        public int WowAccIndex
        {
            get
            {
                return wowAccIndex;
            }
            set
            {
                wowAccIndex = value;
                OnPropertyChanged("WowAccIndex");
            }
        }

        private int charIndex;
        public int CharIndex
        {
            get
            {
                return charIndex;
            }
            set
            {
                charIndex = value;
                OnPropertyChanged("CharIndex");
            }
        }

        private string server;
        public string Server
        {
            get
            {
                return server;
            }
            set
            {
                server = value;
                OnPropertyChanged("Server");
            }
        }

        private string charname;
        public string Charname
        {
            get
            {
                return charname;
            }
            set
            {
                charname = value;
                OnPropertyChanged("Charname");
            }
        }

        private bool isLeader;
        public bool IsLeader
        {
            get
            {
                return isLeader;
            }
            set
            {
                isLeader = value;
                OnPropertyChanged("IsLeader");
            }
        }


        private string defaultProfile;

        public string DefaultProfile
        {
            get
            {
                return defaultProfile;
            }
            set
            {
                defaultProfile = value;
                OnPropertyChanged("DefaultProfile");
            }
        }

        public void VendorItems()
        {
            this.i.InputController.TapKey(System.Windows.Forms.Keys.D4);
            Thread.Sleep(2000);

            this.i.InputController.TapKey(System.Windows.Forms.Keys.D8);
            Thread.Sleep(7000);

            this.i.InputController.TapKey(System.Windows.Forms.Keys.D9);
            Thread.Sleep(1000);

            this.i.InputController.TapKey(System.Windows.Forms.Keys.D9);
            Thread.Sleep(1000);

            this.i.InputController.TapKey(System.Windows.Forms.Keys.Y);
            Thread.Sleep(1000);

            this.i.InputController.TapKey(System.Windows.Forms.Keys.Y);
            Thread.Sleep(1000);

            for (int i = 0; i < 10; i++)
            {
                this.i.InputController.TapKey(System.Windows.Forms.Keys.D0);

                Thread.Sleep(1000);
            }
        }

        private bool ShouldVendor = false;
        private bool ShouldHeal = false;
        private bool ShouldLoot = false;

        private void StartLootTask()
        {
            Task.Run(() =>
            {
                var time = 0;
                while (this.i.State.IsRunning)
                {
                    Thread.Sleep(1000);
                    time += 1000;

                    if (time > this.i.Profile.CombatRoutine.LootMillis)
                    { 
                        this.ShouldLoot = true;
                        time = 0;
                    }
                }
            });
        }

        private void StartVendorTask()
        {
            Task.Run(() =>
            {
                var time = 0;
                while (this.i.State.IsRunning)
                {
                    Thread.Sleep(1000);
                    time += 1000;

                    if (time > this.i.Profile.CombatRoutine.VendorMillis)
                    {
                        this.ShouldVendor = true;
                        time = 0;
                    }
                }
            });
        }

        private void StartHealTask()
        {
            Task.Run(() =>
            {
                var time = 0;
                while (this.i.State.IsRunning)
                {
                    Thread.Sleep(1000);
                    time += 1000;

                    if (time > this.i.Profile.CombatRoutine.HealMillis)
                    {
                        this.ShouldHeal = true;
                        time = 0;
                    }
                }
            });
        }

        public void StopCombatRoutine()
        {
            this.i.State.IsRunning = false;
        }

        public void StartCombatRoutine(CombatRoutine combatRoutine = null)
        {
            this.i.State.IsRunning = true;

            combatRoutine = combatRoutine ?? this.i.Profile.CombatRoutine;

            if (combatRoutine.LootMillis > 0)
            {
                this.StartLootTask();
            }

            if (combatRoutine.VendorMillis > 0)
            {
                this.StartVendorTask();
            }

            if (combatRoutine.HealMillis > 0)
            {
                this.StartHealTask();
            }

            Task.Run(() => {
                while (this.i.State.IsRunning == true)
                {
                    if (this.ShouldVendor)
                    {
                        this.VendorItems();

                        this.ShouldVendor = false;
                    }
                    else if (this.ShouldLoot)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            Thread.Sleep(3500);
                            this.i.InputController.TapKey(System.Windows.Forms.Keys.L);
                        }

                        this.ShouldLoot = false;
                    }
                    else if (this.ShouldHeal)
                    {
                        Thread.Sleep(1000);
                        this.i.InputController.TapKey(System.Windows.Forms.Keys.D4);

                        this.ShouldHeal = false;
                    }
                    else
                    {
                        foreach (var key in combatRoutine.InputKeys)
                        { 
                            this.i.InputController.TapKey(key);
                        }
                    }

                    Thread.Sleep(combatRoutine.MillisBetweenInput);
                }
            });
        }

        public async Task<bool> Launch()
        {
            return await this.i.Initializer.InitializeInternal(this);
        }
    }
}
