using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grindr.DTOs
{
    public class Team
    {
        public List<Member> Member { get; set; } = new List<Member>();
    }
}
