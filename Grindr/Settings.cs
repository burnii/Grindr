using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Grindr
{
    public class Settings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private string wowExePath = @"C:\Program Files (x86)\World of Warcraft\_retail_\Wow.exe";
        public string WowExePath { get { return this.wowExePath; } set { this.wowExePath = value; this.OnPropertyChanged(); } }

        private string username = "Username";
        public string Username { get { return username; } set { this.username = value; this.OnPropertyChanged(); } }
        public string Password { get; set; } = "Password";
        public bool ShouldUseMount { get; set; } = true;
        public bool AlwaysFight { get; set; } = false;

    }
}
