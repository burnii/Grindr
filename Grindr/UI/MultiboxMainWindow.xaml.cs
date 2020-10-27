﻿using Grindr.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        }
    }
}
