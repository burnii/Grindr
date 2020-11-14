using Grindr.DTOs;
using Grindr.VM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Grindr.CombaRoutine
{
    public class CombatRoutine : BaseViewModel
    {
        private int millisBetweenInput;

        public int MillisBetweenInput { 
            get
            {
                return millisBetweenInput;
            } 
            set
            {
                this.millisBetweenInput = value;
                this.OnPropertyChanged();
            } 
        }

        private BindingList<Keys> inputKeys = new BindingList<Keys>();
        public BindingList<Keys> InputKeys {
            get 
            {
                return inputKeys;
            }
            set 
            {
                this.inputKeys = value;
                this.OnPropertyChanged();
            }
        }

        private int lootMillis;
        public int LootMillis { get
            {
                return this.lootMillis;
            }
            set
            {
                this.lootMillis = value;
                this.OnPropertyChanged();
            }
        }

        private int lootAttempts;
        public int LootAttempts {
            get
            {
                return lootAttempts;
            }
            set
            {
                this.lootAttempts = value;
                this.OnPropertyChanged();
            } 
        }

        private int vendorMillis;
        public int VendorMillis {
            get
            {
                return vendorMillis;
            }
            set
            {
                this.vendorMillis = value;
                this.OnPropertyChanged();
            } 
        }

        private int vendorAttempts;
        public int VendorAttempts {
            get
            {
                return vendorAttempts;
            }
            set
            {
                this.vendorAttempts = value;
                this.OnPropertyChanged();
            }
        }

        private int healMillis;
        public int HealMillis {
            get 
            {
                return healMillis;
            }
            set
            {
                this.healMillis = value;
                this.OnPropertyChanged();
            }
        }

        private bool shouldHeal = false;
        public bool ShouldHeal {
            get 
            {
                return shouldHeal;
            }
            set
            {
                this.shouldHeal = value;
                this.OnPropertyChanged();
            }
        }

        private bool shouldVendor = false;
        public bool ShouldVendor {
            get
            {
                return this.shouldVendor;
            }
            set
            {
                this.shouldVendor = value;
                this.OnPropertyChanged();
            }
        }

        private bool shouldLoot = false;
        public bool ShouldLoot {
            get
            {
                return this.shouldLoot;
            }
            set
            {
                this.shouldLoot = value;
                this.OnPropertyChanged();
            }
        }

        private void StartLootTask(TeamVM team)
        {
            Task.Run(() =>
            {
                while (team.Member.Any(x => x.i.State.IsRunning == true))
                {
                    Thread.Sleep(this.LootMillis);
                    this.ShouldLoot = true;
                }
            });
        }

        private void StartVendorTask(TeamVM team)
        {
            Task.Run(() =>
            {
                while (team.Member.Any(x => x.i.State.IsRunning == true))
                {
                    Thread.Sleep(this.VendorMillis);
                    this.ShouldVendor = true;
                }
            });
        }

        private void StartHealTask(TeamVM team)
        {
            Task.Run(() =>
            {
                while (team.Member.Any(x => x.i.State.IsRunning == true))
                {
                    Thread.Sleep(this.HealMillis);
                    this.ShouldHeal = true;
                }
            });
        }

        public void Run(TeamVM team)
        {
            if (LootMillis > 0)
            {
                this.StartLootTask(team);
            }

            if (VendorMillis > 0)
            {
                this.StartVendorTask(team);
            }

            if (HealMillis > 0)
            {
                this.StartHealTask(team);
            }

            Task.Run(() => {
                while (team.Member.Any(x => x.i.State.IsRunning == true))
                {
                    if (this.ShouldVendor)
                    {
                        team.VendorItemsForAllMember();

                        this.ShouldVendor = false;
                    }
                    else if (this.ShouldLoot)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            Thread.Sleep(3500);
                            team.TapKeysForAllMember(Keys.L);
                        }

                        this.ShouldLoot = false;
                    }
                    else if (this.ShouldHeal)
                    {
                        Thread.Sleep(1000);
                        team.TapKeysForAllMember(Keys.D4);

                        this.ShouldHeal = false;
                    }
                    else
                    { 
                        team.TapKeysForAllMember(this.InputKeys.ToArray());
                    }

                    Thread.Sleep(this.MillisBetweenInput);
                }
            });
        }
    }
}
