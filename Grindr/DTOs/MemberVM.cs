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

        private string serverName;
        [JsonProperty(PropertyName = "ServerName")]
        public string ServerName
        {
            get
            {
                return serverName;
            }
            set
            {
                serverName = value;
                OnPropertyChanged("Server");
            }
        }

        private string charName;
        [JsonProperty(PropertyName = "CharName")]
        public string CharName
        {
            get
            {
                return charName;
            }
            set
            {
                charName = value;
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

        private bool isActive = false;
        [JsonProperty(PropertyName = "IsActive")]
        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;
                OnPropertyChanged("IsActive");
            }
        }


        private bool isRunning;
        [JsonIgnore]
        public bool IsRunning
        {
            get => isRunning;
            set
            {
                isRunning = value;
                OnPropertyChanged("IsRunning");
            }
        }

        private bool isAttached;
        [JsonIgnore]
        public bool IsAttached
        {
            get => isAttached;
            set
            {
                isAttached = value;
                OnPropertyChanged("IsAttached");
            }
        }
    }
}
