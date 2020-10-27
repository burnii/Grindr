using Grindr.VM;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Grindr.DTOs
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class TeamVM : BaseViewModel
    {
        private string teamName;
        [JsonProperty(PropertyName = "TeamName")]
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

        [JsonProperty(PropertyName = "Members")]
        public List<MemberVM> Members { get; set; } = new List<MemberVM>();

        private bool teamIsRunning;
        [JsonProperty(PropertyName = "TeamIsRunning")]
        public bool TeamIsRunning
        {
            get
            {
                return teamIsRunning;
            }
            set
            {
                teamIsRunning = value;
                OnPropertyChanged("teamIsRunning");
            }
        }
    }
}
