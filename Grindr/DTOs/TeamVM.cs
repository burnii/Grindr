using Grindr.VM;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Grindr.DTOs
{
    public class TeamVM : BaseViewModel
    {
        [JsonIgnore]
        public static string PathToTeamFiles { get; set; } = "./Teams/";

        private string teamName;
        public string TeamName
        {
            get
            {
                return teamName;
            }
            set
            {
                teamName = value;
                OnPropertyChanged("TeamName");
            }
        }

        public List<MemberVM> Member { get; set; } = new List<MemberVM>();

        public static void UpdateTeams()
        {
            Task.Run(() =>
            {
                var teamFiles = Directory.GetFiles(PathToTeamFiles);

                var newTeams = new List<TeamVM>();

                GlobalState.Instance.Teams.Clear();

                foreach (var teamFile in teamFiles)
                {
                    var serializedTeam = File.ReadAllText(teamFile);

                    var team = JsonConvert.DeserializeObject<TeamVM>(serializedTeam);

                    newTeams.Add(team);

                    InitializeMember(team);

                    GlobalState.Instance.Teams.Add(team);
                }
            });
        }

        public static void AddTeam()
        {
            GlobalState.Instance.Teams.Add(new TeamVM());
        }

        public static void AddMember(TeamVM team)
        {
            var member = new MemberVM
            {
                i = new BotInstance(new System.Windows.Controls.ListBox(), team.Member.Count)
            };

            team.Member.Add(member);
        }

        public void Launch()
        { 
            foreach(var member in this.Member)
            {
                member.Launch();
            }
        }

        private static void InitializeMember(TeamVM team)
        {
            for (int i = 0; i < team.Member.Count; i++)
            {
                InitializeMember(team.Member[i], i);
            }
        }

        private static void InitializeMember(MemberVM member, int index)
        {
            member.i = new BotInstance(null, index);

            var defaultProfile = member.DefaultProfile;

            if (!string.IsNullOrEmpty(defaultProfile) && File.Exists(defaultProfile))
            {
                var serializedProfile = File.ReadAllText(member.DefaultProfile);

                member.i.Profile.NavigationNodes.Clear();

                var profile = JsonConvert.DeserializeObject<Profile>(serializedProfile);

                member.i.Profile.NavigationNodes = profile.NavigationNodes;
                member.i.Profile.Settings = profile.Settings;
                member.i.Profile.Settings.Username = profile.Settings.Username;
            }
        }
    }
}
