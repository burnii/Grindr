﻿

using System;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Notepad
{
    public class Data
    {
        static Data()
        {
            PlayerXCoordinateChanged += (sender, e) => { return; };
            PlayerYCoordinateChanged += (sender, e) => { return; };
            PlayerFacingChanged += (sender, e) => { return; };
            PlayerIsInCombatChanged += (sender, e) => { return; };
            PlayerHasTargetChanged += (sender, e) => { return; };
            IsTargetDeadChanged += (sender, e) => { return; };
        }

        public static event EventHandler PlayerXCoordinateChanged;
        public static event EventHandler PlayerYCoordinateChanged;
        public static event EventHandler PlayerFacingChanged;
        public static event EventHandler PlayerIsInCombatChanged;
        public static event EventHandler PlayerHasTargetChanged;
        public static event EventHandler IsTargetDeadChanged;

        public static Coordinate PlayerCoordinate
        {
            get
            {
                return new Coordinate(PlayerXCoordinate, PlayerYCoordinate);
            }
        }

        private static bool isTargetDead;
        public static bool IsTargetDead
        {
            get
            {
                return isTargetDead;
            }
            set
            {
                isTargetDead = value;

                IsTargetDeadChanged(null, EventArgs.Empty);
            }
        }

        private static double playerXCoordinate;
        public static double PlayerXCoordinate
        {
            get
            {
                return playerXCoordinate;
            }
            set
            {
                playerXCoordinate = value;

                PlayerXCoordinateChanged(null, EventArgs.Empty);
            }
        }

        private static double playerYCoordinate;
        public static double PlayerYCoordinate
        {
            get
            {
                return playerYCoordinate;
            }
            set
            {
                playerYCoordinate = value;

                PlayerYCoordinateChanged(null, EventArgs.Empty);
            }
        }

        private static double playerFacing;
        public static double PlayerFacing
        {
            get
            {
                return playerFacing;
            }
            set
            {
                playerFacing = value;

                PlayerFacingChanged(null, EventArgs.Empty);
            }
        }

        private static bool playerIsInCombat;
        public static bool PlayerIsInCombat
        {
            get
            {
                return playerIsInCombat;
            }
            set
            {
                playerIsInCombat = value;

                PlayerIsInCombatChanged(null, EventArgs.Empty);
            }
        }

        private static bool playerHasTarget;
        public static bool PlayerHasTarget
        {
            get
            {
                return playerHasTarget;
            }
            set
            {
                playerHasTarget = value;

                PlayerHasTargetChanged(null, EventArgs.Empty);
            }
        }

        
    }
}