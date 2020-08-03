using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;

namespace Grindr
{
    public class Grinder
    {
        public ObservableCollection<NavigationNode> NavigationNodes { get; set; }

        public ListBox NavigationCoordinatesListBox { get; set; }

        public InstanceWalkingController InstanceWalkingController { get; set; }
        public WalkingController WalkingController { get; set; }

        private CancellationTokenSource CancellationTokenSource { get; set; }

        private CombatController CombatController { get; set; }

        public Grinder(ListBox navigationCoordinatesListBox)
        {
            this.NavigationCoordinatesListBox = navigationCoordinatesListBox;
            this.NavigationNodes = new ObservableCollection<NavigationNode>();
            this.InstanceWalkingController = new InstanceWalkingController();
            this.WalkingController = new WalkingController();
            this.CombatController = new CombatController();
        }

        internal void MarkLastNavigationNodeAsZoneChangeNode(string playerZone)
        {
            var lastNodeFromZone = this.GetLastNavigationNodeFromZone(this.NavigationNodes.Last(), playerZone);
            this.MarkNavigationNode(lastNodeFromZone, NavigationNodeType.ZoneChange);
        }

        public void MarkNavigationNode(NavigationNode node, NavigationNodeType type)
        {
            node.Type = type;
            this.UpdateNavigationNodeColor(node);
        }

        private void UpdateNavigationNodeColor(NavigationNode node)
        {
            switch (node.Type)
            {
                case NavigationNodeType.Combat:
                    var i = this.NavigationNodes.IndexOf(node);
                    var item = NavigationCoordinatesListBox.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                    Application.Current.Dispatcher.Invoke(new Action(() => { item.Background = new BrushConverter().ConvertFrom("#FFB7B2") as Brush; }));
                    break;
                case NavigationNodeType.WayPoint:
                    i = this.NavigationNodes.IndexOf(node);
                    item = NavigationCoordinatesListBox.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                    if (item != null)
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() => { item.Background = null as Brush; }));
                    }
                    break;
                case NavigationNodeType.ZoneChange:
                    i = this.NavigationNodes.IndexOf(node);
                    item = NavigationCoordinatesListBox.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                    Application.Current.Dispatcher.Invoke(new Action(() => { item.Background = new BrushConverter().ConvertFrom("#E2F0CB") as Brush; }));
                    break;
                case NavigationNodeType.Dungeon:
                    i = this.NavigationNodes.IndexOf(node);
                    item = NavigationCoordinatesListBox.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                    Application.Current.Dispatcher.Invoke(new Action(() => { item.Background = new BrushConverter().ConvertFrom("#C7CEEA") as Brush; }));
                    break;
            }
        }

        private NavigationNode GetLastNavigationNodeFromZone(NavigationNode node, string zone)
        {
            if (node.Zone == zone)
            {
                return node;
            }
            else if (node.PreviousNode == null)
            {
                return null;
            }
            else
            {
                return this.GetLastNavigationNodeFromZone(node.PreviousNode, zone);
            }
        }

        public Task StartJourney()
        {
            this.CancellationTokenSource = new CancellationTokenSource();

            return Task.Run(() =>
            {
                Logger.AddLogEntry("Grinder started");

                //while (State.IsRunning)
                //{
                for (int i = 0; i < this.NavigationNodes.Count; i++)
                {
                    if (State.IsRunning == false)
                    {
                        break;
                    }

                    IWalkingController wc;

                    if (Data.IsInInstance)
                    {
                        wc = this.InstanceWalkingController;
                    }
                    else
                    {
                        wc = this.WalkingController;
                    }

                    Application.Current.Dispatcher.BeginInvoke(new Action(() => { this.NavigationCoordinatesListBox.SelectedIndex = i; }));

                    if (this.NavigationNodes[i].Type == NavigationNodeType.ZoneChange)
                    {
                        wc.Walk(this.NavigationNodes[i].Coordinates, false);
                        wc.WalkUnitilZoneChange();
                    }
                    else
                    {
                        wc.Walk(this.NavigationNodes[i].Coordinates, true);
                    }

                    if (this.NavigationNodes[i].Type == NavigationNodeType.Combat)
                    {
                        this.CombatController.FightWhileInCombat();
                    }
                }
                //}

                Logger.AddLogEntry("Grinder stopped");

            }, this.CancellationTokenSource.Token);
        }

        public NavigationNode AddNavigationNode(Coordinate coordinate, NavigationNodeType type, string zone)
        {
            var index = this.GetIndexToInsert();

            var node = new NavigationNode(
                coordinate.X,
                coordinate.Y,
                type,
                zone,
                this.TryToGetNodeAtIndex(index),
                this.TryToGetNodeAtIndex(index - 1)
            );


            Logger.AddLogEntry($"Recorded a navigationnode at {Logger.GetLogMessageForCoordinate(coordinate)}");

            this.NavigationNodes.Insert(index, node);

            this.UpdateNavigationNodeColor(node);

            return node;
        }

        public void DeleteNavigationNode(int i)
        {
            this.NavigationNodes.RemoveAt(i);
        }

        private int GetIndexToInsert()
        {
            if (this.NavigationCoordinatesListBox.SelectedItem == null)
            {
                return this.NavigationNodes.Count;
            }
            else
            {
                return this.NavigationCoordinatesListBox.SelectedIndex + 1;
            }
        }

        private NavigationNode TryToGetNodeAtIndex(int i)
        {
            if (NavigationNodes.Count >= (i + 1) && i >= 0)
            {
                return NavigationNodes[i];
            }
            else
            {
                return null;
            }
        }
    }
}
