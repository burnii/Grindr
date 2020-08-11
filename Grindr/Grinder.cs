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
            lastNodeFromZone.ZoneChange = true;
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

                var startNode = this.NavigationNodes[selectedIndex];
                var currentNode = startNode;

                while (this.i.State.IsRunning)
                {

                    if (this.i.State.IsRunning == false)
                    {
                        break;
                    }

                    Application.Current.Dispatcher.BeginInvoke(new Action(() => { this.i.NavigationCoordinatesListBox.SelectedIndex = this.NavigationNodes.IndexOf(currentNode); }));

                    IWalkingController wc;

                    if (this.i.Data.IsInInstance)
                    {
                        wc = this.i.InstanceWalkingController;
                    }
                    else
                    {
                        wc = this.i.WalkingController;
                    }

                    if (currentNode.ZoneChange)
                    {
                        wc.Walk(currentNode.Coordinates, false);
                        wc.WalkUnitilZoneChange();
                    }
                    else
                    {
                        wc.Walk(currentNode.Coordinates, false);
                    }

                    if (currentNode.CombatNode)
                    {
                        this.i.CombatController.FightWhileInCombat();
                    }
                    else if (currentNode.Turret)
                    {
                        this.i.CombatController.FightWhileInCombat(true);
                    }

                    if (currentNode.Unstuck)
                    {
                        this.i.WowActions.Unstuck();
                    }

                    if (currentNode.Loot)
                    {
                        this.i.WowActions.TryToLootWithMouseClick();
                    }

                    if (currentNode.Reset)
                    {
                        this.i.WowActions.ResetInstances();
                    }

                    if(currentNode.Action)
                    {
                        this.i.InputController.TapKey(currentNode.ActionHotKey);
                    }

                    this.i.WowActions.SellItemsIfNeeded();

                    currentNode = GetNextNode(startNode, currentNode);
                }

                this.i.Logger.AddLogEntry("Grinder stopped");

            });
        }

        private static NavigationNode GetNextNode(NavigationNode startNode, NavigationNode currentNode)
        {
            if (currentNode.NextNode != null)
            {
                currentNode = currentNode.NextNode;
            }
            else
            {
                currentNode = startNode;
            }

            return currentNode;
        }

        public NavigationNode AddNavigationNode(Coordinate coordinate, string zone)
        {
            var index = this.GetIndexToInsert();

            var previousNode = this.TryToGetNodeAtIndex(index - 1);

            var node = new NavigationNode
            {
                Coordinates = coordinate,
                Zone = zone,
                PreviousNode = previousNode
            };

            if (previousNode != null)
            { 
                previousNode.NextNode = node;
            }

            this.i.Logger.AddLogEntry($"Recorded a navigationnode at {this.i.Logger.GetLogMessageForCoordinate(coordinate)}");

            this.NavigationNodes.Insert(index, node);

            return node;
        }

        public void DeleteNavigationNode(int i)
        {
            if (this.NavigationNodes.Count > i && i >= 0)
            { 
                this.NavigationNodes.RemoveAt(i);
            }
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
