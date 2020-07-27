using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grindr
{
    public class NavigationNode
    {
        public Coordinate Coordinates { get; set; }
        public NavigationNodeType Type { get; set; }
        public NavigationNode NextNode { get; set; }
        public NavigationNode PreviousNode { get; set; }

        public NavigationNode(double x, double y, NavigationNodeType type, NavigationNode next = null, NavigationNode prev = null)
        {
            this.Coordinates = new Coordinate(x, y);
            this.Type = type;
            this.NextNode = next;
            this.PreviousNode = prev;
        }
    }
}
