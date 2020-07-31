using System;
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
                while (State.IsRecording)
                {
                    var currentDistance = CalculationHelper.CalculateDistance(Data.PlayerCoordinate, startCoordinate);

                    if (currentDistance > 0.2)
                    {
                        startCoordinate = Data.PlayerCoordinate;
                        Application.Current.Dispatcher.BeginInvoke(
                            new Action(() => 
                            { 
                                this.Grinder.AddNavigationNode(startCoordinate, NavigationNodeType.WayPoint); 
                            })
                        );
                    }
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