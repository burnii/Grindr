using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Grindr
{
    internal class Recorder
    {
        public Grinder Grinder { get; set; }

        public Recorder(Grinder grinder)
        {
            this.Grinder = grinder;
        }

        public void StartRecording()
        {
            State.IsRecording = true;

            Task.Run(() =>
            {
                Logger.AddLogEntry("Start recording navigation nodes");
                var startCoordinate = Data.PlayerCoordinate;
                var startZone = Data.PlayerZone;
                while (State.IsRecording)
                {
                    var currentDistance = CalculationHelper.CalculateDistance(Data.PlayerCoordinate, startCoordinate);

                    if (startZone != Data.PlayerZone)
                    {
                        this.Grinder.MarkLastNavigationNodeAsZoneChangeNode(startZone);
                        startZone = Data.PlayerZone;
                    }

                    if (currentDistance > 0.1)
                    {
                        startCoordinate = Data.PlayerCoordinate;
                        Application.Current.Dispatcher.BeginInvoke(
                            new Action(() => 
                            { 
                                this.Grinder.AddNavigationNode(startCoordinate, NavigationNodeType.WayPoint, Data.PlayerZone); 
                            })
                        );
                    }

                    Thread.Sleep(100);
                }
            });
        }

        public void StopRecording()
        {
            Logger.AddLogEntry("Stopped recording navigation nodes");
            State.IsRecording = false;
        }
    }
}