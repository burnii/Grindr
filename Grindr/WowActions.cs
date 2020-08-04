using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Grindr
{
    public static class WowActions
    {
        private static InputController inputController = new InputController(Initializer.WindowHandle.Value);

        public static void Unstuck()
        {
            while (Data.IsPlayerDead == false)
            {
                Logger.AddLogEntry("Unstuck player to return to dungeon entrance");
                var input = new InputController(Initializer.WindowHandle.Value);
                input.MouseClick(474, 451);
                Thread.Sleep(1000);
                input.MouseClick(199, 163);
                Thread.Sleep(1000);
                input.MouseClick(29, 225);
                Thread.Sleep(1000);
                input.MouseClick(294, 310);
                Thread.Sleep(12000);
            }

            while (Data.IsPlayerDead == true)
            {
                inputController.TapKey(System.Windows.Forms.Keys.D7);
                Thread.Sleep(1000);
            }
        }
    }
}
