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
    public class WowActions
    {
        public BotInstance i { get; set; }

        public WowActions(BotInstance instance)
        {
            this.i = instance;
        }

        public void Unstuck()
        {
            while (this.i.Data.IsPlayerDead == false)
            {
                this.i.Logger.AddLogEntry("Unstuck player to return to dungeon entrance");
                this.i.InputController.LeftMouseClick(474, 451);
                Thread.Sleep(1000);
                this.i.InputController.LeftMouseClick(199, 163);
                Thread.Sleep(1000);
                this.i.InputController.LeftMouseClick(29, 225);
                Thread.Sleep(1000);
                this.i.InputController.LeftMouseClick(294, 310);
                Thread.Sleep(12000);
            }

            while (this.i.Data.IsPlayerDead == true)
            {
                this.i.InputController.TapKey(Keys.D7);
                Thread.Sleep(1000);
            }
        }

        public Task OpenMapAsync()
        {
            return Task.Run(() =>
            {
                while (this.i.Data.IsMapOpened == false)
                {
                    this.i.InputController.TapKey(Keys.M);
                    Thread.Sleep(1000);
                }
            });

        }

        public void OpenMap()
        {
            while (this.i.Data.IsMapOpened == false)
            {
                this.i.InputController.TapKey(Keys.M);
                Thread.Sleep(200);
            }
        }

        public void CloseMap()
        {
            while (this.i.Data.IsMapOpened == true)
            {
                this.i.InputController.TapKey(Keys.M);
                Thread.Sleep(200);
            }
        }

        public void SellItemsIfNeeded()
        {
            if (this.i.Data.IsOutDoors && this.i.Data.FreeBagSlots < 30)
            {
                Thread.Sleep(5000);

                while (this.i.Data.IsOutDoors && this.i.Data.FreeBagSlots < 30)
                {
                    while (!this.i.Data.IsMounted)
                    {
                        this.i.InputController.TapKey(Keys.D8);
                        Thread.Sleep(5000);
                    }

                    while (this.i.Data.IsMounted && this.i.Data.FreeBagSlots < 90)
                    {
                        this.i.InputController.TapKey(Keys.D9);
                        Thread.Sleep(1000);
                        this.i.InputController.TapKey(Keys.Y);
                        Thread.Sleep(1000);
                        this.i.InputController.TapKey(Keys.D0);
                    }

                }
            }
        }

        public void TryToLootWithMouseClick()
        {
            this.CloseMap();
            for (int i = 0; i < 2; i++)
            {
                this.i.InputController.RightMouseClick(202, 258);
                this.i.InputController.RightMouseClick(190, 242);
                this.i.InputController.RightMouseClick(251, 253);
                Thread.Sleep(1000);
            }
            this.OpenMap();
        }

        public void ResetInstances()
        {
            this.i.InputController.TapKey(Keys.OemMinus);
        }

        public void MountUpIfNeeded()
        {
            while (!this.i.Data.IsMounted && this.i.Data.IsOutDoors && this.i.Profile.Settings.ShouldUseMount)
            {
                this.i.InputController.TapKey(Keys.U);
                Thread.Sleep(5000);
            }
        }
    }
}
