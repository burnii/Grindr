using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Grindr
{
    public static class Logger
    {
        public static ObservableCollection<string> Logs { get; set; } = new ObservableCollection<string>();

        public static void AddLogEntry(string entry)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => Logs.Add($"{DateTime.Now} - {entry}")));
        }

        public static string GetLogMessageForCoordinate(Coordinate coordinate)
        {
            return $"x: {coordinate.X}, y: {coordinate.Y}";
        }
    }
}
