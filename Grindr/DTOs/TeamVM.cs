using Grindr.CombaRoutine;
using Grindr.VM;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace Grindr.DTOs
{
    public class TeamVM : BaseViewModel
    {
        [JsonIgnore]
        public static string PathToTeamFiles { get; set; } = "./Teams/";
        [JsonIgnore]
        public static string PathToProfiles { get; set; } = "./Profiles/";

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

        private CombatRoutine combatRoutine = new CombatRoutine();
        public CombatRoutine CombatRoutine
        {
            get
            {
                return combatRoutine;
            }
            set
            {
                combatRoutine = value;
                OnPropertyChanged();
            }
        }

        private bool shouldUseTeamCombatRoutine = true;
        public bool ShouldUseTeamCombatRoutine
        {
            get 
            {
                return shouldUseTeamCombatRoutine;
            }
            set
            {
                shouldUseTeamCombatRoutine = value;
                OnPropertyChanged();
            }
        }


        public BindingList<MemberVM> Member { get; set; } = new BindingList<MemberVM>();

        public static void UpdateTeams()
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
        }

        public static void SaveTeams()
        {
            foreach (var team in GlobalState.Instance.Teams)
            {
                team.Save();
            }
        }

        public void Save()
        {
            var serializedTeam = JsonConvert.SerializeObject(this);
            File.WriteAllText(Path.Combine(PathToTeamFiles, this.GetTeamFileName()), serializedTeam);
        }

        public void Start()
        {
            foreach (var member in this.Member)
            {
                if (this.shouldUseTeamCombatRoutine)
                {
                    member.StartCombatRoutine(this.CombatRoutine);
                }
                else
                { 
                    member.StartCombatRoutine();
                }
            }
        }

        public void Stop()
        {
            foreach (var member in this.Member)
            {
                member.StopCombatRoutine();
            }
        }

        public static void AddTeam()
        {
            // TODO Popup für die Eingabe des Teamnamen. bis dahin Team + count
            var count = GlobalState.Instance.Teams.Count + 1;
            GlobalState.Instance.Teams.Add(new TeamVM() { TeamName = $"Team{count}" });
        }

        public static void AddMember(TeamVM team)
        {
            var member = new MemberVM
            {
                i = new BotInstance(new System.Windows.Controls.ListBox(), team.Member.Count)
            };

            team.Member.Add(member);
        }

        public void TapKeysForAllMember(params Keys[] keys)
        {
            Parallel.ForEach(this.Member.Where(x => x.i.Initializer.Process != null), (member) =>
            {
                foreach (var key in keys)
                {
                    member.i.InputController.TapKey(key);
                }
            });
        }

        public void VendorItemsForAllMember()
        {
            this.TapKeysForAllMember(Keys.D4);
            Thread.Sleep(2000);

            this.TapKeysForAllMember(Keys.D8);
            Thread.Sleep(7000);

            this.TapKeysForAllMember(Keys.D9);
            Thread.Sleep(1000);
            this.TapKeysForAllMember(Keys.D9);
            Thread.Sleep(1000);

            this.TapKeysForAllMember(Keys.Y);
            Thread.Sleep(1000);
            this.TapKeysForAllMember(Keys.Y);
            Thread.Sleep(1000);

            for (int i = 0; i < 10; i++)
            {
                this.TapKeysForAllMember(Keys.D0);
                Thread.Sleep(1000);
            }
        }

        public async Task LaunchAsync()
        {
            foreach (var member in this.Member)
            {
                Console.WriteLine($"{DateTime.Now}: Start");
                var couldLaunch = await member.Launch();
                Console.WriteLine($"{DateTime.Now}: End");

                if (!couldLaunch)
                {
                    // Member konnte nicht gestartet werden
                }
            }
        }

        private string GetTeamFileName()
        {
            return this.teamName + ".json";
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

            var fullProfilePath = Path.Combine(PathToProfiles, defaultProfile + ".json");

            if (!string.IsNullOrEmpty(defaultProfile) && File.Exists(fullProfilePath))
            {
                var serializedProfile = File.ReadAllText(fullProfilePath);

                member.i.Profile.NavigationNodes.Clear();

                var profile = JsonConvert.DeserializeObject<Profile>(serializedProfile);

                member.i.Profile.NavigationNodes = profile.NavigationNodes;
                member.i.Profile.Settings = profile.Settings;
                member.i.Profile.Settings.Username = profile.Settings.Username;
                member.i.Profile.CombatRoutine = profile.CombatRoutine;
            }
        }
    }
}
