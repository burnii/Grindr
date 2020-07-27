using Notepad.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad
{
    public static class State
    {
        public static bool IsRunning { get; set; } = false;
        public static Mode Mode { get; set; } = Mode.Grind;
    }
}
