using Grindr.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using Grindr.VM;
using System;
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
            this.DataContext = GlobalState.Instance;

            TeamVM.UpdateTeams();
            GlobalState.Instance.UpdateWowExePath();
        }

        private void LaunchButton_MouseDown(object sender, RoutedEventArgs e)
        {
            new Initializer().LaunchTeams();
        }

        private void AddTeamButton_MouseDown(object sender, RoutedEventArgs e)
        {
            TeamVM.AddTeam();
        }

        private void AddMemberButton_MouseDown(object sender, RoutedEventArgs e)
        {
            TeamVM.AddMember(GlobalState.Instance.SelectedTeam);
        }

        private void RefreshTeamButton_MouseDown(object sender, RoutedEventArgs e)
        {
            TeamVM.UpdateTeams();

            // TODO Parse Teams aus ./Teams/ und ergebnis in Globalstate/Teams hinterlegen
        }

        private void listBoxTeam_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Team has changed, binding should refresh automatically
            var listbox = (ListBox)sender;
            GlobalState.Instance.SelectedTeam = (TeamVM)listbox.SelectedItem;
            this.MemberIcList.ItemsSource = GlobalState.Instance.SelectedTeam.Member;
        }

        private void ButtonLaunchTeam_Click(object sender, RoutedEventArgs e)
        {
            // Launch selected Team
            GlobalState.Instance.SelectedTeam.Launch();
        }



        private void DeleteTeam_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NewTeam_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveTeam_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    this.MultiboxMainWindowVM.SaveTeam(this.MultiboxMainWindowVM.CurrentSelectedTeam);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Fehler beim Speichern eines Teams",ex.Message);
            //}
        }
    }
}
