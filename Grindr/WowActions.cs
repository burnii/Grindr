using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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
                input.LeftMouseClick(474, 451);
                Thread.Sleep(1000);
                input.LeftMouseClick(199, 163);
                Thread.Sleep(1000);
                input.LeftMouseClick(29, 225);
                Thread.Sleep(1000);
                input.LeftMouseClick(294, 310);
                Thread.Sleep(12000);
            }

            while (Data.IsPlayerDead == true)
            {
                inputController.TapKey(System.Windows.Forms.Keys.D7);
                Thread.Sleep(1000);
            }
        }

        public static Task OpenMapAsync()
        {
            return Task.Run(() =>
            {
                while (Data.IsMapOpened == false)
                {
                    inputController.TapKey(Keys.M);
                    Thread.Sleep(1000);
                }
            });

        }

        public static void OpenMap()
        {
            while (Data.IsMapOpened == false)
            {
                inputController.TapKey(Keys.M);
                Thread.Sleep(200);
            }
        }

        public static void CloseMap()
        {
            while (Data.IsMapOpened == true)
            {
                inputController.TapKey(Keys.M);
                Thread.Sleep(200);
            }
        }

        public static void SellItemsIfNeeded()
        {
            if (Data.IsOutDoors && Data.FreeBagSlots < 30)
            {
                Thread.Sleep(5000);

                while (Data.IsOutDoors && Data.FreeBagSlots < 30)
                {
                    while (!Data.IsMounted)
                    {
                        inputController.TapKey(Keys.D8);
                        Thread.Sleep(5000);
                    }

                    while (Data.IsMounted && Data.FreeBagSlots < 90)
                    {
                        inputController.TapKey(Keys.D9);
                        Thread.Sleep(1000);
                        inputController.TapKey(Keys.Y);
                        Thread.Sleep(1000);
                        inputController.TapKey(Keys.D0);
                    }

                }
            }
        }

        public static void TryToLootWithMouseClick()
        {
            WowActions.CloseMap();
            for (int i = 0; i < 2; i++)
            {
                inputController.RightMouseClick(202, 258);
                inputController.RightMouseClick(190, 242);
                inputController.RightMouseClick(251, 253);
                Thread.Sleep(1000);
            }
            WowActions.OpenMap();
        }

        public static void ResetInstances()
        {
            inputController.TapKey(Keys.OemMinus);
        }

        public static void MountUpIfNeeded()
        {
            while (!Data.IsMounted && Data.IsOutDoors)
            {
                inputController.TapKey(Keys.U);
                Thread.Sleep(5000);
            }
        }
    }
}
