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
    public class WalkingController : IWalkingController
    {
        public BotInstance i { get; set; }

        public WalkingController(BotInstance instance)
        {
            this.i = instance;
        }

        private void Turn(Coordinate target)
        {
            var direction1 = CalculationHelper.CalculateWowDirection(this.i.Data.PlayerXCoordinate, this.i.Data.PlayerYCoordinate, target.X, target.Y);
            var diff = this.DetermineShortestTurnAngle(direction1, out Keys bestTurnKey);

            
            var turnTime = Convert.ToInt32(Math.Abs(diff) / 0.0030);

            this.i.InputController.PressKey(bestTurnKey);
            var start = DateTime.Now;
            var stop = false;

            Task.Run(() =>
            {
                Thread.Sleep(turnTime);
                stop = true;
            });

            while (Math.Abs(direction1 - this.i.Data.PlayerFacing) > 0.02 && /*(DateTime.Now - start).TotalMilliseconds < turnTime && */this.i.State.IsRunning && stop == false)
            {

            }
            Console.WriteLine(stop);
            this.i.Logger.AddLogEntry($"Start turning to {this.i.Logger.GetLogMessageForCoordinate(target)}");
            //Thread.Sleep(turnTime);
            this.i.Logger.AddLogEntry($"Turned to {this.i.Logger.GetLogMessageForCoordinate(target)}");

            this.i.InputController.ReleaseKey(bestTurnKey);
        }

        private double DetermineShortestTurnAngle(double targetDirection, out Keys bestTurnKey)
        {
            var diff = targetDirection - this.i.Data.PlayerFacing;

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
            this.i.WowActions.MountUpIfNeeded();
            this.i.InputController.PressKey(Keys.W);
            this.i.InputController.PressKey(Keys.W);
            var targetDistance = CalculationHelper.CalculateDistance(this.i.Data.PlayerCoordinate, target);

            var startCoordinate = this.i.Data.PlayerCoordinate;
            var distanceToStart = CalculationHelper.CalculateDistance(this.i.Data.PlayerCoordinate, startCoordinate);

            var distanceDelta = distanceToStart - targetDistance;
            this.i.Logger.AddLogEntry($"Start moving to {this.i.Logger.GetLogMessageForCoordinate(target)}");
            do
            {
                this.i.InputController.PressKey(Keys.W);

                if (this.i.State.IsRunning == false)
                {
                    break;
                }

                if (isGrinding)
                {
                    var coordinates = this.i.CombatController.TryToFightEnemy();
                    if (coordinates != null)
                    {
                        startCoordinate = coordinates;
                        targetDistance = CalculationHelper.CalculateDistance(this.i.Data.PlayerCoordinate, target);
                        this.Turn(target);
                        this.i.InputController.PressKey(Keys.W);
                        this.i.InputController.PressKey(Keys.W);
                        this.i.InputController.PressKey(Keys.W);
                    }
                }

                distanceToStart = CalculationHelper.CalculateDistance(this.i.Data.PlayerCoordinate, startCoordinate);
                distanceDelta = Math.Abs(distanceToStart - targetDistance);
                Thread.Sleep(100);
            }
            while (targetDistance > distanceToStart - 0.001);
            this.i.Logger.AddLogEntry($"Moved to {this.i.Logger.GetLogMessageForCoordinate(target)}");

            this.i.InputController.ReleaseKey(Keys.W);
            this.i.InputController.ReleaseKey(Keys.W);
            this.i.InputController.ReleaseKey(Keys.W);
        }

        public void Walk(Coordinate target, bool isGrinding)
        {
            this.i.Logger.AddLogEntry($"Walk to next waypoint at {this.i.Logger.GetLogMessageForCoordinate(target)} from {this.i.Logger.GetLogMessageForCoordinate(target)}");
            if (this.i.Data.PlayerZone == "ELW")
            { 
            }
            this.Turn(target);
            this.Move(target, isGrinding);
        }

        public void WalkUnitilZoneChange()
        {
            this.i.Logger.AddLogEntry($"Walk until zone change..");
            var startZone = this.i.Data.PlayerZone;
            this.i.InputController.PressKey(Keys.W);

            while (startZone == this.i.Data.PlayerZone)
            {
                if (this.i.State.IsRunning == false)
                {
                    this.i.InputController.ReleaseKey(Keys.W);
                    return;
                }
            }

            Thread.Sleep(200);
            this.i.InputController.ReleaseKey(Keys.W);

            this.i.Logger.AddLogEntry($"Arrived at zone '{this.i.Data.PlayerZone}'");

        }
    }
}
