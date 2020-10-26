using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Grindr
{
    public class Statistics : INotifyPropertyChanged
    {
        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private string runtime;
        public string Runtime
        {
            get
            {
                return runtime;
            }
            set
            {
                runtime = value;

                OnPropertyChanged();
            }
        }

        private int earnedGold = 0;
        public int EarnedGold
        {
            get
            {
                return earnedGold;
            }
            set
            {
                earnedGold = value;

                OnPropertyChanged();
            }
        }

        private int runs = 0;
        public int Runs
        {
            get
            {
                return runs;
            }
            set
            {
                runs = value;

                OnPropertyChanged();
            }
        }

        private double goldPerRun = 0;
        public double GoldPerRun
        {
            get
            {
                return goldPerRun;
            }
            set
            {
                goldPerRun = value;

                OnPropertyChanged();
            }
        }

        private double goldPerHour = 0;
        public double GoldPerHour
        {
            get
            {
                return goldPerHour;
            }
            set
            {
                goldPerHour = value;

                OnPropertyChanged();
            }
        }

        private bool captureMouseClickEnabled = false;
        public bool CaptureMouseClickEnabled
        {
            get
            {
                return captureMouseClickEnabled;
            }
            set
            {
                captureMouseClickEnabled = value;

                OnPropertyChanged();
            }
        }


        private BotInstance i { get; set; }

        public Statistics(BotInstance instance)
        {
            this.i = instance;
        }

        public void Track()
        {
            Task.Run(() =>
            {
                var startGold = this.i.Data.Gold;
                var startTime = DateTime.Now;
                this.GoldPerHour = 0;
                this.runs = 0;
                this.GoldPerRun = 0;

                var runtimeInMin = 0.0;

                while (this.i.State.IsRunning)
                {
                    this.EarnedGold = this.i.Data.Gold - startGold;
                    var diff = DateTime.Now - startTime;
                    runtimeInMin = (DateTime.Now - startTime).TotalMinutes;
                    if (this.EarnedGold > 0 && runtimeInMin > 0)
                    {
                        this.GoldPerHour = Math.Round((double)(this.EarnedGold / (double)runtimeInMin) * 60, 2);
                    }

                    if (this.EarnedGold > 0 && runs > 0)
                    {
                        this.GoldPerRun = Math.Round((double)this.EarnedGold / (double)runs, 2);
                    }

                    this.Runtime = $"{diff.Hours}:{diff.Minutes}:{diff.Seconds}";

                    System.Threading.Thread.Sleep(1000);
                }
            });
        }
    }
}
