using Grindr.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Grindr
{
    public class State : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public bool IsRunning { get; set; } = false;
        public Mode Mode { get; set; } = Mode.Grind;

        public AttachState attachState;
        public AttachState AttachState { get { return attachState; } set { this.attachState = value; this.OnPropertyChanged(); } }
        public bool IsRecording { get; set; } = false;

    }
}
