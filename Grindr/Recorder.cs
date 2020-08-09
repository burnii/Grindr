using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Grindr
{
    public class Recorder
    {
        public BotInstance i { get; set; }

        public Recorder(BotInstance instance)
        {
            this.i = instance;
        }

        public void StartRecording()
        {
            this.i.State.IsRecording = true;

            Task.Run(() =>
            {
                this.i.Logger.AddLogEntry("Start recording navigation nodes");
                var startCoordinate = this.i.Data.PlayerCoordinate;
                var startZone = this.i.Data.PlayerZone;
                while (this.i.State.IsRecording)
                {
                    var currentDistance = CalculationHelper.CalculateDistance(this.i.Data.PlayerCoordinate, startCoordinate);

                    if (startZone != this.i.Data.PlayerZone)
                    {
                        this.i.Grinder.MarkLastNavigationNodeAsZoneChangeNode(startZone);
                        startZone = this.i.Data.PlayerZone;
                    }

                    if (currentDistance > 0.01 && startZone == this.i.Data.PlayerZone)
                    {
                        startCoordinate = this.i.Data.PlayerCoordinate;
                        AddNavigationNode(startCoordinate);
                    }

                    Thread.Sleep(100);
                }
            });
        }

        public void AddNavigationNode(Coordinate coordinate)
        {
            Application.Current.Dispatcher.BeginInvoke(
                                new Action(() =>
                                {
                                    this.i.Grinder.AddNavigationNode(coordinate, NavigationNodeType.WayPoint, this.i.Data.PlayerZone);
                                })
                            );
        }

        public void StopRecording()
        {
            this.i.Logger.AddLogEntry("Stopped recording navigation nodes");
            this.i.State.IsRecording = false;
        }
    }
}