using Grindr.VM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Grindr.UI
{
    /// <summary>
    /// Interaction logic for MultiboxMainWindow.xaml
    /// </summary>
    public partial class MultiboxMainWindow : Window
    {
        private MultiboxMainWindowVM MultiboxMainWindowVM { get; set; } = new MultiboxMainWindowVM();

        public MultiboxMainWindow()
        {
            InitializeComponent();

            this.Initialize();



            // TODO Parse Teams aus ./Teams/ und ergebnis in Globalstate/Teams hinterlegen
        }

        public void Initialize()
        {
            this.MultiboxMainWindowVM.Initialize();
            this.DataContext = this.MultiboxMainWindowVM;

            this.listBoxTeam.ItemsSource = this.MultiboxMainWindowVM.Teams;
            this.listBoxTeam.SelectedIndex = 0;
        }

        private void ButtonAdd1_Click(object sender, RoutedEventArgs e)
        {
            // Remove Button
            this.StackPanelChar1.Children.Remove((UIElement)sender);

            // Enable UI Element
            this.BotControl1.Visibility = Visibility.Visible;
        }

        private void ButtonAdd2_Click(object sender, RoutedEventArgs e)
        {
            // Remove Button
            this.StackPanelChar2.Children.Remove((UIElement)sender);

            // Enable UI Element
            this.BotControl2.Visibility = Visibility.Visible;
        }

        private void ButtonAdd3_Click(object sender, RoutedEventArgs e)
        {
            // Remove Button
            this.StackPanelChar3.Children.Remove((UIElement)sender);

            // Enable UI Element
            this.BotControl3.Visibility = Visibility.Visible;
        }

        private void ButtonAdd4_Click(object sender, RoutedEventArgs e)
        {
            // Remove Button
            this.StackPanelChar4.Children.Remove((UIElement)sender);

            // Enable UI Element
            this.BotControl4.Visibility = Visibility.Visible;
        }

        private void ButtonAdd5_Click(object sender, RoutedEventArgs e)
        {
            // Remove Button
            this.StackPanelChar5.Children.Remove((UIElement)sender);

            // Enable UI Element
            this.BotControl5.Visibility = Visibility.Visible;
        }

        private void listBoxTeam_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Team has changed, binding should refresh automatically
            var item = (ListBox)sender;
            this.MultiboxMainWindowVM.CurrentSelectedTeam = (string)item.SelectedItem;
        }

        private void ButtonLaunchTeam_Click(object sender, RoutedEventArgs e)
        {
            // Launch selected Team
        }


    }
}
