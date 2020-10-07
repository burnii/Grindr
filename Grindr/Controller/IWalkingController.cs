using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grindr
{
    public interface IWalkingController
    {
        void Walk(Coordinate target, bool isGrinding, bool walkStealthed = false);
        void WalkUnitilZoneChange();
        void WalkOutOfInstance();
    }
}
