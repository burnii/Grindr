﻿using Newtonsoft.Json;
using Grindr.Enums;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Grindr.DTOs;
using Gma.System.MouseKeyHook;
using static Grindr.DataReader;

namespace Grindr.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class BotUserControl : System.Windows.Controls.UserControl
    {
        public BotInstance i { get; set; }
        public Point CurrentMousePoint { get; set; }
        public Point CurrentCalculatedMousePoint { get; set; }


        /// <summary>
        /// Global Mouse Hook Interface-Events
        /// </summary>
        private IKeyboardMouseEvents m_GlobalHook;


        public BotUserControl(int botIndex)
        {
            InitializeComponent();
            i = new BotInstance(this.coordinatesListBox, botIndex);
            this.dataStackPanel.DataContext = this.i.Data;

            m_GlobalHook = Hook.GlobalEvents();
            m_GlobalHook.MouseMove += M_GlobalHook_MouseMove;
            m_GlobalHook.MouseClick += M_GlobalHook_MouseClick;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.SetDataBidnings();
            ((INotifyCollectionChanged)loggingListBox.Items).CollectionChanged += LoggingCollectionChanged;
        }

        private void SetDataBidnings()
        {
            this.ModeComboBox.ItemsSource = Enum.GetValues(typeof(Mode));
            this.ModeComboBox.DataContext = this.i.State;
            this.AttachButton.DataContext = this.i.State;
            this.GeneralGrid.DataContext = this.i.Profile.Settings;
            this.ActionBindComboBox.ItemsSource = Enum.GetValues(typeof(Keys));
            this.loggingListBox.ItemsSource = this.i.Logger.Logs;
            this.positionListBox.ItemsSource = this.i.LastClickedPoints;

            this.Statistics.DataContext = this.i.Statistics;
            this.Positioning.DataContext = this.i.Statistics;

            this.coordinatesListBox.DataContext = this.i.Profile.NavigationNodes;
            this.coordinatesListBox.SetBinding(ItemsControl.ItemsSourceProperty, new System.Windows.Data.Binding());
            ((INotifyCollectionChanged)this.coordinatesListBox.Items).CollectionChanged += CoordinatesCollectionChanged;
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
            try
            {
                await this.i.Initializer.Initialize();

                this.SetupBots();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Fehler");
            }
        }

        private void M_GlobalHook_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.i.Statistics.CaptureMouseClickEnabled)
            {
                this.i.LastClickedPoints.Add(new Point(CurrentCalculatedMousePoint.X, CurrentCalculatedMousePoint.Y));
            }
        }

        private void M_GlobalHook_MouseMove(object sender, MouseEventArgs e)
        {
            CurrentMousePoint = new Point(e.X, e.Y);

            // Calculate Pos of window handle
            if (this.i.Initializer.WindowHandle != null)
            {
                RECT rect;
                GetWindowRect((IntPtr)this.i.Initializer.WindowHandle.Value, out rect);

                var offsetY = 30;
                var offsetX = 8;

                var width = rect.Right - rect.Left;

                var x = (int)CurrentMousePoint.X - rect.Right + width - offsetX;
                var y = (int)CurrentMousePoint.Y - rect.Top - offsetY;

                CurrentCalculatedMousePoint = new Point(x, y);
            }
        }

        private async void Attach()
        {
            await this.i.Initializer.Attach();

            this.SetupBots();

            if (File.Exists(@".\Profiles\hdbnew.json"))
            {
                var serializedProfile = File.ReadAllText(@".\Profiles\hdbnew.json");

                this.i.Profile.NavigationNodes.Clear();

                var profile = JsonConvert.DeserializeObject<Profile>(serializedProfile);

                this.i.Profile.NavigationNodes = profile.NavigationNodes;
                this.i.Profile.Settings = profile.Settings;
                this.i.Profile.Settings.Username = profile.Settings.Username;

                this.SetDataBidnings();
            }
        }

        private void SetupBots()
        {
            this.i.DataReader.Start();
        }

        public Data GetBotData()
        {
            return this.i.Data;
        }

        private void CoordinatesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                this.coordinatesListBox.SelectedIndex = e.NewStartingIndex;
                this.coordinatesListBox.ScrollIntoView(this.coordinatesListBox.SelectedItem);
            }
        }

        private void ModeComboBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var mode = (Mode)this.ModeComboBox.SelectedIndex;

            switch (mode)
            {
                case Mode.Grind:
                    this.assistTabControl.Visibility = Visibility.Hidden;
                    this.grindTabControl.Visibility = Visibility.Visible;
                    this.turretTabControl.Visibility = Visibility.Hidden;
                    break;
                case Mode.Assist:
                    this.grindTabControl.Visibility = Visibility.Hidden;
                    this.assistTabControl.Visibility = Visibility.Visible;
                    this.turretTabControl.Visibility = Visibility.Hidden;
                    break;
                case Mode.Turret:
                    this.grindTabControl.Visibility = Visibility.Visible;
                    this.assistTabControl.Visibility = Visibility.Hidden;
                    this.turretTabControl.Visibility = Visibility.Hidden;
                    break;
            }
        }

        private void LaunchButton_Click(object sender, RoutedEventArgs e)
        {
            this.Initialize();
        }

        private void AttachButton_Click(object sender, RoutedEventArgs e)
        {
            switch (this.i.State.AttachState)
            {
                case AttachState.Attach:
                    this.Attach();
                    break;
                case AttachState.Detach:
                    this.Detach();
                    break;
            }
        }

        private void Detach()
        {
            this.i.State.AttachState = AttachState.Attach;
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

        private void DeleteNavigatioNNodeButton_Click(object sender, RoutedEventArgs e)
        {
            this.i.Grinder.DeleteNavigationNode(this.coordinatesListBox.SelectedIndex);
        }

        private void NavigationNodeListBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            NavigationNode node = null;
            if (this.i.Profile.NavigationNodes.Count > this.coordinatesListBox.SelectedIndex && this.coordinatesListBox.SelectedIndex >= 0)
            {
                node = this.i.Profile.NavigationNodes[this.coordinatesListBox.SelectedIndex];
            }

            this.NavigationNodePropertiesGroupBox.DataContext = node;
        }

        public void StopBot()
        {
            this.i.State.IsRunning = false;
        }

        private async void RunButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.i.State.IsRunning == false)
            {
                this.i.State.IsRunning = true;
                this.runButton.Content = "Stop";

                this.i.Statistics.Track();
                switch (this.i.State.Mode)
                {
                    case Mode.Grind:
                        await this.i.Grinder.StartJourney();
                        break;
                    case Mode.Assist:
                        await this.i.Assister.Assist();
                        break;
                    case Mode.Turret:
                        await this.RunFARM();
                        break;
                    case Mode.Healer:
                        await this.i.Healer.Start();
                        break;
                    case Mode.AntiAfk:
                        await Task.Run(() =>
                        {
                            while (this.i.State.IsRunning)
                            {
                                this.i.InputController.TapKey(Keys.Space);
                                Thread.Sleep(300000);
                            }
                        });
                        break;
                    case Mode.Level:
                        await this.RunLevelTask();
                        break;
                }

                this.runButton.Content = "Start";
                this.i.State.IsRunning = false;
            }
            else
            {
                this.i.State.IsRunning = false;
                this.i.Logger.AddLogEntry("Requested to stop grinder");
            }
        }

        private void SaveProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var serializedProfile = JsonConvert.SerializeObject(this.i.Profile);

            Directory.CreateDirectory(@".\Profiles");
            File.WriteAllText(System.IO.Path.Combine(@".\Profiles\", this.profileNameTextBox.Text + ".json"), serializedProfile.ToString());
        }

        private void ImportProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();

            var result = openFileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                var serializedProfile = File.ReadAllText(openFileDialog.FileName);

                this.i.Profile.NavigationNodes.Clear();

                var profile = JsonConvert.DeserializeObject<Profile>(serializedProfile);

                this.i.Profile.NavigationNodes = profile.NavigationNodes;
                this.i.Profile.Settings = profile.Settings;
                this.i.Profile.Settings.Username = profile.Settings.Username;

                this.SetDataBidnings();
            }
        }

        private void RecordButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.i.State.IsRecording == false)
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

        private Task RunFARM()
        {
            bool loot = false;

            Task.Run(() =>
            {
                while (this.i.State.IsRunning)
                {
                    Thread.Sleep(120000);
                    loot = true;
                }
            });

            return Task.Run(() =>
            {
                while (this.i.State.IsRunning)
                {
                    if (loot)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            Thread.Sleep(3500);
                            this.i.InputController.TapKey(Keys.D2);
                        }

                        loot = false;
                    }
                    else
                    {
                        this.i.InputController.TapKey(Keys.D1);
                        Thread.Sleep(500);
                    }

                    if (this.i.Data.FreeBagSlots < 50)
                    {
                        while (this.i.State.IsRunning && this.i.Data.PlayerIsInCombat)
                        {
                            this.i.InputController.TapKey(Keys.D4);
                            Thread.Sleep(500);
                        }

                        this.i.WowActions.SellItemsIfNeeded(50, 100);
                    }
                }
            });
        }

        private Task RunLevelTask()
        {
            //return Task.Run(() =>
            //{
            //    while (this.i.State.IsRunning)
            //    {
            //        if (!this.i.Data.PlayerHasTarget || this.i.Data.IsTargetDead)
            //        {
            //            this.i.InputController.TapKey(Keys.Tab);
            //        }

            //        if (this.i.Data.PlayerHealth < 60)
            //        {
            //            this.i.InputController.TapKey(Keys.D3);
            //        }
            //        else
            //        {
            //            this.i.InputController.TapKey(Keys.D1);
            //            this.i.InputController.TapKey(Keys.D2);
            //        }

            //        Thread.Sleep(500);
            //    }
            //});

            return Task.Run(() =>
            {
                while (this.i.State.IsRunning)
                {
                    if (this.i.Data.PlayerHealth < 60)
                    {
                        this.i.InputController.TapKey(Keys.D3);
                    }
                    else
                    {
                        this.i.InputController.TapKey(Keys.D1);
                        this.i.InputController.TapKey(Keys.D2);
                    }

                    Thread.Sleep(500);
                }
            });
            
        }

        private Task RunTurretTask()
        {
            return this.RunFARM();
            //var keyArray = new Keys[]
            //    {
            //        Keys.D1,
            //        Keys.D3,
            //        Keys.D4,
            //        Keys.D5,
            //        Keys.D6
            //    };

            //return Task.Run(() =>
            //{
            //    while (this.i.State.IsRunning)
            //    {
            //        if (!this.i.Data.PlayerHasTarget || this.i.Data.IsTargetDead)
            //        { 
            //            this.i.InputController.TapKey(Keys.Tab);
            //        }

            //        if (this.i.Data.PlayerHealth < 60)
            //        {
            //            this.i.InputController.TapKey(Keys.D3);
            //        }
            //        else
            //        { 
            //            this.i.InputController.TapKey(Keys.D1);
            //            this.i.InputController.TapKey(Keys.D2);
            //        }

            //        Thread.Sleep(500);

            //        //this.i.InputController.TapKey(Keys.Tab);
            //        //Console.WriteLine("TAB pressed");

            //        //while (this.i.State.IsRunning && !this.i.Data.PlayerHasTarget)
            //        //{
            //        //    this.HealIfNeeded();
            //        //    this.i.InputController.TapKey(Keys.D4);
            //        //    Thread.Sleep(200);
            //        //}
            //        //Thread.Sleep(200);
            //        //while (this.i.State.IsRunning && this.i.Data.PlayerHasTarget && !this.i.Data.IsTargetDead)
            //        //{
            //        //    this.HealIfNeeded();
            //        //    if (this.i.Profile.Settings.ShouldUseBearForm)
            //        //    {
            //        //        this.i.WowActions.Shapeshift(DruidShapeshiftForm.Bear);

            //        //        if (this.i.Data.PlayerHealth < 50)
            //        //        {
            //        //            this.i.InputController.TapKey(Keys.D4);
            //        //        }
            //        //        this.i.InputController.TapKey(Keys.D6);
            //        //        this.i.InputController.TapKey(Keys.D5);
            //        //    }


            //        //    this.i.InputController.TapKey(Keys.D3);
            //        //    this.i.InputController.TapKey(Keys.D1);
            //        //    Thread.Sleep(200);
            //        //}

            //        //this.i.InputController.TapKey(Keys.Tab);
            //        //this.i.InputController.TapKey(Keys.D1);
            //        //Thread.Sleep(1500);
            //        //this.HealIfNeeded();
            //        //if (this.i.Data.PlayerHasTarget == false)
            //        //{
            //        //    this.i.InputController.TapKey(Keys.Tab);
            //        //}
            //        //this.i.InputController.TapKey(Keys.D3);
            //        //Thread.Sleep(1500);
            //        //if (this.i.Data.PlayerHasTarget == false)
            //        //{
            //        //    this.i.InputController.TapKey(Keys.Tab);
            //        //}
            //        //this.i.InputController.TapKey(Keys.D4);
            //        //this.i.InputController.TapKey(Keys.D3);
            //        //Thread.Sleep(1500);
            //        //this.i.InputController.TapKey(Keys.D5);
            //        //Thread.Sleep(1500);
            //        //this.i.InputController.TapKey(Keys.D6);
            //        //Thread.Sleep(1500);
            //    }


            //});
        }

        private void HealIfNeeded()
        {
            while (this.i.Data.PlayerHealth < 60 || this.i.Data.PlayerHealth > 500 && this.i.State.IsRunning)
            {
                this.i.InputController.TapKey(Keys.D2);
                Thread.Sleep(1000);
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            this.i.Profile.NavigationNodes.Clear();
        }

        private void WowPath_MouseDown(object sender, RoutedEventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                var result = dialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    //this.i.Profile.Settings.WowExePath = dialog.FileName;
                }
            }
        }

        private void VendorProfile_MouseDown(object sender, RoutedEventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                var result = dialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    this.i.Profile.Settings.VendorProfilePath = dialog.FileName;
                }
            }
        }

        private void ClearLastClicks(object sender, RoutedEventArgs e)
        {
            // Clear datasource
            this.i.LastClickedPoints.Clear();
        }

        private void SimulateSelectedClick(object sender, RoutedEventArgs e)
        {
            var selectedIndex = this.positionListBox.SelectedIndex;

            var x = this.i.LastClickedPoints[selectedIndex].X;
            var y = this.i.LastClickedPoints[selectedIndex].Y;

            this.i.InputController.LeftMouseClick((int)x, (int)y);
        }
    }
}
