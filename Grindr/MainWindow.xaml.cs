using System;
using System.Collections.Generic;
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

namespace Grindr
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private List<BotUserControl> botUserControls = new List<BotUserControl>() { new BotUserControl(0) };

        private void AddBotButton_Click(object sender, RoutedEventArgs e)
        {
            var tabItem = new TabItem();
            tabItem.Header = "Bot";
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
