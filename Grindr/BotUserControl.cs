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
using Grindr.DTOs;
using Newtonsoft.Json.Linq;

namespace Grindr
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class BotUserControl : System.Windows.Controls.UserControl
    {
        public BotInstance i { get; set; }

        public BotUserControl(int botIndex)
        {
            InitializeComponent();
            i = new BotInstance(this.coordinatesListBox, botIndex);
            this.dataStackPanel.DataContext = this.i.Data;
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
                    this.grindTabControl.Visibility = Visibility.Hidden;
                    this.assistTabControl.Visibility = Visibility.Hidden;
                    this.turretTabControl.Visibility = Visibility.Visible;
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

                switch (this.i.State.Mode)
                {
                    case Mode.Grind:
                        await this.i.Grinder.StartJourney();
                        break;
                    case Mode.Assist:
                        await this.i.Assister.Assist();
                        break;
                    case Mode.Turret:
                        await this.RunTurretTask();
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
                this.i.Profile.UpdatePreviousNodeNextNode();
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

        private Task RunTurretTask()
        {
            var keyArray = new Keys[]
                {
                    Keys.D1,
                    Keys.D3,
                    Keys.D4,
                    Keys.D5,
                    Keys.D6
                };

            return Task.Run(() =>
            {
                while (this.i.State.IsRunning)
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
                    this.i.InputController.TapKey(Keys.D5);
                    Thread.Sleep(1500);
                    this.i.InputController.TapKey(Keys.D6);
                    Thread.Sleep(1500);
                }
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.i.InputController.TapKey(Keys.I);
            Thread.Sleep(100);
            this.i.InputController.LeftMouseClick(58, 264);
            Thread.Sleep(100);
            this.i.InputController.LeftMouseClick(185, 157);
            Thread.Sleep(100);
            this.i.InputController.LeftMouseClick(167, 319);
            Thread.Sleep(100);
            this.i.InputController.TapKey(Keys.I);
            this.i.InputController.TapKey(Keys.J);
            this.i.InputController.TapKey(Keys.K);
            Thread.Sleep(100);
            this.i.InputController.LeftMouseClick(164, 300);
            Thread.Sleep(100);
            this.i.InputController.LeftMouseClick(298, 319);

        }

        private void HealIfNeeded()
        {
            while (this.i.Data.PlayerHealth < 70 || this.i.Data.PlayerHealth > 500)
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
            using (var dialog = new FolderBrowserDialog())
            {
                var result = dialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    this.WowPathTextBox.Text = dialog.SelectedPath;
                }
            }
        }
    }
}
