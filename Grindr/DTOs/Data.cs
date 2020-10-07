

using Grindr.Enums;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Channels;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Grindr
{
    public class Data : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public Coordinate PlayerCoordinate
        {
            get
            {
                return new Coordinate(PlayerXCoordinate, PlayerYCoordinate);
            }
        }

        private  bool isTargetDead;
        public  bool IsTargetDead
        {
            get
            {
                return isTargetDead;
            }
            set
            {
                isTargetDead = value;

                OnPropertyChanged();
            }
        }

        private  double playerXCoordinate;
        public  double PlayerXCoordinate
        {
            get
            {
                return playerXCoordinate;
            }
            set
            {
                playerXCoordinate = value;
                OnPropertyChanged();
            }
        }

        private  double playerYCoordinate;
        public  double PlayerYCoordinate
        {
            get
            {
                return playerYCoordinate;
            }
            set
            {
                playerYCoordinate = value;

                OnPropertyChanged();
            }
        }

        private  double playerFacing;
        public  double PlayerFacing
        {
            get
            {
                return playerFacing;
            }
            set
            {
                playerFacing = value;

                OnPropertyChanged();
            }
        }

        private  bool playerIsInCombat;
        public  bool PlayerIsInCombat
        {
            get
            {
                return playerIsInCombat;
            }
            set
            {
                playerIsInCombat = value;

                OnPropertyChanged();
            }
        }

        private  bool playerHasTarget;
        public  bool PlayerHasTarget
        {
            get
            {
                return playerHasTarget;
            }
            set
            {
                playerHasTarget = value;

                OnPropertyChanged();
            }
        }

        private  string playerZone;
        public  string PlayerZone
        {
            get
            {
                return playerZone;
            }
            set
            {
                playerZone = value;

                OnPropertyChanged();
            }
        }

        private  bool isInInstance;
        public  bool IsInInstance
        {
            get
            {
                return isInInstance;
            }
            set
            {
                isInInstance = value;

                OnPropertyChanged();
            }
        }

        private  bool isMapOpened;
        public  bool IsMapOpened
        {
            get
            {
                return isMapOpened;
            }
            set
            {
                isMapOpened = value;

                OnPropertyChanged();
            }
        }

        private  bool isPlayerDead;
        public  bool IsPlayerDead
        {
            get
            {
                return isPlayerDead;
            }
            set
            {
                isPlayerDead = value;

                OnPropertyChanged();
            }
        }

        private  bool targetIsInInteractRange;
        public  bool TargetIsInInteractRange
        {
            get
            {
                return targetIsInInteractRange;
            }
            set
            {
                targetIsInInteractRange = value;

                OnPropertyChanged();
            }
        }

        private  bool isTargetAttackingPlayer;
        public  bool IsTargetAttackingPlayer
        {
            get
            {
                return isTargetAttackingPlayer;
            }
            set
            {
                isTargetAttackingPlayer = value;

                OnPropertyChanged();
            }
        }

        private  bool isOutDoors;
        public  bool IsOutDoors
        {
            get
            {
                return isOutDoors;
            }
            set
            {
                isOutDoors = value;

                OnPropertyChanged();
            }
        }

        private  int freeBagSlots;
        public  int FreeBagSlots
        {
            get
            {
                return freeBagSlots;
            }
            set
            {
                freeBagSlots = value;

                OnPropertyChanged();
            }
        }

        private  bool isMounted;
        public  bool IsMounted
        {
            get
            {
                return isMounted;
            }
            set
            {
                isMounted = value;

                OnPropertyChanged();
            }
        }

        private  int playerHealth;
        public  int PlayerHealth
        {
            get
            {
                return playerHealth;
            }
            set
            {
                playerHealth = value;

                OnPropertyChanged();
            }
        }

        private string playerName;
        public string PlayerName
        {
            get
            {
                return playerName;
            }
            set
            {
                playerName = value;

                OnPropertyChanged();
            }
        }

        private bool playerIsInGroup;
        public bool PlayerIsInGroup
        {
            get
            {
                return playerIsInGroup;
            }
            set
            {
                playerIsInGroup = value;

                OnPropertyChanged();
            }
        }

        private DruidShapeshiftForm druidShapeshiftForm;
        public DruidShapeshiftForm DruidShapeshiftForm
        {
            get
            {
                return druidShapeshiftForm;
            }
            set
            {
                druidShapeshiftForm = value;

                OnPropertyChanged();
            }
        }

        private int party1Health;
        public int Party1Health
        {
            get
            {
                return party1Health;
            }
            set
            {
                party1Health = value;

                OnPropertyChanged();
            }
        }

        private int party2Health;
        public int Party2Health
        {
            get
            {
                return party2Health;
            }
            set
            {
                party2Health = value;

                OnPropertyChanged();
            }
        }

        private int party3Health;
        public int Party3Health
        {
            get
            {
                return party3Health;
            }
            set
            {
                party3Health = value;

                OnPropertyChanged();
            }
        }

        private int party4Health;
        public int Party4Health
        {
            get
            {
                return party4Health;
            }
            set
            {
                party4Health = value;

                OnPropertyChanged();
            }
        }

        private bool isStealthed;
        public bool IsStealthed
        {
            get
            {
                return isStealthed;
            }
            set
            {
                isStealthed = value;

                OnPropertyChanged();
            }
        }

        private int gold;
        public int Gold
        {
            get
            {
                return gold;
            }
            set
            {
                gold = value;

                OnPropertyChanged();
            }
        }

    }
}
