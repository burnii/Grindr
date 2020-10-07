using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grindr.DTOs
{
    public class Profile
    {
        public Settings Settings { get; set; }
        public ObservableCollection<NavigationNode> NavigationNodes { get; set; }

        [JsonIgnore]
        public BotInstance i { get; set; }

        public Profile(BotInstance instance)
        {
            this.Settings = new Settings();
            this.NavigationNodes = new ObservableCollection<NavigationNode>();
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
