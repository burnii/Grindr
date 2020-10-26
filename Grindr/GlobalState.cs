using Grindr.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grindr
{
    public class GlobalState
    {
        public static ObservableCollection<Team> Teams { get; set; }
    }
}
