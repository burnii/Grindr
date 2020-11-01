﻿using Grindr.VM;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

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

        private bool isAttached;

        [JsonIgnore]
        public bool IsAttached
        {
            get
            {
                return isAttached;
            }
            set
            {
                isAttached = value;
                OnPropertyChanged("IsAttached");
            }
        }

        private string defaultProfile;

        public MemberVM()
        {
          
        }

        public void Initialize()
        {
        }

        internal void Start()
        {
            while (this.i.State.IsRunning)
            { 
                
            }
        }

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

        public async Task<bool> Launch()
        {
            return await this.i.Initializer.InitializeInternal(this);
        }
    }
}
