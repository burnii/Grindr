using Grindr.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grindr
{
    public class State
    {
        public bool IsRunning { get; set; } = false;
        public Mode Mode { get; set; } = Mode.Grind;
        public bool IsAttached { get; set; }
        public bool IsRecording { get; set; } = false;
    }
}
