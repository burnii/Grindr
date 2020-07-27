using Newtonsoft.Json;
using Notepad.Enums;
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

namespace Notepad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Grinder grinder;
        private Assister assister;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.loggingListBox.ItemsSource = Logger.Logs;
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
            await Initializer.Initialize();

            this.SetupBots();
        }

        private async void Attach()
        {
            await Initializer.Attach();

            this.SetupBots();
        }

        private void SetupBots()
        {
            DataReader.Start();
            this.grinder = new Grinder(this.coordinatesListBox);
            this.assister = new Assister();

            this.coordinatesListBox.DataContext = this.grinder.NavigationNodes;
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
            this.Attach();
        }

        private void AddNavigationNodeButton_Click(object sender, RoutedEventArgs e)
        {
            this.grinder.AddNavigationNode(Data.PlayerXCoordinate, Data.PlayerYCoordinate, NavigationNodeType.WayPoint);
        }

        private void MarkAsCombatNodeButton_Click(object sender, RoutedEventArgs e)
        {
            var index = this.coordinatesListBox.SelectedIndex;
            this.grinder.NavigationNodes[index].Type = NavigationNodeType.Combat;
        }

        private void MarkAsWayPointButton_Click(object sender, RoutedEventArgs e)
        {
            var index = this.coordinatesListBox.SelectedIndex;
            this.grinder.NavigationNodes[index].Type = NavigationNodeType.WayPoint;
        }

        private void DeleteNavigatioNNodeButton_Click(object sender, RoutedEventArgs e)
        {
            this.grinder.DeleteNavigationNode(this.coordinatesListBox.SelectedIndex);
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
                        await this.grinder.StartJourney();
                        break;
                    case Mode.Assist:
                        await this.assister.Assist("bla");
                        break;
                }


                this.runButton.Content = "Start";
                State.IsRunning = false;
            }
            else
            {
                State.IsRunning = false;
                Logger.AddLogEntry("Requested to stop grinder");
            }
        }

        private void SaveProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var serializedNavigationNodes = JsonConvert.SerializeObject(this.grinder.NavigationNodes);

            File.WriteAllText(System.IO.Path.Combine(Settings.ProfilePath, this.profileNameTextBox.Text + ".json"), serializedNavigationNodes.ToString());
        }

        private void importProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();

            var result = openFileDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                var serializedNavigationNodes = File.ReadAllText(openFileDialog.FileName);

                foreach (var navNode in JsonConvert.DeserializeObject<ObservableCollection<NavigationNode>>(serializedNavigationNodes))
                {
                    this.grinder.NavigationNodes.Add(navNode);
                }
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
    }
}
