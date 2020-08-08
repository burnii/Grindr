using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Grindr
{
    public class BotInstance
    {
        public CombatController CombatController { get; set; }
        public WalkingController WalkingController { get; set; }
        public InstanceWalkingController InstanceWalkingController { get; set; }
        public InputController InputController { get; set; }
        public Data Data { get; set; }
        public Assister Assister { get; set; }
        public Grinder Grinder { get; set; }
        public DataReader DataReader { get; set; }
        public Initializer Initializer { get; set; }
        public Logger Logger { get; set; }
        public Recorder Recorder { get; set; }
        public Settings Settings { get; set; }
        public WowActions WowActions { get; set; }
        public ListBox NavigationCoordinatesListBox { get; set; }

        public BotInstance(ListBox lb)
        {
            this.CombatController = new CombatController(this);
            this.WalkingController = new WalkingController(this);
            this.InstanceWalkingController = new InstanceWalkingController(this);
            this.InputController = new InputController(this);
            this.Data = new Data();
            this.Assister = new Assister(this);
            this.Grinder = new Grinder(this);
            this.DataReader = new DataReader(this);
            this.Initializer = new Initializer(this);
            this.Logger = new Logger();
            this.Recorder = new Recorder(this);
            this.Settings = new Settings();
            this.WowActions = new WowActions(this);
            this.NavigationCoordinatesListBox = lb;
        }
    }
}
