﻿using Grindr.DTOs;
using Grindr.VM;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
            this.MultiboxMainWindowVM.CreateNewTeam();
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

        private void listBoxTeam_LostFocus(object sender, RoutedEventArgs e)
        {
            this.listBoxTeam.SelectedItem = this.MultiboxMainWindowVM.CurrentSelectedTeam;
        }

        private void EditTeam_Click(object sender, RoutedEventArgs e)
        {
            //var selectedItem = (ListBoxItem)this.listBoxTeam.SelectedItem;

            //// Iterate whole listbox tree and search for this items
            //TextBox nameBox = this.FindDescendant<TextBox>(selectedItem);
            //nameBox.IsReadOnly = !nameBox.IsReadOnly;
        }

        private T FindDescendant<T>(DependencyObject obj) where T : DependencyObject
        {
            // Check if this object is the specified type
            if (obj is T)
                return obj as T;

            // Check for children
            int childrenCount = VisualTreeHelper.GetChildrenCount(obj);
            if (childrenCount < 1)
                return null;

            // First check all the children
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child is T)
                    return child as T;
            }

            // Then check the childrens children
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = FindDescendant<T>(VisualTreeHelper.GetChild(obj, i));
                if (child != null && child is T)
                    return child as T;
            }

            return null;
        }
    }
}