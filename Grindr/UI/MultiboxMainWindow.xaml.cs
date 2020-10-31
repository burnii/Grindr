using Grindr.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using Grindr.VM;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;

namespace Grindr.UI
{
    /// <summary>
    /// Interaction logic for MultiboxMainWindow.xaml
    /// </summary>
    public partial class MultiboxMainWindow : Window
    {
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
            this.MemberIcList.ItemsSource = GlobalState.Instance.SelectedTeam.Member;
        }

        private void RefreshTeamButton_MouseDown(object sender, RoutedEventArgs e)
        {
            TeamVM.UpdateTeams();

            // TODO Parse Teams aus ./Teams/ und ergebnis in Globalstate/Teams hinterlegen
        }

        private void listBoxTeam_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MemberDetailsGrid.Visibility = Visibility.Hidden;
            GridCharacters.Visibility = Visibility.Visible;

            // Team has changed, binding should refresh automatically
            var listbox = (ListBox)sender;
            GlobalState.Instance.SelectedTeam = (TeamVM)listbox.SelectedItem;
            this.MemberIcList.ItemsSource = GlobalState.Instance.SelectedTeam.Member;
        }

        private void ButtonLaunchTeam_Click(object sender, RoutedEventArgs e)
        {
            // Launch selected Team
            GlobalState.Instance.SelectedTeam.LaunchAsync();
        }

        private void ButtonAddMember_Click(object sender, RoutedEventArgs e)
        { 
            
        }

        private void MemberButton_Click(object sender, RoutedEventArgs e)
        {
            MemberDetailsGrid.Visibility = Visibility.Visible;
            GridCharacters.Visibility = Visibility.Hidden;

            var memberVm = (MemberVM)((FrameworkElement)sender).DataContext;
            MemberDetailsGrid.DataContext = memberVm;
        }

        private void StartTeam_Click(object sender, RoutedEventArgs e)
        {
            GlobalState.Instance.SelectedTeam.Start();
        }

        private void StopTeam_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PauseTeam_Click(object sender, RoutedEventArgs e)
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
