using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Grindr.VM
{
    public class MultiboxMainWindowVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        public BindingList<string> Teams { get; set; } = new BindingList<string>();


        private string currentSelectedTeam;

        public string CurrentSelectedTeam
        {
            get
            {
                return currentSelectedTeam;
            }
            set
            {
                currentSelectedTeam = value;

                OnPropertyChanged();
            }
        }


        public MultiboxMainWindowVM()
        {

        }

        public void Initialize()
        {
            // 1) this.Teams => LoadTeamsFromJson();
            this.Teams.Add("Team1");
            this.Teams.Add("Team2");
            this.Teams.Add("Team3");
        }

    }
}
