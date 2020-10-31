using Grindr.VM;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

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

        public int Progress
        {
            get
            {
                var member = this.Member.Count;
                var finishedMember = this.Member.Where(c => c.i.Initializer.WindowHandle != null).ToArray().Length;

                return Convert.ToInt32(finishedMember / member);
            }
        }

        public BindingList<MemberVM> Member { get; set; } = new BindingList<MemberVM>();

        public static void UpdateTeams()
        {
            //Task.Run(() =>
            //{
            //    var teamFiles = Directory.GetFiles(PathToTeamFiles);

            //    var newTeams = new List<TeamVM>();

            //    GlobalState.Instance.Teams.Clear();

            //    foreach (var teamFile in teamFiles)
            //    {
            //        var serializedTeam = File.ReadAllText(teamFile);

            //        var team = JsonConvert.DeserializeObject<TeamVM>(serializedTeam);

            //        newTeams.Add(team);

            //        InitializeMember(team);

            //        GlobalState.Instance.Teams.Add(team);
            //    }
            //});

            Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
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
                }));
            });
        }

        public void Start()
        {
            foreach (var member in GlobalState.Instance.SelectedTeam.Member)
            {
                //member.i.InputController.TapKey()
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

        //public void LaunchAsync()
        //{
        //    Task.Run(() =>
        //    {
        //        foreach (var member in this.Member)
        //        {
        //            member.Launch();
        //        }
        //    });

        //}

        public void LaunchAsync()
        {
            Task.Run(async () =>
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

             });
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
