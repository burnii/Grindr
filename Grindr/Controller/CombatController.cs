using QuickGraph;
using System;
using System.Collections.Generic;
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
            Thread.Sleep(100);
            if (this.i.Data.PlayerHasTarget == true /*&& Data.TargetIsInInteractRange == true*/)
            {

                this.i.Logger.AddLogEntry($"Found enemy, stop walking");
                this.i.InputController.ReleaseKey(Keys.W);
                this.i.InputController.ReleaseKey(Keys.W);
                this.i.InputController.ReleaseKey(Keys.W);
                coordinates = this.i.Data.PlayerCoordinate;
                this.Fight();
                TryToLootEnemy();

            }

            return coordinates;
        }

        public void FightWhileInCombat(bool turret = false)
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
            //TryToLootEnemy();
            this.i.WowActions.OpenMap();
        }

        private void TryToLootEnemy()
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

            Thread.Sleep(1000);
        }

        public void TryToFindTarget()
        {
            this.i.InputController.TapKey(Keys.Tab);
        }

        public void SearchForAttackingTarget()
        {
            while (this.i.Data.PlayerIsInCombat && this.i.State.IsRunning)
            {
                this.TryToFindTarget();

                if (this.i.Data.PlayerHasTarget && this.i.Data.IsTargetAttackingPlayer && !this.i.Data.IsTargetDead)
                {
                    break;
                }

                Thread.Sleep(1000);
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
                    if (!assist && !this.i.Data.IsTargetAttackingPlayer)
                    {
                        break;
                    }

                    var current = DateTime.Now;

                    if (!turret || (current - start).TotalMilliseconds > 20000)
                    {
                        this.i.InputController.TapKey(Keys.Y);
                    }

                    Thread.Sleep(2000);
                }
            });

           
            while (this.i.Data.PlayerHasTarget == true && !this.i.Data.IsTargetDead && this.i.State.IsRunning)
            {
                if (!assist && !this.i.Data.IsTargetAttackingPlayer)
                {
                    break;
                }

                //this.i.InputController.TapKey(Keys.D3);
                this.i.InputController.TapKey(Keys.D2);
                this.i.InputController.TapKey(Keys.D1);
                //this.i.InputController.TapKey(Keys.D4);
                //this.i.InputController.TapKey(Keys.D5);
                Thread.Sleep(100);
            }

            this.i.Logger.AddLogEntry($"Killed target");
        }

    }
}
