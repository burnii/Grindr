using Grindr.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Grindr
{
    public class GlobalState : INotifyPropertyChanged
    {
        public static GlobalState Instance = new GlobalState();

        private GlobalState()
        {
        }

        public void UpdatedWowExePath()
        {
            var wowExePaths = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Wow.exe", System.IO.SearchOption.AllDirectories);

            if (wowExePaths.Length > 1)
            {
                var paths = string.Join(", ", wowExePaths);
                throw new Exception($"Es konnte mehr als eine Wow.exe gefunden werden: {paths}");
            }
            else if (wowExePath.Length == 0)
            {
                throw new Exception("Es konnte keine Wow.exe gefunden werden");
            }
            else
            {
                Instance.wowExePath = wowExePaths.Single();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        //private string wowExePath = @"C:\Program Files (x86)\World of Warcraft\_retail_\Wow.exe";
        private string wowExePath = @"D:\Battle.net\Games\World of Warcraft\_retail_\Wow.exe";
        public string WowExePath { get { return Instance.wowExePath; } set { Instance.wowExePath = value; this.OnPropertyChanged(); } }

        public ObservableCollection<Team> Teams { get; set; } = new ObservableCollection<Team>();
    }
}
