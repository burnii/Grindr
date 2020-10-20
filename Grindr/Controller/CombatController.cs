using QuickGraph;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Grindr
{
    public class CombatController
    {
        private BotInstance i;

        public CombatController(BotInstance instance)
        {
            i = instance;
        }

        public Coordinate TryToFightEnemy()
        {
            Coordinate coordinates = null;

            this.TryToFindTarget();
            Thread.Sleep(1000);
            if (this.i.Data.PlayerHasTarget == true)
            {
                this.i.Logger.AddLogEntry($"Found enemy, stop walking");
                this.i.InputController.ReleaseKey(Keys.W);
                this.i.InputController.ReleaseKey(Keys.W);
                this.i.InputController.ReleaseKey(Keys.W);
                coordinates = this.i.Data.PlayerCoordinate;
                this.Fight();
                //TryToLootEnemy();

            }


            return coordinates;
        }

        public void FightWhileInCombat(bool turret = false)
        {
            if (this.i.Data.PlayerIsInCombat)
            {
                this.i.Logger.AddLogEntry($"Start fighting until out of combat");

                while (this.i.Data.PlayerIsInCombat && this.i.State.IsRunning)
                {
                    this.i.WowActions.CloseMap();
                    this.SearchForAttackingTarget();
                    this.Fight(turret);
                    //TryToLootEnemy();
                    //Thread.Sleep(1000);
                }
                this.i.Logger.AddLogEntry($"Out of combat");
                TryToLootEnemy();
                //this.i.WowActions.OpenMap();
            }
            
        }

        private void TryToLootEnemy()
        {
            var startNumberOfItems = this.i.Data.FreeBagSlots;
            var trys = 0;
            var startGold = this.i.Data.Gold;

            while (this.i.State.IsRunning && this.i.Data.FreeBagSlots >= startNumberOfItems && trys < 2 && this.i.Data.Gold <= startGold)
            {
                this.i.Logger.AddLogEntry($"Looting ...");
                this.i.InputController.TapKey(Keys.D6);
                this.i.InputController.TapKey(Keys.D6);
                this.i.InputController.TapKey(Keys.D6);
                Thread.Sleep(500);
                if (this.i.Data.IsTargetDead == true)
                {
                    this.i.InputController.TapKey(Keys.Y);
                }
                trys++;
                Thread.Sleep(2000);
                this.i.InputController.TapKey(Keys.W);
                this.i.WowActions.TryToLootWithMouseClick();
            }
        }

        public void TryToFindTarget()
        {
            this.i.InputController.TapKey(Keys.Tab);
        }

        public void SearchForAttackingTarget()
        {
            var startTime = DateTime.Now;
            int i = 0;
            while (this.i.Data.PlayerIsInCombat && i < 8 && this.i.State.IsRunning)
            {
                this.TryToFindTarget();

                if ((DateTime.Now - startTime).Seconds > 3)
                {
                    this.i.InputController.PressKey(Keys.D);
                    Thread.Sleep(500);
                    this.i.InputController.ReleaseKey(Keys.D);
                    this.i.InputController.TapKey(Keys.D1);
                    startTime = DateTime.Now;
                    i++;   
                }

                if (this.i.Data.PlayerHasTarget && this.i.Data.IsTargetAttackingPlayer && !this.i.Data.IsTargetDead)
                {
                    break;
                }

                Thread.Sleep(1000);
            }
        }

        public void Fight()
        {
            while (this.i.Data.PlayerIsInCombat && this.i.State.IsRunning) 
            {
                Thread.Sleep(1000);
                if (this.i.Data.TargetIsInInteractRange && this.i.Data.IsTargetAttackingPlayer)
                {
                    this.i.InputController.TapKey(Keys.Y);
                }

                if (this.i.Profile.Settings.ShouldUseBearForm)
                {
                    this.i.WowActions.Shapeshift(Enums.DruidShapeshiftForm.Bear);
                }

                if (this.i.Data.PlayerHealth < 50)
                {
                    this.i.InputController.TapKey(Keys.D3);
                }

                this.i.InputController.TapKey(Keys.D2);
                this.i.InputController.TapKey(Keys.D1);
            }
        }
        public void Fight(bool turret = false, bool assist = false)
        {
            this.i.Logger.AddLogEntry($"Fight current target");
            if (this.i.Profile.Settings.ShouldUseBearForm)
            { 
                this.i.WowActions.Shapeshift(Enums.DruidShapeshiftForm.Bear);
            }

            var start = DateTime.Now;

            Task.Run(() =>
            {
                while (this.i.Data.PlayerHasTarget == true && !this.i.Data.IsTargetDead && this.i.State.IsRunning)
                {
                    if (assist)
                    {
                        break;
                    }
                    if (!assist && !this.i.Data.IsTargetAttackingPlayer)
                    {
                        break;
                    }

                    var current = DateTime.Now;

                    if (!turret || (current - start).TotalMilliseconds > 3000)
                    {
                        this.i.InputController.TapKey(Keys.Y);
                    }

                    Thread.Sleep(5000);
                }
            });

            var i = 0;
            var j = false;
            while (this.i.Data.PlayerHasTarget == true && !this.i.Data.IsTargetDead && this.i.State.IsRunning)
            {
                if (!assist && !this.i.Data.IsTargetAttackingPlayer)
                {
                    break;
                }
                if (assist)
                {
                    this.i.InputController.TapKey(Keys.F3);
                }

                if (this.i.Data.TargetIsInInteractRange)
                {
                    //this.i.InputController.TapKey(Keys.D3);
                    if (this.i.Data.PlayerHealth < 50)
                    {
                        //this.i.InputController.TapKey(Keys.D3);
                        this.i.InputController.TapKey(Keys.D4);
                    }

                    if (i == 20)
                    {
                        this.i.InputController.TapKey(Keys.D5);
                        Thread.Sleep(100);
                        this.i.InputController.TapKey(Keys.D6);
                        j = false;
                        i = 0;
                    }

                    if (j == false)
                    {
                        this.i.InputController.TapKey(Keys.D1);
                        j = true;
                    }
                    this.i.InputController.TapKey(Keys.D2);
                    //this.i.InputController.TapKey(Keys.D4);
                    //this.i.InputController.TapKey(Keys.D5);
                    i++;
                }
                
                Thread.Sleep(100);
            }

            this.i.Logger.AddLogEntry($"Killed target");
        }

    }
}
