using QuickGraph;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
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
        public BotInstance i { get; set; }

        public Grinder(BotInstance instance)
        {
            this.i = instance;
        }

        internal void MarkLastNavigationNodeAsZoneChangeNode(string playerZone)
        {
            var lastNodeFromZone = this.GetLastNavigationNodeFromZone(playerZone);
            lastNodeFromZone.ZoneChange = true;
        }

        private NavigationNode GetLastNavigationNodeFromZone(string zone)
        {
            for (var i = 0; i < this.i.Profile.NavigationNodes.Count; i++)
            {
                var node = this.i.Profile.NavigationNodes[i];
                if (node.Zone == zone && this.i.Profile.NavigationNodes[i + 1].Zone != zone)
                {
                    return node;
                }
            }

            return null;
        }

        public Task StartJourney()
        {
            var selectedIndex = this.i.NavigationCoordinatesListBox.SelectedIndex;
            return Task.Run(() =>
            {
                this.i.Logger.AddLogEntry("Grinder started");
                var startIndex = 0;

                if (this.i.Profile.Settings.StartFromSelectedNode)
                {
                    startIndex = selectedIndex;
                }

                while (this.i.State.IsRunning)
                {
                    for (int i = startIndex; i < this.i.Profile.NavigationNodes.Count; i++)
                    {
                        var currentNode = this.i.Profile.NavigationNodes[i];
                        if (this.i.State.IsRunning == false)
                        {
                            break;
                        }

                        Application.Current.Dispatcher.BeginInvoke(new Action(() => { this.i.NavigationCoordinatesListBox.SelectedIndex = this.i.Profile.NavigationNodes.IndexOf(currentNode); }));

                        IWalkingController wc;

                        if (currentNode.Reset)
                        {
                            this.i.WowActions.ResetInstances();
                        }

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
                            wc.Walk(currentNode.Coordinates, this.i.Profile.Settings.AlwaysFight);
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

                        if (currentNode.Action)
                        {
                            this.i.InputController.TapKey(currentNode.ActionHotKey);
                        }

                        if (currentNode.FastDungeonExit)
                        {
                            this.i.WowActions.FastExitDungeon();
                        }

                        if (currentNode.WaitForZoneChange)
                        {
                            this.i.WowActions.WaitForZoneChange();
                        }

                        this.i.WowActions.SellItemsIfNeeded();
                    }
                }

                this.i.Logger.AddLogEntry("Grinder stopped");

            });
        }

        public NavigationNode AddNavigationNode(Coordinate coordinate, string zone)
        {
            var index = this.GetIndexToInsert();

            var previousNode = this.TryToGetNodeAtIndex(index - 1);

            var node = new NavigationNode
            {
                Coordinates = coordinate,
                Zone = zone,
            };

            this.i.Logger.AddLogEntry($"Recorded a navigationnode at {this.i.Logger.GetLogMessageForCoordinate(coordinate)}");

            this.i.Profile.NavigationNodes.Insert(index, node);

            return node;
        }

        public void DeleteNavigationNode(int i)
        {
            if (this.i.Profile.NavigationNodes.Count > i && i >= 0)
            {
                this.i.Profile.NavigationNodes.RemoveAt(i);
            }
        }

        private int GetIndexToInsert()
        {
            if (this.i.NavigationCoordinatesListBox.SelectedItem == null)
            {
                return this.i.Profile.NavigationNodes.Count;
            }
            else
            {
                return this.i.NavigationCoordinatesListBox.SelectedIndex + 1;
            }
        }

        private NavigationNode TryToGetNodeAtIndex(int i)
        {
            if (this.i.Profile.NavigationNodes.Count >= (i + 1) && i >= 0)
            {
                return this.i.Profile.NavigationNodes[i];
            }
            else
            {
                return null;
            }
        }
    }
}
