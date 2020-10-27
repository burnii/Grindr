using Grindr.DTOs;
<<<<<<< HEAD
using System;
using System.Collections.Generic;
using System.IO;
=======
using Grindr.VM;
using System;
>>>>>>> feature/multiboxBaseUI
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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

            Team.UpdateTeams();
            GlobalState.Instance.UpdatedWowExePath();
        }

        private void LaunchButton_MouseDown(object sender, RoutedEventArgs e)
        {
            new Initializer().LaunchTeams();
        }

        private void AddTeamButton_MouseDown(object sender, RoutedEventArgs e)
        {
            Team.AddTeam();
        }

        private void AddMemberButton_MouseDown(object sender, RoutedEventArgs e)
        {
            Team.AddMember(GlobalState.Instance.SelectedTeam);
        }

        private void RefreshTeamButton_MouseDown(object sender, RoutedEventArgs e)
        {
            Team.UpdateTeams();
            this.Initialize();



            // TODO Parse Teams aus ./Teams/ und ergebnis in Globalstate/Teams hinterlegen
        }

        public void Initialize()
        {
            this.MultiboxMainWindowVM.Initialize();
            this.DataContext = this.MultiboxMainWindowVM;

            if (this.MultiboxMainWindowVM.Teams.Count > 0)
            {
                this.listBoxTeam.SelectedItem = this.MultiboxMainWindowVM.Teams.FirstOrDefault();
            }
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
            this.MultiboxMainWindowVM.CurrentSelectedTeam = (TeamVM)item.SelectedItem;
        }

        private void ButtonLaunchTeam_Click(object sender, RoutedEventArgs e)
        {
            // Launch selected Team
        }

        private void DeleteTeam_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NewTeam_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveTeam_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.MultiboxMainWindowVM.SaveTeam(this.MultiboxMainWindowVM.CurrentSelectedTeam);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fehler beim Speichern eines Teams",ex.Message);
            }
        }
    }
}
