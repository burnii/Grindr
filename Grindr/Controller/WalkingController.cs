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
using System.Windows.Media.Media3D;

namespace Grindr
{
    public class WalkingController
    {
        public InputController InputController { get; set; }
        public CombatController CombatController { get; set; }

        public WalkingController()
        {
            this.InputController = new InputController(Initializer.WindowHandle.Value);
            this.CombatController = new CombatController(this.InputController);
        }

        private void Turn(Coordinate target)
        {
            var direction1 = CalculationHelper.CalculateWowDirection(Data.PlayerXCoordinate, Data.PlayerYCoordinate, target.X, target.Y);
            var diff = this.DetermineShortestTurnAngle(direction1, out Keys bestTurnKey);
            var turnTime = Convert.ToInt32(Math.Round(Math.Abs(diff) / 0.0033));

            this.InputController.PressKey(bestTurnKey);

            Logger.AddLogEntry($"Start turning to {Logger.GetLogMessageForCoordinate(target)}");
            Thread.Sleep(turnTime);
            Logger.AddLogEntry($"Turned to {Logger.GetLogMessageForCoordinate(target)}");

            this.InputController.ReleaseKey(bestTurnKey);
        }

        private double DetermineShortestTurnAngle(double targetDirection, out Keys bestTurnKey)
        {
            var diff = targetDirection - Data.PlayerFacing;

            if (diff < 0)
            {
                diff += Math.PI * 2;
            }

            if (diff < Math.PI)
                bestTurnKey = Keys.A;
            else
                bestTurnKey = Keys.D;

            return Math.Min(Math.PI * 2 - Math.Abs(diff), Math.Abs(diff));
        }

        private void Move(Coordinate target, bool isGrinding)
        {
            this.InputController.PressKey(Keys.W);
            this.InputController.PressKey(Keys.W);
            this.InputController.PressKey(Keys.W);
            var targetDistance = CalculationHelper.CalculateDistance(Data.PlayerCoordinate, target);

            var startCoordinate = Data.PlayerCoordinate;
            var distanceToStart = CalculationHelper.CalculateDistance(Data.PlayerCoordinate, startCoordinate);

            var distanceDelta = distanceToStart - targetDistance;
            Logger.AddLogEntry($"Start moving to {Logger.GetLogMessageForCoordinate(target)}");
            var currentLocation = Data.PlayerZone;
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
                        this.InputController.PressKey(Keys.W);
                        this.InputController.PressKey(Keys.W);
                    }
                }

                //if (Data.PlayerLocation != currentLocation)
                //{
                //    startCoordinate = Data.PlayerCoordinate;
                //    targetDistance = CalculationHelper.CalculateDistance(Data.PlayerCoordinate, target);
                //}

                distanceToStart = CalculationHelper.CalculateDistance(Data.PlayerCoordinate, startCoordinate);
                distanceDelta = Math.Abs(distanceToStart - targetDistance);
            }
            while (/*distanceDelta > 0.01 &&*/ targetDistance > distanceToStart);
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

        public void WalkUnitilZoneChange()
        {
            Logger.AddLogEntry($"Walk until zone change..");
            var startZone = Data.PlayerZone;
            this.InputController.PressKey(Keys.W);

            while (startZone == Data.PlayerZone)
            {
                if (State.IsRunning == false)
                {
                    this.InputController.ReleaseKey(Keys.W);
                    return;
                }
            }

            this.InputController.ReleaseKey(Keys.W);

            Logger.AddLogEntry($"Arrived at zone '{Data.PlayerZone}'");

        }
    }
}
