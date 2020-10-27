using Grindr.VM;
using Newtonsoft.Json;

namespace Grindr.DTOs
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class MemberVM : BaseViewModel
    {
        private string accName;
        [JsonProperty(PropertyName = "AccName")]
        public string AccName
        {
            get
            {
                return accName;
            }
            set
            {
                accName = value;
                OnPropertyChanged("AccName");
            }
        }

        private string password;
        [JsonProperty(PropertyName = "Password")]
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
                OnPropertyChanged("Password");
            }
        }

        private int wowAccIndex;
        [JsonProperty(PropertyName = "WowAccIndex")]
        public int WowAccIndex
        {
            get
            {
                return wowAccIndex;
            }
            set
            {
                wowAccIndex = value;
                OnPropertyChanged("WowAccIndex");
            }
        }

        private int charIndex;
        [JsonProperty(PropertyName = "CharIndex")]
        public int CharIndex
        {
            get
            {
                return charIndex;
            }
            set
            {
                charIndex = value;
                OnPropertyChanged("CharIndex");
            }
        }

        private string server;
        [JsonProperty(PropertyName = "Server")]
        public string Server
        {
            get
            {
                return server;
            }
            set
            {
                server = value;
                OnPropertyChanged("Server");
            }
        }

        private string charname;
        [JsonProperty(PropertyName = "Charname")]
        public string Charname
        {
            get
            {
                return charname;
            }
            set
            {
                charname = value;
                OnPropertyChanged("Charname");
            }
        }

        private bool isLeader;
        [JsonProperty(PropertyName = "IsLeader")]
        public bool IsLeader
        {
            get
            {
                return isLeader;
            }
            set
            {
                isLeader = value;
                OnPropertyChanged("IsLeader");
            }
        }

        private string defaultProfile;
        [JsonProperty(PropertyName = "DefaultProfile")]
        public string DefaultProfile
        {
            get
            {
                return defaultProfile;
            }
            set
            {
                defaultProfile = value;
                OnPropertyChanged("DefaultProfile");
            }
        }
    }
}
