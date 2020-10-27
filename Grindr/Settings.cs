using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Grindr
{
    public class Settings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private string username = "Username";
        public string Username { get { return username; } set { this.username = value; this.OnPropertyChanged(); } }
        public string Password { get; set; } = "Password";
        public int CharacterPosition { get; set; } = 1;
        public int BlizzAccountIndex { get; set; } = 1;
        public bool ShouldUseMount { get; set; } = true;
        public bool AlwaysFight { get; set; } = false;
        private bool shouldUseTravelForm = false;
        public bool ShouldUseTravelForm { get { return shouldUseTravelForm; } set { this.shouldUseTravelForm = value; this.OnPropertyChanged(); } }
        private bool shouldUseBearForm = false;
        public bool ShouldUseBearForm { get { return shouldUseBearForm; } set { this.shouldUseBearForm = value; this.OnPropertyChanged(); } }
        public bool StartFromSelectedNode { get; set; } = false;
        private string vendorProfilePath = @"C:\Users\dbern\source\repos\Grindr\Grindr\bin\Debug\Profiles\grimgrimi.json";
        public string VendorProfilePath { get { return vendorProfilePath; } set { this.vendorProfilePath = value; this.OnPropertyChanged(); } }
        private bool shouldVendorItems = false;
        public bool ShouldVendorItems { get { return shouldVendorItems; } set { this.shouldVendorItems = value; this.OnPropertyChanged(); } }
        public bool ShouldUseVendorMount { get; set; } = false;
        private bool shouldUseCatFormMovement = false;
        public bool ShouldUseCatFormMovement { get { return shouldUseCatFormMovement; } set { this.shouldUseCatFormMovement = value; this.OnPropertyChanged(); } }

    }
}
