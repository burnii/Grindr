using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Grindr
{
    internal class Recorder
    {
        public static Grinder Grinder { get; set; }

        public Recorder(Grinder grinder)
        {
            Grinder = grinder;
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
                        Grinder.MarkLastNavigationNodeAsZoneChangeNode(startZone);
                        startZone = Data.PlayerZone;
                    }

                    if (currentDistance > 0.01 && startZone == Data.PlayerZone)
                    {
                        startCoordinate = Data.PlayerCoordinate;
                        AddNavigationNode(startCoordinate);
                    }

                    Thread.Sleep(100);
                }
            });
        }

        public static void AddNavigationNode(Coordinate coordinate)
        {
            Application.Current.Dispatcher.BeginInvoke(
                                new Action(() =>
                                {
                                    Grinder.AddNavigationNode(coordinate, NavigationNodeType.WayPoint, Data.PlayerZone);
                                })
                            );
        }

        public void StopRecording()
        {
            Logger.AddLogEntry("Stopped recording navigation nodes");
            State.IsRecording = false;
        }
    }
}