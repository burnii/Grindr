using Grindr.DTOs;
using Newtonsoft.Json;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Grindr.VM
{
    public class MultiboxMainWindowVM : BaseViewModel
    {
        public BindingList<TeamVM> Teams { get; set; } = new BindingList<TeamVM>();

        private TeamVM currentSelectedTeam;

        public TeamVM CurrentSelectedTeam
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

        private string RootPath { get; set; } = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);


        public MultiboxMainWindowVM()
        {

        }

        public void Initialize()
        {
            // Json to Teams
            this.LoadTeams();
        }

        public void SaveTeam(TeamVM team)
        {
            // serialize JSON to a string and then write string to a file 
            var fileName = team.TeamName;
            var filePath = $@"{this.RootPath}\Teams\{fileName}";

            File.WriteAllText(filePath, JsonConvert.SerializeObject(team));

            // serialize JSON directly to a file
            using (StreamWriter file = File.CreateText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, team);
            }
        }

        private void LoadTeams()
        {
            if (!Directory.Exists($@"{this.RootPath}\Teams"))
            {
                //throw new Exception("Keine Teams gefunden");
                return;
            }

            var teams = Directory.GetFiles($@"{this.RootPath}\Teams");

            foreach (var teamPath in teams)
            {
                // deserialize JSON directly from a file
                using (StreamReader file = File.OpenText(teamPath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    TeamVM team = (TeamVM)serializer.Deserialize(file, typeof(TeamVM));
                    this.Teams.Add(team);
                }
            }
        }

    }
}
