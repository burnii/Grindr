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
        public InputController InputController { get; set; }

        public CombatController(InputController inputController = null)
        {
            if (inputController != null)
            {
                this.InputController = inputController;
            }
            else
            {
                this.InputController = new InputController(Initializer.WindowHandle.Value);
            }
        }

        public Coordinate TryToFightEnemy()
        {
            Coordinate coordinates = null;

            this.TryToFindTarget();
            Thread.Sleep(100);
            if (Data.PlayerHasTarget == true /*&& Data.TargetIsInInteractRange == true*/)
            {
              
                Logger.AddLogEntry($"Found enemy, stop walking");
                this.InputController.ReleaseKey(Keys.W);
                this.InputController.ReleaseKey(Keys.W);
                this.InputController.ReleaseKey(Keys.W);
                coordinates = Data.PlayerCoordinate; 
                this.Fight();
                TryToLootEnemy();
             
            }

            return coordinates;
        }

        public void FightWhileInCombat()
        {
            Logger.AddLogEntry($"Start fighting until out of combat");
            
            while (Data.PlayerIsInCombat)
            {
                WowActions.CloseMap();
                this.SearchForAttackingTarget();
                this.Fight();
                TryToLootEnemy();
                Thread.Sleep(1000);
            }
            Logger.AddLogEntry($"Out of combat");
            TryToLootEnemy();
            WowActions.OpenMap();
        }

        private void TryToLootEnemy()
        {
            Logger.AddLogEntry($"Looting ...");
            this.InputController.TapKey(Keys.D6);
            this.InputController.TapKey(Keys.D6);
            this.InputController.TapKey(Keys.D6);
            Thread.Sleep(500);
            if (Data.IsTargetDead == true)
            {
                this.InputController.TapKey(Keys.Y);
                this.InputController.TapKey(Keys.Y);
                this.InputController.TapKey(Keys.Y);
            }
            
            Thread.Sleep(1000);
        }

        public void TryToFindTarget()
        {
            this.InputController.TapKey(Keys.Tab);
        }

        public void SearchForAttackingTarget()
        {
            while (Data.PlayerIsInCombat)
            {
                this.TryToFindTarget();

                if (Data.PlayerHasTarget && Data.IsTargetAttackingPlayer)
                {
                    break;
                }

                Thread.Sleep(1000);
            }
        }

        public void Fight()
        {
            Logger.AddLogEntry($"Fight current target");

            while (Data.PlayerHasTarget == true && !Data.IsTargetDead && Data.IsTargetAttackingPlayer)
            {
                this.InputController.TapKey(Keys.Y);
                this.InputController.TapKey(Keys.D3);
                this.InputController.TapKey(Keys.D2);
                this.InputController.TapKey(Keys.D1);
                Thread.Sleep(1000);
            }

            Logger.AddLogEntry($"Killed target");
        }

    }
}
