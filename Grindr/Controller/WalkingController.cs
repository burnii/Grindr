using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Notepad
{
    public class WalkingController
    {
        public InputController InputController { get; set; }
        public CombatController CombatController { get; set; }

        public WalkingController()
        {
            this.InputController = new InputController(Initializer.WindowHandle);
            this.CombatController = new CombatController(this.InputController);
        }

        private void Turn(Coordinate target)
        {
            var direction1 = CalculationHelper.CalculateWowDirection(Data.PlayerXCoordinate, Data.PlayerYCoordinate, target.X, target.Y);
            var deg = CalculationHelper.RadToDeg(direction1 - Data.PlayerFacing);
            var deg2 = CalculationHelper.RadToDeg(Data.PlayerFacing - direction1);

            var bestTurnKey = Keys.A;

            if (deg2 > deg)
            {
                bestTurnKey = Keys.D;
            }
            this.InputController.PressKey(bestTurnKey);

            Logger.AddLogEntry($"Start turning to {Logger.GetLogMessageForCoordinate(target)}");
            while (Math.Abs(Math.Abs(direction1) - Math.Abs(Data.PlayerFacing)) > 0.1)
            {
                if (State.IsRunning == false)
                {
                    break;
                }
            }
            Logger.AddLogEntry($"Turned to {Logger.GetLogMessageForCoordinate(target)}");

            this.InputController.ReleaseKey(bestTurnKey);
        }

        private void Move(Coordinate target, bool isGrinding)
        {
            this.InputController.PressKey(Keys.W);
            var targetDistance = CalculationHelper.CalculateDistance(Data.PlayerCoordinate, target);

            var startCoordinate = Data.PlayerCoordinate;
            var distanceToStart = CalculationHelper.CalculateDistance(Data.PlayerCoordinate, startCoordinate);

            var distanceDelta = distanceToStart - targetDistance;
            Logger.AddLogEntry($"Start moving to {Logger.GetLogMessageForCoordinate(target)}");
            do
            {
                if (State.IsRunning == false)
                {
                    break;
                }

                if (isGrinding)
                {
                    var coordinates = this.CombatController.TryToFightEnemy();
                    if (coordinates != null)
                    {
                        startCoordinate = coordinates;
                        targetDistance = CalculationHelper.CalculateDistance(Data.PlayerCoordinate, target);
                        this.Turn(target);
                        this.InputController.PressKey(Keys.W);
                    }
                }

                distanceToStart = CalculationHelper.CalculateDistance(Data.PlayerCoordinate, startCoordinate);
                distanceDelta = Math.Abs(distanceToStart - targetDistance);
            }
            while (distanceDelta > 0.01);
            Logger.AddLogEntry($"Moved to {Logger.GetLogMessageForCoordinate(target)}");

            this.InputController.ReleaseKey(Keys.W);
            this.InputController.ReleaseKey(Keys.W);
            this.InputController.ReleaseKey(Keys.W);
        }

        public void Walk(Coordinate target, bool isGrinding)
        {
            Logger.AddLogEntry($"Walk to next waypoint at {Logger.GetLogMessageForCoordinate(target)}");
            this.Turn(target);
            this.Move(target, isGrinding);
        }
    }
}
