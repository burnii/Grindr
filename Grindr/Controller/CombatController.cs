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

        public void FightWhileInCombat()
        {
            this.i.Logger.AddLogEntry($"Start fighting until out of combat");
            
            while (this.i.Data.PlayerIsInCombat)
            {
                this.i.WowActions.CloseMap();
                this.SearchForAttackingTarget();
                this.Fight();
                TryToLootEnemy();
                Thread.Sleep(1000);
            }
            this.i.Logger.AddLogEntry($"Out of combat");
            TryToLootEnemy();
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
                this.i.InputController.TapKey(Keys.Y);
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
            while (this.i.Data.PlayerIsInCombat)
            {
                this.TryToFindTarget();

                if (this.i.Data.PlayerHasTarget && this.i.Data.IsTargetAttackingPlayer)
                {
                    break;
                }

                Thread.Sleep(1000);
            }
        }

        public void Fight()
        {
            this.i.Logger.AddLogEntry($"Fight current target");

            while (this.i.Data.PlayerHasTarget == true && !this.i.Data.IsTargetDead && this.i.Data.IsTargetAttackingPlayer)
            {
                this.i.InputController.TapKey(Keys.Y);
                this.i.InputController.TapKey(Keys.D3);
                this.i.InputController.TapKey(Keys.D2);
                this.i.InputController.TapKey(Keys.D1);
                Thread.Sleep(1000);
            }

            this.i.Logger.AddLogEntry($"Killed target");
        }

    }
}
