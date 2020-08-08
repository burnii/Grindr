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

        public BotInstance i { get; set; }

        public Grinder(BotInstance instance)
        {
            this.NavigationNodes = new ObservableCollection<NavigationNode>();
            this.i = instance;
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
            var i = this.NavigationNodes.IndexOf(node);
            var item = this.i.NavigationCoordinatesListBox.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
            if (item != null)
            {
                switch (node.Type)
                {
                    case NavigationNodeType.Combat:
                        Application.Current.Dispatcher.Invoke(new Action(() => { item.Background = new BrushConverter().ConvertFrom("#FFB7B2") as Brush; }));
                        break;
                    case NavigationNodeType.WayPoint:
                        if (item != null)
                        {
                            Application.Current.Dispatcher.Invoke(new Action(() => { item.Background = null as Brush; }));
                        }
                        break;
                    case NavigationNodeType.ZoneChange:
                        Application.Current.Dispatcher.Invoke(new Action(() => { item.Background = new BrushConverter().ConvertFrom("#E2F0CB") as Brush; }));
                        break;
                    case NavigationNodeType.Dungeon:
                        Application.Current.Dispatcher.Invoke(new Action(() => { item.Background = new BrushConverter().ConvertFrom("#C7CEEA") as Brush; }));
                        break;
                    case NavigationNodeType.Unstuck:
                        Application.Current.Dispatcher.Invoke(new Action(() => { item.Background = new BrushConverter().ConvertFrom("#F9E79F") as Brush; }));
                        break;
                    case NavigationNodeType.Loot:
                        Application.Current.Dispatcher.Invoke(new Action(() => { item.Background = new BrushConverter().ConvertFrom("#F9E79F") as Brush; }));
                        break;
                }
            }
        }

        public void UpdateNavigationNodeColors()
        {
            foreach (var node in this.NavigationNodes)
            {
                this.UpdateNavigationNodeColor(node);
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
            var selectedIndex = this.i.NavigationCoordinatesListBox.SelectedIndex;
            return Task.Run(() =>
            {
                this.i.Logger.AddLogEntry("Grinder started");
                
                while (State.IsRunning)
                {
                    for (int i = selectedIndex; i < this.NavigationNodes.Count; i++)
                    {
                        if (State.IsRunning == false)
                        {
                            break;
                        }

                        IWalkingController wc;

                        if (this.i.Data.IsInInstance)
                        {
                            wc = this.i.InstanceWalkingController;
                        }
                        else
                        {
                            wc = this.i.WalkingController;
                        }

                        Application.Current.Dispatcher.BeginInvoke(new Action(() => { this.i.NavigationCoordinatesListBox.SelectedIndex = i; }));

                        if (this.NavigationNodes[i].Type == NavigationNodeType.ZoneChange)
                        {
                            wc.Walk(this.NavigationNodes[i].Coordinates, false);
                            wc.WalkUnitilZoneChange();
                        }
                        else
                        {
                            wc.Walk(this.NavigationNodes[i].Coordinates, false);
                        }

                        switch (this.NavigationNodes[i].Type)
                        {
                            case NavigationNodeType.Combat:
                                this.i.CombatController.FightWhileInCombat();
                                break;
                            case NavigationNodeType.Unstuck:
                                this.i.WowActions.Unstuck();
                                break;
                            case NavigationNodeType.Loot:
                                this.i.WowActions.TryToLootWithMouseClick();
                                break;
                            case NavigationNodeType.Reset:
                                this.i.WowActions.ResetInstances();
                                break;
                        }

                        this.i.WowActions.SellItemsIfNeeded();
                    }
                }

                this.i.Logger.AddLogEntry("Grinder stopped");

            });
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


            this.i.Logger.AddLogEntry($"Recorded a navigationnode at {this.i.Logger.GetLogMessageForCoordinate(coordinate)}");

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
            if (this.i.NavigationCoordinatesListBox.SelectedItem == null)
            {
                return this.NavigationNodes.Count;
            }
            else
            {
                return this.i.NavigationCoordinatesListBox.SelectedIndex + 1;
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
