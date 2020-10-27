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

                    GlobalState.Instance.Teams.Add(team);
                }
            });
        }
    }
}
