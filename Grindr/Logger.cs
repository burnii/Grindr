using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Grindr
{
    public class Logger
    {
        public ObservableCollection<string> Logs { get; set; } = new ObservableCollection<string>();

        public void AddLogEntry(string entry)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => Logs.Add($"{DateTime.Now} - {entry}")));
        }

        public string GetLogMessageForCoordinate(Coordinate coordinate)
        {
            return $"x: {coordinate.X}, y: {coordinate.Y}";
        }
    }
}
