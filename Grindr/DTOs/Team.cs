using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grindr.DTOs
{
    public class Team
    {
        public static string PathToTeamFiles { get; set; } = "./Teams/";
        public List<Member> Member { get; set; } = new List<Member>();

        public static void UpdateTeams()
        {
            Task.Run(() =>
            {
                var teamFiles = Directory.GetFiles(PathToTeamFiles);

                var newTeams = new List<Team>();

                GlobalState.Instance.Teams.Clear();

                foreach (var teamFile in teamFiles)
                {
                    var serializedTeam = File.ReadAllText(teamFile);

                    var team = JsonConvert.DeserializeObject<Team>(serializedTeam);

                    newTeams.Add(team);

                    InitializeMember(team);

                    GlobalState.Instance.Teams.Add(team);
                }
            });
        }

        public static void AddTeam()
        {
            GlobalState.Instance.Teams.Add(new Team());
        }

        public static void AddMember(Team team)
        {
            var member = new Member
            {
                i = new BotInstance(new System.Windows.Controls.ListBox(), team.Member.Count)
            };
            
            team.Member.Add(member);
        }

        private static void InitializeMember(Team team)
        {
            for (int i = 0; i < team.Member.Count; i++)
            {
                InitializeMember(team.Member[i], i);
            }
        }

        private static void InitializeMember(Member member, int index)
        {
            member.i = new BotInstance(new System.Windows.Controls.ListBox(), index);

            var defaultProfile = member.DefaultProfile;

            if (!string.IsNullOrEmpty(defaultProfile))
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
