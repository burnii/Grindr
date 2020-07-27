using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Notepad
{
    public class Assister
    {
        public Task Assist(string name)
        {
            var inputController = new InputController(Initializer.WindowHandle);
            var combatController = new CombatController(inputController);
            

            return Task.Run(() =>
            {
                while (State.IsRunning)
                {
                    inputController.TapKey(Keys.D4);
                    inputController.TapKey(Keys.D5);
                    Thread.Sleep(500);

                    if (Data.PlayerHasTarget && !Data.IsTargetDead)
                    {
                        combatController.Fight();
                    }
                }
            });
        }
    }
}
