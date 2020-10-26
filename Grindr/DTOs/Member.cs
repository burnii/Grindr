using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grindr.DTOs
{
    public class Member
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public int WowAccIndex { get; set; }
        public int CharIndex { get; set; }
        public string Server { get; set; }
        public string Charname { get; set; }
        public bool IsLeader { get; set; }
        public string DefaultProfile { get; set; }
    }
}
