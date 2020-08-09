using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Grindr
{
    public class Assister
    {
        public BotInstance i { get; set; }
        public Assister(BotInstance instance)
        {
            this.i = instance;
        }

        public Task Assist(string name)
        {
            return Task.Run(() =>
            {
                while (this.i.State.IsRunning)
                {
                    this.i.InputController.TapKey(Keys.D4);
                    this.i.InputController.TapKey(Keys.D5);
                    Thread.Sleep(500);

                    if (this.i.Data.PlayerHasTarget && !this.i.Data.IsTargetDead)
                    {
                        this.i.CombatController.Fight();
                    }
                }
            });
        }
    }
}
