using Grindr.VM;
using Newtonsoft.Json;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using System.Windows;

namespace Grindr.DTOs
{
    public class TeamVM : BaseViewModel
    {
        ManagementEventWatcher stopWatcher;

        private void stopWatcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            var processName = (string)e.NewEvent.Properties["ProcessName"].Value;
            var processIdAsObject = e.NewEvent.Properties["ProcessID"].Value;

            if (Int32.TryParse(processIdAsObject.ToString(), out int res))
            {
                var processId = res;
                var corruptMember = this.Member.Where(c => c.i.Initializer?.Process?.Id == (int)processId).Select(c => c).SingleOrDefault();
                if (corruptMember != null)
                {
                    corruptMember.IsAttached = false;
                    corruptMember.i.Initializer.Process = null;
                    corruptMember.i.Initializer.WindowHandle = null;
                    UpdateProgress();
                }
            }
            //Console.WriteLine(string.Format("{0} stopped", (string)e.NewEvent["ProcessName"]));
        }


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

        private int progress;
        public int Progress
        {
            get
            {
                return progress;
            }
            set
            {
                progress = value;
                OnPropertyChanged("Progress");
            }
        }

        public TeamVM()
        {
            stopWatcher = new ManagementEventWatcher("Select * From Win32_ProcessStopTrace");
            stopWatcher.EventArrived += new EventArrivedEventHandler(stopWatcher_EventArrived);
            stopWatcher.Start();
        }

        public BindingList<MemberVM> Member { get; set; } = new BindingList<MemberVM>();

        public void UpdateTeams()
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

            member.Initialize();

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
                     var couldLaunch = await member.Launch();
                     member.IsAttached = couldLaunch;
                     UpdateProgress();

                     if (!couldLaunch)
                     {
                         // Member konnte nicht gestartet werden                         
                     }
                 }
             });
        }

        private void UpdateProgress()
        {
            this.Progress = (this.Member.Count(c => c.IsAttached) * 100) / this.Member.Count;
        }

        private void InitializeMember(TeamVM team)
        {
            for (int i = 0; i < team.Member.Count; i++)
            {
                InitializeMember(team.Member[i], i);
            }
        }

        private void InitializeMember(MemberVM member, int index)
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
