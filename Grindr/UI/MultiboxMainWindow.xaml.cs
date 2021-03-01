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
using System.ComponentModel;
using System.Windows.Forms;
using Newtonsoft.Json;
using Grindr.Enums;

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
            this.InputsDropDown.ItemsSource = Enum.GetValues(typeof(Keys));
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

        private void AddInputButton_MouseDown(object sender, RoutedEventArgs e)
        {
            var bindings = ((BindingList<Keys>)this.InputListbox.ItemsSource);

            bindings.Add((Keys)this.InputsDropDown.SelectedItem);
        }

        private void RemoveInputButton_MouseDown(object sender, RoutedEventArgs e)
        {
            var selectedIndex = this.InputListbox.SelectedIndex;

            if (selectedIndex >= 0)
            {
                var bindings = ((BindingList<Keys>)this.InputListbox.ItemsSource);
                bindings.RemoveAt(selectedIndex);
            }
        }

        private void ClearInputsButton_MouseDown(object sender, RoutedEventArgs e)
        {
            var bindings = ((BindingList<Keys>)this.InputListbox.ItemsSource);
            bindings.Clear();
        }

        private void RefreshTeamButton_MouseDown(object sender, RoutedEventArgs e)
        {
            TeamVM.UpdateTeams();
        }

        private void ImportProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            var member = (MemberVM)this.MemberDetailsGrid.DataContext;

            var result = openFileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                var serializedProfile = File.ReadAllText(openFileDialog.FileName);

                var profile = JsonConvert.DeserializeObject<Profile>(serializedProfile);

                member.i.Profile.CombatRoutine = profile.CombatRoutine;
                MemberDetailsGrid.DataContext = member;
                this.InputListbox.ItemsSource = member.i.Profile.CombatRoutine.InputKeys;
                this.CombatRoutineStackPanel.DataContext = member.i.Profile.CombatRoutine;
                var fileNameWithExtension = openFileDialog.FileName.Split('\\').Last();
                this.ProfileNameTextBox.Text = fileNameWithExtension.Substring(0, fileNameWithExtension.IndexOf("."));
            }
        }

        private void SaveProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var member = (MemberVM)this.MemberDetailsGrid.DataContext;

            var serializedProfile = JsonConvert.SerializeObject(member.i.Profile);

            Directory.CreateDirectory(@".\Profiles");
            File.WriteAllText(System.IO.Path.Combine(@".\Profiles\", this.ProfileNameTextBox.Text + ".json"), serializedProfile.ToString());
        }

        private void listBoxTeam_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MemberDetailsGrid.Visibility = Visibility.Hidden;
            GridCharacters.Visibility = Visibility.Visible;

            // Team has changed, binding should refresh automatically
            var listbox = (System.Windows.Controls.ListBox)sender;
            GlobalState.Instance.SelectedTeam = (TeamVM)listbox.SelectedItem;

            if (GlobalState.Instance.SelectedTeam != null)
            {
                this.MemberIcList.ItemsSource = GlobalState.Instance.SelectedTeam.Member;
                this.GridCharacters.DataContext = GlobalState.Instance.SelectedTeam;
            }
        }

        private void ButtonLaunchTeam_Click(object sender, RoutedEventArgs e)
        {
            // Launch selected Team
            GlobalState.Instance.SelectedTeam.LaunchAsync();
        }

        private void ButtonAddMember_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveButton_MouseDown(object sender, RoutedEventArgs e)
        {
            TeamVM.SaveTeams();
        }

        private void AttachButton_Click(object sender, RoutedEventArgs e)
        {
            var member = (MemberVM)this.MemberDetailsGrid.DataContext;

            switch (member.i.State.AttachState)
            {
                case AttachState.Attach:
                    member.i.Initializer.Attach();
                    break;
                case AttachState.Detach:
                    member.i.State.AttachState = AttachState.Attach;
                    break;
            }
        }

        private void MemberButton_Click(object sender, RoutedEventArgs e)
        {
            MemberDetailsGrid.Visibility = Visibility.Visible;
            GridCharacters.Visibility = Visibility.Hidden;

            var memberVm = (MemberVM)((FrameworkElement)sender).DataContext;
            MemberDetailsGrid.DataContext = memberVm;
            this.InputListbox.ItemsSource = memberVm.i.Profile.CombatRoutine.InputKeys;
            this.CombatRoutineStackPanel.DataContext = memberVm.i.Profile.CombatRoutine;
            this.ProfileNameTextBox.Text = "";
            this.ButtonsStackPanel.DataContext = memberVm.i.State;
            this.dataStackPanel.DataContext = memberVm.i.Data;
        }

        private void DefaultProfileTextBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            var member = (MemberVM)this.MemberDetailsGrid.DataContext;

            var result = openFileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                var fileNameWithExtension = openFileDialog.FileName.Split('\\').Last();
                member.DefaultProfile = fileNameWithExtension.Substring(0, fileNameWithExtension.IndexOf("."));
            }
        }

        private void StartTeam_Click(object sender, RoutedEventArgs e)
        {
            GlobalState.Instance.SelectedTeam.Start();
        }

        private void StopTeam_Click(object sender, RoutedEventArgs e)
        {
            GlobalState.Instance.SelectedTeam.Stop();
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

        private void DefaultProfileTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (var member in GlobalState.Instance.SelectedTeam.Member)
            {
                member.i.Profile.CombatRoutine = GlobalState.Instance.SelectedTeam.CombatRoutine;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MemberDetailsGrid.Visibility = Visibility.Visible;
            GridCharacters.Visibility = Visibility.Hidden;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            GlobalState.Instance.Teams.Clear();

            var drives = System.Environment.GetLogicalDrives();

            var info = new DirectoryInfo(@"C:\Program Files (x86)\World of Warcraft\_retail_\WTF\Account");
            var subs = info.GetDirectories();

            var elements = (from x in subs
                            where x.Name.Contains("#")
                            let bla = x.Name.Split('#').First()
                            orderby x.CreationTime descending
                            group x by bla into g
                            select g).ToDictionary(g => g.Key, g => g.ToArray());

            var newTeams = new List<TeamVM>();

            foreach (var element in elements)
            {
                var team = new TeamVM();

                var teamName = "";
                var index = 1;
                foreach (var acc in element.Value)
                {
                    var subDirectories = acc.GetDirectories();
                    var serverDirectory = subDirectories.First();

                    var member = new MemberVM
                    {
                        WowAccIndex = index,
                        Charname = serverDirectory.GetDirectories().Count() > 0 ? serverDirectory.GetDirectories().First().Name : null,
                        IsLeader = index == 1,
                        Password = "yh9tegjmg",
                        Server = serverDirectory.Name,
                        i = new BotInstance(null, index)
                    };

                    team.Member.Add(member);

                    if (teamName != "")
                    {
                        teamName += ", ";
                    }

                    teamName += member.Charname;

                    index++;
                }

                team.TeamName = teamName;

                newTeams.Add(team);
            }

            foreach (var team in newTeams)
            {
                GlobalState.Instance.Teams.Add(team);
            }

            //Directory.GetDirectories(asd2, "*.*", SearchOption.TopDirectoryOnly);
        }
    }
}
