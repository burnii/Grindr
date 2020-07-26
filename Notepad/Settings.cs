using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad
{
    public static class Settings
    {
        public static string ProfilePath { get; set; } = @"C:\Users\dbern\OneDrive\Dokumente\Profiles";
        public static string Username { get; set; }
        public static string Password { get; set; }
    }
}
