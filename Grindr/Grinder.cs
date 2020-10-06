using Grindr.DTOs;
using Newtonsoft.Json;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
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

        private void RunProfile(Profile profile, int startIndex, bool shouldRepeat = true)
        {
            for (int i = startIndex; i < profile.NavigationNodes.Count; i++)
            {
                var currentNode = profile.NavigationNodes[i];
                if (this.i.State.IsRunning == false)
                {
                    break;
                }

                System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(() => { this.i.NavigationCoordinatesListBox.SelectedIndex = profile.NavigationNodes.IndexOf(currentNode); }));

                IWalkingController wc;

                if (currentNode.Reset)
                {
                    this.i.WowActions.ResetInstances();
                }

                if (currentNode.Action)
                {
                    this.i.InputController.TapKey(currentNode.ActionHotKey);
                }

                if (this.i.Data.IsInInstance)
                {
                    wc = this.i.InstanceWalkingController;
                }
                else
                {
                    wc = this.i.WalkingController;
                }

                if (currentNode.VendorNode)
                {
                    var serializedProfile = File.ReadAllText(this.i.Profile.Settings.VendorProfilePath);

                    var vendorProfile = JsonConvert.DeserializeObject<Profile>(serializedProfile);
                    this.RunProfile(vendorProfile, 0, false);
                }
                else if (currentNode.WalkNode)
                {
                    this.i.WowActions.CloseMap();
                    this.i.InputController.PressKey(Keys.W);
                    Thread.Sleep(2000);
                    this.i.InputController.ReleaseKey(Keys.W);
                }
                else if (currentNode.TurnNode)
                {
                    this.i.InputController.PressKey(System.Windows.Forms.Keys.D);
                    Thread.Sleep(1000);
                    this.i.InputController.ReleaseKey(System.Windows.Forms.Keys.D);
                    wc.WalkUnitilZoneChange();
                    Thread.Sleep(1000);
                }
                else if (currentNode.ZoneChange)
                {
                    wc.Walk(currentNode.Coordinates, false, currentNode.WalkStealthed);
                    wc.WalkOutOfInstance();
                    Thread.Sleep(2000);
                }
                else
                {
                    wc.Walk(currentNode.Coordinates, profile.Settings.AlwaysFight, currentNode.WalkStealthed);
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

                if (currentNode.FastDungeonExit)
                {
                    this.i.WowActions.FastExitDungeon();
                }

                if (currentNode.WaitForZoneChange)
                {
                    this.i.WowActions.WaitForZoneChange();
                }

                if (currentNode.SellNode)
                {
                    this.i.WowActions.SellItems(100);
                }

                if(currentNode.SleepNode)
                {
                    Thread.Sleep(5000);
                }

                this.i.WowActions.SellItemsIfNeeded(30, 90);

                if (shouldRepeat && i == profile.NavigationNodes.Count - 1)
                {
                    this.i.Statistics.Runs++;
                    i = 0;
                }
            }
        }

        public Task UnstuckTask()
        {
            return Task.Run(() =>
            {
                while (this.i.State.IsRunning)
                {
                    var coordinate1 = this.i.Data.PlayerCoordinate;
                    Thread.Sleep(30000);
                    if (coordinate1.Equals(this.i.Data.PlayerCoordinate))
                    {
                        this.i.WowActions.CloseMap();
                        this.i.WowActions.OpenMap();
                    }
                }
            });
        }

        public Task StartJourney()
        {
            var selectedIndex = this.i.NavigationCoordinatesListBox.SelectedIndex;
            return Task.Run(() =>
            {
                this.i.Logger.AddLogEntry("Grinder started");
                var startIndex = 0;

                if (this.i.Profile.Settings.StartFromSelectedNode && selectedIndex >= 0)
                {
                    startIndex = selectedIndex;
                }

                //Task.Run(() => {
                //    var startTime = DateTime.Now;
                //    while (this.i.State.IsRunning)
                //    {
                //        if ((DateTime.Now - startTime).TotalMinutes > 2)
                //        {
                //            this.i.WowActions.FastExitDungeon();
                //        }

                //        Thread.Sleep(2000);
                //    }
                //});

                while (this.i.State.IsRunning)
                {
                    this.RunProfile(this.i.Profile, startIndex);
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
