using Grindr.VM;
using Newtonsoft.Json;

namespace Grindr.DTOs
{
    public class MemberVM : BaseViewModel
    {
        [JsonIgnore]
        public BotInstance i { get; set; }

        private string accName;
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

        public void Launch()
        {
            this.i.Initializer.InitializeInternal(this);
        }
    }
}
