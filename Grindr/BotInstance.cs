using Grindr.DTOs;
using System.ComponentModel;
using System.Windows.Controls;

namespace Grindr
{
    public class BotInstance
    {
        public int BotIndex { get; set; }
        public CombatController CombatController { get; set; }
        public WalkingController WalkingController { get; set; }
        public InstanceWalkingController InstanceWalkingController { get; set; }
        public InputController InputController { get; set; }
        public Data Data { get; set; }
        public Assister Assister { get; set; }
        public Grinder Grinder { get; set; }
        public Healer Healer { get; set; }
        public DataReader DataReader { get; set; }
        public Initializer Initializer { get; set; }
        public Logger Logger { get; set; }
        public BindingList<System.Windows.Point> LastClickedPoints { get; set; } = new BindingList<System.Windows.Point>();
        public Recorder Recorder { get; set; }
        public WowActions WowActions { get; set; }
        public ListBox NavigationCoordinatesListBox { get; set; }
        public Profile Profile { get; set; }
        public State State { get; set; }
        public Statistics Statistics { get; set; }

        public BotInstance(ListBox lb, int botIndex)
        {
            this.BotIndex = botIndex;
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
            this.Profile = new Profile(this);
            this.WowActions = new WowActions(this);
            //this.NavigationCoordinatesListBox = lb;
            this.State = new State();
            this.Statistics = new Statistics(this);
            this.Healer = new Healer(this);
        }
    }
}
