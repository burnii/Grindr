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
    public class InstanceWalkingController : IWalkingController

    {
        public InputController InputController { get; set; }
        public CombatController CombatController { get; set; }

        public InstanceWalkingController()
        {
            this.InputController = new InputController(Initializer.WindowHandle.Value);
            this.CombatController = new CombatController(this.InputController);
        }

        private void Turn(Coordinate target)
        {
            var direction1 = CalculationHelper.GetAngle(Data.PlayerCoordinate.X, Data.PlayerCoordinate.Y - 5, target.X, target.Y, Data.PlayerCoordinate.X, Data.PlayerCoordinate.Y);
            var diff = this.DetermineShortestTurnAngle(direction1, out Keys bestTurnKey);

            var turnTime = Convert.ToInt32(Math.Abs(diff) / 0.003141);

            this.InputController.PressKey(bestTurnKey);
            Logger.AddLogEntry($"Start turning to {Logger.GetLogMessageForCoordinate(target)}");
            Thread.Sleep(turnTime);
            this.InputController.ReleaseKey(bestTurnKey);
            Logger.AddLogEntry($"Turned to {Logger.GetLogMessageForCoordinate(target)}");
        }

        private double DetermineShortestTurnAngle(double targetDirection, out Keys bestTurnKey)
        {
            var diff = targetDirection - Data.PlayerFacing;

            if (diff < 0)
            {
                diff += Math.PI * 2;
            }

            if (diff > Math.PI)
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

                distanceToStart = CalculationHelper.CalculateDistance(Data.PlayerCoordinate, startCoordinate);
                distanceDelta = Math.Abs(distanceToStart - targetDistance);

                Thread.Sleep(100);
            }
            while (targetDistance > distanceToStart);
            Console.WriteLine(targetDistance);
            Console.WriteLine(distanceToStart);
            Logger.AddLogEntry($"Moved to {Logger.GetLogMessageForCoordinate(target)}");

            this.InputController.ReleaseKey(Keys.W);
            this.InputController.ReleaseKey(Keys.W);
            this.InputController.ReleaseKey(Keys.W);
        }

        public void Walk(Coordinate target, bool isGrinding)
        {
            Logger.AddLogEntry($"Walk to next waypoint at {Logger.GetLogMessageForCoordinate(target)} from {Logger.GetLogMessageForCoordinate(target)}");

            while (Data.IsMapOpened == false || Data.PlayerXCoordinate == int.MaxValue || Data.PlayerYCoordinate == int.MaxValue)
            {

            }

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

            Thread.Sleep(200);
            this.InputController.ReleaseKey(Keys.W);

            Logger.AddLogEntry($"Arrived at zone '{Data.PlayerZone}'");

        }
    }
}
