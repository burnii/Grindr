using Newtonsoft.Json;
using Grindr.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Cursor = System.Windows.Forms.Cursor;
using Point = System.Drawing.Point;

namespace Grindr
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class BotUserControl : System.Windows.Controls.UserControl
    {
        public BotInstance i { get; set; }

        public BotUserControl()
        {
            InitializeComponent();
            i = new BotInstance(this.coordinatesListBox);
            this.dataStackPanel.DataContext = this.i.Data;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            this.loggingListBox.ItemsSource = this.i.Logger.Logs;
            ((INotifyCollectionChanged)loggingListBox.Items).CollectionChanged += LoggingCollectionChanged;
        }

        private void LoggingCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                this.loggingListBox.ScrollIntoView(this.loggingListBox.Items[this.loggingListBox.Items.Count - 1]);
            }
        }

        private async void Initialize()
        {
            await this.i.Initializer.Initialize();

            this.SetupBots();
        }

        private async void Attach()
        {
            await this.i.Initializer.Attach();

            this.SetupBots();
        }

        private void SetupBots()
        {
            this.i.DataReader.Start();

            this.coordinatesListBox.DataContext = this.i.Grinder.NavigationNodes;
            this.coordinatesListBox.SetBinding(ItemsControl.ItemsSourceProperty, new System.Windows.Data.Binding());
            ((INotifyCollectionChanged)this.coordinatesListBox.Items).CollectionChanged += CoordinatesCollectionChanged;
        }

        private void CoordinatesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                this.coordinatesListBox.SelectedIndex = e.NewStartingIndex;
                this.coordinatesListBox.ScrollIntoView(this.coordinatesListBox.SelectedItem);
            }
        }

        private void LaunchButton_Click(object sender, RoutedEventArgs e)
        {
            this.Initialize();
        }

        private void AttachButton_Click(object sender, RoutedEventArgs e)
        {
            if (!State.IsAttached)
            {
                this.Attach();
            }
            else
            {
                this.Detach();
            }
        }

        private void Detach()
        {
            State.IsAttached = false;
        }

        private void AddNavigationNodeButton_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                if (this.i.Data.IsInInstance)
                {
                    this.i.WowActions.OpenMap();
                }
                this.i.Recorder.AddNavigationNode(this.i.Data.PlayerCoordinate);
                this.i.WowActions.CloseMap();
            });
        }

        private void MarkAsCombatNodeButton_Click(object sender, RoutedEventArgs e)
        {
            var i = this.coordinatesListBox.SelectedIndex;
            this.i.Grinder.MarkNavigationNode(this.i.Grinder.NavigationNodes[i], NavigationNodeType.Combat);
        }

        private void MarkAsWayPointButton_Click(object sender, RoutedEventArgs e)
        {
            var i = this.coordinatesListBox.SelectedIndex;
            this.i.Grinder.MarkNavigationNode(this.i.Grinder.NavigationNodes[i], NavigationNodeType.WayPoint);
        }

        private void MarkAsZoneChangeButton_Click(object sender, RoutedEventArgs e)
        {
            var i = this.coordinatesListBox.SelectedIndex;
            this.i.Grinder.MarkNavigationNode(this.i.Grinder.NavigationNodes[i], NavigationNodeType.ZoneChange);
        }

        private void MarkAsUnstuckButton_Click(object sender, RoutedEventArgs e)
        {
            var i = this.coordinatesListBox.SelectedIndex;
            this.i.Grinder.MarkNavigationNode(this.i.Grinder.NavigationNodes[i], NavigationNodeType.Unstuck);
        }

        private void MarkAsLootButton_Click(object sender, RoutedEventArgs e)
        {
            var i = this.coordinatesListBox.SelectedIndex;
            this.i.Grinder.MarkNavigationNode(this.i.Grinder.NavigationNodes[i], NavigationNodeType.Loot);
        }

        private void MarkAsResetButton_Click(object sender, RoutedEventArgs e)
        {
            var i = this.coordinatesListBox.SelectedIndex;
            this.i.Grinder.MarkNavigationNode(this.i.Grinder.NavigationNodes[i], NavigationNodeType.Reset);
        }

        private void DeleteNavigatioNNodeButton_Click(object sender, RoutedEventArgs e)
        {
            this.i.Grinder.DeleteNavigationNode(this.coordinatesListBox.SelectedIndex);
        }

        private async void RunButton_Click(object sender, RoutedEventArgs e)
        {
            if (State.IsRunning == false)
            {
                State.IsRunning = true;
                this.runButton.Content = "Stop";

                switch (State.Mode)
                {
                    case Mode.Grind:
                        await this.i.Grinder.StartJourney();
                        break;
                    case Mode.Assist:
                        await this.i.Assister.Assist("bla");
                        break;
                }


                this.runButton.Content = "Start";
                State.IsRunning = false;
            }
            else
            {
                State.IsRunning = false;
                this.i.Logger.AddLogEntry("Requested to stop grinder");
            }
        }

        private void SaveProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var serializedNavigationNodes = JsonConvert.SerializeObject(this.i.Grinder.NavigationNodes);

            File.WriteAllText(System.IO.Path.Combine(this.i.Settings.ProfilePath, this.profileNameTextBox.Text + ".json"), serializedNavigationNodes.ToString());
        }

        private void ImportProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();

            var result = openFileDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                var serializedNavigationNodes = File.ReadAllText(openFileDialog.FileName);

                this.i.Grinder.NavigationNodes.Clear();

                foreach (var navNode in JsonConvert.DeserializeObject<ObservableCollection<NavigationNode>>(serializedNavigationNodes))
                {
                    this.i.Grinder.NavigationNodes.Add(navNode);
                }

                Task.Run(() =>
                {
                    // Wait that the listBox list is synchronized with the navigationNodes list.
                    // TODO NICOLAS FRAGEN
                    //Thread.Sleep(2000);
                    //this.grinder.UpdateNavigationNodeColors();
                });
            }
        }

        private void GrindButton_Click(object sender, RoutedEventArgs e)
        {
            this.assistTabControl.Visibility = Visibility.Hidden;
            this.grindTabControl.Visibility = Visibility.Visible;

            State.Mode = Mode.Grind;
        }

        private void AssistButton_Click(object sender, RoutedEventArgs e)
        {
            this.grindTabControl.Visibility = Visibility.Hidden;
            this.assistTabControl.Visibility = Visibility.Visible;

            State.Mode = Mode.Assist;
        }

        private void RecordButton_Click(object sender, RoutedEventArgs e)
        {
            if (State.IsRecording == false)
            {
                this.i.Recorder.StartRecording();
                this.recordButton.Content = "Stop recording";
            }
            else
            {
                this.i.Recorder.StopRecording();
                this.recordButton.Content = "Start recording";
            }
        }


        private bool stop = true;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (stop == true)
            {
                stop = false;
            }
            else
            {
                stop = true;
            }

            var keyArray = new Keys[]
                {
                    Keys.D1,
                    Keys.D3,
                    Keys.D4,
                    Keys.D5,
                    Keys.D6
                };

            //Task.Run(() =>
            //{
            //    while (!stop)
            //    {
            //        this.i.InputController.TapKey(Keys.Tab);
            //        Thread.Sleep(2000);
            //    }
            //});

            //Task.Run(() =>
            //{
            //    var i = 0;
            //    while (!stop)
            //    {
            //        if (i > 4)
            //        {
            //            i = 0;
            //        }

            //        if (this.i.Data.PlayerHealth < 70 || this.i.Data.PlayerHealth > 500)
            //        {
            //            this.i.InputController.TapKey(Keys.D2);
            //        }
            //        else
            //        {
            //            this.i.InputController.TapKey(keyArray[i]);
            //            i++;
            //        }
            //        Thread.Sleep(700);
            //    }
            //});

            Task.Run(() =>
            {
                while (!stop)
                {
                    this.i.InputController.TapKey(Keys.Tab);
                    this.i.InputController.TapKey(Keys.D1);
                    Thread.Sleep(1500);
                    this.HealIfNeeded();
                    if (this.i.Data.PlayerHasTarget == false)
                    {
                        this.i.InputController.TapKey(Keys.Tab);
                    }
                    this.i.InputController.TapKey(Keys.D3);
                    Thread.Sleep(1500);
                    if (this.i.Data.PlayerHasTarget == false)
                    {
                        this.i.InputController.TapKey(Keys.Tab);
                    }
                    this.i.InputController.TapKey(Keys.D4);
                    this.i.InputController.TapKey(Keys.D3);
                    Thread.Sleep(1500);
                }
            });
        }

        private void HealIfNeeded()
        {
            while (this.i.Data.PlayerHealth < 70 || this.i.Data.PlayerHealth > 500)
            {
                this.i.InputController.TapKey(Keys.D2);
                Thread.Sleep(1000);
            }
        }
    }
}
