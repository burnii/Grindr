using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Notepad
{
    public class Grinder
    {
        public ObservableCollection<NavigationNode> NavigationNodes { get; set; }

        public ListBox NavigationCoordinatesListBox { get; set; }

        public WalkingController WalkingController { get; set; }

        private CancellationTokenSource CancellationTokenSource { get; set; }

        private CombatController CombatController { get; set; }

        public Grinder(ListBox navigationCoordinatesListBox)
        {
            this.NavigationCoordinatesListBox = navigationCoordinatesListBox;
            this.NavigationNodes = new ObservableCollection<NavigationNode>();
            this.WalkingController = new WalkingController();
            this.CombatController = new CombatController();
        }

        public Task StartJourney()
        {
            this.CancellationTokenSource = new CancellationTokenSource();

            return Task.Run(() =>
            {
                Logger.AddLogEntry("Grinder started");

                while (State.IsRunning)
                {
                    for (int i = 0; i < this.NavigationNodes.Count; i++)
                    {
                        if (State.IsRunning == false)
                        {
                            break;
                        }

                        // TODO Only fight while on navigationnode -> try to fight sometimes, always fight when in combat
                        Application.Current.Dispatcher.BeginInvoke(new Action(() => { this.NavigationCoordinatesListBox.SelectedIndex = i; }));
                        
                        this.WalkingController.Walk(this.NavigationNodes[i].Coordinates, true);

                        if (this.NavigationNodes[i].Type == NavigationNodeType.Combat)
                        {
                            this.CombatController.FightWhileInCombat();
                        }
                    }
                }

                Logger.AddLogEntry("Grinder stopped");
                
            }, this.CancellationTokenSource.Token);
        }

        public NavigationNode AddNavigationNode(double x, double y, NavigationNodeType type)
        {
            var index = this.GetIndexToInsert();

            var node = new NavigationNode(
                x,
                y, 
                type, 
                this.TryToGetNodeAtIndex(index),
                this.TryToGetNodeAtIndex(index - 1)
            );

            this.NavigationNodes.Insert(index, node);

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
