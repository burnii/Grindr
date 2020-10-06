using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Grindr
{
    public class Healer
    {
        public BotInstance i { get; set; }
        public Healer(BotInstance instance)
        {
            this.i = instance;
        }

        public Task Start()
        {
            return Task.Run(() =>
            {
                while (this.i.State.IsRunning)
                {
                    this.i.InputController.TapKey(Keys.F4);
                    var key = this.GetHealTargetShortcut();

                    if (key != null)
                    {
                        this.i.InputController.TapKey(key.Value);
                        Thread.Sleep(100);
                        this.i.InputController.TapKey(Keys.D3);
                        Thread.Sleep(1000);
                        this.i.InputController.TapKey(Keys.D2);
                        Thread.Sleep(1000);

                        var key2 = this.GetHealTargetShortcut();
                        while (key == key2)
                        {
                            this.i.InputController.TapKey(Keys.D1);
                            key2 = this.GetHealTargetShortcut();
                            Thread.Sleep(100);
                        }

                    }

                    Thread.Sleep(500);
                }
            });
        }

        private Keys? GetHealTargetShortcut()
        {
            Keys? targetShortCut = null;
            if (this.IsThisTheLowest(this.i.Data.PlayerHealth, this.i.Data.Party1Health, this.i.Data.Party2Health, this.i.Data.Party3Health, this.i.Data.Party4Health) && this.i.Data.PlayerHealth < 70)
            {
                targetShortCut = Keys.F7;
                Console.WriteLine("F7");

            }
            else if (this.IsThisTheLowest(this.i.Data.Party1Health, this.i.Data.PlayerHealth, this.i.Data.Party2Health, this.i.Data.Party3Health, this.i.Data.Party4Health) && this.i.Data.Party1Health < 70)
            {
                targetShortCut = Keys.F9;
                Console.WriteLine("F9");

            }
            else if (this.IsThisTheLowest(this.i.Data.Party2Health, this.i.Data.PlayerHealth, this.i.Data.Party1Health, this.i.Data.Party3Health, this.i.Data.Party4Health) && this.i.Data.Party2Health < 70)
            {
                targetShortCut = Keys.F10;
                Console.WriteLine("F10");

            }
            else if (this.IsThisTheLowest(this.i.Data.Party3Health, this.i.Data.PlayerHealth, this.i.Data.Party1Health, this.i.Data.Party2Health, this.i.Data.Party4Health) && this.i.Data.Party3Health < 70)
            {
                targetShortCut = Keys.F11;
                Console.WriteLine("F11");

            }
            else if (this.IsThisTheLowest(this.i.Data.Party4Health, this.i.Data.PlayerHealth, this.i.Data.Party1Health, this.i.Data.Party2Health, this.i.Data.Party3Health) && this.i.Data.Party4Health < 70)
            {
                targetShortCut = Keys.F12;
                Console.WriteLine("F12");

            }

            return targetShortCut;
        }

        private bool IsThisTheLowest(int number1, params int[] comparables)
        {
            foreach (var comparable in comparables)
            {
                if (number1 < comparable)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
