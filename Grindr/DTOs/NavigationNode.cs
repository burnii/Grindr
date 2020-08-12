using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Grindr
{
    public class NavigationNode
    {
        public Coordinate Coordinates { get; set; }
        //[JsonIgnore]
        //public NavigationNode NextNode { get; set; }
        //[JsonIgnore]
        //public NavigationNode PreviousNode { get; set; }
        public string Zone { get; set; }
        public bool CombatNode { get; set; }
        public bool Turret { get; set; }
        public bool ZoneChange { get; set; }
        public bool Unstuck { get; set; }
        public bool Loot { get; set; }
        public bool Reset { get; set; }
        public bool Action { get; set; }
        public bool FastDungeonExit { get; set; }
        public Keys ActionHotKey { get; set; }
        public bool WaitForZoneChange { get; set; }
        public bool WalkStealthed { get; set; }
    }
}
