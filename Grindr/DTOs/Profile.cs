using Grindr.CombaRoutine;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace Grindr.DTOs
{
    public class Profile
    {
        public Settings Settings { get; set; }
        public ObservableCollection<NavigationNode> NavigationNodes { get; set; }
        public CombatRoutine CombatRoutine { get; set; }

        [JsonIgnore]
        public BotInstance i { get; set; }

        public Profile(BotInstance instance)
        {
            this.Settings = new Settings();
            this.NavigationNodes = new ObservableCollection<NavigationNode>();
            this.CombatRoutine = new CombatRoutine();
            this.i = instance;
        }

        private NavigationNode TryToGetNodeAtIndex(int i)
        {
            if (this.i.Profile.NavigationNodes.Count >= (i + 1) && i >= 0)
            {
                return this.i.Profile.NavigationNodes[i];
            }
            else
            {
                return null;
            }
        }
    }
}
