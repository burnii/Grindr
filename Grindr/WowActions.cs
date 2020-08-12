using Grindr.Enums;
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
            while (this.i.Data.IsPlayerDead == false && this.i.State.IsRunning)
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

            while (this.i.Data.IsPlayerDead == true && this.i.State.IsRunning)
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
                Thread.Sleep(400);
            }
        }

        public void SellItemsIfNeeded()
        {
            if (this.i.Data.IsOutDoors && this.i.Data.FreeBagSlots < 30 && !this.i.Data.PlayerIsInCombat)
            {
                this.CloseMap();

                while (this.i.Data.IsOutDoors && this.i.Data.FreeBagSlots < 30 && this.i.State.IsRunning)
                {
                    while (!this.i.Data.IsMounted && this.i.State.IsRunning && this.i.State.IsRunning)
                    {
                        this.i.InputController.TapKey(Keys.D8);
                        Thread.Sleep(5000);
                    }

                    while (this.i.Data.IsMounted && this.i.Data.FreeBagSlots < 90 && this.i.State.IsRunning)
                    {
                        this.i.InputController.TapKey(Keys.D9);
                        Thread.Sleep(1000);
                        this.i.InputController.TapKey(Keys.Y);
                        Thread.Sleep(1000);
                        this.i.InputController.TapKey(Keys.D0);
                    }

                }

                this.OpenMap();
            }
        }

        public void Shapeshift(DruidShapeshiftForm druidShapeshiftForm)
        {
            Keys hotkey = Keys.None;

            switch (druidShapeshiftForm)
            {
                case DruidShapeshiftForm.Bear:
                    hotkey = Keys.F1;
                    break;
                case DruidShapeshiftForm.Travel:
                    hotkey = Keys.U;
                    break;
            }

            while (this.i.State.IsRunning && this.i.Data.DruidShapeshiftForm != druidShapeshiftForm)
            {
                this.i.InputController.TapKey(hotkey);
                Thread.Sleep(700);
            }
        }

        public void TryToLootWithMouseClick()
        {
            this.CloseMap();
            this.i.InputController.TapKey(Keys.F8);
            for (int i = 0; i < 1; i++)
            {
                Thread.Sleep(200);
                this.i.InputController.RightMouseClick(202, 258);
                Thread.Sleep(100);
                this.i.InputController.RightMouseClick(190, 242);
                Thread.Sleep(100);
                this.i.InputController.RightMouseClick(251, 253);
                Thread.Sleep(100);
                this.i.InputController.RightMouseClick(235, 225);
                Thread.Sleep(2000);
            }
            this.i.InputController.TapKey(Keys.F8);
            this.OpenMap();
        }

        public void WaitForZoneChange()
        {
            var startZone = this.i.Data.PlayerZone;

            while (startZone == this.i.Data.PlayerZone)
            {
                Thread.Sleep(1000);
            }
        }

        public void ResetInstances()
        {
            this.i.InputController.TapKey(Keys.OemMinus);
        }

        public void FastExitDungeon()
        {
            this.CloseMap();
            while (!this.i.Data.PlayerIsInGroup && this.i.State.IsRunning)
            {
                this.i.InputController.TapKey(Keys.I);
                Thread.Sleep(100);
                this.i.InputController.LeftMouseClick(58, 264);
                Thread.Sleep(100);
                this.i.InputController.LeftMouseClick(185, 157);
                Thread.Sleep(100);
                this.i.InputController.LeftMouseClick(167, 319);
                Thread.Sleep(100);
                this.i.InputController.TapKey(Keys.I);
                this.i.InputController.TapKey(Keys.J);
                this.i.InputController.TapKey(Keys.K);
                Thread.Sleep(100);
                this.i.InputController.LeftMouseClick(164, 300);
                Thread.Sleep(100);
                this.i.InputController.LeftMouseClick(298, 319);
                Thread.Sleep(1000);
            }
            
            this.i.InputController.TapKey(Keys.D5);
            this.OpenMap();
        }

        public void MountUpIfNeeded()
        {
            if (!this.i.Profile.Settings.ShouldUseTravelForm)
            {
                while (!this.i.Data.IsMounted && this.i.Data.IsOutDoors && this.i.Profile.Settings.ShouldUseMount && this.i.State.IsRunning)
                {
                    this.i.InputController.TapKey(Keys.U);
                    Thread.Sleep(5000);
                }
            }
            else
            {
                this.i.WowActions.Shapeshift(DruidShapeshiftForm.Travel);
            }
        }
    }
}
