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

        [JsonIgnore]
        public bool IsChar1Visible
        {
            get
            {
                return this.Members.ElementAt(0) != null ? true : false;
            }
        }

        [JsonIgnore]
        public bool IsChar2Visible
        {
            get
            {
                return this.Members.ElementAt(1) != null ? true : false;
            }
        }

        [JsonIgnore]
        public bool IsChar3Visible
        {
            get
            {
                return this.Members.ElementAt(2) != null ? true : false;
            }
        }

        [JsonIgnore]
        public bool IsChar4Visible
        {
            get
            {
                return this.Members.ElementAt(3) != null ? true : false;
            }
        }

        [JsonIgnore]
        public bool IsChar5Visible
        {
            get
            {
                return this.Members.ElementAt(4) != null ? true : false;
            }
        }
    }
}
