using Grindr.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grindr
{
    public static class State
    {
        public static bool IsRunning { get; set; } = false;
        public static Mode Mode { get; set; } = Mode.Grind;
        public static bool IsAttached { get; set; }
        public static bool IsRecording { get; set; } = false;
    }
}
