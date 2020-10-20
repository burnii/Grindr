using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Grindr
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<BotUserControl> botUserControls = new List<BotUserControl>() { new BotUserControl(0) };


        public MainWindow()
        {
            InitializeComponent();

            Task.Run(() =>
            {
                while (true)
                {
                    for (var i = 1; i < botUserControls.Count; i++)
                    {
                        var data = botUserControls[i].GetBotData();

                        var playerName = "";
                        playerName = data.PlayerName;
                        if (string.IsNullOrWhiteSpace(playerName))
                        {
                            playerName = "Bot" + i;
                        }

                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            this.BotTabControl.Items.Cast<TabItem>().ToArray()[i].Header = playerName;
                        }));

                    }
                    Thread.Sleep(2000);
                }
            });

        }


        private void AddBotButton_Click(object sender, RoutedEventArgs e)
        {
            var tabItem = new TabItem();
            tabItem.Header = "Bot" + (this.BotTabControl.Items.Count);
            var control = new BotUserControl(this.BotTabControl.Items.Count - 1);
            tabItem.Content = control;
            botUserControls.Add(control);
            this.BotTabControl.Items.Add(tabItem);
        }

        private void RemoveBotButton_Click(object sender, RoutedEventArgs e)
        {
            var i = this.BotTabControl.SelectedIndex;

            if (i > 0)
            {
                this.botUserControls[i].StopBot();
                this.botUserControls.RemoveAt(i);
                this.BotTabControl.Items.RemoveAt(i);
            }
        }
    }
}
